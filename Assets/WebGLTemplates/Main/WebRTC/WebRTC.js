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
    var i = "ws://localhost:12777";
    var config = ["stun:stun.services.mozilla.com"];
    if (e == null || typeof e !== "string" || e.length === 0) {
        console.error("invalid configuration. use default");
        //var r = new SignalingServerConnection(i);
        gCAPIWebRtcNetworkInstances[t] = null//new PeerNetwork(r, ["stun:stun.services.mozilla.com"])
    } else {
        console.debug("parsing configuration");
        var a = JSON.parse(e);
        if (a) {
            if (a.signaling) {
                n = a.signaling.class;
                i = a.signaling.param
            }
            if (a.iceServers) {
                config = a.iceServers
            }
        }
        var signalingServerConnection = new SignalingServerConnection(i);
        console.debug("setup webrtc network");
        var peerNetwork = new PeerNetwork(signalingServerConnection, config);
        gCAPIWebRtcNetworkInstances[t] = new WebRtcNetwork(signalingServerConnection,peerNetwork);
    }
    return t
}

function CAPIWebRtcNetworkRelease(e) {
    if (e in gCAPIWebRtcNetworkInstances) {
        gCAPIWebRtcNetworkInstances[e].Dispose();
        delete gCAPIWebRtcNetworkInstances[e]
    }
}

function CAPIWebRtcNetworkUpdateNetwork(e) {
    gCAPIWebRtcNetworkInstances[e].UpdateNetwork()
}
function CAPIWebRtcNetworkFlush(e) {
    gCAPIWebRtcNetworkInstances[e].Flush()
}
function CAPIWebRtcNetworkConnectToServer(e) {
    return gCAPIWebRtcNetworkInstances[e].signalingServerConnection.ConnectToServer()
}
function CAPIWebRtcNetworkConnectToRoom(e, t) {
    return gCAPIWebRtcNetworkInstances[e].signalingServerConnection.ConnectToRoom(t)
}
function CAPIWebRtcNetworkCreateRoom(e, t) {
    gCAPIWebRtcNetworkInstances[e].signalingServerConnection.CreateRoom(t)
}
function CAPIWebRtcNetworkLeaveRoom(e) {
    gCAPIWebRtcNetworkInstances[e].signalingServerConnection.LeaveRoom()
}
function CAPIWebRtcNetworkDisconnectFromServer(e) {
    gCAPIWebRtcNetworkInstances[e].signalingServerConnection.DisconnectFromServer()
}
function CAPIWebRtcNetworkDisconnectFromPeer(e, t) {
    gCAPIWebRtcNetworkInstances[e].peerNetwork.DisconnectFromPeer(new ConnectionId(t))
}
function CAPIWebRtcNetworkSendData(e, t, n, i) {
    gCAPIWebRtcNetworkInstances[e].peerNetwork.SendData(new ConnectionId(t), n, i)
}
function Empty(e, t, n, i) {
}
/*$(document).ready(function() {
    document.onkeydown = function(e) {
        console.log("CharCode " + e.keyCode);
        if (e.keyCode == 88){ //x
            var t0 = performance.now();
            gCAPIWebRtcNetworkInstances[1].peerNetwork.TestSendData();
           var t1 = performance.now();
            console.log('Took ' +  (t1 - t0).toFixed(4)+ ' milliseconds to send:');
        }
    }
});*/
function CAPIWebRtcNetworkSendPeerDataEm(e, t, n, i, o, r) {
    var a = new Uint8Array(n.buffer, i, o);
    gCAPIWebRtcNetworkInstances[e].peerNetwork.SendData(new ConnectionId(t), a, r);
}
function CAPIWebRtcNetworkSendSignalingDataEm(e, connectionId,type_int, content) {
    console.log("Send Signaling Event : " + connectionId + "     " + type_int);
    gCAPIWebRtcNetworkInstances[e].signalingServerConnection.SendData(type_int, new ConnectionId(connectionId), content);
}
function CAPIWebRtcNetworkShutdown(e) {
    gCAPIWebRtcNetworkInstances[e].Shutdown();
}

function CAPIWebRtcNetworkPeekEventDataLength(signaling,e) {
    var u;
    if (signaling)
        u = gCAPIWebRtcNetworkInstances[e].signalingServerConnection.signalingEvents.Peek();
    else
        u = gCAPIWebRtcNetworkInstances[e].peerNetwork.mEvents.Peek();
    return CAPIWebRtcNetworkCheckEventLength(u);
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
function CAPIWebRtcNetworkDequeueEm(signaling, e, t, n, i, o, r, a, s, c, l) {
    var u;
    if (signaling)
        u = gCAPIWebRtcNetworkInstances[e].signalingServerConnection.signalingEvents.Dequeue();
    else {
        u = gCAPIWebRtcNetworkInstances[e].peerNetwork.mEvents.Dequeue();
    }
    if (u == null) return false;
    t[n] = u.Type;
    i[o] = u.ConnectionId.id;
    var g = CAPIWebRtcNetworkEventDataToUint8Array(u.RawData, r, a, s);
    c[l] = g;
    return true
}
function CAPIWebRtcNetworkPeekEm(signaling,e, t, n, i, o, r, a, s, c, l) {
    var u;
    if (signaling)
        u = gCAPIWebRtcNetworkInstances[e].signalingServerConnection.signalingEvents.Peek();
    else
        u = gCAPIWebRtcNetworkInstances[e].peerNetwork.mEvents.Peek();
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
