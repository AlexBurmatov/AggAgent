using System;
using System.IO;
using com.tibbo.aggregate.common.device;
using com.tibbo.aggregate.common.util;
using JavaCompatibility;

namespace com.tibbo.aggregate.common.command
{
    public class CommandBuffer : MemoryStream
    {
        private const int BUFFER_SIZE = 1024;

        private short startChar;
        private short endChar;
        private readonly short endChar2;
        private readonly Boolean needBoth;
        private readonly Boolean waitEndChar2Enabled;

        private Boolean started;
        private Boolean waitingEndChar2;
        private StreamWrapper streamWrapper;
        private Boolean full;

        private readonly ByteBuffer buffer = new ByteBuffer(BUFFER_SIZE);

        private CommandBufferListener listener;

        public CommandBuffer(StreamWrapper wrapper, short startChar, short endChar)
        {
            init(wrapper, startChar, endChar);
        }

        public CommandBuffer(StreamWrapper wrapper, short startChar, short endChar, short endChar2, Boolean needBoth)
        {
            init(wrapper, startChar, endChar);
            this.endChar2 = endChar2;
            this.needBoth = needBoth;
            waitEndChar2Enabled = true;
        }

        private void init(StreamWrapper wrapper, short startCharShort, short endCharShort)
        {
            streamWrapper = wrapper;
            startChar = startCharShort;
            endChar = endCharShort;

            buffer.flip();
            clearCommand();
        }

        public void clearCommand()
        {
            started = false;
            waitingEndChar2 = false;
            full = false;
            reset();
        }

        private void reset()
        {
            Position = 0;
            SetLength(0);
        }

        public void readCommand()
        {
            if (processBufferContents())
            {
                return;
            }

            while (true)
            {
                buffer.clear();

                try
                {
                    var read = buffer.readFrom(streamWrapper.GetReadStream());

                    buffer.flip();

                    if (read > 0)
                    {
                        if (listener != null)
                        {
                            listener.newDataReceived();
                        }
                    }
                    else if (read == 0)
                    {
                        streamWrapper.Close();
                        throw new DisconnectionException("Peer disconnection detected while reading command");
                    }
                }
                catch (Exception ex)
                {
                    throw new DisconnectionException(ex.Message);
                }
               
                if (processBufferContents())
                {
                    return;
                }
            }
        }

        private Boolean processBufferContents()
        {
            while (buffer.hasRemaining())
            {
                byte cur = buffer.get();

                if (processByte(cur))
                {
                    return true;
                }
            }

            return false;
        }

        private Boolean processByte(byte cur)
        {
            if (cur == startChar)
            {
                started = true;
                reset();
            }
            else
            {
                if (started)
                {
                    if (waitingEndChar2)
                    {
                        if (cur == endChar2)
                        {
                            full = true;
                            return true;
                        }
                    }
                    else
                    {
                        if (cur == endChar || (waitEndChar2Enabled && !needBoth && cur == endChar2))
                        {
                            if (waitEndChar2Enabled && needBoth)
                            {
                                waitingEndChar2 = true;
                                full = false;
                                return false;
                            }
                            full = true;
                            return true;
                        }
                        WriteByte(cur);
                    }
                }
            }

            return false;
        }

        public Boolean isFull()
        {
            return full;
        }

        public void setListener(CommandBufferListener aListener)
        {
            listener = aListener;
        }
    }
}