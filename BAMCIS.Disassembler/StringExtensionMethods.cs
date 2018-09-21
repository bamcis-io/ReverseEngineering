using System;
using System.Collections.Generic;
using System.Text;

namespace BAMCIS.Disassembler
{
    public static class StringExtensionMethods
    {
        public static string Tail(this string source, int tailLength)
        {
            if (tailLength >= source.Length)
                return source;
            return source.Substring(source.Length - tailLength);
        }
    }
}
