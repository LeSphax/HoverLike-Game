﻿using Byn.Net;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class ClientTimeStrategy : TimeStrategy
{
    private ConnectionId otherId = ConnectionId.INVALID;
    private float latency;
    private float networkTime;
    private float lastNetworkUpdate;

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
            InvokeNewConnection(id);
        }
        Assert.IsTrue(id == otherId);
        ServerTimePacket packet = ServerTimePacket.Deserialize(data);
        latency = (Time.realtimeSinceStartup - packet.timeReceived) * 1000;
        networkTime = packet.networkTime;
        lastNetworkUpdate = Time.realtimeSinceStartup;
        management.SetLatency(id, latency);
    }

    public override float GetLatencyInMiliseconds(ConnectionId id)
    {
        Assert.IsTrue(id == otherId);
        Debug.LogError("Shouldn't be called on a client");
        return latency;
    }

    public override float GetMyLatencyInMiliseconds()
    {
        return latency;
    }

    internal override float GetNetworkTimeInSeconds()
    {
        return networkTime + Time.realtimeSinceStartup - lastNetworkUpdate;
    }
}

public struct ClientTimePacket
{
    public float time;
    public float latency;

    public ClientTimePacket(float time, float latency)
    {
        this.time = time;
        this.latency = latency;
    }

    public byte[] Serialize()
    {
        byte[] data = BitConverter.GetBytes(time);
        return ArrayExtensions.Concatenate(data, BitConverter.GetBytes(latency));
    }

    public static ClientTimePacket Deserialize(byte[] data)
    {
        float time = BitConverter.ToSingle(data, 0);
        float latency = BitConverter.ToSingle(data, 4);
        return new ClientTimePacket(time, latency);
    }
}