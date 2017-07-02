using System.Collections.Generic;

public class QueueDictionary<T, R> : Dictionary<T, Queue<R>>
{

    public void AddInQueue(T key, R value)
    {
        Queue<R> queue;
        if (!TryGetValue(key, out queue))
        {
            queue = new Queue<R>();
            Add(key, queue);
        }
        queue.Enqueue(value);
    }

    public R PopQueue(T key)
    {
        Queue<R> queue;
        if (TryGetValue(key, out queue))
        {
            if (queue.Count > 0)
                return queue.Dequeue();
        }
        return default(R);
    }

    public int QueueCount(T key)
    {
        Queue<R> queue;
        if (TryGetValue(key, out queue))
        {
            if (queue.Count > 0)
                return queue.Count;
        }
        return 0;
    }
}
