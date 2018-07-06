using System;
using System.Text;
using com.tibbo.aggregate.common.context;

namespace com.tibbo.aggregate.common.util
{
    public class Util
    {
        public static bool equals(Object o1, Object o2)
        {
            if (o1 == null)
            {
                return o2 == null;
            }
            return o1.Equals(o2);
        }

        public static bool equals(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
                return false;
            for (var i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                    return false;
            }
            return true;
        }

        public static String descriptionToName(String value)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                sb.Append(ContextUtils.isValidContextNameChar(c) ? c : '_');
            }
            return sb.ToString();
        }
    }
}
