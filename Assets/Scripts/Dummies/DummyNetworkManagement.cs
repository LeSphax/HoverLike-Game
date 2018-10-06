using Byn.Net;
using SlideBall.Networking;
using UnityEngine;

public class DummyNetworkManagement : ANetworkManagement
{
    public override bool IsServer
    {
        get
        {
            return true;
        }
    }

    public override bool CurrentlyPlaying
    {
        get
        {
            return true;
        }

        set
        {
            throw new System.NotImplementedException();
        }
    }

    public override bool IsConnected
    {
        get
        {
            return true;
        }
    }

    public override event EmptyEventHandler ReceivedAllBufferedMessages;

    public override int GetNumberPlayers()
    {
        return 1;
    }

    public override void RefreshRoomData()
    {
        Debug.Log("Dummy Refresh Room Data");
    }

    public override void Reset()
    {
        Debug.Log("Dummy Reset");
    }

    public override void SendData(short viewId, MessageType type, byte[] data)
    {
        Debug.Log("Dummy SendData");
    }

    public override void SendData(short viewId, MessageType type, byte[] data, ConnectionId id)
    {
        Debug.Log("Dummy SendData");
    }

    public override void SendData(short viewId, short subId, MessageType type, byte[] data)
    {
        Debug.Log("Dummy SendData");
    }

    public override void SendData(short viewId, short subId, MessageType type, byte[] data, ConnectionId id)
    {
        Debug.Log("Dummy SendData");
    }

    public override void SendNetworkMessage(NetworkMessage message)
    {
        Debug.Log("Dummy SendNetworkMessage");
    }

    public override void SendNetworkMessage(NetworkMessage message, params ConnectionId[] connectionIds)
    {
        Debug.Log("Dummy SendNetworkMessage");
    }
}
