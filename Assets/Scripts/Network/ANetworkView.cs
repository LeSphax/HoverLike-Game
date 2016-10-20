using Byn.Net;
using System;
using UnityEngine;

public abstract class ANetworkView : MonoBehaviour
{
    [SerializeField]
    private short viewId;
    public short ViewId
    {
        get
        {
            return viewId;
        }
        set
        {
            viewId = value;
        }
    }

    protected virtual void Start()
    {
        if (!registered)
        {
            MyGameObjects.NetworkViewsManagement.RegisterView(this);
        }
    }

    [NonSerialized]
    public bool registered;
    public abstract void ReceiveNetworkMessage(ConnectionId id, NetworkMessage message);
}