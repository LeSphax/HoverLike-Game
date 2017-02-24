using Byn.Net;
using System;
using System.Collections.Generic;

public interface IWebRtcNetwork : IDisposable
{
    void UpdateNetwork();
    void Flush();
    void ConnectToServer();
    void ConnectToRoom(string roomName);
    void CreateRoom(string roomName);
    void LeaveRoom();
    void DisconnectFromServer();
    void DisconnectFromPeer(ConnectionId id);

    void SendSignalingEvent(ConnectionId id,string content, NetEventType type);
    void SendPeerEvent(ConnectionId id, byte[] data, int offset, int length, bool isReliable);

    void Shutdown();

    Queue<NetworkEvent> SignalingEvents { get; }
    Queue<NetworkEvent> PeerEvents { get; }
}