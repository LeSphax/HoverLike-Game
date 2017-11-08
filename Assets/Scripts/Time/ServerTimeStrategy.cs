using Byn.Net;
using SlideBall.Networking;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ServerTimeStrategy : TimeStrategy
{
    public Dictionary<ConnectionId, float> latencies = new Dictionary<ConnectionId, float>();

    public ServerTimeStrategy(TimeManagement management) : base(management)
    {
    }

    public override float GetLatencyInMiliseconds(ConnectionId id)
    {
        if (id == ConnectionId.INVALID)
            return 0;
        return latencies[id];
    }

    public override float GetMyLatencyInMiliseconds()
    {
        return 0;
    }

    public override void PacketReceived(ConnectionId id, byte[] data)
    {
        ClientTimePacket packet = ClientTimePacket.Deserialize(data);
        if (!latencies.ContainsKey(id))
            InvokeNewConnection(id);
        latencies[id] = packet.latency;
        management.SetLatency(id, packet.latency);
        management.View.SendData(management.observedId, MessageType.ViewPacket, new ServerTimePacket(TimeSimulation.TimeInSeconds, packet.time).Serialize(), id);
    }

    internal override byte[] CreatePacket()
    {
        Debug.LogError("This method shouldn't be called ");
        return null;
    }

    internal override float GetNetworkTimeInSeconds()
    {
        Debug.LogError("This shouldn't ever be called on the server");
        return TimeSimulation.TimeInSeconds;
    }

    internal override bool IsSendingPackets()
    {
        return false;
    }

    internal void Dispose()
    {

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

    public byte[] Serialize()
    {
        byte[] data = BitConverter.GetBytes(networkTime);
        return data.Concatenate(BitConverter.GetBytes(timeReceived));
    }

    public static ServerTimePacket Deserialize(byte[] data)
    {
        float networkTime = BitConverter.ToSingle(data, 0);
        float timeReceived = BitConverter.ToSingle(data, 4);
        return new ServerTimePacket(networkTime, timeReceived);
    }
}