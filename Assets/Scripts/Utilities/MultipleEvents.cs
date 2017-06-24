using System;
using System.Collections.Generic;
using System.Reflection;


public class MultipleEvents<T>
{
    public T producer;

    public delegate void EventHandler(T producer);

    public Dictionary<TypeAttributes, EventHandler> listeners = new Dictionary<TypeAttributes, EventHandler>();

    public MultipleEvents(T producer)
    {
        this.producer = producer;
    }

    public void ListenToEvents(EventHandler handler, params TypeAttributes[] observedAttributes)
    {
        observedAttributes.ForEach(att => listeners.Add(att, handler));
    }

    public void FireEvent(params TypeAttributes[] changedAttributes)
    {
        HashSet<EventHandler> listenersToNotify = new HashSet<EventHandler>();
        changedAttributes.ForEach(att => listenersToNotify.Add(listeners[att]));
        listenersToNotify.ForEach(listener => listener.Invoke(producer));
    }
}
