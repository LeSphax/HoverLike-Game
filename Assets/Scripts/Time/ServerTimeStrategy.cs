using Byn.Net;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ServerTimeStrategy : TimeStrategy
{
    public Dictionary<ConnectionId, float> latencies = new Dictionary<ConnectionId, float>();

    public ServerTimeStrategy(TimeManagement management) : base(management)
    {
    }

    public override float GetLatency(ConnectionId id)
    {
        return latencies[id];
    }

    public override void PacketReceived(ConnectionId id, byte[] data)
    {
        ClientTimePacket packet = NetworkExtensions.Deserialize<ClientTimePacket>(data);
        latencies[id] = packet.latency;
        management.SetLatency(id, packet.latency);
        management.View.SendData(management.observedId, MessageType.ViewPacket, new ServerTimePacket(GetNetworkTime(), packet.time).Serialize(), id);
    }

    internal override byte[] CreatePacket()
    {
        Debug.LogError("This method should'nt be called ");
        return null;
    }

    internal override float GetNetworkTime()
    {
        return Time.realtimeSinceStartup;
    }

    internal override bool IsSendingPackets()
    {
        return false;
    }
}

[Serializable]
public struct ServerTimePacket
{
    public float networkTime;
    public float timeReceived;

    public ServerTimePacket(float networkTime, float timeReceived)
    {
        this.networkTime = networkTime;
        this.timeReceived = timeReceived;
    }
}