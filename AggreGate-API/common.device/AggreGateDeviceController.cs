using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using com.tibbo.aggregate.common.command;
using com.tibbo.aggregate.common.context;
using com.tibbo.aggregate.common.datatable;
using JavaCompatibility;

namespace com.tibbo.aggregate.common.device
{
    public abstract class AggreGateDeviceController : CommandBufferListener
    {
        private Boolean connected;

        private Boolean connecting;

        private Boolean loggedIn;

        private Boolean loggingIn;

        private CommandBuffer commandBuffer;
        private AsyncDeviceCommandProcessor processor;

        private AggreGateDevice device;

        private RemoteDeviceContextManager<AggreGateDevice> contextManager;

        private readonly long commandTimeout;
        private Boolean resetTimeoutsOnData;
        private Boolean avoidSendingFormats;

        private readonly FormatCache formatCache;

        protected AggreGateDeviceController(AggreGateDevice device, long commandTimeout)
        {
            formatCache = new FormatCache(this);
            this.device = device;
            this.commandTimeout = commandTimeout;
        }

        public RemoteDeviceContextManager<AggreGateDevice> getContextManager()
        {
            return contextManager;
        }

        public void setContextManager(RemoteDeviceContextManager<AggreGateDevice> aContextManager)
        {
            contextManager = aContextManager;
        }

        public void setDevice(AggreGateDevice aDevice)
        {
            device = aDevice;
        }

        protected void setCommandBuffer(CommandBuffer aCommandBuffer)
        {
            commandBuffer = aCommandBuffer;
            aCommandBuffer.setListener(this);
        }

        public AggreGateDevice getDevice()
        {
            return device;
        }

        public FormatCache getFormatCache()
        {
            return formatCache;
        }

        public ClassicEncodingSettings createClassicEncodingSettings(Boolean forSending)
        {
            var es = new ClassicEncodingSettings(false);
            if (!forSending)
            {
                es.setFormatCache(formatCache);
            }
            es.setEncodeFormat(!avoidSendingFormats);
            return es;
        }

        public Boolean isConnected()
        {
            return connected;
        }

        public Boolean isLoggedIn()
        {
            return loggedIn;
        }

        protected void setConnected(Boolean connectedBoolean)
        {
            connected = connectedBoolean;
        }

        public void setLoggedIn(Boolean loggedInBoolean)
        {
            loggedIn = loggedInBoolean;
        }

        public void setResetTimeoutsOnData(Boolean resetTimeoutWhenDataReceived)
        {
            resetTimeoutsOnData = resetTimeoutWhenDataReceived;
        }

        protected void setAvoidSendingFormats(Boolean avoidSendingFormatsBoolean)
        {
            avoidSendingFormats = avoidSendingFormatsBoolean;
        }

        public Boolean isAvoidSendingFormats()
        {
            return avoidSendingFormats;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void connect()
        {
            if (isConnected() || connecting)
            {
                return;
            }

            try
            {
                connecting = true;
                if (connectImpl())
                {
                    setConnected(true);
                }
            }
            finally
            {
                connecting = false;
            }
        }

        protected virtual Boolean connectImpl()
        {
            processor = new AsyncDeviceCommandProcessor(this, device, commandTimeout);

            processor.start();

            var ans = sendCommand(ClientCommandUtils.startMessage());

            if (!ans.getReplyCode().Equals(AggreGateCommand.REPLY_CODE_OK))
            {
                throw new RemoteDeviceErrorException(Cres.get().getString("devUncompatibleVersion"));
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void login()
        {
            if (isLoggedIn() || loggingIn)
            {
                return;
            }

            try
            {
                loggingIn = true;
                if (loginImpl())
                {
                    setLoggedIn(true);
                }
            }
            finally
            {
                loggingIn = false;
            }
        }

        protected abstract Boolean loginImpl();

        public abstract void start();

        public void destroy()
        {
        }

        protected abstract void send(OutgoingAggreGateCommand cmd);

        public void newDataReceived()
        {
            if (resetTimeoutsOnData)
            {
                processor.resetSentCommandTimeouts();
            }
        }

        protected void received(IncomingAggreGateCommand cmd)
        {
        }

        public IncomingAggreGateCommand sendCommand(OutgoingAggreGateCommand cmd)
        {
            connect();
            return processor.sendSyncCommand(cmd);
        }

        public IncomingAggreGateCommand sendCommandAndCheckReplyCode(OutgoingAggreGateCommand cmd)
        {
            var ans = sendCommand(cmd);

            if (ans.getReplyCode().Equals(AggreGateCommand.REPLY_CODE_DENIED))
            {
                throw new ContextSecurityException(String.Format(Cres.get().getString("devAccessDeniedReply"),
                                                                 ans.getReplyCode(), cmd));
            }

            if (ans.getReplyCode().Equals(AggreGateCommand.REPLY_CODE_ERROR))
            {
                var message = ans.getNumberOfParameters() > AggreGateCommand.INDEX_REPLY_MESSAGE
                                  ? ": " +
                                    DataTableUtils.transferDecode(ans.getParameter(AggreGateCommand.INDEX_REPLY_MESSAGE))
                                  : "";
                var details = ans.getNumberOfParameters() > AggreGateCommand.INDEX_REPLY_DETAILS
                                  ? DataTableUtils.transferDecode(ans.getParameter(AggreGateCommand.INDEX_REPLY_DETAILS))
                                  : null;
                throw new RemoteDeviceErrorException(Cres.get().getString("devServerReturnedError") + message, details);
            }

            if (!ans.getReplyCode().Equals(AggreGateCommand.REPLY_CODE_OK))
            {
                throw new RemoteDeviceErrorException(Cres.get().getString("devServerReturnedError") + ": " +
                                                     ans + " (error code: '" + ans.getReplyCode() + "')");
            }

            return ans;
        }

        public Boolean isActive()
        {
            if (connecting || loggingIn)
            {
                return true;
            }
            return processor != null && processor.isActive();
        }

        public virtual void disconnect()
        {
            setLoggedIn(false);

            setConnected(false);

            if (processor != null)
            {
                processor.interrupt();
            }
            if (getContextManager() != null)
            {
                getContextManager().stop();
            }
        }

        public class AsyncDeviceCommandProcessor : AsyncCommandProcessor<IncomingAggreGateCommand, OutgoingAggreGateCommand>
        {
            private AggreGateDeviceController owner;

            public AsyncDeviceCommandProcessor(AggreGateDeviceController aDeviceController,
                                               AggreGateDevice device, Int64 commandTimeout)
                : base(device.ToString(), commandTimeout, true)
            {
                owner = aDeviceController;
            }

            protected override void sendCommandImplementation(OutgoingAggreGateCommand cmd)
            {
                try
                {
                    owner.send(cmd);
                }
                catch (ThreadInterruptedException ex)
                {
                    throw new IOException("", ex);
                }
            }

            protected override IncomingAggreGateCommand receiveCommandImplementation()
            {
                owner.commandBuffer.readCommand();
                if (owner.commandBuffer.isFull())
                {
                    var cmd = new IncomingAggreGateCommand(owner.commandBuffer);
                    owner.received(cmd);
                    return cmd;
                }
                return null;
            }

            protected override Boolean isAsync(IncomingAggreGateCommand cmd)
            {
                return cmd.isMessage();
            }

            protected override void processAsyncCommand(IncomingAggreGateCommand cmd)
            {
                Logger.getLogger(Log.COMMANDS).debug("Async command received from server: " + cmd);

                var icmd = cmd;
                if (icmd.getMessageCode()[0] == AggreGateCommand.MESSAGE_CODE_EVENT)
                {
                    ThreadPool.QueueUserWorkItem(state =>
                        {
                            if (!owner.isConnected())
                            {
                                return;
                            }

                            try
                            {
                                var contextName = icmd.getParameter(AggreGateCommand.INDEX_EVENT_CONTEXT);
                                var eventName = icmd.getParameter(AggreGateCommand.INDEX_EVENT_NAME);

                                var level = Int32.Parse(icmd.getParameter(AggreGateCommand.INDEX_EVENT_LEVEL));

                                var idstr = icmd.getParameter(AggreGateCommand.INDEX_EVENT_ID);
                                var id = idstr.Length > 0 ? (Int64?)Int64.Parse(idstr) : null;

                                var listenerstr = icmd.getParameter(AggreGateCommand.INDEX_EVENT_LISTENER);
                                var listener = listenerstr.Length > 0 ? (Int32?)Int32.Parse(listenerstr) : null;

                                var con = this.owner.getContextManager().get(contextName);

                                if (con == null)
                                {
                                    Log.CONTEXT_EVENTS.info("Error firing event '" + eventName + "': context '" + contextName + "' not found");
                                    return;
                                }

                                var ed = con.getEventDefinition(eventName);

                                if (ed == null)
                                {
                                    Log.CONTEXT_EVENTS.warn("Error firing event: event '" + eventName +
                                                                              "' not found in context '" + contextName + "'");
                                    return;
                                }

                                var data = con.decodeRemoteDataTable(ed.getFormat(), icmd.getEncodedDataTableFromEventMessage());

                                con.fireEvent(ed.getName(), data, level, id, listener, null, null);
                            }
                            catch (Exception ex)
                            {
                                Logger.getLogger(Log.COMMANDS).error("Error processing async command '" + icmd + "'", ex);
                            }
                        });
                }
            }

            protected override void onStop()
            {
            }
        }
    }
}