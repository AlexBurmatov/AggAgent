using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using com.tibbo.aggregate.common.util;

namespace com.tibbo.aggregate.common.command
{
    public class IncomingAggreGateCommand : AggreGateCommand
    {
        private const String EMPTY_ID = "";

        protected List<String> parameters;

        public IncomingAggreGateCommand(CommandBuffer c)
        {
            Write(c.ToArray(), 0, c.ToArray().Length);

            c.clearCommand();
            parse();
        }

        public IncomingAggreGateCommand(String str)
        {
            Write(Encoding.Default.GetBytes(str), 0, Encoding.Default.GetBytes(str).Length);
            parse();
        }

        public IncomingAggreGateCommand(MemoryStream s)
        {
            Write(s.ToArray(), 0, (int) s.Length);
            parse();
        }

        protected void parse()
        {
            if (getContent().Length == 0)
            {
                throw new SyntaxErrorException("Zero length command received");
            }

            parameters = StringUtils.split(getContent(), CLIENT_COMMAND_SEPARATOR);
        }

        public int getNumberOfParameters()
        {
            return parameters != null ? parameters.Count : 0;
        }

        public String getParameter(int number)
        {
            if (number > getNumberOfParameters())
            {
                throw new InvalidOperationException("Trying to access parameter #" + number + " of command that has only " + getNumberOfParameters() + " parameters");
            }
            return parameters[number];
        }

        public List<String> getParameters(int number)
        {
            if (number != getNumberOfParameters())
            {
                throw new InvalidOperationException("Error getting command parameters: command has " + getNumberOfParameters() + " parameters, should have " + number);
            }

            return parameters;
        }

        public Boolean isReply()
        {
            if (getParameter(0).Length > 1)
            {
                throw new InvalidOperationException("Invalid command type: " + getParameter(0));
            }

            return (getParameter(0)[0] == COMMAND_CODE_REPLY);
        }

        public Boolean isMessage()
        {
            if (getParameter(0).Length > 1)
            {
                throw new InvalidOperationException("Invalid command type: " + getParameter(0));
            }

            return (getParameter(0)[0] == COMMAND_CODE_MESSAGE);
        }

        public String getReplyCode()
        {
            if (!isReply())
            {
                throw new NotSupportedException("Command is not a reply");
            }

            return getParameter(INDEX_REPLY_CODE);
        }

        public String getMessageCode()
        {
            if (!isMessage())
            {
                throw new NotSupportedException("Command is not a message");
            }

            return getParameter(INDEX_MESSAGE_CODE);
        }

        public String getEncodedDataTable(int index)
        {
            return getParameter(index);
        }

        public String getEncodedDataTableFromReply()
        {
            if (!isReply())
            {
                throw new NotSupportedException("Command is not reply");
            }

            return getEncodedDataTable(INDEX_DATA_TABLE_IN_REPLY);
        }

        public String getEncodedDataTableFromOperationMessage()
        {
            if (!isMessage())
            {
                throw new NotSupportedException("Command is not message");
            }

            return getEncodedDataTable(INDEX_DATA_TABLE_IN_OPERATION_MESSAGE);
        }

        public String getEncodedDataTableFromEventMessage()
        {
            if (!isMessage())
            {
                throw new NotSupportedException("Command is not message");
            }

            return getEncodedDataTable(INDEX_DATA_TABLE_IN_EVENT_MESSAGE);
        }

        public override String getId()
        {
            return getNumberOfParameters() > INDEX_ID ? getParameter(INDEX_ID) : EMPTY_ID;
        }
    }
}