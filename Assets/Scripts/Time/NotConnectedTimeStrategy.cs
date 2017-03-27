using Byn.Net;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class NotConnectedTimeStrategy : TimeStrategy
{
    public NotConnectedTimeStrategy(TimeManagement management) : base(management)
    {
    }

    internal override byte[] CreatePacket()
    {
        throw new NotImplementedException();
    }

    internal override bool IsSendingPackets()
    {
        return false;
    }

    public override void PacketReceived(ConnectionId id, byte[] data)
    {
        throw new NotImplementedException();
    }

    public override float GetLatencyInMiliseconds(ConnectionId id)
    {
        throw new NotImplementedException();
    }

    public override float GetMyLatencyInMiliseconds()
    {
        return 0;
    }

    internal override float GetNetworkTimeInSeconds()
    {
        return 0;
    }
}