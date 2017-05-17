using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Utilities
{
    public static string ToUpperFirstChar(this string a)
    {
        a = a.Substring(0, 1).ToUpper() + a.Substring(1);
        return a;
    }

    public static string ToLowerFirstChar(this string a)
    {
        a = a.Substring(0, 1).ToLower() + a.Substring(1);
        return a;
    }
}
