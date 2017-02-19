using Byn.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Byn.Net.Native
{
    public class NativeWebRtcNetwork : IPeerNetwork, INetwork, IDisposable
    {
        public enum WebRtcNetworkServerState
        {
            Invalid,
            Offline,
            Starting,
            Connected,
        }

        private PeersSignalingManager peerSignalingManager;

        private ConnectionId mNextId = new ConnectionId(1);

        private IServerConnection mSignaling;

        public Queue<NetworkEvent> mEvents = new Queue<NetworkEvent>();

        public Dictionary<ConnectionId, WebRtcDataPeer> mIdToConnection = new Dictionary<ConnectionId, WebRtcDataPeer>();

        public List<ConnectionId> mConnectionIds = new List<ConnectionId>();

        private NativeWebRtcNetworkFactory mFactory;

        private string[] mIceServers;

        private bool mIsDisposed;

        internal Dictionary<ConnectionId, WebRtcDataPeer> IdToConnection
        {
            get
            {
                return this.mIdToConnection;
            }
        }

        public NativeWebRtcNetworkFactory Factory
        {
            get
            {
                return this.mFactory;
            }
        }

        internal NativeWebRtcNetwork(IServerConnection signalingServer, string[] urls, NativeWebRtcNetworkFactory factory)
        {
            peerSignalingManager = new PeersSignalingManager(this, signalingServer);
            this.mSignaling = signalingServer;
            mSignaling.SetPeerNetwork(this);
            this.mIceServers = urls;
            this.mFactory = factory;
        }

        internal void IncomingSignalingEvent(NetworkEvent evt)
        {
            if (evt.Type == NetEventType.NewConnection)
            {
                WebRtcDataPeer connection;

                if (peerSignalingManager.TryGetSignalingPeer(evt.ConnectionId, out connection))
                {
                    connection.StartSignaling();
                }
                else
                {
                    AddIncomingConnection(evt.ConnectionId);
                }
            }
            else if (evt.Type == NetEventType.ConnectionFailed)
            {
                peerSignalingManager.SignalingFailed(evt.ConnectionId);
            }
            else if (evt.Type == NetEventType.Disconnected)
            {
                WebRtcDataPeer peer;
                if (peerSignalingManager.TryGetSignalingPeer(evt.ConnectionId, out peer))
                {
                    peer.SignalingInfo.SignalingDisconnected();
                }
            }
            else if (evt.Type == NetEventType.ReliableMessageReceived)
            {
                WebRtcDataPeer connection2;
                if (peerSignalingManager.TryGetSignalingPeer(evt.ConnectionId, out connection2))
                {
                    connection2.AddSignalingMessage(evt.MessageData.AsStringUnicode());
                    evt.MessageData.Dispose();
                }
                else
                {
                    SLog.LW("Signaling message from unknown connection received", new string[0]);
                }
            }
        }

        public bool Dequeue(out NetworkEvent evt)
        {
            if (this.mEvents.Count > 0)
            {
                evt = this.mEvents.Dequeue();
                return true;
            }
            evt = default(NetworkEvent);
            return false;
        }

        public bool Peek(out NetworkEvent evt)
        {
            if (this.mEvents.Count > 0)
            {
                evt = this.mEvents.Peek();
                return true;
            }
            evt = default(NetworkEvent);
            return false;
        }

        public void CheckSignalingState()
        {
            peerSignalingManager.CheckSignalingState();
        }

        public void Flush()
        {
        }

        public void SendData(ConnectionId id, byte[] data, int offset, int length, bool reliable)
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
            WebRtcDataPeer peer = null;
            if (this.mIdToConnection.TryGetValue(id, out peer))
            {
                peer.SendData(data, offset, length, reliable);
                return;
            }
            SLog.LW("unknown connection id", new string[0]);
        }

        public void Disconnect(ConnectionId id)
        {
            WebRtcDataPeer peer;
            if (this.mIdToConnection.TryGetValue(id, out peer))
            {
                this.HandleDisconnect(id);
            }
        }

        public void Shutdown()
        {
            peerSignalingManager.Shutdown();
            ConnectionId[] array = this.mConnectionIds.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                ConnectionId id = array[i];
                this.Disconnect(id);
            }
        }

        protected virtual WebRtcDataPeer CreatePeer(ConnectionId peerId, string[] urls, NativeWebRtcNetworkFactory factory)
        {
            return new WebRtcDataPeer(peerId, this.mIceServers, this.mFactory);
        }

        public Queue<NetworkEvent> UpdateNetwork()
        {
            List<ConnectionId> disconnects = new List<ConnectionId>();
            foreach (KeyValuePair<ConnectionId, WebRtcDataPeer> v in this.mIdToConnection)
            {
                v.Value.Update();
                NetworkEvent ev;
                while (v.Value.DequeueEvent(out ev))
                {
                    this.mEvents.Enqueue(ev);
                }
                if (v.Value.State == AWebRtcPeer.PeerState.Closed)
                {
                    disconnects.Add(v.Value.ConnectionId);
                }
            }
            foreach (ConnectionId id in disconnects)
            {
                this.HandleDisconnect(id);
            }
            return mEvents;
        }

        internal ConnectionId AddOutgoingConnection(ConnectionId signalingConId)
        {
            SLog.L("new outgoing connection", new string[0]);
            SignalingInfo info = new SignalingInfo(signalingConId, false, DateTime.Now);
            WebRtcDataPeer peer = this.CreatePeer(this.NextConnectionId(), this.mIceServers, this.mFactory);
            peer.SignalingInfo = info;
            peerSignalingManager.AddSignalingPeer(signalingConId, peer);
            return peer.ConnectionId;
        }

        internal ConnectionId AddIncomingConnection(ConnectionId signalingConId)
        {
            SLog.L("new incoming connection", new string[0]);
            SignalingInfo info = new SignalingInfo(signalingConId, true, DateTime.Now);
            WebRtcDataPeer peer = CreatePeer(this.NextConnectionId(), this.mIceServers, this.mFactory);
            peer.SignalingInfo = info;

            peerSignalingManager.AddSignalingPeer(signalingConId, peer);
            peer.NegotiateSignaling();
            return peer.ConnectionId;
        }

        private void HandleDisconnect(ConnectionId id)
        {
            WebRtcDataPeer peer;
            if (this.mIdToConnection.TryGetValue(id, out peer))
            {
                peer.Dispose();
            }
            this.mConnectionIds.Remove(id);
            this.mIdToConnection.Remove(id);
            NetworkEvent ev = new NetworkEvent(NetEventType.Disconnected, id, null);
            this.mEvents.Enqueue(ev);
        }

        private ConnectionId NextConnectionId()
        {
            this.mNextId.id = (short)(this.mNextId.id + 1);
            return new ConnectionId(mNextId.id);
        }

        public void Dispose()
        {
            if (!this.mIsDisposed)
            {
                this.Shutdown();
                this.mIsDisposed = true;
            }
        }
    }
}
