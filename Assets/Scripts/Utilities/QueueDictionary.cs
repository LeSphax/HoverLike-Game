using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class QueueDictionary<T, R> : Dictionary<T, Queue<R>>
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
            return queue.Dequeue();
        }
        return default(R);
    }
}
