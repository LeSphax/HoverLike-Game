using Byn.Net;
using System.Collections.Generic;
using System;

public class WebRtcNetwork : IWebRtcNetwork
{
    private bool mIsDisposed = false;

    private AWebRtcNetworkFactory mFactory;

    public IServerConnection serverConnection;
    public IPeerNetwork peerNetwork;

    private NetworkUpdateResult result;

    public Queue<NetworkEvent> SignalingEvents
    {
        get
        {
            return serverConnection.SignalingEvents;
        }
    }
    public Queue<NetworkEvent> PeerEvents
    {
        get
        {
            return peerNetwork.PeerEvents;
        }
    }

    public WebRtcNetwork(IServerConnection serverConnection, IPeerNetwork peerNetwork, AWebRtcNetworkFactory factory)
    {
        this.serverConnection = serverConnection;
        this.peerNetwork = peerNetwork;
        this.mFactory = factory;
    }

    public void Dispose()
    {
        if (!this.mIsDisposed)
        {
            this.Shutdown();
            this.mFactory.OnNetworkDestroyed(this);
            this.mIsDisposed = true;
        }
    }

    public void UpdateNetwork()
    {
        peerNetwork.CheckSignalingState();
        result.signalingEvents = serverConnection.UpdateNetwork();
        result.peerEvents = peerNetwork.UpdateNetwork();
    }

    public void Shutdown()
    {
        serverConnection.Dispose();
        peerNetwork.Dispose();
    }

    public void Flush()
    {
        peerNetwork.Flush();
        serverConnection.Flush();
    }

    public void ConnectToServer()
    {
        serverConnection.ConnectToServer();
    }

    public void ConnectToRoom(string roomName)
    {
        serverConnection.ConnectToRoom(roomName);
    }

    public void CreateRoom(string roomName)
    {
        serverConnection.CreateRoom(roomName);
    }

    public void LeaveRoom()
    {
        serverConnection.LeaveRoom();
    }

    public void SendSignalingEvent(ConnectionId id, string content, NetEventType type)
    {
        serverConnection.SendSignalingEvent(id, content, type);
    }

    public void SendPeerEvent(ConnectionId id, byte[] data, int offset, int length, bool isReliable)
    {
        peerNetwork.SendEvent(id, data, offset, length, isReliable);
    }

    public void DisconnectFromServer()
    {
        throw new NotImplementedException();
    }

    public void DisconnectFromPeer(ConnectionId id)
    {
        throw new NotImplementedException();
    }
}

public struct NetworkUpdateResult
{
    public Queue<NetworkEvent> signalingEvents;
    public Queue<NetworkEvent> peerEvents;
}
