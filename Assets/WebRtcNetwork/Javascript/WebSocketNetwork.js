var WebsocketNetwork = function() {
    function e(e) {
        this.mStatus = WebsocketConnectionStatus.Uninitialized;
        this.mOutgoingQueue = new Array;
        this.mIncomingQueue = new Array;
        this.mServerStatus = WebsocketServerStatus.Offline;
        this.mConnecting = new Array;
        this.mConnections = new Array;
        this.mNextOutgoingConnectionId = new ConnectionId(1);
        this.mUrl = null;
        this.mIsDisposed = false;
        this.mUrl = e;
        this.mStatus = WebsocketConnectionStatus.NotConnected
    }
    e.prototype.getStatus = function() {
        return this.mStatus
    };
    e.prototype.WebsocketConnect = function() {
        var e = this;
        this.mStatus = WebsocketConnectionStatus.Connecting;
        this.mSocket = new WebSocket(this.mUrl);
        this.mSocket.binaryType = "arraybuffer";
        this.mSocket.onopen = function() {
            e.OnWebsocketOnOpen()
        };
        this.mSocket.onerror = function(t) {
            e.OnWebsocketOnError(t)
        };
        this.mSocket.onmessage = function(t) {
            e.OnWebsocketOnMessage(t)
        };
        this.mSocket.onclose = function(t) {
            e.OnWebsocketOnClose(t)
        }
    };
    e.prototype.WebsocketCleanup = function() {
        this.mSocket.onopen = null;
        this.mSocket.onerror = null;
        this.mSocket.onmessage = null;
        this.mSocket.onclose = null;
        if (this.mSocket.readyState == this.mSocket.OPEN || this.mSocket.readyState == this.mSocket.CONNECTING) {
            this.mSocket.close()
        }
        this.mSocket = null
    };
    e.prototype.EnsureServerConnection = function() {
        if (this.mStatus == WebsocketConnectionStatus.NotConnected) {
            this.WebsocketConnect()
        }
    };
    e.prototype.CheckSleep = function() {
        if (this.mStatus == WebsocketConnectionStatus.Connected && this.mServerStatus == WebsocketServerStatus.Offline && this.mConnecting.length == 0 && this.mConnections.length == 0) {
            this.Cleanup()
        }
    };
    e.prototype.OnWebsocketOnOpen = function() {
        console.log("onWebsocketOnOpen");
        this.mStatus = WebsocketConnectionStatus.Connected
    };
    e.prototype.OnWebsocketOnClose = function(e) {
        console.log("Closed: " + JSON.stringify(e));
        if (this.mStatus == WebsocketConnectionStatus.Disconnecting || this.mStatus == WebsocketConnectionStatus.NotConnected) return;
        this.Cleanup();
        this.mStatus = WebsocketConnectionStatus.NotConnected
    };
    e.prototype.OnWebsocketOnMessage = function(e) {
        if (this.mStatus == WebsocketConnectionStatus.Disconnecting || this.mStatus == WebsocketConnectionStatus.NotConnected) return;
        var t = NetworkEvent.fromByteArray(new Uint8Array(e.data));
        this.HandleIncomingEvent(t)
    };
    e.prototype.OnWebsocketOnError = function(e) {
        if (this.mStatus == WebsocketConnectionStatus.Disconnecting || this.mStatus == WebsocketConnectionStatus.NotConnected) return;
        console.log("WebSocket Error " + e)
    };
    e.prototype.Cleanup = function() {
        if (this.mStatus == WebsocketConnectionStatus.Disconnecting || this.mStatus == WebsocketConnectionStatus.NotConnected) return;
        this.mStatus = WebsocketConnectionStatus.Disconnecting;
        for (var e = 0, t = this.mConnecting; e < t.length; e++) {
            var n = t[e];
            this.EnqueueIncoming(new NetworkEvent(NetEventType.ConnectionFailed, new ConnectionId(n), null))
        }
        this.mConnecting = new Array;
        for (var i = 0, o = this.mConnections; i < o.length; i++) {
            var n = o[i];
            this.EnqueueIncoming(new NetworkEvent(NetEventType.Disconnected, new ConnectionId(n), null))
        }
        this.mConnections = new Array;
        if (this.mServerStatus == WebsocketServerStatus.Starting) {
            this.EnqueueIncoming(new NetworkEvent(NetEventType.ServerInitFailed, ConnectionId.INVALID, null))
        } else if (this.mServerStatus == WebsocketServerStatus.Online) {
            this.EnqueueIncoming(new NetworkEvent(NetEventType.ServerClosed, ConnectionId.INVALID, null))
        } else if (this.mServerStatus == WebsocketServerStatus.ShuttingDown) {
            this.EnqueueIncoming(new NetworkEvent(NetEventType.ServerClosed, ConnectionId.INVALID, null))
        }
        this.mServerStatus = WebsocketServerStatus.Offline;
        this.mOutgoingQueue = new Array;
        this.WebsocketCleanup();
        this.mStatus = WebsocketConnectionStatus.NotConnected
    };
    e.prototype.EnqueueOutgoing = function(e) {
        this.mOutgoingQueue.push(e)
    };
    e.prototype.EnqueueIncoming = function(e) {
        this.mIncomingQueue.push(e)
    };
    e.prototype.TryRemoveConnecting = function(e) {
        var t = this.mConnecting.indexOf(e.id);
        if (t != -1) {
            this.mConnecting.splice(t, 1)
        }
    };
    e.prototype.TryRemoveConnection = function(e) {
        var t = this.mConnections.indexOf(e.id);
        if (t != -1) {
            this.mConnections.splice(t, 1)
        }
    };
    e.prototype.HandleIncomingEvent = function(e) {
        if (e.Type == NetEventType.NewConnection) {
            this.TryRemoveConnecting(e.ConnectionId);
            this.mConnections.push(e.ConnectionId.id)
        } else if (e.Type == NetEventType.ConnectionFailed) {
            this.TryRemoveConnecting(e.ConnectionId)
        } else if (e.Type == NetEventType.Disconnected) {
            this.TryRemoveConnection(e.ConnectionId)
        } else if (e.Type == NetEventType.ServerInitialized) {
            this.mServerStatus = WebsocketServerStatus.Online
        } else if (e.Type == NetEventType.ServerInitFailed) {
            this.mServerStatus = WebsocketServerStatus.Offline
        } else if (e.Type == NetEventType.ServerClosed) {
            this.mServerStatus = WebsocketServerStatus.ShuttingDown;
            this.mServerStatus = WebsocketServerStatus.Offline
        }
        this.EnqueueIncoming(e)
    };
    e.prototype.HandleOutgoingEvents = function() {
        while (this.mOutgoingQueue.length > 0) {
            var e = this.mOutgoingQueue.shift();
            var t = NetworkEvent.toByteArray(e);
            this.mSocket.send(t)
        }
    };
    e.prototype.NextConnectionId = function() {
        var e = this.mNextOutgoingConnectionId;
        this.mNextOutgoingConnectionId = new ConnectionId(this.mNextOutgoingConnectionId.id + 1);
        return e
    };
    e.prototype.GetRandomKey = function() {
        var e = "";
        for (var t = 0; t < 7; t++) {
            e += String.fromCharCode(65 + Math.round(Math.random() * 25))
        }
        return e
    };
    e.prototype.Dequeue = function() {
        if (this.mIncomingQueue.length > 0) return this.mIncomingQueue.shift();
        return null
    };
    e.prototype.Peek = function() {
        if (this.mIncomingQueue.length > 0) return this.mIncomingQueue[0];
        return null
    };
    e.prototype.Update = function() {
        this.CheckSleep()
    };
    e.prototype.Flush = function() {
        if (this.mStatus == WebsocketConnectionStatus.Connected) this.HandleOutgoingEvents()
    };
    e.prototype.SendData = function(e, t, n) {
        if (e == null || t == null || t.length == 0) return;
        var i;
        if (n) {
            i = new NetworkEvent(NetEventType.ReliableMessageReceived, e, t)
        } else {
            i = new NetworkEvent(NetEventType.UnreliableMessageReceived, e, t)
        }
        this.EnqueueOutgoing(i)
    };
    e.prototype.Disconnect = function(e) {
        var t = new NetworkEvent(NetEventType.Disconnected, e, null);
        this.EnqueueOutgoing(t)
    };
    e.prototype.Shutdown = function() {
        this.Cleanup();
        this.mStatus = WebsocketConnectionStatus.NotConnected
    };
    e.prototype.Dispose = function() {
        if (this.mIsDisposed == false) {
            this.Shutdown();
            this.mIsDisposed = true
        }
    };
    e.prototype.StartServer = function(e) {
        if (e == null) {
            e = "" + this.GetRandomKey()
        }
        if (this.mServerStatus == WebsocketServerStatus.Offline) {
            this.EnsureServerConnection();
            this.mServerStatus = WebsocketServerStatus.Starting;
            this.EnqueueOutgoing(new NetworkEvent(NetEventType.ServerInitialized, ConnectionId.INVALID, e))
        } else {
            this.EnqueueIncoming(new NetworkEvent(NetEventType.ServerInitFailed, ConnectionId.INVALID, e))
        }
    };
    e.prototype.StopServer = function() {
        this.EnqueueOutgoing(new NetworkEvent(NetEventType.ServerClosed, ConnectionId.INVALID, null))
    };
    e.prototype.Connect = function(e) {
        this.EnsureServerConnection();
        var t = this.NextConnectionId();
        this.mConnecting.push(t.id);
        var n = new NetworkEvent(NetEventType.NewConnection, t, e);
        this.EnqueueOutgoing(n);
        return t
    };
    return e
}();