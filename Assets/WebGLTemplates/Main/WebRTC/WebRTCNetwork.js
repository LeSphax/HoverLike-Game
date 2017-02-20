var WebRtcNetwork = function() {
    function e(e, t) {
        this.mTimeout = 6e4;
        this.mInSignaling = {};
        this.mNextId = new ConnectionId(1);
        this.mSignaling = null;
        this.mEvents = new Queue;
        this.mIdToConnection = {};
        this.mConnectionIds = new Array;
        this.mServerState = WebRtcNetworkServerState.Offline;
        this.mIsDisposed = false;
        this.mSignaling = e;
        this.mSignalingNetwork = this.mSignaling.GetNetwork();
        this.mConfigUrls = t
    }
    Object.defineProperty(e.prototype, "IdToConnection", {
        get: function() {
            return this.mIdToConnection
        },
        enumerable: true,
        configurable: true
    });
    e.prototype.GetConnections = function() {
        return this.mConnectionIds
    };
    e.prototype.SetLog = function(e) {
        this.mLogDelegate = e
    };
    e.prototype.StartServer = function(e) {
        this.mServerState = WebRtcNetworkServerState.Starting;
        this.mSignalingNetwork.StartServer(e)
    };
    e.prototype.StopServer = function() {
        if (this.mServerState == WebRtcNetworkServerState.Starting) {
            this.mSignalingNetwork.StopServer()
        } else if (this.mServerState == WebRtcNetworkServerState.Online) {
            this.mSignalingNetwork.StopServer()
        }
    };
    e.prototype.Connect = function(e) {
        console.log("Connecting ...");
        return this.AddOutgoingConnection(e)
    };
    e.prototype.Update = function() {
        this.CheckSignalingState();
        this.UpdateSignalingNetwork();
        this.UpdatePeers()
    };
    e.prototype.Dequeue = function() {
        if (this.mEvents.Count() > 0) return this.mEvents.Dequeue();
        return null
    };
    e.prototype.Peek = function() {
        if (this.mEvents.Count() > 0) return this.mEvents.Peek();
        return null
    };
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
    e.prototype.Disconnect = function(e) {
        var t = this.mIdToConnection[e.id];
        if (t) {
            this.HandleDisconnect(e)
        }
    };
    e.prototype.Shutdown = function() {
        for (var e = 0, t = this.mConnectionIds; e < t.length; e++) {
            var n = t[e];
            this.Disconnect(n)
        }
        this.StopServer();
        this.mSignalingNetwork.Shutdown()
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
    e.prototype.CheckSignalingState = function() {
        var e = new Array;
        var t = new Array;
        for (var n in this.mInSignaling) {
            var i = this.mInSignaling[n];
            i.Update();
            var o = i.SignalingInfo.GetCreationTimeMs();
            var r = new Output;
            while (i.DequeueSignalingMessage(r)) {
                var a = this.StringToBuffer(r.val);
                this.mSignalingNetwork.SendData(new ConnectionId((+n)), a, true)
            }
            if (i.GetState() == WebRtcPeerState.Connected) {
                e.push(i.SignalingInfo.ConnectionId)
            } else if (i.GetState() == WebRtcPeerState.SignalingFailed || o > this.mTimeout) {
                t.push(i.SignalingInfo.ConnectionId)
            }
        }
        for (var s = 0, c = e; s < c.length; s++) {
            var l = c[s];
            this.ConnectionEstablished(l)
        }
        for (var u = 0, g = t; u < g.length; u++) {
            var l = g[u];
            this.SignalingFailed(l)
        }
    };
    e.prototype.UpdateSignalingNetwork = function() {
        this.mSignalingNetwork.Update();
        var e;
        while ((e = this.mSignalingNetwork.Dequeue()) != null) {
            if (e.Type == NetEventType.ServerInitialized) {
                this.mServerState = WebRtcNetworkServerState.Online;
                this.mEvents.Enqueue(new NetworkEvent(NetEventType.ServerInitialized, ConnectionId.INVALID, e.RawData))
            } else if (e.Type == NetEventType.ServerInitFailed) {
                this.mServerState = WebRtcNetworkServerState.Offline;
                this.mEvents.Enqueue(new NetworkEvent(NetEventType.ServerInitFailed, ConnectionId.INVALID, e.RawData))
            } else if (e.Type == NetEventType.ServerClosed) {
                this.mServerState = WebRtcNetworkServerState.Offline;
                this.mEvents.Enqueue(new NetworkEvent(NetEventType.ServerClosed, ConnectionId.INVALID, e.RawData))
            } else if (e.Type == NetEventType.NewConnection) {
                var t = this.mInSignaling[e.ConnectionId.id];
                if (t) {
                    t.StartSignaling()
                } else {
                    this.AddIncomingConnection(e.ConnectionId)
                }
            } else if (e.Type == NetEventType.ConnectionFailed) {
                this.SignalingFailed(e.ConnectionId)
            } else if (e.Type == NetEventType.Disconnected) {
                var t = this.mInSignaling[e.ConnectionId.id];
                if (t) {
                    t.SignalingInfo.SignalingDisconnected()
                }
            } else if (e.Type == NetEventType.ReliableMessageReceived) {
                var t = this.mInSignaling[e.ConnectionId.id];
                if (t) {
                    var n = this.BufferToString(e.MessageData);
                    t.AddSignalingMessage(n)
                } else {
                    Debug.LogWarning("Signaling message from unknown connection received")
                }
            }
        }
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
            var a = r[o];
            this.HandleDisconnect(a)
        }
    };
    e.prototype.AddOutgoingConnection = function(e) {
        Debug.Log("new outgoing connection");
        var t = this.mSignalingNetwork.Connect(e);
        var n = new SignalingInfo(t, false, Date.now());
        var i = this.CreatePeer(this.NextConnectionId(), this.mConfigUrls);
        i.SetSignalingInfo(n);
        this.mInSignaling[t.id] = i;
        return i.ConnectionId
    };
    e.prototype.AddIncomingConnection = function(e) {
        Debug.Log("new incoming connection");
        var t = new SignalingInfo(e, true, Date.now());
        var n = this.CreatePeer(this.NextConnectionId(), this.mConfigUrls);
        n.SetSignalingInfo(t);
        this.mInSignaling[e.id] = n;
        n.NegotiateSignaling();
        return n.ConnectionId
    };
    e.prototype.ConnectionEstablished = function(e) {
        var t = this.mInSignaling[e.id];
        delete this.mInSignaling[e.id];
        this.mSignalingNetwork.Disconnect(e);
        this.mConnectionIds.push(t.ConnectionId);
        this.mIdToConnection[t.ConnectionId.id] = t;
        this.mEvents.Enqueue(new NetworkEvent(NetEventType.NewConnection, t.ConnectionId, null))
    };
    e.prototype.SignalingFailed = function(e) {
        var t = this.mInSignaling[e.id];
        if (t) {
            delete this.mInSignaling[e.id];
            this.mEvents.Enqueue(new NetworkEvent(NetEventType.ConnectionFailed, t.ConnectionId, null));
            if (t.SignalingInfo.IsSignalingConnected()) {
                this.mSignalingNetwork.Disconnect(e)
            }
            t.Dispose()
        }
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
        var i = new NetworkEvent(NetEventType.Disconnected, e, null);
        this.mEvents.Enqueue(i)
    };
    e.prototype.NextConnectionId = function() {
        var e = new ConnectionId(this.mNextId.id);
        this.mNextId.id++;
        return e
    };
    e.prototype.StringToBuffer = function(e) {
        var t = new ArrayBuffer(e.length * 2);
        var n = new Uint16Array(t);
        for (var i = 0, o = e.length; i < o; i++) {
            n[i] = e.charCodeAt(i)
        }
        var r = new Uint8Array(t);
        return r
    };
    e.prototype.BufferToString = function(e) {
        var t = new Uint16Array(e.buffer, e.byteOffset, e.byteLength / 2);
        return String.fromCharCode.apply(null, t)
    };
    return e
}();