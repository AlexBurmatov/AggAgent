using System;
using System.Text.RegularExpressions;

namespace JavaCompatibility
{
    public class Pattern
    {
        public static Boolean matches(String pattern, String input)
        {
            return new Regex(pattern).IsMatch(input);
        }
    }
}