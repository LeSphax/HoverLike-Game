using System;
using System.Collections.Generic;
using UnityEngine;

public static class StringExtensions
{
    public static string RemoveWhitespace(this string str)
    {
        return string.Join("", str.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
    }

}
