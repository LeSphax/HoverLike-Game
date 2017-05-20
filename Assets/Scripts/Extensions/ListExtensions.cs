using System;
using System.Collections.Generic;
using System.Linq;

public static class ListExtensions
{

    public static bool ContainsIndex<T>(this List<T> list, int index)
    {
        return list.ElementAtOrDefault(index) != null;
    }

    public static string PrintContent<T>(this List<T> list)
    {
        string result = "";
        foreach (T elem in list)
        {
            result += elem + ", ";
        }
        return result;
    }

    public static double? Median<TColl, TValue>(
    this IEnumerable<TColl> source,
    Func<TColl, TValue> selector)
    {
        return source.Select<TColl, TValue>(selector).Median();
    }

    public static double? Median<T>(
        this IEnumerable<T> source)
    {
        if (Nullable.GetUnderlyingType(typeof(T)) != null)
            source = source.Where(x => x != null);

        int count = source.Count();
        if (count == 0)
            return null;

        source = source.OrderBy(n => n);

        int midpoint = count / 2;
        if (count % 2 == 0)
            return (Convert.ToDouble(source.ElementAt(midpoint - 1)) + Convert.ToDouble(source.ElementAt(midpoint))) / 2.0;
        else
            return Convert.ToDouble(source.ElementAt(midpoint));
    }

    public static double? Percentile<T>(
        this IEnumerable<T> source, int percentile, int division)
    {
        if (Nullable.GetUnderlyingType(typeof(T)) != null)
            source = source.Where(x => x != null);

        int count = source.Count();
        if (count == 0)
            return null;

        source = source.OrderBy(n => n);

        int midpoint = percentile * count / division;
        if (count % division == 0)
            return (Convert.ToDouble(source.ElementAt(midpoint - 1)) + Convert.ToDouble(source.ElementAt(midpoint))) / 2.0;
        else
            return Convert.ToDouble(source.ElementAt(midpoint));
    }
}
