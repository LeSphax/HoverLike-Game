using Byn.Net;
using System.Collections.Generic;
using System;

public delegate void LatencyChange(float newLatency);
public delegate void SpecificLatencyChange(ConnectionId id, float newLatency);

public class TimeManagement : ObservedComponent
{
    private static TimeStrategy strategy;

    public Dictionary<ConnectionId, LatencyChange> latencyListeners = new Dictionary<ConnectionId, LatencyChange>();

    public event ConnectionEventHandler NewLatency;
    public event SpecificLatencyChange LatencyChanged;

    public float NetworkTimeInSeconds
    {
        get
        {
            return strategy.GetNetworkTimeInSeconds();
        }
    }

    public float LatencyInMiliseconds
    {
        get
        {
            return strategy.GetMyLatencyInMiliseconds();
        }
    }

    public float GetLatencyInMiliseconds(ConnectionId id)
    {
        return strategy.GetLatencyInMiliseconds(id);
    }

    protected void Awake()
    {
        Reset();
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

    public override void SimulationUpdate()
    {
        //DoNothing
    }

    protected override byte[] CreatePacket(out Dictionary<ConnectionId,byte[]> dataSpecificToClients)
    {
        dataSpecificToClients = null;
        return strategy.CreatePacket();
    }

    protected override bool IsSendingPackets()
    {
        return strategy.IsSendingPackets();
    }

    internal void SetLatency(ConnectionId id, float latency)
    {
        if (latencyListeners.ContainsKey(id))
        {
            latencyListeners[id].Invoke(latency);
        }
        if (LatencyChanged != null)
        {
            LatencyChanged.Invoke(id, latency);
        }
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

    internal void Reset()
    {
        MyComponents.NetworkManagement.ConnectedToRoom -= CreateClientStrategy;
        MyComponents.NetworkManagement.RoomCreated -= CreateServerStrategy;
        latencyListeners = new Dictionary<ConnectionId, LatencyChange>();
        strategy = new NotConnectedTimeStrategy(this);
        MyComponents.NetworkManagement.ConnectedToRoom += CreateClientStrategy;
        MyComponents.NetworkManagement.RoomCreated += CreateServerStrategy;
    }

    private void CreateServerStrategy()
    {
        strategy = new ServerTimeStrategy(this);
        strategy.NewConnection += InvokeNewLatency;
    }

    private void CreateClientStrategy()
    {
        strategy = new ClientTimeStrategy(this);
        strategy.NewConnection += InvokeNewLatency;
    }
}




