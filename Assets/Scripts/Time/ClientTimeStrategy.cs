using Byn.Net;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class ClientTimeStrategy : TimeStrategy
{
    private ConnectionId otherId = ConnectionId.INVALID;
    private float latency;
    private float networkTime;

    public ClientTimeStrategy(TimeManagement management) : base(management)
    {
    }

    internal override byte[] CreatePacket()
    {
        return new ClientTimePacket(Time.realtimeSinceStartup, latency).Serialize();
    }

    internal override bool IsSendingPackets()
    {
        return true;
    }

    public override void PacketReceived(ConnectionId id, byte[] data)
    {
        if (otherId == ConnectionId.INVALID)
        {
            otherId = id;
        }
        Assert.IsTrue(id == otherId);
        ServerTimePacket packet = NetworkExtensions.Deserialize<ServerTimePacket>(data);
        latency = (Time.realtimeSinceStartup - packet.timeReceived) * 1000;
        networkTime = packet.networkTime + latency / 2000f;
        management.SetLatency(id, latency);
    }

    public override float GetLatency(ConnectionId id)
    {
        Assert.IsTrue(id ==  otherId);
        return latency;
    }

    internal override float GetNetworkTime()
    {
        return networkTime;
    }
}

[Serializable]
public struct ClientTimePacket
{
    public float time;
    public float latency;

    public ClientTimePacket(float time, float latency)
    {
        this.time = time;
        this.latency = latency;
    }
}