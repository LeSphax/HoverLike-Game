using System;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayExtensions
{
    public static T[] Concatenate<T>(T[] array1, T[] array2)
    {
        if (array1.Length == 0)
        {
            return array2;
        }
        if (array2.Length == 0)
        {
            return array1;
        }
        var result = new T[array1.Length + array2.Length];
        result.CopyTo(array1, 0);
        result.CopyTo(array2, array1.Length);
        return result;
    }

}
