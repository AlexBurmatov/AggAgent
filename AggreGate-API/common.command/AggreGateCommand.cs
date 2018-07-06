using System;
using System.Runtime.CompilerServices;
using com.tibbo.aggregate.common.datatable;

namespace com.tibbo.aggregate.common.command
{
    public class AggreGateCommand : Command
    {
        public const char COMMAND_CODE_MESSAGE = 'M';
        public const char COMMAND_CODE_REPLY = 'R';

        public const char MESSAGE_CODE_START = 'S';
        public const char MESSAGE_CODE_OPERATION = 'O';
        public const char MESSAGE_CODE_EVENT = 'E';

        public const String REPLY_CODE_OK = "A";
        public const String REPLY_CODE_DENIED = "D";
        public const String REPLY_CODE_ERROR = "E";

        public const char COMMAND_OPERATION_GET_VAR = 'G';
        public const char COMMAND_OPERATION_SET_VAR = 'S';
        public const char COMMAND_OPERATION_CALL_FUNCTION = 'C';
        public const char COMMAND_OPERATION_ADD_EVENT_LISTENER = 'L';
        public const char COMMAND_OPERATION_REMOVE_EVENT_LISTENER = 'R';

        public const int INDEX_COMMAND_CODE = 0;
        public const int INDEX_ID = 1;

        public const int INDEX_MESSAGE_CODE = 2;

        public const int INDEX_PROTOCOL_VERSION = 3;

        public const int INDEX_OPERATION_CODE = 3;
        public const int INDEX_OPERATION_CONTEXT = 4;
        public const int INDEX_OPERATION_TARGET = 5;
        public const int INDEX_OPERATION_LISTENER = 6;

        public const int INDEX_EVENT_CONTEXT = 3;
        public const int INDEX_EVENT_NAME = 4;
        public const int INDEX_EVENT_LEVEL = 5;
        public const int INDEX_EVENT_ID = 6;
        public const int INDEX_EVENT_LISTENER = 7;
        public const int INDEX_DATA_TABLE_IN_EVENT_MESSAGE = 8;

        public const int INDEX_DATA_TABLE_IN_OPERATION_MESSAGE = 6;

        public const int INDEX_REPLY_CODE = 2;
        public const int INDEX_REPLY_MESSAGE = 3;
        public const int INDEX_REPLY_DETAILS = 4;
        public const int INDEX_DATA_TABLE_IN_REPLY = 3;

        public static readonly char CLIENT_COMMAND_SEPARATOR = '\u0017';

        private static int generatedId;

        private const int MAX_PRINTED_LENGTH = 1000;

        public override String ToString()
        {
            var s = base.ToString();

            var len = s.Length;

            s = s.Substring(0, Math.Min(MAX_PRINTED_LENGTH, s.Length));

            if (s.Length >= MAX_PRINTED_LENGTH)
            {
                s += "... (" + len + ")";
            }

            s = s.Replace(CLIENT_COMMAND_SEPARATOR, '/');

            s = s.Replace(DataTableUtils.ELEMENT_START, DataTableUtils.ELEMENT_VISIBLE_START);
            s = s.Replace(DataTableUtils.ELEMENT_END, DataTableUtils.ELEMENT_VISIBLE_END);
            s = s.Replace(DataTableUtils.ELEMENT_NAME_VALUE_SEPARATOR,
                          DataTableUtils.ELEMENT_VISIBLE_NAME_VALUE_SEPARATOR);

            s = s.Replace(DataTableUtils.DATA_TABLE_NULL, DataTableUtils.DATA_TABLE_VISIBLE_NULL);

            return s;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static String generateId()
        {
            return (++generatedId).ToString();
        }
    }
}