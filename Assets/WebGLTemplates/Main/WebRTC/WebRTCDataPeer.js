var WebRtcDataPeer = function(e) {
    __extends(t, e);

    function t(t, n) {
        e.call(this, n);
        this.mInfo = null;
        this.mEvents = new Queue;
        this.mReliableDataChannelReady = false;
        this.mUnreliableDataChannelReady = false;
        this.mConnectionId = t
    }
    Object.defineProperty(t.prototype, "ConnectionId", {
        get: function() {
            return this.mConnectionId
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(t.prototype, "SignalingInfo", {
        get: function() {
            return this.mInfo
        },
        enumerable: true,
        configurable: true
    });
    t.prototype.SetSignalingInfo = function(e) {
        this.mInfo = e
    };
    t.prototype.OnSetup = function() {
        var e = this;
        this.mPeer.ondatachannel = function(t) {
            e.OnDataChannel(t.channel)
        }
    };
    t.prototype.OnStartSignaling = function() {
        var e = {};
        this.mReliableDataChannel = this.mPeer.createDataChannel(t.sLabelReliable, e);
        this.RegisterObserverReliable();
        var n = {};
        n.maxRetransmits = 0;
        n.ordered = false;
        this.mUnreliableDataChannel = this.mPeer.createDataChannel(t.sLabelUnreliable, n);
        this.RegisterObserverUnreliable()
    };
    t.prototype.OnCleanup = function() {
        if (this.mReliableDataChannel != null) this.mReliableDataChannel.close();
        if (this.mUnreliableDataChannel != null) this.mUnreliableDataChannel.close()
    };
    t.prototype.RegisterObserverReliable = function() {
        var e = this;
        this.mReliableDataChannel.onmessage = function(t) {
            e.ReliableDataChannel_OnMessage(t)
        };
        this.mReliableDataChannel.onopen = function(t) {
            e.ReliableDataChannel_OnOpen()
        };
        this.mReliableDataChannel.onclose = function(t) {
            e.ReliableDataChannel_OnClose()
        };
        this.mReliableDataChannel.onerror = function(t) {
            e.ReliableDataChannel_OnError("")
        }
    };
    t.prototype.RegisterObserverUnreliable = function() {
        var e = this;
        this.mUnreliableDataChannel.onmessage = function(t) {
            e.UnreliableDataChannel_OnMessage(t)
        };
        this.mUnreliableDataChannel.onopen = function(t) {
            e.UnreliableDataChannel_OnOpen()
        };
        this.mUnreliableDataChannel.onclose = function(t) {
            e.UnreliableDataChannel_OnClose()
        };
        this.mUnreliableDataChannel.onerror = function(t) {
            e.UnreliableDataChannel_OnError("")
        }
    };
    t.prototype.SendData = function(e, t) {
        var n = e;
        if (t) {
            this.mReliableDataChannel.send(n)
        } else {
            this.mUnreliableDataChannel.send(n)
        }
    };
    t.prototype.DequeueEvent = function(e) {
        {
            if (this.mEvents.Count() > 0) {
                e.val = this.mEvents.Dequeue();
                return true
            }
        }
        return false
    };
    t.prototype.Enqueue = function(e) {
        {
            this.mEvents.Enqueue(e)
        }
    };
    t.prototype.OnDataChannel = function(e) {
        var n = e;
        if (n.label == t.sLabelReliable) {
            this.mReliableDataChannel = n;
            this.RegisterObserverReliable()
        } else if (n.label == t.sLabelUnreliable) {
            this.mUnreliableDataChannel = n;
            this.RegisterObserverUnreliable()
        } else {
            Debug.LogError("Datachannel with unexpected label " + n.label)
        }
    };
    t.prototype.RtcOnMessageReceived = function(e, t) {
        var n = NetEventType.UnreliableMessageReceived;
        if (t) {
            n = NetEventType.ReliableMessageReceived
        }
        if (e.data instanceof ArrayBuffer) {
            var i = new Uint8Array(e.data);
            this.Enqueue(new NetworkEvent(n, this.mConnectionId, i))
        } else if (e.data instanceof Blob) {
            var o = this.mConnectionId;
            var r = new FileReader;
            var a = this;
            r.onload = function() {
                var e = new Uint8Array(this.result);
                a.Enqueue(new NetworkEvent(n, a.mConnectionId, e))
            };
            r.readAsArrayBuffer(e.data)
        } else {
            Debug.LogError("Invalid message type. Only blob and arraybuffer supported: " + e.data)
        }
    };
    t.prototype.ReliableDataChannel_OnMessage = function(e) {
        this.RtcOnMessageReceived(e, true)
    };
    t.prototype.ReliableDataChannel_OnOpen = function() {
        Debug.Log("mReliableDataChannelReady");
        this.mReliableDataChannelReady = true;
        if (this.IsRtcConnected()) {
            this.RtcSetConnected();
            Debug.Log("Fully connected")
        }
    };
    t.prototype.ReliableDataChannel_OnClose = function() {
        this.RtcSetClosed()
    };
    t.prototype.ReliableDataChannel_OnError = function(e) {
        Debug.LogError(e);
        this.RtcSetClosed()
    };
    t.prototype.UnreliableDataChannel_OnMessage = function(e) {
        this.RtcOnMessageReceived(e, false)
    };
    t.prototype.UnreliableDataChannel_OnOpen = function() {
        Debug.Log("mUnreliableDataChannelReady");
        this.mUnreliableDataChannelReady = true;
        if (this.IsRtcConnected()) {
            this.RtcSetConnected();
        }
    };
    t.prototype.UnreliableDataChannel_OnClose = function() {
        this.RtcSetClosed()
    };
    t.prototype.UnreliableDataChannel_OnError = function(e) {
        Debug.LogError(e);
        this.RtcSetClosed()
    };
    t.prototype.IsRtcConnected = function() {
        return this.mReliableDataChannelReady && this.mUnreliableDataChannelReady
    };
    t.sLabelReliable = "reliable";
    t.sLabelUnreliable = "unreliable";
    return t
}(AWebRtcPeer);