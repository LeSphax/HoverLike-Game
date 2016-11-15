using Byn.Net;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MyNetworkView))]
public abstract class ObservedComponent : SlideBall.MonoBehaviour
{
    [NonSerialized]
    public short observedId;

    public int sendRate = 60;

    public void StartUpdating()
    {
        InvokeRepeating("SendPacket", 0, 1f / sendRate);
    }

    protected virtual void SendPacket()
    {
        if (IsSendingPackets())
        {
            MessageFlags flags;
            Dictionary<ConnectionId, byte[]> dataSpecificToClient;
            byte[] packet = CreatePacket(out dataSpecificToClient);
            if (packet != null)
            {
                if (dataSpecificToClient == null)
                {
                    if (SetFlags(out flags))
                    {
                        SendData(MessageType.ViewPacket, packet, flags);
                    }
                    else
                        SendData(MessageType.ViewPacket, packet);
                }
                else
                {
                    if (SetFlags(out flags))
                    {
                        foreach (var pair in dataSpecificToClient)
                        {
                            byte[] newPacket = ArrayExtensions.Concatenate(pair.Value, packet);
                            SendData(MessageType.ViewPacket, newPacket, pair.Key, flags);
                        }
                    }
                    else
                        foreach (var pair in dataSpecificToClient)
                        {
                            byte[] newPacket = ArrayExtensions.Concatenate(pair.Value, packet);
                            SendData(MessageType.ViewPacket, newPacket, pair.Key);
                        }
                }
            }
        }
    }

    protected abstract bool IsSendingPackets();

    public abstract void SimulationUpdate();

    protected abstract byte[] CreatePacket(out Dictionary<ConnectionId, byte[]> dataSpecificToClient);

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

    protected void SendData(MessageType type, byte[] data, ConnectionId id, MessageFlags flags)
    {
        View.SendData(observedId, type, data, id, flags);
    }

    // True is flags shouled be set, false otherwise
    protected virtual bool SetFlags(out MessageFlags flags)
    {
        flags = MessageFlags.None;
        return false;
    }


}

