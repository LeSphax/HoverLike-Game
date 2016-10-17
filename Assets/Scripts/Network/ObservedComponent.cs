using Byn.Net;
using System;
using UnityEngine;

[RequireComponent(typeof(MyNetworkView))]
public abstract class ObservedComponent : SlideBall.MonoBehaviour
{
    [NonSerialized]
    public int observedId;

    public int sendRate = 60;

    //TODO : use an int with a sliding window
    private long sendId = 0;

    public void StartUpdating()
    {
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
            MessageFlags flags;
            if (SetFlags(out flags))
            {
                SendData(MessageType.ViewPacket, CreatePacket(sendId), flags);
            }
            else
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

    protected void SendData(MessageType type, byte[] data, MessageFlags flags)
    {
        View.SendData(observedId, type, data, flags);
    }

    // True is flags shouled be set, false otherwise
    protected virtual bool SetFlags(out MessageFlags flags)
    {
        flags = MessageFlags.None;
        return false;
    }


}

