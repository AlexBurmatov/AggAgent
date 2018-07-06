using System;
using System.Collections.Generic;
using System.Text;
using com.tibbo.aggregate.common.context;
using com.tibbo.aggregate.common.datatable;
using com.tibbo.aggregate.common.server;
using com.tibbo.aggregate.common.util;
using com.tibbo.aggregate.common.command;

namespace com.tibbo.aggregate.common.data
{
    public class Data : ICloneable, StringEncodable
    {
        private Int64? id;
        private String name;
        private byte[] preview;
        private byte[] data;

        private const int TRANSCODER_VERSION = 0;
        public const float BUFFER_MULTIPLIER = 1.15f;

        private const char SEPARATOR = '/';

        private Dictionary<String, Object> attachments = new Dictionary<String, Object>();

        public Data()
        {
        }

        public Data(byte[] data)
        {
            this.data = data;
        }

        public Data(String name, byte[] data) : this(data)
        {
            this.name = name;
        }

        public void setPreview(byte[] bytes)
        {
            preview = bytes;
        }

        public void setId(Int64? idLong)
        {
            id = idLong;
        }

        public void setData(byte[] bytes)
        {
            data = bytes;
        }

        public void setName(String aString)
        {
            name = aString;
        }

        public byte[] getPreview()
        {
            return preview;
        }

        public String getName()
        {
            return name;
        }

        public Int64? getId()
        {
            return id;
        }

        public byte[] getData()
        {
            return data;
        }

        public Dictionary<String, Object> getAttachments()
        {
            return attachments;
        }

        public byte[] fetchData(ContextManager cm, CallerController<CallerData> cc)
        {
            if (data != null)
            {
                return data;
            }

            if (id == null)
            {
                return null;
            }

            if (cm == null)
            {
                return null;
            }

            var dt = cm.get(ContextUtils.CTX_UTILS, cc).callFunction(UtilsContextConstants.F_GET_DATA, cc, id);

            var receivedData =
                StringUtils.ASCII_CHARSET.GetBytes(dt.rec().getString(UtilsContextConstants.FOF_GET_DATA_DATA));
            setData(receivedData);

            return receivedData;
        }

        private static int checksum(IEnumerable<byte> bytes)
        {
            var sum = 0;
            foreach (var b in bytes)
            {
                sum += b;
            }
            return sum;
        }

        public String toDetailedString()
        {
            return "Data [id: " + (id != null ? id.ToString() : "null") + ", name: " + (name ?? "null") +
                   ", preview: "
                   + (preview != null ? "len=" + preview.Length + " checksum=" + checksum(preview) : "null") +
                   ", data: " + (data != null ? "len=" + data.Length + " checksum=" + checksum(data) : "null") + "]";
        }

        public override String ToString()
        {
            return "Data [id: " + (id != null ? id.ToString() : "null") + ", name: " + (name ?? "null") +
                   ", preview: " + (preview != null ? "len=" + preview.Length : "null") + ", data: "
                   + (data != null ? "len=" + data.Length : "null") + "]";
        }

        public object Clone()
        {
            var cl = (Data) MemberwiseClone();
            cl.preview = (byte[]) CloneUtils.deepClone(preview);
            cl.data = (byte[]) CloneUtils.deepClone(data);
            return cl;
        }

        public override Boolean Equals(Object obj)
        {
            if (obj is Data)
            {
                var od = (Data) obj;
                return Util.equals(id, od.id) && Util.equals(name, od.name) && Util.equals(od.preview, preview) &&
                       Util.equals(data, od.data);
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = (id.HasValue ? id.Value.GetHashCode() : 0);
                result = (result*397) ^ (name != null ? name.GetHashCode() : 0);
                result = (result*397) ^ (preview != null ? preview.GetHashCode() : 0);
                result = (result*397) ^ (data != null ? data.GetHashCode() : 0);
                result = (result*397) ^ (attachments != null ? attachments.GetHashCode() : 0);
                return result;
            }
        }

        public void setAttachments(Dictionary<String, Object> attachmentsDictionary)
        {
            attachments = attachmentsDictionary;
        }

        public String encode()
        {
            return encode(new StringBuilder(), null, false, 0).ToString();
        }

        public StringBuilder encode(StringBuilder sb, ClassicEncodingSettings settings, bool isTransferEncode, int encodeLevel)
        {
            if (sb.Length + (estimateDataSize() * BUFFER_MULTIPLIER) > sb.Capacity)
                sb.EnsureCapacity((int)(sb.Capacity + (estimateDataSize() * BUFFER_MULTIPLIER)));

            StringBuilder tempSB = new StringBuilder();

            tempSB.Append(TRANSCODER_VERSION.ToString());

            tempSB.Append(SEPARATOR);

            tempSB.Append(getId() != null ? getId().ToString() : DataTableUtils.DATA_TABLE_NULL);

            tempSB.Append(SEPARATOR);

            tempSB.Append(getName() != null ? getName() : DataTableUtils.DATA_TABLE_NULL);

            tempSB.Append(SEPARATOR);

            tempSB.Append(getPreview() != null ? getPreview().Length.ToString() : "-1");

            tempSB.Append(SEPARATOR);

            tempSB.Append(getData() != null ? getData().Length.ToString() : "-1");

            tempSB.Append(SEPARATOR);

            if (isTransferEncode)
                sb.Append(DataTableUtils.transferEncode(tempSB.ToString()));
            else
                sb.Append(tempSB);

            appendBytes(sb, getPreview(), isTransferEncode, encodeLevel);
            appendBytes(sb, getData(), isTransferEncode, encodeLevel);

            return sb;
        }

        private void appendBytes(StringBuilder sb, byte[] data, bool isTransferEncode, int encodeLevel)
        {
            if (data != null)
            {
                int newLength = (int)((data.Length + sb.Length) * BUFFER_MULTIPLIER);

                sb.EnsureCapacity(newLength);

                for (int i = 0; i <= data.Length; i += TransferEncodingHelper.LARGE_DATA_SIZE)
                {
                    int end = i + TransferEncodingHelper.LARGE_DATA_SIZE;
                    if (end > data.Length)
                        end = data.Length;

                    byte []sub = SubArray(data, i, end);
                    String tempString = System.Text.Encoding.ASCII.GetString(sub); ;
                    if (isTransferEncode)
                        TransferEncodingHelper.encode(tempString, sb, encodeLevel);
                    else
                        sb.Append(tempString);
                }
            }
        }

        public byte[] SubArray(byte[] data, int index, int length)
        {
            byte[] result = new byte[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public int estimateDataSize()
        {
            int size = long.MaxValue.ToString().Length;
            if (getName() != null)
                size += getName().Length;
            if (getPreview() != null)
                size += getPreview().Length + getPreview().Length.ToString().Length;
            if (getData() != null)
                size += getData().Length + getData().Length.ToString().Length;
            return size;
        }
    }
}