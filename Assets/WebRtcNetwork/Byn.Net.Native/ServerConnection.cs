using Byn.Common;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using WebSocketSharp;

namespace Byn.Net.Native
{
    public class ServerConnection : IServerConnection, INetwork, IDisposable
    {
        private Queue<NetworkEvent> signalingEvents = new Queue<NetworkEvent>();
        public Queue<NetworkEvent> SignalingEvents
        {
            get
            {
                return signalingEvents;
            }
        }

        private NativeWebRtcNetwork peerNetwork;

        private WebSocketConnection websocket;

        public static readonly string LOGTAG = "WebsocketNetwork";

        private Queue<NetworkEvent> mOutgoingQueue = new Queue<NetworkEvent>();

        private Queue<NetworkEvent> mIncomingQueue = new Queue<NetworkEvent>();

        private WebsocketServerStatus mServerStatus;

        private List<ConnectionId> mConnecting = new List<ConnectionId>();

        private List<ConnectionId> mConnections = new List<ConnectionId>();

        private ConnectionId mNextOutgoingConnectionId = new ConnectionId(1);

        private bool mIsDisposed;

        public void SetPeerNetwork(IPeerNetwork network)
        {
            Assert.IsTrue(network is NativeWebRtcNetwork);
            this.peerNetwork = (NativeWebRtcNetwork)network;
        }

        public ServerConnection(string url)
        {
            Debug.LogWarning("New server connection " + url);
            this.websocket = new WebSocketConnection(this, url);
        }

        public Queue<NetworkEvent> UpdateNetwork()
        {
            this.CheckSleep();
            signalingEvents.Clear();
            NetworkEvent evt;
            while (Dequeue(out evt))
            {
                Debug.Log(evt);
                Debug.Log(evt.Type + "    " + evt.RawData);
                peerNetwork.IncomingSignalingEvent(evt);
                if (evt.Type == NetEventType.RoomCreated || evt.Type == NetEventType.SignalingConnectionFailed || evt.Type == NetEventType.RoomCreationFailed || evt.Type == NetEventType.RoomJoinFailed || evt.Type == NetEventType.RoomClosed || evt.Type == NetEventType.UserCommand)
                {
                    signalingEvents.Enqueue(evt);
                }
            }
            return signalingEvents;
        }

        private void CheckSleep()
        {
            if (websocket.status == WebsocketConnectionStatus.Connected && this.mServerStatus == WebsocketServerStatus.Offline && this.mConnecting.Count == 0 && this.mConnections.Count == 0)
            {
                this.Cleanup();
            }
        }

        internal void Cleanup()
        {
            if (websocket.status == WebsocketConnectionStatus.Disconnecting || websocket.status == WebsocketConnectionStatus.NotConnected)
            {
                return;
            }
            websocket.status = WebsocketConnectionStatus.Disconnecting;
            foreach (ConnectionId conId in this.mConnecting)
            {
                this.EnqueueIncoming(new NetworkEvent(NetEventType.ConnectionFailed, conId, null));
            }
            this.mConnecting.Clear();
            foreach (ConnectionId conId2 in this.mConnections)
            {
                this.EnqueueIncoming(new NetworkEvent(NetEventType.Disconnected, conId2, null));
            }
            this.mConnections.Clear();
            if (this.mServerStatus == WebsocketServerStatus.Starting)
            {
                this.EnqueueIncoming(new NetworkEvent(NetEventType.SignalingConnectionFailed, ConnectionId.INVALID, null));
            }
            else if (this.mServerStatus == WebsocketServerStatus.Online)
            {
                this.EnqueueIncoming(new NetworkEvent(NetEventType.SignalingConnectionFailed, ConnectionId.INVALID, null));
            }
            else if (this.mServerStatus == WebsocketServerStatus.ShuttingDown)
            {
                this.EnqueueIncoming(new NetworkEvent(NetEventType.SignalingConnectionFailed, ConnectionId.INVALID, null));
            }
            this.mServerStatus = WebsocketServerStatus.Offline;
            Queue<NetworkEvent> obj = this.mOutgoingQueue;
            lock (obj)
            {
                this.mOutgoingQueue.Clear();
            }
            websocket.Cleanup();
            websocket.status = WebsocketConnectionStatus.NotConnected;
        }

        private void EnqueueOutgoing(NetworkEvent evt)
        {
            Queue<NetworkEvent> obj = this.mOutgoingQueue;
            lock (obj)
            {
                this.mOutgoingQueue.Enqueue(evt);
            }
        }

        private void EnqueueIncoming(NetworkEvent evt)
        {
            Queue<NetworkEvent> obj = this.mIncomingQueue;
            lock (obj)
            {
                this.mIncomingQueue.Enqueue(evt);
            }
        }

        private void TryRemoveConnecting(ConnectionId id)
        {
            this.mConnecting.Remove(id);
        }

        private void TryRemoveConnection(ConnectionId id)
        {
            this.mConnections.Remove(id);
        }

        internal void HandleIncomingEvent(NetworkEvent evt)
        {
            if (evt.Type == NetEventType.NewConnection)
            {
                this.TryRemoveConnecting(evt.ConnectionId);
                this.mConnections.Add(evt.ConnectionId);
            }
            else if (evt.Type == NetEventType.ConnectionFailed)
            {
                this.TryRemoveConnecting(evt.ConnectionId);
            }
            else if (evt.Type == NetEventType.Disconnected)
            {
                this.mConnections.Remove(evt.ConnectionId);
            }
            else if (evt.Type == NetEventType.ConnectionToSignalingServerEstablished)
            {
                this.mServerStatus = WebsocketServerStatus.Online;
            }
            this.EnqueueIncoming(evt);
        }

        private void HandleOutgoingEvents()
        {
            Queue<NetworkEvent> obj = this.mOutgoingQueue;
            lock (obj)
            {
                while (this.mOutgoingQueue.Count > 0)
                {
                    websocket.Send(mOutgoingQueue.Dequeue());
                }
            }
        }

        private ConnectionId NextConnectionId()
        {
            ConnectionId arg_1F_0 = this.mNextOutgoingConnectionId;
            this.mNextOutgoingConnectionId = new ConnectionId((short)(mNextOutgoingConnectionId.id + 1));
            return arg_1F_0;
        }

        private string GetRandomKey()
        {
            System.Random rnd = new System.Random((int)DateTime.Now.Ticks);
            string result = "";
            for (int i = 0; i < 7; i++)
            {
                result += ((char)(65 + rnd.Next(0, 26))).ToString();
            }
            return result;
        }

        public bool Dequeue(out NetworkEvent evt)
        {
            Queue<NetworkEvent> obj = this.mIncomingQueue;
            lock (obj)
            {
                if (this.mIncomingQueue.Count > 0)
                {
                    evt = this.mIncomingQueue.Dequeue();
                    return true;
                }
            }
            evt = default(NetworkEvent);
            return false;
        }

        public bool Peek(out NetworkEvent evt)
        {
            Queue<NetworkEvent> obj = this.mIncomingQueue;
            lock (obj)
            {
                if (this.mIncomingQueue.Count > 0)
                {
                    evt = this.mIncomingQueue.Peek();
                    return true;
                }
            }
            evt = default(NetworkEvent);
            return false;
        }

        public void Flush()
        {
            if (websocket.status == WebsocketConnectionStatus.Connected)
            {
                this.HandleOutgoingEvents();
            }
        }

        public void SendEvent(ConnectionId id, byte[] data, int offset, int length, bool reliable)
        {
            NetEventType type;
            if (reliable)
                type = NetEventType.ReliableMessageReceived;
            else
                type = NetEventType.UnreliableMessageReceived;
            SendSignalingEvent(id, data, offset, length, type);
        }

        public void SendSignalingEvent(ConnectionId id, byte[] data, int offset, int length, NetEventType type)
        {
            if (data == null || data.Length == 0 || length == 0)
            {
                SLog.LW("Invalid SendData argument: pointer null or length zero", new string[0]);
                return;
            }
            if (offset < 0 || offset >= data.Length)
            {
                SLog.LW("Invalid SendData argument: invalid offset", new string[0]);
                return;
            }
            if (data.Length < offset + length)
            {
                SLog.LW("Invalid SendData argument: offset and length not within bounds of the data buffer!", new string[0]);
                return;
            }
            NetworkEvent evt;

            evt = new NetworkEvent(type, id, new ByteArrayBuffer(data, offset, length));

            this.EnqueueOutgoing(evt);
        }

        public void SendSignalingEvent(ConnectionId id, string data, NetEventType type)
        {
            NetworkEvent evt;

            evt = new NetworkEvent(type, id, data);

            this.EnqueueOutgoing(evt);
        }

        public void Disconnect(ConnectionId id)
        {
            NetworkEvent evt = new NetworkEvent(NetEventType.Disconnected, id, null);
            this.EnqueueOutgoing(evt);
        }

        public void Shutdown()
        {
            this.Cleanup();
        }

        public void LeaveRoom()
        {
            this.EnqueueOutgoing(new NetworkEvent(NetEventType.RoomClosed, ConnectionId.INVALID, null));
        }

        public void CreateRoom(string address = null)
        {
            if (address == null)
            {
                address = (this.GetRandomKey() ?? "");
            }
            if (this.mServerStatus == WebsocketServerStatus.Online || this.mServerStatus == WebsocketServerStatus.Starting)
            {
                Debug.Log("Creating Room " + address);
                this.EnqueueOutgoing(new NetworkEvent(NetEventType.RoomCreated, ConnectionId.INVALID, address));
            }
            else
            {
                Debug.LogError("Can't create room, not connected to the signaling server");
            }
        }

        public void ConnectToServer()
        {
            if (this.mServerStatus == WebsocketServerStatus.Offline)
            {
                websocket.EnsureServerConnection();
                this.mServerStatus = WebsocketServerStatus.Starting;
                return;
            }
            Debug.LogError("Already connected to the server");
            //this.EnqueueIncoming(new NetworkEvent(NetEventType.ServerConnectionFailed, ConnectionId.INVALID, "Already connected to the server"));
        }

        public void Disconnect()
        {
            this.EnqueueOutgoing(new NetworkEvent(NetEventType.RoomClosed, ConnectionId.INVALID, null));
        }

        public ConnectionId ConnectToRoom(string address)
        {
            websocket.EnsureServerConnection();
            ConnectionId newConId = this.NextConnectionId();
            this.mConnecting.Add(newConId);
            NetworkEvent evt = new NetworkEvent(NetEventType.NewConnection, newConId, address);
            this.EnqueueOutgoing(evt);
            peerNetwork.AddOutgoingConnection(evt.ConnectionId);

            return newConId;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.mIsDisposed)
            {
                if (disposing)
                {
                    this.Shutdown();
                }
                this.mIsDisposed = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }
    }
}
