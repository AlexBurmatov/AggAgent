using System;
using System.Threading;
using JavaCompatibility;

namespace com.tibbo.aggregate.common.command
{
    public class ReplyMonitor<C, R> where C : Command where R : Command
    {
        private readonly C command;
        private R reply;
        private readonly DateTime time;

        private Boolean timeoutWasReset;

        public ReplyMonitor(C command)
        {
            this.command = command;
            time = DateTime.Now;
        }

        public C getCommand()
        {
            return command;
        }

        public R getReply()
        {
            return reply;
        }

        public void setReply(R replyR)
        {
            Monitor.Enter(this);

            try
            {
                reply = replyR;
                Monitor.PulseAll(this);
            }
            finally
            {
                Monitor.Exit(this);
            }
            Logger.getLogger(Log.COMMANDS).debug("Command replied in " + (DateTime.Now - time).Milliseconds +
                                                 " ms: command '" + command + "', replyR '" + replyR + "'");
        }

        public void resetTime()
        {
            Monitor.Enter(this);
            try
            {
                timeoutWasReset = true;
                Monitor.PulseAll(this);
            }
            finally
            {
                Monitor.Exit(this);
            }
        }

        public Boolean waitReply(long timeout)
        {
            Monitor.Enter(this);
            try
            {
                do
                {
                    if (reply != null)
                    {
                        return true;
                    }
                    timeoutWasReset = false;
                    Monitor.Wait(this, TimeSpan.FromMilliseconds(timeout));
                } while (timeoutWasReset);
            }
            finally
            {
                Monitor.Exit(this);
            }

            return reply != null;
        }

        public DateTime getTime()
        {
            return time;
        }

        public override String ToString()
        {
            return "ReplyMonitor [command: " + command + ", reply: " + reply + "]";
        }
    }
}