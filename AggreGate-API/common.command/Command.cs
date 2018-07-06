using System.Threading;

namespace com.tibbo.aggregate.common.command
{
    using System;

    using System.IO;

    using com.tibbo.aggregate.common.util;

    using JavaCompatibility;

    public class Command
    {
        public const short END_CHAR = 0x0D;
        public const short END_CHAR2 = 0x0A;
        public const short START_CHAR = 0x02;

        public void Write(StreamWrapper wrapper)
        {
            if (isAsync())
            {
                lock (wrapper)
                {
                    wrapper.GetWriteStream().Write(this.stream.GetBuffer(), 0, (int) this.stream.Length);
                }
            }
            else
            {
                lock (wrapper)
                {
                    wrapper.GetWriteStream().Write(this.stream.GetBuffer(), 0, (int) this.stream.Length);
                    wrapper.Flush();
                }
            }
        }

        public string getContent()
        {
                return StringUtils.UTF8_CHARSET.GetString(this.stream.GetBuffer(), 0, (int)this.stream.Length);
        }

        private MemoryStream stream = new MemoryStream();


        public virtual string getId()
        {
            throw new NotSupportedException();
        }

        public void send(StreamWrapper wrapper)
        {
            // wrapper is instance of NetStream in TcpClient
            send(wrapper, true);
        }

        public void send(StreamWrapper wrapper, bool encapsulateBool)
        {
            var cmd = encapsulateBool ? this.encapsulate() : this;
            cmd.Write(wrapper);
        }

        protected virtual Command encapsulate()
        {
            Logger.getLogger(Log.COMMANDS).warn("Command encapsulation must be implemented");
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return StringUtils.UTF8_CHARSET.GetString(this.stream.GetBuffer(), 0, (int)stream.Length);
        }

        public void WriteByte(byte aByte)
        {
            this.stream.WriteByte(aByte);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            this.stream.Write(buffer, offset, count);
        }

        public virtual bool isAsync()
        {
            return false;
        }

        public MemoryStream Stream
        {
            get
            {
                return stream;
            }
        }

    }
}