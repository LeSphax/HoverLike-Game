using Byn.Net.Native;
using System;
using System.Collections.Generic;

namespace Byn.Net
{
	public interface IServerConnection : INetwork, IDisposable
	{
        void CreateRoom(string address = null);

        void LeaveRoom();

        void ConnectToServer();

		void Disconnect();

		ConnectionId ConnectToRoom(string address);

        void SetPeerNetwork(IPeerNetwork network);

        Queue<NetworkEvent> SignalingEvents { get; }

        void SendSignalingEvent(ConnectionId id, string data, NetEventType type);


    }
}
