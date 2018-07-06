using System;
using System.Collections.Generic;
using System.Text;
using com.tibbo.aggregate.common.datatable;

namespace com.tibbo.aggregate.common.command
{
    using util;

    public static class TransferEncodingHelper
    {
        public const char ESCAPE_CHAR = '%';
        public const char SEPARATOR_CHAR = '/';
        public const char START_CHAR = '^';
        public const char END_CHAR = '$';

        public const int KILO = 1024;
        public const int KB = KILO;
        public const int MB = KB * KILO;
        public const int GB = MB * KILO;
        public const long TB = (long)GB * KILO;

        // TODO: Check all usages of LARGE_DATA_SIZE for improvements (AGG-5534)
        public const int LARGE_DATA_SIZE = MB * 50;


        public static readonly Dictionary<Char, Char?> DIRECT = new Dictionary<char, char?>();
        public static readonly Dictionary<Char, Char?> REVERSE = new Dictionary<char, char?>();

        static TransferEncodingHelper()
        {
            DIRECT.Add(ESCAPE_CHAR, ESCAPE_CHAR);
            DIRECT.Add(AggreGateCommand.CLIENT_COMMAND_SEPARATOR, SEPARATOR_CHAR);
            DIRECT.Add(DataTableUtils.ELEMENT_START, DataTableUtils.ELEMENT_VISIBLE_START);
            DIRECT.Add(DataTableUtils.ELEMENT_END, DataTableUtils.ELEMENT_VISIBLE_END);
            DIRECT.Add(DataTableUtils.ELEMENT_NAME_VALUE_SEPARATOR, DataTableUtils.ELEMENT_VISIBLE_NAME_VALUE_SEPARATOR);
            DIRECT.Add((char)AggreGateCommand.START_CHAR, START_CHAR);
            DIRECT.Add((char)AggreGateCommand.END_CHAR, END_CHAR);

            REVERSE.Add(ESCAPE_CHAR, ESCAPE_CHAR);
            REVERSE.Add(SEPARATOR_CHAR, AggreGateCommand.CLIENT_COMMAND_SEPARATOR);
            REVERSE.Add(DataTableUtils.ELEMENT_VISIBLE_START, DataTableUtils.ELEMENT_START);
            REVERSE.Add(DataTableUtils.ELEMENT_VISIBLE_END, DataTableUtils.ELEMENT_END);
            REVERSE.Add(DataTableUtils.ELEMENT_VISIBLE_NAME_VALUE_SEPARATOR, DataTableUtils.ELEMENT_NAME_VALUE_SEPARATOR);
            REVERSE.Add(START_CHAR, (char)AggreGateCommand.START_CHAR);
            REVERSE.Add(END_CHAR, (char)AggreGateCommand.END_CHAR);

        }

        public static String encode(String strVal, int encodeLevel)
        {
            return encode(strVal, DIRECT, encodeLevel);
        }

        public static String encode(String s, Dictionary<char, char?> mapping, int encodeLevel)
        {
            if (s == null)
            {
                return null;
            }

            StringBuilder result = encode(s, null, mapping, encodeLevel);
            return result.ToString();
        }

        public static StringBuilder encode(string strFrom, StringBuilder strTo, int? encodeLevel)
        {
            return encode(strFrom, strTo, DIRECT, encodeLevel);
        }

        public static StringBuilder encode(string source, StringBuilder result, Dictionary<char, char?> mapping, int? encodeLevel)
        {

            if (source == null)
                return null;

            if (result == null)
                result = new StringBuilder();

            for (int i = 0; i < source.Length; i++)
            {
                encodeChar(mapping, source[i], result, encodeLevel);
            }

            return result;
        }


        public static void encodeChar(char c, StringBuilder sb)
        {
            encodeChar(DIRECT, c, sb, 0);
        }

        public static void encodeChar(Dictionary<char, char?> mapping, char c, StringBuilder sb, int? encodeLevel)
        {
            if (mapping.ContainsKey(c))
            {
                double recalcedEncode;

                if (c == ESCAPE_CHAR)
                    recalcedEncode = Math.Pow(2, (double)encodeLevel) - 1;
                else
                    recalcedEncode = Math.Pow(2, (double)(encodeLevel - 1));

                for (int i = 0; i < recalcedEncode; i++)
                {
                    sb.Append(ESCAPE_CHAR);
                }

                sb.Append((char)mapping[c]);
            }
            else
            {
                sb.Append(c);
            }
        }


        public static String decode(String s)
        {
            if (s == null)
            {
                return null;
            }

            var len = s.Length;

            var output = new StringBuilder(len);

            for (var i = 0; i < len; i++)
            {
                var c = s[i];

                if (c == ESCAPE_CHAR && i < len - 1)
                {
                    var next = s[i + 1];
                    var orig = REVERSE.ContainsKey(next) ? REVERSE[next] : null;

                    if (orig != null)
                    {
                        output.Append(orig);
                        i += 1;
                    }
                    else
                    {
                        // Failover code
                        output.Append(c);
                        output.Append(next);
                    }
                }
                else
                {
                    output.Append(c);
                }
            }

            return output.ToString();
        }
    }
}