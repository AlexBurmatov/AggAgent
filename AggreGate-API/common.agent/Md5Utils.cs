using System;
using System.Text;
using System.Security.Cryptography;

namespace com.tibbo.aggregate.common.agent
{
    public class Md5Utils
    {
        public static String hexHash(String source)
        {
            return hexRepresentation(getMessageDigest().ComputeHash(Encoding.Default.GetBytes(source)));
        }

        public static String hexRepresentation(byte[] bytes)
        {
            var strHash = String.Empty;
            foreach (var b in bytes)
            {
                strHash += b.ToString("X2");
            }
            return strHash;
        }

        public static MD5CryptoServiceProvider getMessageDigest()
        {
            return new MD5CryptoServiceProvider();
        }
    }
}