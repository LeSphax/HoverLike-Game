var LocalNetwork = function() {
    function e() {
        this.mNextNetworkId = new ConnectionId(1);
        this.mServerAddress = null;
        this.mEvents = new Queue;
        this.mConnectionNetwork = {};
        this.mIsDisposed = false;
        this.mId = e.sNextId;
        e.sNextId++
    }
    Object.defineProperty(e.prototype, "IsServer", {
        get: function() {
            return this.mServerAddress != null
        },
        enumerable: true,
        configurable: true
    });
    e.prototype.CreateRoom = function(t) {
        if (t === void 0) {
            t = null
        }
        if (t == null) t = "" + this.mId;
        if (t in e.mServers) {
            this.Enqueue(NetEventType.ServerInitFailed, ConnectionId.INVALID, t);
            return
        }
        e.mServers[t] = this;
        this.mServerAddress = t;
        this.Enqueue(NetEventType.ServerInitialized, ConnectionId.INVALID, t)
    };
    e.prototype.LeaveRoom = function() {
        if (this.IsServer) {
            this.Enqueue(NetEventType.ServerClosed, ConnectionId.INVALID, this.mServerAddress);
            delete e.mServers[this.mServerAddress];
            this.mServerAddress = null
        }
    };
    e.prototype.ConnectToRoom = function(t) {
        var n = this.NextConnectionId();
        var i = false;
        if (t in e.mServers) {
            var o = e.mServers[t];
            if (o != null) {
                o.ConnectClient(this);
                this.mConnectionNetwork[n.id] = e.mServers[t];
                this.Enqueue(NetEventType.NewConnection, n, null);
                i = true
            }
        }
        if (i == false) {
            this.Enqueue(NetEventType.ConnectionFailed, n, "Couldn't connect to the given server with id " + t)
        }
        return n
    };
    e.prototype.Shutdown = function() {
        for (var e in this.mConnectionNetwork) {
            this.DisconnectPeerFromServer(new ConnectionId((+e)))
        }
        this.LeaveRoom()
    };
    e.prototype.Dispose = function() {
        if (this.mIsDisposed == false) {
            this.Shutdown()
        }
    };
    e.prototype.SendData = function(e, t, n) {
        if (e.id in this.mConnectionNetwork) {
            var i = this.mConnectionNetwork[e.id];
            i.ReceiveData(this, t, n)
        }
    };
    e.prototype.Update = function() {
        this.CleanupWreakReferences()
    };
    e.prototype.Dequeue = function() {
        return this.mEvents.Dequeue()
    };
    e.prototype.Peek = function() {
        return this.mEvents.Peek()
    };
    e.prototype.Flush = function() {};
    e.prototype.DisconnectPeerFromServer = function(e) {
        if (e.id in this.mConnectionNetwork) {
            var t = this.mConnectionNetwork[e.id];
            if (t != null) {
                t.InternalDisconnectNetwork(this);
                this.InternalDisconnect(e)
            } else {
                this.CleanupWreakReferences()
            }
        }
    };
    e.prototype.FindConnectionId = function(e) {
        for (var t in this.mConnectionNetwork) {
            var n = this.mConnectionNetwork[t];
            if (n != null) {
                return new ConnectionId((+t))
            }
        }
        return ConnectionId.INVALID
    };
    e.prototype.NextConnectionId = function() {
        var e = this.mNextNetworkId;
        this.mNextNetworkId = new ConnectionId(e.id + 1);
        return e
    };
    e.prototype.ConnectClient = function(e) {
        var t = this.NextConnectionId();
        this.mConnectionNetwork[t.id] = e;
        this.Enqueue(NetEventType.NewConnection, t, null)
    };
    e.prototype.Enqueue = function(e, t, n) {
        var i = new NetworkEvent(e, t, n);
        this.mEvents.Enqueue(i)
    };
    e.prototype.ReceiveData = function(e, t, n) {
        var i = this.FindConnectionId(e);
        var o = new Uint8Array(t.length);
        for (var r = 0; r < o.length; r++) {
            o[r] = t[r]
        }
        var a = NetEventType.UnreliableMessageReceived;
        if (n) a = NetEventType.ReliableMessageReceived;
        this.Enqueue(a, i, o)
    };
    e.prototype.InternalDisconnect = function(e) {
        if (e.id in this.mConnectionNetwork) {
            this.Enqueue(NetEventType.Disconnected, e, null);
            delete this.mConnectionNetwork[e.id]
        }
    };
    e.prototype.InternalDisconnectNetwork = function(e) {
        this.InternalDisconnect(this.FindConnectionId(e))
    };
    e.prototype.CleanupWreakReferences = function() {};
    e.sNextId = 1;
    e.mServers = {};
    return e
}();
