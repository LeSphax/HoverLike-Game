function runTests(){
    My_WebRtcNetwork_test1();
    //WebsocketNetwork_sharedaddress();
    //CAPIWebRtcNetwork_test1();
    //WebsocketNetwork_test1();
}

function My_WebRtcNetwork_test1() {
    console.log("My Test");
    var e = "test1234";
    
    var signalingUrl = "ws://sphaxtest.herokuapp.com";
    //if (window.location.protocol != "https:") {
    //    signalingUrl = "ws://localhost:12776"
    //} else {
    //    signalingUrl = "wss://localhost:12777"
    //}
    var n = new Array;
    n.push("stun:stun.services.mozilla.com");
    var signalingNetwork1 = new SignalingServerConnection(signalingUrl)
    var peerNetwork1 = new PeerNetwork(signalingNetwork1, n);
    var network1 = new WebRtcNetwork(signalingNetwork1, peerNetwork1);
    signalingNetwork1.ConnectToServer();
    signalingNetwork1.CreateRoom("Jambon");


    var signalingNetwork2 = new SignalingServerConnection(signalingUrl)
    var peerNetwork2 = new PeerNetwork(signalingNetwork2, n);
    var network2 = new WebRtcNetwork(signalingNetwork2, peerNetwork2);

    setInterval(function () {
        network1.UpdateNetwork();
        var event = null;
        while (event = network1.signalingServerConnection.signalingEvents.Dequeue()) {
            console.log("server inc: " + event.toString());
            if (event.Type == NetEventType.ServerInitialized) {
                console.log("server started. Address " + event.Info);
                console.log(network2);
                network2.signalingServerConnection.ConnectToRoom("Jambon");
            } else if (event.Type == NetEventType.ServerInitFailed) {
                console.error("server start failed")
            }
        }
        while (event = network1.peerNetwork.mEvents.Dequeue()) {
            if (event.Type == NetEventType.NewConnection) {
                console.log("server new incoming connection")
            } else if (event.Type == NetEventType.Disconnected) {
                console.log("server peer disconnected");
                console.log("server shutdown");
                network1.Shutdown()
            } else if (event.Type == NetEventType.ReliableMessageReceived) {
                network1.peerNetwork.SendData(event.ConnectionId, event.MessageData, true)
            } else if (event.Type == NetEventType.UnreliableMessageReceived) {
                network1.peerNetwork.SendData(event.ConnectionId, event.MessageData, false)
            }
        }
        network1.Flush();

        network2.UpdateNetwork();
        while (event = network2.signalingServerConnection.signalingEvents.Dequeue()) {
            console.log(event);
        }
        while (event = network2.peerNetwork.mEvents.Dequeue()) {
            console.log("client inc: " + event.toString());
            if (event.Type == NetEventType.NewConnection) {
                console.log("client connection established");
                var n = stringToBuffer(e);
                network2.peerNetwork.SendData(event.ConnectionId, n, true)
            } else if (event.Type == NetEventType.ReliableMessageReceived) {
                var r = bufferToString(event.MessageData);
                if (r != e) {
                    console.error("Test failed sent string %s but received string %s", e, r)
                }
                var n = stringToBuffer(e);
                network2.peerNetwork.SendData(event.ConnectionId, n, false)
            } else if (event.Type == NetEventType.UnreliableMessageReceived) {
                var r = bufferToString(event.MessageData);
                if (r != e) {
                    console.error("Test failed sent string %s but received string %s", e, r)
                }
                console.log("client disconnecting");
                network2.peerNetwork.DisconnectFromPeer(event.ConnectionId);
                console.log("client shutting down");
                network2.Shutdown()
            }
        }
        network2.Flush()
    }, 100)
}

function WebRtcNetwork_test1() {
    console.log("test1");
    var e = "test1234";
    var t;
    if (window.location.protocol != "https:") {
        t = "ws://localhost:12776"
    } else {
        t = "wss://localhost:12777"
    }
    var n = new Array;
    n.push("stun:stun.services.mozilla.com");
    var i = new PeerNetwork(new SignalingConfig(new LocalNetwork), n);
    i.CreateRoom();
    var o = new PeerNetwork(new SignalingConfig(new LocalNetwork), n);
    setInterval(function() {
        i.Update();
        var t = null;
        while (t = i.Dequeue()) {
            console.log("server inc: " + t.toString());
            if (t.Type == NetEventType.ServerInitialized) {
                console.log("server started. Address " + t.Info);
                o.ConnectToRoom(t.Info)
            } else if (t.Type == NetEventType.ServerInitFailed) {
                console.error("server start failed")
            } else if (t.Type == NetEventType.NewConnection) {
                console.log("server new incoming connection")
            } else if (t.Type == NetEventType.Disconnected) {
                console.log("server peer disconnected");
                console.log("server shutdown");
                i.Shutdown()
            } else if (t.Type == NetEventType.ReliableMessageReceived) {
                i.SendData(t.ConnectionId, t.MessageData, true)
            } else if (t.Type == NetEventType.UnreliableMessageReceived) {
                i.SendData(t.ConnectionId, t.MessageData, false)
            }
        }
        i.Flush();
        o.Update();
        while (t = o.Dequeue()) {
            console.log("client inc: " + t.toString());
            if (t.Type == NetEventType.NewConnection) {
                console.log("client connection established");
                var n = stringToBuffer(e);
                o.SendData(t.ConnectionId, n, true)
            } else if (t.Type == NetEventType.ReliableMessageReceived) {
                var r = bufferToString(t.MessageData);
                if (r != e) {
                    console.error("Test failed sent string %s but received string %s", e, r)
                }
                var n = stringToBuffer(e);
                o.SendData(t.ConnectionId, n, false)
            } else if (t.Type == NetEventType.UnreliableMessageReceived) {
                var r = bufferToString(t.MessageData);
                if (r != e) {
                    console.error("Test failed sent string %s but received string %s", e, r)
                }
                console.log("client disconnecting");
                o.DisconnectFromPeer(t.ConnectionId);
                console.log("client shutting down");
                o.Shutdown()
            }
        }
        o.Flush()
    }, 100)
}

function WebsocketNetwork_sharedaddress() {
    console.log("SignalingServerConnection shared address test");
    var e = "test1234";
    var t = false;
    var n = true;
    var i;
    var o;
    if (window.location.protocol != "https:" && n) {
        i = "wss://because-why-not.com:12776/testshare";
        if (t) i = "ws://localhost:12776/testshare"
    } else {
        i = "wss://because-why-not.com:12777/testshare";
        if (t) i = "wss://localhost:12777/testshare"
    }
var r = "sharedaddresstest";
var a = new SignalingServerConnection(i);
var s = new SignalingServerConnection(i);
var c = new SignalingServerConnection(i);
var l = stringToBuffer("network1 says hi");
var u = stringToBuffer("network2 says hi");
var g = stringToBuffer("network3 says hi");
a.CreateRoom(r);
s.CreateRoom(r);
c.CreateRoom(r);

function f(e, t) {
    e.Update();
    var n = null;
    while (n = e.Dequeue()) {
        if (n.Type == NetEventType.ServerInitFailed || n.Type == NetEventType.ConnectionFailed || n.Type == NetEventType.ServerClosed) {
            console.error(t + "inc: " + n.toString())
        } else {
            console.log(t + "inc: " + n.toString())
        }
        if (n.Type == NetEventType.ServerInitialized) {} else if (n.Type == NetEventType.ServerInitFailed) {} else if (n.Type == NetEventType.NewConnection) {
            var i = stringToBuffer(t + "says hi!");
            e.SendData(n.ConnectionId, i, true)
        } else if (n.Type == NetEventType.Disconnected) {} else if (n.Type == NetEventType.ReliableMessageReceived) {
            var o = bufferToString(n.MessageData);
            console.log(t + " received: " + o)
        } else if (n.Type == NetEventType.UnreliableMessageReceived) {}
    }
    e.Flush()
}
var h = 0;
setInterval(function() {
    f(a, "network1 ");
    f(s, "network2 ");
    f(c, "network3 ");
    h += 100;
    if (h == 1e4) {
        console.log("network1 shutdown");
        a.Shutdown()
    }
    if (h == 15e3) {
        console.log("network2 shutdown");
        s.Shutdown()
    }
    if (h == 2e4) {
        console.log("network3 shutdown");
        c.Shutdown()
    }
}, 100)
}

function CAPIWebRtcNetwork_test1() {
    console.log("test1");
    var e = "test1234";
    var t = '{ "signaling" :  { "class": "LocalNetwork", "param" : null}, "iceServers":["stun:stun.services.mozilla.com"]}';
    var n = CAPIWebRtcNetworkCreate(t);
    CAPIWebRtcNetworkStartServer(n, "Room1");
    var i = CAPIWebRtcNetworkCreate(t);
    setInterval(function() {
        CAPIWebRtcNetworkUpdate(n);
        var t = null;
        while (t = CAPIWebRtcNetworkDequeue(n)) {
            console.log("server inc: " + t.toString());
            if (t.Type == NetEventType.ServerInitialized) {
                console.log("server started. Address " + t.Info);
                CAPIWebRtcNetworkConnect(i, t.Info)
            } else if (t.Type == NetEventType.ServerInitFailed) {
                console.error("server start failed")
            } else if (t.Type == NetEventType.NewConnection) {
                console.log("server new incoming connection")
            } else if (t.Type == NetEventType.Disconnected) {
                console.log("server peer disconnected");
                console.log("server shutdown");
                CAPIWebRtcNetworkShutdown(n)
            } else if (t.Type == NetEventType.ReliableMessageReceived) {
                CAPIWebRtcNetworkSendData(n, t.ConnectionId.id, t.MessageData, true)
            } else if (t.Type == NetEventType.UnreliableMessageReceived) {
                CAPIWebRtcNetworkSendData(n, t.ConnectionId.id, t.MessageData, false)
            }
        }
        CAPIWebRtcNetworkFlush(n);
        CAPIWebRtcNetworkUpdate(i);
        while (t = CAPIWebRtcNetworkDequeue(i)) {
            console.log("client inc: " + t.toString());
            if (t.Type == NetEventType.NewConnection) {
                console.log("client connection established");
                var o = stringToBuffer(e);
                CAPIWebRtcNetworkSendData(i, t.ConnectionId.id, o, true)
            } else if (t.Type == NetEventType.ReliableMessageReceived) {
                var r = bufferToString(t.MessageData);
                if (r != e) {
                    console.error("Test failed sent string %s but received string %s", e, r)
                }
                var o = stringToBuffer(e);
                CAPIWebRtcNetworkSendData(i, t.ConnectionId.id, o, false)
            } else if (t.Type == NetEventType.UnreliableMessageReceived) {
                var r = bufferToString(t.MessageData);
                if (r != e) {
                    console.error("Test failed sent string %s but received string %s", e, r)
                }
                console.log("client disconnecting");
                CAPIWebRtcNetworkDisconnect(i, t.ConnectionId.id);
                console.log("client shutting down");
                CAPIWebRtcNetworkShutdown(i)
            }
        }
        CAPIWebRtcNetworkFlush(i)
    }, 100)
}


function WebsocketNetwork_test1() {
    console.log("test1");
    var e = "test1234";
    var t = false;
    var n = false;
    var i;
    var o;
    if (window.location.protocol != "https:" && n) {
        i = "wss://because-why-not.com:12776";
        if (t) i = "ws://localhost:12776"
    } else {
        i = "wss://because-why-not.com:12777";
        if (t) i = "wss://localhost:12777"
    }
var r = new SignalingServerConnection(i);
r.CreateRoom();
var a = new SignalingServerConnection(i);
setInterval(function() {
    r.Update();
    var t = null;
    while (t = r.Dequeue()) {
        console.log("server inc: " + t.toString());
        if (t.Type == NetEventType.ServerInitialized) {
            console.log("server started. Address " + t.Info);
            a.ConnectToRoom(t.Info)
        } else if (t.Type == NetEventType.ServerInitFailed) {
            console.error("server start failed")
        } else if (t.Type == NetEventType.NewConnection) {
            console.log("server new incoming connection")
        } else if (t.Type == NetEventType.Disconnected) {
            console.log("server peer disconnected");
            console.log("server shutdown");
            r.Shutdown()
        } else if (t.Type == NetEventType.ReliableMessageReceived) {
            r.SendData(t.ConnectionId, t.MessageData, true)
        } else if (t.Type == NetEventType.UnreliableMessageReceived) {
            r.SendData(t.ConnectionId, t.MessageData, false)
        }
    }
    r.Flush();
    a.Update();
    while (t = a.Dequeue()) {
        console.log("client inc: " + t.toString());
        if (t.Type == NetEventType.NewConnection) {
            console.log("client connection established");
            var n = stringToBuffer(e);
            a.SendData(t.ConnectionId, n, true)
        } else if (t.Type == NetEventType.ReliableMessageReceived) {
            var i = bufferToString(t.MessageData);
            if (i != e) {
                console.error("Test failed sent string %s but received string %s", e, i)
            }
            var n = stringToBuffer(e);
            a.SendData(t.ConnectionId, n, false)
        } else if (t.Type == NetEventType.UnreliableMessageReceived) {
            var i = bufferToString(t.MessageData);
            if (i != e) {
                console.error("Test failed sent string %s but received string %s", e, i)
            }
            console.log("client disconnecting");
            a.DisconnectFromPeer(t.ConnectionId);
            console.log("client shutting down");
            a.Shutdown()
        }
    }
    a.Flush()
}, 100)
}