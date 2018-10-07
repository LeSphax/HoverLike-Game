using PlayerManagement;
using System.Collections.Generic;
using UnityEngine;

public class MultipleEvents
{
    public Player producer;

    public delegate void EventHandler(Player producer);

    public ListDictionary<PlayerFlags, EventHandler> listeners = new ListDictionary<PlayerFlags, EventHandler>();
    public HashSet<PlayerFlags> changedAttributes = new HashSet<PlayerFlags>();

    public MultipleEvents(Player producer)
    {
        this.producer = producer;
    }

    public void ListenToEvents(EventHandler handler, params PlayerFlags[] observedAttributes)
    {
        observedAttributes.ForEach(att => listeners.AddItem(att, handler));
    }

    public void StopListeningToEvents(EventHandler handler, params PlayerFlags[] observedAttributes)
    {
        observedAttributes.ForEach(att => listeners.RemoveItem(att, handler));
    }

    public void FireEvents()
    {
        HashSet<PlayerFlags> copy = new HashSet<PlayerFlags>(changedAttributes);
        changedAttributes.Clear();

        HashSet<EventHandler> listenersToNotify = new HashSet<EventHandler>();
        copy.ForEach(att =>
        {
            List<EventHandler> handlers;
            if (listeners.TryGetValue(att, out handlers))
                handlers.ForEach(handler => listenersToNotify.Add(handler));
        });

        listenersToNotify.ForEach(listener => listener.Invoke(producer));
    }
}
