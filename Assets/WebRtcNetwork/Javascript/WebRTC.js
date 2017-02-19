var gCAPIWebRtcNetworkInstances = {};
var gCAPIWebRtcNetworkInstancesNextIndex = 1;

function CAPIWebRtcNetworkIsAvailable() {
    if (window.RTCPeerConnection || window.webkitRTCPeerConnection || window.mozRTCPeerConnection) return true;
    return false
}

function CAPIWebRtcNetworkCreate(e) {
    console.debug("CAPIWebRtcNetworkCreate called");
    var t = gCAPIWebRtcNetworkInstancesNextIndex;
    gCAPIWebRtcNetworkInstancesNextIndex++;
    var n = "WebsocketNetwork";
    var i = "ws://localhost:12777";
    var o = ["stun:stun.services.mozilla.com"];
    if (e == null || typeof e !== "string" || e.length === 0) {
        console.debug("invalid configuration. use default");
        var r = new SignalingConfig(new WebsocketNetwork(i));
        gCAPIWebRtcNetworkInstances[t] = new WebRtcNetwork(r, ["stun:stun.services.mozilla.com"])
    } else {
        console.debug("parsing configuration");
        var a = JSON.parse(e);
        if (a) {
            if (a.signaling) {
                n = a.signaling.class;
                i = a.signaling.param
            }
            if (a.iceServers) {
                o = a.iceServers
            }
        }
        var s = window[n];
        var r = new SignalingConfig(new s(i));
        console.debug("setup webrtc network");
        gCAPIWebRtcNetworkInstances[t] = new WebRtcNetwork(r, o)
    }
    return t
}

function CAPIWebRtcNetworkRelease(e) {
    if (e in gCAPIWebRtcNetworkInstances) {
        gCAPIWebRtcNetworkInstances[e].Dispose();
        delete gCAPIWebRtcNetworkInstances[e]
    }
}

function CAPIWebRtcNetworkConnect(e, t) {
    return gCAPIWebRtcNetworkInstances[e].Connect(t)
}

function CAPIWebRtcNetworkStartServer(e, t) {
    gCAPIWebRtcNetworkInstances[e].StartServer(t)
}

function CAPIWebRtcNetworkStopServer(e) {
    gCAPIWebRtcNetworkInstances[e].StopServer()
}

function CAPIWebRtcNetworkDisconnect(e, t) {
    gCAPIWebRtcNetworkInstances[e].Disconnect(new ConnectionId(t))
}

function CAPIWebRtcNetworkShutdown(e) {
    gCAPIWebRtcNetworkInstances[e].Shutdown()
}

function CAPIWebRtcNetworkUpdate(e) {
    gCAPIWebRtcNetworkInstances[e].Update()
}

function CAPIWebRtcNetworkFlush(e) {
    gCAPIWebRtcNetworkInstances[e].Flush()
}

function CAPIWebRtcNetworkSendData(e, t, n, i) {
    gCAPIWebRtcNetworkInstances[e].SendData(new ConnectionId(t), n, i)
}

function CAPIWebRtcNetworkSendDataEm(e, t, n, i, o, r) {
    var a = new Uint8Array(n.buffer, i, o);
    gCAPIWebRtcNetworkInstances[e].SendData(new ConnectionId(t), a, r)
}

function CAPIWebRtcNetworkDequeue(e) {
    return gCAPIWebRtcNetworkInstances[e].Dequeue()
}

function CAPIWebRtcNetworkPeek(e) {
    return gCAPIWebRtcNetworkInstances[e].Peek()
}

function CAPIWebRtcNetworkPeekEventDataLength(e) {
    var t = gCAPIWebRtcNetworkInstances[e].Peek();
    return CAPIWebRtcNetworkCheckEventLength(t)
}

function CAPIWebRtcNetworkCheckEventLength(e) {
    if (e == null) {
        return -1
    } else if (e.RawData == null) {
        return 0
    } else if (typeof e.RawData === "string") {
        return e.RawData.length
    } else {
        return e.RawData.length
    }
}

function CAPIWebRtcNetworkEventDataToUint8Array(e, t, n, i) {
    if (e == null) {
        return 0
    } else if (typeof e === "string") {
        var o = 0;
        for (o = 0; o < e.length && o < i; o++) {
            t[n + o] = e.charCodeAt(o)
        }
        return o
    } else {
        var o = 0;
        for (o = 0; o < e.length && o < i; o++) {
            t[n + o] = e[o]
        }
        return o
    }
}

function CAPIWebRtcNetworkDequeueEm(e, t, n, i, o, r, a, s, c, l) {
    var u = CAPIWebRtcNetworkDequeue(e);
    if (u == null) return false;
    t[n] = u.Type;
    i[o] = u.ConnectionId.id;
    var g = CAPIWebRtcNetworkEventDataToUint8Array(u.RawData, r, a, s);
    c[l] = g;
    return true
}

function CAPIWebRtcNetworkPeekEm(e, t, n, i, o, r, a, s, c, l) {
    var u = CAPIWebRtcNetworkPeek(e);
    if (u == null) return false;
    t[n] = u.Type;
    i[o] = u.ConnectionId.id;
    var g = CAPIWebRtcNetworkEventDataToUint8Array(u.RawData, r, a, s);
    c[l] = g;
    return true
}
var DefaultValues = function() {
    function e() {}
    Object.defineProperty(e, "DefaultIceServers", {
        get: function() {
            return e.mDefaultIceServer
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(e, "DefaultSignalingServer", {
        get: function() {
            return e.mDefaultSignalingServer
        },
        enumerable: true,
        configurable: true
    });
    e.mDefaultIceServer = ["stun:stun.services.mozilla.com"];
    e.mDefaultSignalingServer = "wss://because-why-not.com:12777";
    return e
}();
