using System.IO;

namespace com.tibbo.aggregate.common.util
{
    public class StreamWrapper
    {
        private readonly Stream stream;
        private readonly BufferedStream bs;

        public StreamWrapper(Stream stream, int bufferSize)
        {
            this.stream = stream;

            if (bufferSize > 0)
                bs = new BufferedStream(stream, bufferSize);
            else
                bs = null;
        }

        public System.IO.Stream GetReadStream()
        {
            return stream;
        }

        public System.IO.Stream GetWriteStream()
        {
            if (bs != null) return bs;
            return stream;
        }

        public void Close()
        {
            if (bs != null) bs.Close();
            stream.Close();
        }

        public void Flush()
        {
            if (bs != null) bs.Flush();
            else stream.Flush();
        }
    }
}
