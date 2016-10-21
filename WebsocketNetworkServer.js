"use strict";
var inet = require('./INetwork');
var WebsocketNetworkServer = (function () {
    function WebsocketNetworkServer() {
        this.mPool = {};
    }
    WebsocketNetworkServer.prototype.onConnection = function (socket, appname) {
        console.log("WebsocketNetworkServer OnConnection " + socket + "    " + appname);
        this.mPool[appname].add(socket);
    };
    WebsocketNetworkServer.prototype.addSocketServer = function (websocketServer, appConfig) {
        var _this = this;
        if (this.mPool[appConfig.name] == null) {
            this.mPool[appConfig.name] = new PeerPool(appConfig);
        }
        var name = appConfig.name;
        console.log("<WebSocketConnection>  </WebSocketConnection>");
        websocketServer.on('connection', function (socket) { _this.onConnection(socket, name); });
    };
    return WebsocketNetworkServer;
}());
exports.WebsocketNetworkServer = WebsocketNetworkServer;
;
var PeerPool = (function () {
    function PeerPool(config) {
        this.mConnections = new Array();
        this.mServers = {};
        this.mAddressSharing = false;
        this.maxAddressLength = 256;
        this.mAppConfig = config;
        if (this.mAppConfig.address_sharing) {
            this.mAddressSharing = this.mAppConfig.address_sharing;
        }
    }
    PeerPool.prototype.hasAddressSharing = function () {
        return this.mAddressSharing;
    };
    PeerPool.prototype.add = function (socket) {
        this.mConnections.push(new SignalingPeer(this, socket));
    };
    PeerPool.prototype.getServerConnection = function (address) {
        return this.mServers[address];
    };
    PeerPool.prototype.isAddressAvailable = function (address) {
        if (address.length <= this.maxAddressLength&& (this.mServers[address] == null || this.mAddressSharing)) {
            return true;
        }
        return false;
    };

    PeerPool.prototype.getRooms = function () {
        var rooms = "___GetRooms@";
        for (var key in this.mServers) {
            rooms+= key+"@";
        }
        //Remove the last @
        rooms = rooms.substring(0, rooms.length - 1);
        return rooms;
    }

    PeerPool.prototype.addServer = function (client, address) {
        if (this.mServers[address] == null) {
            this.mServers[address] = new Array();
        }
        this.mServers[address].push(client);
    };
    PeerPool.prototype.removeServer = function (client, address) {
        var index = this.mServers[address].indexOf(client);
        if (index != -1) {
            this.mServers[address].splice(index, 1);
        }
        if (this.mServers[address].length == 0) {
            delete this.mServers[address];
        }
    };
    PeerPool.prototype.removeConnection = function (client) {
        var index = this.mConnections.indexOf(client);
        if (index != -1) {
            this.mConnections.splice(index, 1);
        }
        else {
            console.warn("Tried to remove unknown SignalingClientConnection. Bug?" + client);
        }
    };
    PeerPool.prototype.count = function () {
        return this.mConnections.length;
    };
    return PeerPool;
}());


var SignalingConnectionState;
(function (SignalingConnectionState) {
    SignalingConnectionState[SignalingConnectionState["Uninitialized"] = 0] = "Uninitialized";
    SignalingConnectionState[SignalingConnectionState["Connecting"] = 1] = "Connecting";
    SignalingConnectionState[SignalingConnectionState["Connected"] = 2] = "Connected";
    SignalingConnectionState[SignalingConnectionState["Disconnecting"] = 3] = "Disconnecting";
    SignalingConnectionState[SignalingConnectionState["Disconnected"] = 4] = "Disconnected";
})(SignalingConnectionState || (SignalingConnectionState = {}));
;


var SignalingPeer = (function () {
    function SignalingPeer(pool, socket) {
        var _this = this;
        this.mState = SignalingConnectionState.Uninitialized;
        this.mConnections = {};
        this.mNextIncomingConnectionId = new inet.ConnectionId(16384);
        this.mConnectionPool = pool;
        this.mSocket = socket;
        this.mState = SignalingConnectionState.Connecting;

        console.log("connected " + this.mSocket.upgradeReq.connection.remoteAddress + ":" + this.mSocket.upgradeReq.connection.remotePort);

        socket.on('message', function (message, flags) {
            _this.onMessage(message, flags);
        });

        socket.on('error', function (error) {
            console.error(error);
        });

        socket.on('close', function (code, message) { _this.onClose(code, message); });
        this.mState = SignalingConnectionState.Connected;
    }

    SignalingPeer.prototype.onMessage = function (message, flags) {
        try {
            var evt = inet.NetworkEvent.fromByteArray(message);
            console.log("Inc:" + message.length + " " + evt.toString());
            this.handleIncomingEvent(evt);
        }
        catch (err) {
            console.warn("Invalid message received: " + message + "  \n Error: " + err);
        }
    };

    SignalingPeer.prototype.sendToClient = function (evt) {
        if (this.mState == SignalingConnectionState.Connected&& this.mSocket.readyState == this.mSocket.OPEN) {
            console.log("Out:" + evt.toString());
            var msg = inet.NetworkEvent.toByteArray(evt);
            this.mSocket.send(msg);
        }
    };
    SignalingPeer.prototype.onClose = function (code, error) {
        this.mState = SignalingConnectionState.Disconnecting;
        this.Cleanup();
    };
    SignalingPeer.prototype.Cleanup = function () {
        this.mConnectionPool.removeConnection(this);
        console.log("disconnected "
            + this.mSocket.upgradeReq.connection.remoteAddress
            + ":" + this.mSocket.upgradeReq.connection.remotePort
            + " " + this.mConnectionPool.count()
            + " connections left.");
        var test = this.mConnections;
        for (var v in this.mConnections) {
            if (this.mConnections.hasOwnProperty(v))
                this.disconnect(new inet.ConnectionId(+v));
        }
        if (this.mServerAddress != null) {
            this.stopServer();
        }
        this.mState = SignalingConnectionState.Disconnected;
    };
    SignalingPeer.prototype.handleIncomingEvent = function (evt) {
        var address = evt.Info;
        var newConnectionId = evt.ConnectionId;
        if (evt.Type == inet.NetEventType.NewConnection) {
            console.log("NewConnection "+evt.Info);
            if (evt.Info == "___GetRooms"){
                console.log("___GetRooms "+ evt.ConnectionId);
                this.sendToClient(new inet.NetworkEvent(inet.NetEventType.ServerInitFailed, newConnectionId, this.mConnectionPool.getRooms()));
            }
            else{
                this.connect(address);
            }
        }
        else if (evt.Type == inet.NetEventType.ConnectionFailed) {
        }
        else if (evt.Type == inet.NetEventType.Disconnected) {
            var otherPeerId = evt.ConnectionId;
            this.disconnect(otherPeerId);
        }
        else if (evt.Type == inet.NetEventType.ServerInitialized) {
            console.log("Initialized " + evt.Info);

            this.startServer(evt.Info);

        }
        else if (evt.Type == inet.NetEventType.ServerInitFailed) {
        }
        else if (evt.Type == inet.NetEventType.ServerClosed) {
            this.stopServer();
        }
        else if (evt.Type == inet.NetEventType.ReliableMessageReceived) {
            this.sendData(evt.ConnectionId, evt.MessageData, true);
        }
        else if (evt.Type == inet.NetEventType.UnreliableMessageReceived) {
            this.sendData(evt.ConnectionId, evt.MessageData, false);
        }
    };
    SignalingPeer.prototype.internalAddIncomingPeer = function (peer,id) {
            console.log("Incoming" + id.id);
        this.mConnections[id.id] = peer;
        this.sendToClient(new inet.NetworkEvent(inet.NetEventType.NewConnection, id, null));
    };
    SignalingPeer.prototype.internalAddOutgoingPeer = function (peer, id) {
            console.log("Outgoing"+id.id);
        this.mConnections[id.id] = peer;
        this.sendToClient(new inet.NetworkEvent(inet.NetEventType.NewConnection, id, null));
    };
    SignalingPeer.prototype.internalRemovePeer = function (id) {
        delete this.mConnections[id.id];
        this.sendToClient(new inet.NetworkEvent(inet.NetEventType.Disconnected, id, null));
    };
    SignalingPeer.prototype.findPeerConnectionId = function (otherPeer) {
        for (var peer in this.mConnections) {
            if (this.mConnections[peer] === otherPeer) {
                return new inet.ConnectionId(+peer);
            }
        }
    };
    SignalingPeer.prototype.nextConnectionId = function () {
        var result = this.mNextIncomingConnectionId;
        this.mNextIncomingConnectionId = new inet.ConnectionId(this.mNextIncomingConnectionId.id + 1);
        return result;
    };
    SignalingPeer.prototype.connect = function (address) {
        var serverConnections = this.mConnectionPool.getServerConnection(address);
        if (serverConnections != null && serverConnections.length == 1) {
            var newConnectionId = serverConnections[0].nextConnectionId();
            console.log(newConnectionId);
            serverConnections[0].internalAddIncomingPeer(this, newConnectionId);
            this.internalAddOutgoingPeer(serverConnections[0], newConnectionId);
        }
        else {
            this.sendToClient(new inet.NetworkEvent(inet.NetEventType.ConnectionFailed, newConnectionId, null));
        }
    };
    SignalingPeer.prototype.connectJoin = function (address) {
        var serverConnections = this.mConnectionPool.getServerConnection(address);
        if (serverConnections != null) {
            for (var _i = 0, serverConnections_1 = serverConnections; _i < serverConnections_1.length; _i++) {
                var v = serverConnections_1[_i];
                if (v != this) {
                    v.internalAddIncomingPeer(this);
                    this.internalAddIncomingPeer(v);
                }
            }
        }
    };
    SignalingPeer.prototype.disconnect = function (connectionId) {
        var otherPeer = this.mConnections[connectionId.id];
        if (otherPeer != null) {
            var idOfOther = otherPeer.findPeerConnectionId(this);
            this.internalRemovePeer(connectionId);
            otherPeer.internalRemovePeer(idOfOther);
        }
        else {
        }
    };
    SignalingPeer.prototype.startServer = function (address) {
        if (this.mServerAddress != null)
            this.stopServer();
        if (this.mConnectionPool.isAddressAvailable(address)) {
            this.mServerAddress = address;
            this.mConnectionPool.addServer(this, address);
            this.sendToClient(new inet.NetworkEvent(inet.NetEventType.ServerInitialized, inet.ConnectionId.INVALID, address));
            if (this.mConnectionPool.hasAddressSharing()) {
                this.connectJoin(address);
            }
        }
        else {
            this.sendToClient(new inet.NetworkEvent(inet.NetEventType.ServerInitFailed, inet.ConnectionId.INVALID, address));
        }
    };
    SignalingPeer.prototype.stopServer = function () {
        if (this.mServerAddress != null) {
            this.mConnectionPool.removeServer(this, this.mServerAddress);
            this.sendToClient(new inet.NetworkEvent(inet.NetEventType.ServerClosed, inet.ConnectionId.INVALID, null));
            this.mServerAddress = null;
        }
    };
    SignalingPeer.prototype.forwardMessage = function (senderPeer, msg, reliable) {
        var id = this.findPeerConnectionId(senderPeer);
        if (reliable)
            this.sendToClient(new inet.NetworkEvent(inet.NetEventType.ReliableMessageReceived, id, msg));
        else
            this.sendToClient(new inet.NetworkEvent(inet.NetEventType.UnreliableMessageReceived, id, msg));
    };
    SignalingPeer.prototype.sendData = function (id, msg, reliable) {
        var peer = this.mConnections[id.id];
        if (peer != null)
            peer.forwardMessage(this, msg, reliable);
    };
    return SignalingPeer;
}());
