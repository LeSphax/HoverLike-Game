﻿using Byn.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public delegate void NetworkPropertyChanged(object previousValue, object newValue);

public class NetworkProperties : ANetworkView
{
    public Dictionary<string, object> properties;
    public Dictionary<string, NetworkPropertyChanged> listeners;


    void Awake()
    {
        Reset();
    }

    internal void Reset()
    {
        listeners = new Dictionary<string, NetworkPropertyChanged>();
        properties = new Dictionary<string, object>();
        Debug.LogWarning("Reset walleh");
    }

    public T GetProperty<T>(string propertyName)
    {
        object property;
        if (properties.TryGetValue(propertyName, out property))
        {
            Assert.IsTrue(typeof(T).IsAssignableFrom(property.GetType()));
            return (T)property;
        }
        return default(T);
    }

    public object GetProperty(string propertyName)
    {
        object property;
        if (properties.TryGetValue(propertyName, out property))
        {
            return property;
        }
        return null;
    }

    public bool TryGetProperty(string propertyName, out object property)
    {
        return properties.TryGetValue(propertyName, out property);
    }

    public bool ContainsKey(string key)
    {
        return properties.ContainsKey(key);
    }

    public void SetProperty(string key, object property)
    {
        Assert.IsTrue(MyComponents.NetworkManagement.isServer);

        object previousValue = null;
        properties.TryGetValue(key, out previousValue);

        properties[key] = property;
        PropertyChanged(key, property, previousValue);
    }

    public void PropertyChanged(string key)
    {
        PropertyChanged(key, properties[key], null);
    }

    private void PropertyChanged(string key, object property, object previousValue)
    {
        MyComponents.NetworkManagement.SendData(ViewId, MessageType.Properties, new PropertiesPacket(key, property).Serialize());
        NotifiyListeners(key, previousValue, property);
    }

    public void SendProperties()
    {
        Debug.Log("SendProperties");
        MyComponents.NetworkManagement.SendData(ViewId, MessageType.Properties, new PropertiesPacket(properties.Keys.ToArray(), properties.Values.ToArray()).Serialize());
    }

    public override void ReceiveNetworkMessage(ConnectionId id, NetworkMessage message)
    {
        Assert.IsTrue(message.type == MessageType.Properties);
        PropertiesPacket packet = NetworkExtensions.Deserialize<PropertiesPacket>(message.data);
        for (int i = 0; i < packet.keys.Length; i++)
        {
            string key = packet.keys[i];
            object value = packet.properties[i];
            Debug.Log("Properties packet " + key + "   " + value);
            object previousValue = null;
            properties.TryGetValue(key, out previousValue);
            properties[key] = value;
            NotifiyListeners(key, previousValue, value);
        }
    }

    private void NotifiyListeners(string key, object previousValue, object newValue)
    {
        NetworkPropertyChanged handler;
        if (listeners.TryGetValue(key, out handler))
        {
            handler.Invoke(previousValue, newValue);
        }
    }

    public void AddListener(string key, NetworkPropertyChanged handler)
    {
        Debug.Log("AddListener" + key);
        if (!listeners.ContainsKey(key))
            listeners.Add(key, handler);
        else
            listeners[key] += handler;
    }
}

[Serializable]
public struct PropertiesPacket
{
    public string[] keys;
    public object[] properties;

    public PropertiesPacket(string key, object property)
    {
        this.keys = new string[1] { key };
        this.properties = new object[1] { property };
    }

    public PropertiesPacket(string[] keys, object[] properties)
    {
        Assert.IsTrue(keys.Length == properties.Length);
        for (int i = 0; i < keys.Length; i++)
        {
            Assert.IsTrue(MyComponents.Properties.GetProperty<object>(keys[i]) == properties[i]);
        }
        this.keys = keys;
        this.properties = properties;
    }
}
