using Byn.Net;
using System;
using UnityEngine;

public abstract class ANetworkView : MonoBehaviour
{
    public static int nextViewId;
    [HideInInspector]
    public int viewId;

    protected virtual void Start()
    {
        if (!registered)
            MyGameObjects.NetworkManagement.RegisterView(this);
    }

    [NonSerialized]
    public bool registered;
    public abstract void ReceiveNetworkMessage(ConnectionId id, NetworkMessage message);
}