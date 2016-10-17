using Byn.Net;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

delegate void LatencyChange(ConnectionId id, float newLatency);

public class TimeManagement : ObservedComponent
{
    private Dictionary<ConnectionId, Text> textfields = new Dictionary<ConnectionId, Text>();
    private float currentYPosition = 0;
    public GameObject textPrefab;

    private static TimeStrategy strategy;

    public static float NetworkTime
    {
        get
        {
            return strategy.GetNetworkTime();
        }
    }

    protected void Awake()
    {
        strategy = new NotConnectedTimeStrategy(this);
        MyGameObjects.NetworkManagement.ConnectedToServer += () => strategy = new ClientTimeStrategy(this);
        MyGameObjects.NetworkManagement.ServerCreated += () => strategy = new ServerTimeStrategy(this);
    }

    public override void PacketReceived(ConnectionId id, byte[] data)
    {
        strategy.PacketReceived(id, data);
    }

    protected override void OwnerUpdate()
    {
        //DoNothing
    }

    protected override void SimulationUpdate()
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
        Text textfield;
        if (!textfields.TryGetValue(id,out textfield))
        {
            GameObject text = this.InstantiateAsChild(textPrefab);
            textfield = text.GetComponent<Text>();
            textfields.Add(id, textfield);
            currentYPosition -= 50;
            textfield.rectTransform.localPosition += new Vector3(0, 1, 0) * currentYPosition;
        }
        textfield.text = id + " : " + latency + " ms";
    }

    protected override bool SetFlags(out MessageFlags flags)
    {
        flags = MessageFlags.NotDistributed;
        return true;
    }
}




