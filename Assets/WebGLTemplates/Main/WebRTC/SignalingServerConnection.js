var SignalingServerConnection = function () {
    function e(url) {
        this.signalingEvents = new Queue;
        this.mOutgoingQueue = new Array;
        this.mIncomingQueue = new Array;
        this.mServerStatus = WebsocketServerStatus.Offline;
        this.mConnecting = new Array;
        this.mConnections = new Array;
        this.mNextOutgoingConnectionId = new ConnectionId(1);
        this.mIsDisposed = false;
        this.socketConnection = new WebSocketConnection(this, url);
        this.peerNetwork = null;
    }
    e.prototype.SetPeerNetwork = function (peerNetwork) {
        this.peerNetwork = peerNetwork;
    }
    e.prototype.getStatus = function () {
        return this.socketConnection.status
    };

    e.prototype.CheckSleep = function () {
        if (this.socketConnection.mStatus == WebsocketConnectionStatus.Connected && this.mServerStatus == WebsocketServerStatus.Offline && this.mConnecting.length == 0 && this.mConnections.length == 0) {
            this.Cleanup()
        }
    };

    e.prototype.UpdateSignalingNetwork = function () {
        this.CheckSleep();
        var event;
        while ((event = this.Dequeue()) != null) {
            console.log("new event " + event);
            this.peerNetwork.IncomingSignalingEvent(event);
            if (event.Type == NetEventType.ServerInitialized || event.Type == NetEventType.ServerInitFailed || event.Type == NetEventType.ServerClosed) {
            console.log("Enqueue event in signaling " + event);
                this.signalingEvents.Enqueue(new NetworkEvent(event.Type, ConnectionId.INVALID, event.RawData));
            }
        }
    };

    e.prototype.Cleanup = function () {
        if (this.socketConnection.status == WebsocketConnectionStatus.Disconnecting || this.socketConnection.status == WebsocketConnectionStatus.NotConnected) return;
        this.socketConnection.status = WebsocketConnectionStatus.Disconnecting;
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
        this.socketConnection.Cleanup();
        this.socketConnection.status = WebsocketConnectionStatus.NotConnected
    };
    e.prototype.EnqueueOutgoing = function (e) {
        this.mOutgoingQueue.push(e)
    };
    e.prototype.EnqueueIncoming = function (e) {
        this.mIncomingQueue.push(e)
    };
    e.prototype.TryRemoveConnecting = function (e) {
        var t = this.mConnecting.indexOf(e.id);
        if (t != -1) {
            this.mConnecting.splice(t, 1)
        }
    };
    e.prototype.TryRemoveConnection = function (e) {
        var t = this.mConnections.indexOf(e.id);
        if (t != -1) {
            this.mConnections.splice(t, 1)
        }
    };
    e.prototype.HandleIncomingEvent = function (e) {
        console.log(e.Type);
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
            this.mServerStatus = WebsocketServerStatus.Offline
        }
        this.EnqueueIncoming(e)
    };
    e.prototype.HandleOutgoingEvents = function () {
        while (this.mOutgoingQueue.length > 0) {
            this.socketConnection.Send(this.mOutgoingQueue.shift());
        }
    };
    e.prototype.NextConnectionId = function () {
        var e = this.mNextOutgoingConnectionId;
        this.mNextOutgoingConnectionId = new ConnectionId(this.mNextOutgoingConnectionId.id + 1);
        return e
    };
    e.prototype.GetRandomKey = function () {
        var e = "";
        for (var t = 0; t < 7; t++) {
            e += String.fromCharCode(65 + Math.round(Math.random() * 25))
        }
        return e
    };
    e.prototype.Dequeue = function () {
        if (this.mIncomingQueue.length > 0) return this.mIncomingQueue.shift();
        return null
    };
    e.prototype.Peek = function () {
        if (this.mIncomingQueue.length > 0) return this.mIncomingQueue[0];
        return null
    };
    e.prototype.Flush = function () {
        if (this.socketConnection.status == WebsocketConnectionStatus.Connected) this.HandleOutgoingEvents()
    };
    e.prototype.SendData = function (e, t, n) {
        if (e == null || t == null || t.length == 0) return;
        var i;
        if (n) {
            i = new NetworkEvent(NetEventType.ReliableMessageReceived, e, t)
        } else {
            i = new NetworkEvent(NetEventType.UnreliableMessageReceived, e, t)
        }
        this.EnqueueOutgoing(i)
    };
    e.prototype.DisconnectPeerFromServer = function (e) {
        var t = new NetworkEvent(NetEventType.Disconnected, e, null);
        this.EnqueueOutgoing(t)
    };
    e.prototype.Shutdown = function () {
        this.Cleanup();
        this.LeaveRoom();
        this.socketConnection.status = WebsocketConnectionStatus.NotConnected
    };
    e.prototype.Dispose = function () {
        if (this.mIsDisposed == false) {
            this.Shutdown();
            this.mIsDisposed = true
        }
    };
    e.prototype.ConnectToServer = function () {

        if (this.mServerStatus == WebsocketServerStatus.Offline) {
            this.mServerStatus = WebsocketServerStatus.Starting;
            this.socketConnection.EnsureServerConnection();
        } else {
            this.EnqueueIncoming(new NetworkEvent(NetEventType.ServerInitFailed, ConnectionId.INVALID, e))
        }
    }
    e.prototype.DisconnectFromServer = function (e) {
        console.error("Disconnect From Server  : Not implemented yet");
    };

    e.prototype.CreateRoom = function (e) {
        if (e == null) {
            e = "" + this.GetRandomKey()
        }
        this.EnqueueOutgoing(new NetworkEvent(NetEventType.ServerInitialized, ConnectionId.INVALID, e))
    };

    e.prototype.LeaveRoom = function () {
        this.EnqueueOutgoing(new NetworkEvent(NetEventType.ServerClosed, ConnectionId.INVALID, null))
    };

    e.prototype.ConnectToRoom = function (e) {
        console.log("ConnectToRoom");
        this.socketConnection.EnsureServerConnection();
        var t = this.NextConnectionId();
        this.mConnecting.push(t.id);
        var n = new NetworkEvent(NetEventType.NewConnection, t, e);
        this.EnqueueOutgoing(n);
        this.peerNetwork.AddOutgoingConnection(t);

        return t
    };
    return e
}();