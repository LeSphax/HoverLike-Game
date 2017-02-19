using System.Collections.Generic;
using System.Linq;

namespace Byn.Net.Native
{
    internal class PeersSignalingManager
    {

        private NativeWebRtcNetwork webRtcNetwork;
        private IServerConnection signalingNetwork;

        private int mTimeout = 60000;

        private Dictionary<ConnectionId, WebRtcDataPeer> signalingPeers = new Dictionary<ConnectionId, WebRtcDataPeer>();

        internal PeersSignalingManager(NativeWebRtcNetwork webRtcNetwork, IServerConnection signaling)
        {
            this.webRtcNetwork = webRtcNetwork;
            this.signalingNetwork = signaling;
        }

        internal bool TryGetSignalingPeer(ConnectionId id, out WebRtcDataPeer peer)
        {
            return signalingPeers.TryGetValue(id, out peer);
        }

        internal void AddSignalingPeer(ConnectionId id, WebRtcDataPeer peer)
        {
            signalingPeers.Add(id, peer);
        }

        public void CheckSignalingState()
        {
            List<ConnectionId> connected = new List<ConnectionId>();
            List<ConnectionId> failed = new List<ConnectionId>();
            foreach (KeyValuePair<ConnectionId, WebRtcDataPeer> v in this.signalingPeers)
            {
                v.Value.Update();
                int timeAlive = v.Value.SignalingInfo.CreationTimeMs;
                string msg;
                while (v.Value.DequeueSignalingMessage(out msg))
                {
                    MessageDataBuffer buffer = MessageDataBufferExt.StringToBuffer(msg);
                    signalingNetwork.SendData(v.Key, buffer.Buffer, buffer.Offset, buffer.ContentLength, true);
                }
                if (v.Value.State == AWebRtcPeer.PeerState.Connected)
                {
                    connected.Add(v.Key);
                }
                else if (v.Value.State == AWebRtcPeer.PeerState.SignalingFailed || timeAlive > this.mTimeout)
                {
                    failed.Add(v.Key);
                }
            }
            foreach (ConnectionId v2 in connected)
            {
                this.ConnectionEtablished(v2);
            }
            connected.Clear();
            foreach (ConnectionId v3 in failed)
            {
                this.SignalingFailed(v3);
            }
            failed.Clear();
        }

        private void ConnectionEtablished(ConnectionId signalingConId)
        {
            WebRtcDataPeer peer = this.signalingPeers[signalingConId];
            this.signalingPeers.Remove(signalingConId);
            this.signalingNetwork.Disconnect(signalingConId);
            webRtcNetwork.mConnectionIds.Add(peer.ConnectionId);
            webRtcNetwork.mIdToConnection[peer.ConnectionId] = peer;
            webRtcNetwork.mEvents.Enqueue(new NetworkEvent(NetEventType.NewConnection, peer.ConnectionId, null));
        }

        internal void SignalingFailed(ConnectionId signalingConId)
        {
            WebRtcDataPeer peer;
            if (this.signalingPeers.TryGetValue(signalingConId, out peer))
            {
                this.signalingPeers.Remove(signalingConId);
                webRtcNetwork.mEvents.Enqueue(new NetworkEvent(NetEventType.ConnectionFailed, peer.ConnectionId, null));
                if (peer.SignalingInfo.IsSignalingConnected)
                {
                    this.signalingNetwork.Disconnect(signalingConId);
                }
                peer.Dispose();
            }
        }

        internal void Shutdown()
        {
            ConnectionId[] array = signalingPeers.Keys.ToArray<ConnectionId>();
            for (int i = 0; i < array.Length; i++)
            {
                ConnectionId v = array[i];
                this.SignalingFailed(v);
            }
            this.signalingPeers.Clear();
        }
    }
}
