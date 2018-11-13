
using Byn.Net;
using PlayerManagement;
using SlideBall.Networking;
using System;
using UnityEngine.Assertions;

public class PlayersSynchronisation : ANetworkView
{
    public const short INVALID_SYNC_ID = -1;

    private ListDictionary<short, ConnectionId> synchronisations;

    private short currentSynchronisationId;

    void Awake()
    {
        Reset();
    }

    internal void Reset()
    {
        currentSynchronisationId = 0;
        synchronisations = new ListDictionary<short, ConnectionId>();
    }

    public short GetNewSynchronisationId()
    {
        Assert.IsTrue(NetworkingState.IsServer);
        currentSynchronisationId++;
        Assert.IsFalse(currentSynchronisationId - 1 == INVALID_SYNC_ID);
        return (short)(currentSynchronisationId - 1);
    }
    public override void ReceiveNetworkMessage(ConnectionId id, NetworkMessage message)
    {
        Assert.IsTrue(NetworkingState.IsServer);
        short syncId = BitConverter.ToInt16(message.data, 0);
        ConnectionId connectionId = new ConnectionId(BitConverter.ToInt16(message.data, 2));
        Synchronise(syncId, connectionId);
    }

    private void Synchronise(short syncId, ConnectionId connectionId)
    {
        Assert.IsFalse(syncId == INVALID_SYNC_ID);
        synchronisations.AddItem(syncId, connectionId);
    }

    public bool IsSynchronised(short syncId)
    {
        Assert.IsFalse(syncId == INVALID_SYNC_ID);
        return synchronisations.CountList(syncId) == MyComponents.Players.players.Count && MyComponents.Players.players.Count!=0;
    }

    public void SendSynchronisation(short syncId)
    {
        Assert.IsFalse(syncId == INVALID_SYNC_ID);
        ConnectionId connectionId = MyComponents.Players.MyPlayerId;
        if (!NetworkingState.IsServer)
        {
            byte[] syncIdData = BitConverter.GetBytes(syncId);
            byte[] connectionIdData = BitConverter.GetBytes(connectionId.id);
            byte[] data = new byte[4] { syncIdData[0], syncIdData[1], connectionIdData[0], connectionIdData[1] };
            NetworkMessage message = new NetworkMessage(ViewId, MessageType.Synchronisation, data);
            MyComponents.NetworkManagement.SendNetworkMessage(message);
        }
        else
        {
            Synchronise(syncId, connectionId);
        }

    }

    public void ResetSyncId(short syncId)
    {
        Assert.IsFalse(syncId == INVALID_SYNC_ID);
        synchronisations.Remove(syncId);
    }
}


