function runTests(){
    WebRtcNetwork_test1();
    //WebsocketNetwork_sharedaddress();
    //CAPIWebRtcNetwork_test1();
    //WebsocketNetwork_test1();
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
    var i = new WebRtcNetwork(new SignalingConfig(new LocalNetwork), n);
    i.StartServer();
    var o = new WebRtcNetwork(new SignalingConfig(new LocalNetwork), n);
    setInterval(function() {
        i.Update();
        var t = null;
        while (t = i.Dequeue()) {
            console.log("server inc: " + t.toString());
            if (t.Type == NetEventType.ServerInitialized) {
                console.log("server started. Address " + t.Info);
                o.Connect(t.Info)
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
                o.Disconnect(t.ConnectionId);
                console.log("client shutting down");
                o.Shutdown()
            }
        }
        o.Flush()
    }, 100)
}

function WebsocketNetwork_sharedaddress() {
    console.log("WebsocketNetwork shared address test");
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
var a = new WebsocketNetwork(i);
var s = new WebsocketNetwork(i);
var c = new WebsocketNetwork(i);
var l = stringToBuffer("network1 says hi");
var u = stringToBuffer("network2 says hi");
var g = stringToBuffer("network3 says hi");
a.StartServer(r);
s.StartServer(r);
c.StartServer(r);

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
var r = new WebsocketNetwork(i);
r.StartServer();
var a = new WebsocketNetwork(i);
setInterval(function() {
    r.Update();
    var t = null;
    while (t = r.Dequeue()) {
        console.log("server inc: " + t.toString());
        if (t.Type == NetEventType.ServerInitialized) {
            console.log("server started. Address " + t.Info);
            a.Connect(t.Info)
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
            a.Disconnect(t.ConnectionId);
            console.log("client shutting down");
            a.Shutdown()
        }
    }
    a.Flush()
}, 100)
}