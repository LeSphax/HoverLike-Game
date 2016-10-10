using Byn.Net;
using System;
using UnityEngine;

[RequireComponent(typeof(MyNetworkView))]
public abstract class ObservedComponent : MonoBehaviour
{
    private MyNetworkView view;
    public MyNetworkView View
    {
        get
        {
            if (view == null)
            {
                view = GetComponent<MyNetworkView>();
            }
            return view;
        }
    }
    [NonSerialized]
    public int observedId;

    public int sendRate = 60;

    private long sendId = 0;

    protected virtual void Awake()
    {
        Debug.LogWarning(gameObject.name +  " SendId shouldn't be a long");
        InvokeRepeating("SendPacket", 0, 1f / sendRate);
    }

    void FixedUpdate()
    {
        if (View.isMine)
        {
            OwnerUpdate();
        }
        else
        {
            SimulationUpdate();
        }
    }

    protected virtual void SendPacket()
    {
        if (IsSendingPackets())
        {
            SendData(MessageType.ViewPacket, CreatePacket(sendId));
            sendId++;
        }
    }

    protected abstract bool IsSendingPackets();

    protected abstract void OwnerUpdate();
    protected abstract void SimulationUpdate();

    protected abstract byte[] CreatePacket(long sendId);

    public abstract void PacketReceived(ConnectionId id, byte[] data);

    protected void SendData(MessageType type, byte[] data)
    {
        View.SendData(observedId, type, data);
    }
    protected void SendData(MessageType type, byte[] data, ConnectionId id)
    {
        View.SendData(observedId, type, data, id);
    }

}

