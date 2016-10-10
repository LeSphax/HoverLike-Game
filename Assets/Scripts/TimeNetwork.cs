using Byn.Net;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeNetwork : ObservedComponent
{

    public const int TIME_ID = 3;

    public Dictionary<string, object> properties = new Dictionary<string, object>();

    void Start()
    {
        Debug.LogWarning("Need to review this class, the time doesn't take latency into account");
        View.viewId = TIME_ID;
    }

    private static float time;
    public static float Time
    {
        get
        {
            if (MyGameObjects.NetworkManagement.isServer)
            {
                return UnityEngine.Time.time;
            }
            return time;
        }
    }

    public override void PacketReceived(ConnectionId id, byte[] data)
    {
        TimePacket packet = NetworkExtensions.Deserialize<TimePacket>(data);
        time = packet.time;
    }

    protected override byte[] CreatePacket(long sendId)
    {
        return new TimePacket(Time).Serialize();
    }

    protected override void OwnerUpdate()
    {
        //DoNothing
    }

    protected override void SimulationUpdate()
    {
        //DoNothing
    }

    protected override bool IsSendingPackets()
    {
        return !MyGameObjects.NetworkManagement.isServer;
    }

    [Serializable]
    private struct TimePacket
    {
        public float time;

        public TimePacket(float time)
        {
            this.time = time;
        }
    }
}
