using System;
using System.IO;
using System.Net.Sockets;

namespace JavaCompatibility
{
    public class ByteBuffer : MemoryStream
    {
        public ByteBuffer(int bufferSize) : base (bufferSize)
        {
    
        }

        public void flip()
        {
            Position = 0;
        }

        public void clear()
        {
            Position = 0;
            SetLength(0);
        }

        public int readFrom(Stream reader)
        {
            var buf = new byte[Capacity];

            try
            {
                var read = reader.Read(buf, 0, Capacity);
                Write(buf, 0, read);

                SetLength(read);
                return read;
            }
            catch (IOException ex)
            {
                var socketExept = ex.InnerException as SocketException;
                if (socketExept == null || socketExept.ErrorCode != 10060)
                    throw new IOException (ex.Message);

               return -1; // Read timeout for sync socket
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool hasRemaining()
        {
            return Position < Length;
        }

        public byte get()
        {
            return (byte)ReadByte();
        }
    }
}