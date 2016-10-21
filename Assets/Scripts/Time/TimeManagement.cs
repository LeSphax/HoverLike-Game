using Byn.Net;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void LatencyChange(float newLatency);

public class TimeManagement : ObservedComponent
{
    private static TimeStrategy strategy;

    public Dictionary<ConnectionId, LatencyChange> latencyListeners = new Dictionary<ConnectionId, LatencyChange>();

    public event ConnectionEventHandler NewLatency;

    public static float NetworkTime
    {
        get
        {
            return strategy.GetNetworkTime();
        }
    }

    public static float Latency
    {
        get
        {
            return strategy.GetMyLatency();
        }
    }

    protected void Awake()
    {
        strategy = new NotConnectedTimeStrategy(this);
        MyGameObjects.NetworkManagement.ConnectedToRoom += () => { Debug.LogError("ConnectedToRoom"); strategy = new ClientTimeStrategy(this); strategy.NewConnection += InvokeNewLatency; };
        MyGameObjects.NetworkManagement.RoomCreated += () => { Debug.LogError("RoomCreated"); strategy = new ServerTimeStrategy(this); strategy.NewConnection += InvokeNewLatency; };

    }

    private void InvokeNewLatency(ConnectionId id)
    {
        if (NewLatency != null)
        {
            NewLatency.Invoke(id);
        }
    }

    public override void PacketReceived(ConnectionId id, byte[] data)
    {
        strategy.PacketReceived(id, data);
    }

    public override void OwnerUpdate()
    {
        //DoNothing
    }

    public override void SimulationUpdate()
    {
        //DoNothing
    }

    protected override byte[] CreatePacket(long sendId)
    {
        return strategy.CreatePacket();
    }

    protected override bool IsSendingPackets()
    {
        return strategy.IsSendingPackets();
    }

    internal void SetLatency(ConnectionId id, float latency)
    {
        if (latencyListeners.ContainsKey(id))
            latencyListeners[id].Invoke(latency);
    }

    protected override bool SetFlags(out MessageFlags flags)
    {
        flags = MessageFlags.NotDistributed;
        return true;
    }

    public void AddLatencyChangeListener(ConnectionId id, LatencyChange callback)
    {
        if (latencyListeners.ContainsKey(id))
        {
            latencyListeners[id] += callback;
        }
        else
        {
            latencyListeners.Add(id, callback);
        }
    }
}




