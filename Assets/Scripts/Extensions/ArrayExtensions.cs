using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class ArrayExtensions
{
    public delegate void Mapper<T>(T element);

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
        array1.CopyTo(result, 0);
        array2.CopyTo(result, array1.Length);
        return result;
    }


    public static string Print<T>(this T[] array)
    {
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < array.Length; i++)
        {
            builder.Append(array[i]);
        }
        return builder.ToString();
    }

    public static T[] SubArray<T>(this T[] data, int index, int length)
    {
        T[] result = new T[length];
        Array.Copy(data, index, result, 0, length);
        return result;
    }

    public static void Map<T>(this T[] array, Mapper<T> func)
    {
        for (int i = 0; i < array.Length; i++)
        {
            func(array[i]);
        }
    }
}
