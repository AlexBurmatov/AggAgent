using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using com.tibbo.aggregate.common.datatable;

namespace com.tibbo.aggregate.common.util
{
    public class StringUtils
    {
        public static readonly Encoding ISO_8859_1_CHARSET = Encoding.GetEncoding("ISO-8859-1");
        public static readonly Encoding UTF8_CHARSET = Encoding.UTF8;
        public static readonly Encoding ASCII_CHARSET = Encoding.ASCII;

        public const String DEFAULT_COLLECTION_PRINT_SEPARATOR = ", ";

        public static ElementList elements(String source, bool useVisibleSeparators)
        {
            var res = new ElementList();

            var elStart = useVisibleSeparators ? DataTableUtils.ELEMENT_VISIBLE_START : DataTableUtils.ELEMENT_START;
            var elEnd = useVisibleSeparators ? DataTableUtils.ELEMENT_VISIBLE_END : DataTableUtils.ELEMENT_END;
            var elNameValSep = useVisibleSeparators
                                   ? DataTableUtils.ELEMENT_VISIBLE_NAME_VALUE_SEPARATOR
                                   : DataTableUtils.ELEMENT_NAME_VALUE_SEPARATOR;

            var depth = 0;
            var startPos = -1;
            var nameValSepPos = -1;

            var len = source.Length;

            for (var i = 0; i < len; i++)
            {
                var c = source[i];

                if (c == elStart)
                {
                    depth++;

                    if (depth == 1)
                    {
                        startPos = i;
                    }
                }

                if (c == elNameValSep)
                {
                    if (depth == 1 && nameValSepPos == -1)
                    {
                        nameValSepPos = i;
                    }
                }

                if (c != elEnd) continue;
                depth--;

                if (depth < 0)
                {
                    throw new ArgumentException("Invalid closing element at position " + i);
                }

                if (depth != 0) continue;
                String name = null;
                String value;

                if (nameValSepPos == -1)
                {
                    value = source.Substring(startPos + 1, i - (startPos + 1));
                }
                else
                {
                    name = source.Substring(startPos + 1, nameValSepPos - (startPos + 1));
                    value = source.Substring(nameValSepPos + 1, i - (nameValSepPos + 1));
                }

                res.Add(new Element(name, value));

                nameValSepPos = -1;
            }

            if (depth >= 1)
            {
                throw new ArgumentException("Missing closing element(s)");
            }

            return res;
        }

        public static List<string> split(String str, char ch)
        {
            return split(str, ch, -1);
        }

        public static List<String> split(String str, char ch, int limit)
        {
            var res = new List<String>();

            var index = 0;

            var finished = false;

            for (;;)
            {
                var newindex = str.IndexOf(ch, index);

                if (newindex == -1)
                {
                    finished = true;
                    newindex = str.Length;
                }

                res.Add(str.Substring(index, newindex - index));

                if (limit > -1 && res.Count >= limit)
                {
                    res.Add(str.Substring(Math.Min(newindex + 1, str.Length - 1)));
                    break;
                }

                if (finished)
                {
                    break;
                }

                index = newindex + 1;

                if (index != str.Length) continue;
                res.Add("");
                break;
            }

            return res;
        }

        public static String print<T>(IEnumerable<T> col)
        {
            return print(col, DEFAULT_COLLECTION_PRINT_SEPARATOR, false);
        }

        public static String print<T>(IEnumerable<T> col, String separator)
        {
            return print(col, separator, false);
        }

        public static String print<T>(IEnumerable<T> col, String separator, Boolean skipNullElements)
        {
            if (col == null)
            {
                return "null";
            }

            var res = new StringBuilder();
            var i = 0;

            foreach (Object elem in col)
            {
                if (elem == null && skipNullElements)
                {
                    continue;
                }

                if (i > 0)
                {
                    res.Append(separator);
                }
                i++;
                res.Append(elem != null ? elem.ToString() : "null");
            }

            return res.ToString();
        }

        public static String colorToString(Color? c)
        {
            if (c == null)
            {
                return null;
            }
            var color = (Color) c;

            StringBuilder s = new StringBuilder("#");

            s.Append(byteToHexString(color.R));
            s.Append(byteToHexString(color.G));
            s.Append(byteToHexString(color.B));

            return s.ToString();
        }

        public static String byteToHexString(int i)
        {
            var str = (i & 0xFF).ToString("X2", CultureInfo.GetCultureInfo("en"));
            return str;
        }
    }
}