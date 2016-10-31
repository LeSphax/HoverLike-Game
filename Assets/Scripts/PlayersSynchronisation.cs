
using Byn.Net;
using PlayerManagement;
using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class PlayersSynchronisation : ANetworkView
{
    private static ListDictionary<short, ConnectionId> synchronisations = new ListDictionary<short, ConnectionId>();

    public static short currentSynchronisationId;

    public static short GetNewSynchronisationId()
    {
        Assert.IsTrue(MyGameObjects.NetworkManagement.isServer);
        currentSynchronisationId++;
        return (short)(currentSynchronisationId - 1);
    }
    public override void ReceiveNetworkMessage(ConnectionId id, NetworkMessage message)
    {
        Assert.IsTrue(MyGameObjects.NetworkManagement.isServer);
        short syncId = BitConverter.ToInt16(message.data, 0);
        ConnectionId connectionId = new ConnectionId(BitConverter.ToInt16(message.data, 2));
        Synchronise(syncId, connectionId);
    }

    private static void Synchronise(short syncId, ConnectionId connectionId)
    {
        synchronisations.AddItem(syncId, connectionId);
    }

    public static bool IsSynchronised(short syncId)
    {
        return synchronisations.CountList(syncId) == Players.players.Count;
    }

    public void SendSynchronisation(short syncId, ConnectionId connectionId)
    {
        if (!MyGameObjects.NetworkManagement.isServer)
        {
            byte[] syncIdData = BitConverter.GetBytes(syncId);
            byte[] connectionIdData = BitConverter.GetBytes(connectionId.id);
            byte[] data = new byte[4] { syncIdData[0], syncIdData[1], connectionIdData[0], connectionIdData[1] };
            MyGameObjects.NetworkManagement.SendData(ViewId, MessageType.Synchronisation, data);
        }
        else
        {
            Synchronise(syncId, connectionId);
        }

    }
}


