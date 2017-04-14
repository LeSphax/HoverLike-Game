var PeerNetwork = function() {
    function e(signalingServerConnection, configUrls) {
        this.mNextId = new ConnectionId(1);
        this.mEvents = new Queue;
        this.mIdToConnection = {};
        this.mConnectionIds = new Array;
        this.mIsDisposed = false;
        this.mSignalingNetwork = signalingServerConnection;
        this.mConfigUrls = configUrls;
        this.peerSignalingManager = new PeerSignalingManager(this.mSignalingNetwork, this)
    }
    Object.defineProperty(e.prototype, "IdToConnection", {
        get: function() {
            return this.mIdToConnection
        },
        enumerable: true,
        configurable: true
    });
    e.prototype.IncomingSignalingEvent = function (event) {
        if (event.Type != NetEventType.Log)
            console.log("IncomingSignalingEvent " + event);
        if (event.Type == NetEventType.NewConnection) {
            var t = this.peerSignalingManager.GetSignalingPeer(event.ConnectionId);
            if (t) {
                t.StartSignaling()
            } else {
                this.AddIncomingConnection(event.ConnectionId)
            }
        } else if (event.Type == NetEventType.ConnectionFailed) {
            this.peerSignalingManager.SignalingFailed(event.ConnectionId)
        } else if (event.Type == NetEventType.Disconnected) {
            var t = this.peerSignalingManager.GetSignalingPeer(event.ConnectionId);
            if (t) {
                t.SignalingInfo.SignalingDisconnected()
            }
        } else if (event.Type == NetEventType.ReliableMessageReceived) {
            var t = this.peerSignalingManager.GetSignalingPeer(event.ConnectionId);
            if (t) {
                var n = bufferToString(event.MessageData);
                t.AddSignalingMessage(n)
            } else {
                Debug.LogWarning("Signaling message from unknown connection received")
            }
        }
    }
    e.prototype.Flush = function() {
        this.mSignalingNetwork.Flush()
    };
    e.prototype.SendData = function(e, t, n) {
        if (e == null || t == null || t.length == 0) return;
        var i = this.mIdToConnection[e.id];
        if (i) {
            i.SendData(t, n)
        } else {
            Debug.LogWarning("unknown connection id")
        }
    };
    e.prototype.DisconnectFromPeer = function(e) {
        var t = this.mIdToConnection[e.id];
        if (t) {
            this.HandleDisconnect(e)
        }
    };
    e.prototype.Shutdown = function() {
        for (var e = 0, t = this.mConnectionIds; e < t.length; e++) {
            var n = t[e];
            this.DisconnectFromPeer(n)
        }
    };

    e.prototype.DisposeInternal = function() {
        if (this.mIsDisposed == false) {
            this.Shutdown();
            this.mIsDisposed = true
        }
    };
    e.prototype.Dispose = function() {
        this.DisposeInternal()
    };
    e.prototype.CreatePeer = function(e, t) {
        var n = new WebRtcDataPeer(e, t);
        return n
    };
    e.prototype.UpdatePeers = function() {
        var e = new Array;
        for (var t in this.mIdToConnection) {
            var n = this.mIdToConnection[t];
            n.Update();
            var i = new Output;
            while (n.DequeueEvent(i)) {
                this.mEvents.Enqueue(i.val)
            }
            if (n.GetState() == WebRtcPeerState.Closed) {
                e.push(n.ConnectionId)
            }
        }
        for (var o = 0, r = e; o < r.length; o++) {
            this.HandleDisconnect(r[o])
        }
    };
    e.prototype.AddOutgoingConnection = function(connectionId) {
        console.log("new outgoing connection");
        var n = new SignalingInfo(connectionId, false, Date.now());
        var peer = this.CreatePeer(this.NextConnectionId(), this.mConfigUrls);
        peer.SetSignalingInfo(n);
        this.peerSignalingManager.AddSignalingPeer(connectionId, peer);
        return peer.ConnectionId;
    };
    e.prototype.AddIncomingConnection = function (connectionId) {
        Debug.Log("new incoming connection");
        var signalingInfo = new SignalingInfo(connectionId, true, Date.now());
        var peer = this.CreatePeer(this.NextConnectionId(), this.mConfigUrls);
        peer.SetSignalingInfo(signalingInfo);
        this.peerSignalingManager.AddSignalingPeer(connectionId, peer);
        peer.NegotiateSignaling();
        return peer.ConnectionId
    };
    e.prototype.HandleDisconnect = function(e) {
        var t = this.mIdToConnection[e.id];
        if (t) {
            t.Dispose()
        }
        var n = this.mConnectionIds.indexOf(e);
        if (n != -1) {
            this.mConnectionIds.splice(n, 1)
        }
        delete this.mIdToConnection[e.id];
        this.mEvents.Enqueue(new NetworkEvent(NetEventType.Disconnected, e, null));
    };
    e.prototype.NextConnectionId = function() {
        var e = new ConnectionId(this.mNextId.id);
        this.mNextId.id++;
        return e
    };
    return e
}();