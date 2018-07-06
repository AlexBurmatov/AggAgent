using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using Collections;
using com.tibbo.aggregate.common.device;
using JavaCompatibility;

namespace com.tibbo.aggregate.common.command
{
    public abstract class AsyncCommandProcessor<I, O> where I : Command where O : Command
    {
        private const long PENDING_COMMAND_TIMEOUT = 1000000; // 1000 Seconds

        protected Thread thread;

        protected Int64 timeout; // Milliseconds
        protected Boolean sendImmediately;

        private readonly BlockingQueue<ReplyMonitor<O, I>> unsentCommandsQueue = new BlockingQueue<ReplyMonitor<O, I>>();

        private readonly SynchronizedList<ReplyMonitor<O, I>> sentCommandsQueue =
            new SynchronizedList<ReplyMonitor<O, I>>(new List<ReplyMonitor<O, I>>());

        private bool shouldStop;

        public void stop()
        {
            shouldStop = true;
        }

        protected AsyncCommandProcessor(String name, long timeout, Boolean sendImmediately)
        {
            thread = new Thread(run) { IsBackground = true };
            thread.Name = "AsyncCommandProcessor/" + name + "/" + thread.Name;
            this.timeout = timeout;
            this.sendImmediately = sendImmediately;
        }

        protected abstract void sendCommandImplementation(O cmd);

        protected abstract I receiveCommandImplementation();

        protected abstract Boolean isAsync(I cmd);

        protected abstract void processAsyncCommand(I cmd);

        protected abstract void onStop();

        [MethodImpl(MethodImplOptions.Synchronized)]
        public ReplyMonitor<O, I> sendCommand(O cmd)
        {
            if (!thread.IsAlive)
            {
                lock (sentCommandsQueue)
                {
                    foreach (var cur in sentCommandsQueue)
                    {
                        cur.setReply(null);
                    }
                }

                lock (unsentCommandsQueue)
                {
                    foreach (var cur in unsentCommandsQueue)
                    {
                        cur.setReply(null);
                    }
                }

                throw new DisconnectionException(Cres.get().getString("cmdDisconnected"));
            }

            var mon = new ReplyMonitor<O, I>(cmd);

            if (sendImmediately)
            {
                addSentCommand(mon);
                doSend(cmd);
            }
            else
            {
                unsentCommandsQueue.enqueue(mon);
            }

            return mon;
        }

        private void doSend(O cmd)
        {
            sendCommandImplementation(cmd);
            Logger.getLogger(Log.COMMANDS).debug("Sent command: " + cmd);
        }

        public I sendSyncCommand(O cmd)
        {
            var mon = sendCommand(cmd);

            return waitReplyMonitor(mon);
        }

        private I waitReplyMonitor(ReplyMonitor<O, I> mon)
        {
            if (mon.getReply() == null)
            {
                mon.waitReply(timeout);
            }

            sentCommandsQueue.Remove(mon);

            if (mon.getReply() != null)
            {
                return mon.getReply();
            }
            throw new IOException(String.Format(Cres.get().getString("cmdTimeout"), mon.getCommand()));
        }

        protected void addSentCommand(ReplyMonitor<O, I> mon)
        {
            sentCommandsQueue.Add(mon);
        }

        public void resetSentCommandTimeouts()
        {
            lock (sentCommandsQueue)
            {
                foreach (var cur in sentCommandsQueue)
                {
                    cur.resetTime();
                }
            }
        }

        public void run()
        {
            I cmd;

            try
            {
                while (!shouldStop)
                {
                    while (!unsentCommandsQueue.isEmpty())
                    {
                        var mon = unsentCommandsQueue.dequeue();
                        lock (this)
                        {
                            addSentCommand(mon);

                            doSend(mon.getCommand());
                        }
                    }

                    cmd = receiveCommandImplementation();

                    if (cmd == null) continue;
                    Logger.getLogger(Log.COMMANDS).debug("Received command: " + cmd);

                    if (isAsync(cmd))
                    {
                        processAsyncCommand(cmd);
                    }
                    else
                    {
                        lock (sentCommandsQueue)
                        {
                            var found = false;

                            var thisTime = DateTime.Now; 

                            var selected = new List<ReplyMonitor<O, I>>();
                            var expired = new List<ReplyMonitor<O, I>>();

                            foreach (var cur in sentCommandsQueue)
                            {
                                if (cur.getCommand().getId().Equals(cmd.getId()))
                                {
                                    selected.Add(cur);
                                    found = true;
                                }
                                if ((thisTime - cur.getTime()).Milliseconds > PENDING_COMMAND_TIMEOUT)
                                {
                                    expired.Add(cur);
                                }
                            }
                            foreach (var each in selected)
                            {
                                each.setReply(cmd);
                                sentCommandsQueue.Remove(each);
                            }
                            foreach (var each in expired)
                            {
                                Logger.getLogger(Log.COMMANDS).info("Removing expired reply monitor from queue: " + each);
                                sentCommandsQueue.Remove(each);
                            }
                            if (!found)
                            {
                                Logger.getLogger(Log.COMMANDS).warn("Reply cannot be matched to a sent command: " + cmd + ", commands in progress: " + sentCommandsQueue);
                            }
                        }
                    }
                }
            }
            catch (DisconnectionException ex)
            {
                Logger.getLogger(Log.COMMANDS).debug("Disconnection of peer detected in async processor: " + ex);
            }
            catch (ThreadInterruptedException ex)
            {
                Logger.getLogger(Log.COMMANDS).debug("Async processor interrupted: " + ex);
            }
            catch (SocketException ex)
            {
                Logger.getLogger(Log.COMMANDS).debug("Socket error in async processor", ex);
            }
            catch (IOException ex)
            {
                Logger.getLogger(Log.COMMANDS).debug("I/O error in async processor", ex);
            }
            catch (Exception ex)
            {
                Logger.getLogger(Log.COMMANDS).error("Error in async processor", ex);
            }
            finally
            {
                unsentCommandsQueue.clear();
                sentCommandsQueue.Clear();
                onStop();
            }
        }

        public Boolean isActive()
        {
            return !(unsentCommandsQueue.isEmpty() && sentCommandsQueue.isEmpty());
        }

        public void start()
        {
            thread.Start();
        }

        public void interrupt()
        {
            shouldStop = true;
            thread.Interrupt();
        }
    }
}