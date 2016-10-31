using System.Collections.Generic;

public class ListDictionary<T, T2> : Dictionary<T, List<T2>>
{
    public void AddItem(T key, T2 item)
    {
        List<T2> list;
        if (!TryGetValue(key, out list))
        {
            list = new List<T2>();
            Add(key, list);
        }
        list.Add(item);
    }

    public int CountList(T key)
    {
        List<T2> list;
        if (TryGetValue(key, out list))
        {
            return list.Count;
        }
        return 0;
    }
}
