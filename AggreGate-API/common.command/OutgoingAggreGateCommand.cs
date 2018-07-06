using System;

using com.tibbo.aggregate.common.datatable;
using com.tibbo.aggregate.common.util;

namespace com.tibbo.aggregate.common.command
{
    using System.IO;
    using System.Text;

    public class OutgoingAggreGateCommand : AggreGateCommand
    {
        private bool async;

        public OutgoingAggreGateCommand()
        {
            
        }

        private OutgoingAggreGateCommand(byte[] bytes, int offset, int count)
        {
            this.WriteByte((byte)START_CHAR);
            this.Write(bytes, offset, count);
            this.WriteByte((byte)END_CHAR);
        }

        protected override Command encapsulate()
        {
            var enc = new OutgoingAggreGateCommand();
            enc.WriteByte((byte)START_CHAR);
            enc.Write(this.Stream.GetBuffer(), 0, (int)this.Stream.Length);
            enc.WriteByte((byte)END_CHAR);

            enc.setAsync(isAsync());
            return enc;
        }
        

        public void addParam(String param)
        {
            if (paramCount != 0)
            {
                WriteByte(CLIENT_COMMAND_SEPARATOR_BYTE);
            }

            if (paramCount == INDEX_ID)
            {
                id = param;
            }

            var buffer = StringUtils.UTF8_CHARSET.GetBytes(param);
            Write(buffer, 0, buffer.Length);
            
            paramCount++;
        }

        public override String getId()
        {
            return id;
        }

        public void constructReply(String idString, String code)
        {
            if (paramCount > 0)
            {
                throw new InvalidOperationException("Can't construct reply - parameters already added to command");
            }

            addParam(COMMAND_CODE_REPLY.ToString());
            addParam(idString);
            addParam(code);
        }

        public void constructReply(String idString, String code, String message)
        {
            constructReply(idString, code);
            addParam(DataTableUtils.transferEncode(message));
        }

        public void constructReply(String idString, String code, String message, String details)
        {
            constructReply(idString, code, message);
            addParam(DataTableUtils.transferEncode(details));
        }

        public void constructEvent(String context, String name, Int32? level, String encodedDataTable, Int64? eventId, DateTime? creationtime, Int32? listener)
        {
            this.id = string.Empty;

            this.setAsync(true);

            this.addParam(Convert.ToString(AggreGateCommand.COMMAND_CODE_MESSAGE));
            this.addParam(this.id);
            this.addParam(Convert.ToString(AggreGateCommand.MESSAGE_CODE_EVENT));
            this.addParam(context);
            this.addParam(name);
            this.addParam(Convert.ToString(level));
            this.addParam(eventId != null ? eventId.ToString() : string.Empty);
            this.addParam(listener != null ? listener.ToString() : string.Empty);
            this.addParam(encodedDataTable);
            this.addParam(creationtime != null ? Convert.ToString((long)((DateTime)creationtime - new DateTime(1970, 1, 1)).TotalMilliseconds) : string.Empty);
        }

        protected virtual ClassicEncodingSettings createClassicEncodingSettings(Boolean useFormatCache)
        {
            return new ClassicEncodingSettings(false);
        }


        private int paramCount;

        private String id;

        static readonly byte CLIENT_COMMAND_SEPARATOR_BYTE = Convert.ToByte(CLIENT_COMMAND_SEPARATOR);

        public void setAsync(bool async)
        {
            this.async = async;
        }

        public override bool isAsync()
        {
            return async;
        }
    }
}