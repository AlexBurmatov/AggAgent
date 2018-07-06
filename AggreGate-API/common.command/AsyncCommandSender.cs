using com.tibbo.aggregate.common.util;

namespace com.tibbo.aggregate.common.command
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public class AsyncCommandSender
    {
        private readonly BlockingCollection<SendCommandTask> _tasks = new BlockingCollection<SendCommandTask>(1000000);

        private readonly Timer timer;

        public AsyncCommandSender()
        {
            var senderThread = new Thread(this.Run)
            {
                IsBackground = true,
                Name = "Async Command Sender"
            };
            senderThread.Start();

            this.timer = new Timer(
                x =>
                    {
                        Console.WriteLine("Command Sender Queue Size: " + this._tasks.Count);
                    },
                null, 0, 5000);

            Stopped = false;
        }

        public bool Stopped { get; set; }

        public void Run()
        {
            foreach (var task in this._tasks.GetConsumingEnumerable())
            {
                task.Execute();
            }
        }

        public void SendAsync(Command cmd, StreamWrapper wrapper)
        {
            new SendCommandTask(cmd, wrapper, this).Execute();
        }

        public void SendInstantly(Command cmd, StreamWrapper wrapper)
        {
            new SendCommandTask(cmd, wrapper, this).Execute();
            wrapper.Flush();
        }

        public Timer Timer()
        {
            return this.timer;
        }

        private class SendCommandTask
        {
            public SendCommandTask(Command aCommand, StreamWrapper wrapper, AsyncCommandSender anAsyncCommandSender)
            {
                this._command = aCommand;
                this._wrapper = wrapper;
                this._sender = anAsyncCommandSender;
            }

            public void Execute()
            {
                if (_sender.Stopped)
                {
                    return;
                }

                _command.Write(_wrapper);
            }

            private readonly Command _command;
            private readonly StreamWrapper _wrapper;
            private AsyncCommandSender _sender;

        }
    }
}