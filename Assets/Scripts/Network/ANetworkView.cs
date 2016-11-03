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
            MyComponents.NetworkViewsManagement.RegisterView(this);
        }
    }

    [NonSerialized]
    public bool registered;
    public abstract void ReceiveNetworkMessage(ConnectionId id, NetworkMessage message);

    void OnDestroy()
    {
        if (MyComponents.NetworkViewsManagement != null)
            MyComponents.NetworkViewsManagement.UnregisterView(this);
    }
}