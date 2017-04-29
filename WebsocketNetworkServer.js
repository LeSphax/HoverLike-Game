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
        this.mRoomsInfo = {};
        this.maxAddressLength = 256;
        this.mAppConfig = config;
    }
    PeerPool.prototype.add = function (socket) {
        var peer = new SignalingPeer(this, socket)
        this.mConnections.push(peer);
        peer.sendToClient(new inet.NetworkEvent(inet.NetEventType.ConnectionToSignalingServerEstablished, inet.ConnectionId.INVALID, null));
    };
    PeerPool.prototype.getServerConnection = function (address) {
        return this.mServers[address];
    };
    PeerPool.prototype.isAddressAvailable = function (address) {
        if (address.length <= this.maxAddressLength && this.mServers[address] == null) {
            return true;
        }
        return false;
    };

    PeerPool.prototype.getRooms = function () {
        var rooms = "GetRooms@";
        for (var key in this.mServers) {
			rooms += key + "|" + this.mRoomsInfo[key] + "@";
        }
        //Remove the last @
        rooms = rooms.substring(0, rooms.length - 1);
        return rooms;
    }

    PeerPool.prototype.addServer = function (client, address) {
        if (this.mServers[address] == null) {
            this.mServers[address] = new Array();
            this.mRoomsInfo[address] = 1;
        }
        this.mServers[address].push(client);
        console.log("ADD SERVER ------ " + address);
    };
    PeerPool.prototype.removeClientFromServer = function (client, address) {
        var index = this.mServers[address].indexOf(client);
        if (index != -1) {
            this.mServers[address].splice(index, 1);
        }
        if (this.mServers[address].length == 0) {
            delete this.mServers[address];
			delete this.mRoomsInfo[address];
            console.log("REMOVE SERVER -------- " + address);
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
        this.mWaitingForConnection = {};
        this.mNextIncomingConnectionId = new inet.ConnectionId(16384);
        this.mConnectionPool = pool;
        this.mSocket = socket;
        this.mState = SignalingConnectionState.Connecting;


        this.changeContext();

        console.log("connected " + this.mSocket.upgradeReq.connection.remoteAddress + ":" + this.mSocket.upgradeReq.connection.remotePort);

        socket.on('message', function (message, flags) {
            _this.onMessage(message, flags);
        });

        socket.on('error', function (error) {
            console.error("Socket Error " + error);
        });

        socket.on('close', function (code, message) { _this.onClose(code, message); clearInterval(_this.myInterval)});
        
        this.mState = SignalingConnectionState.Connected;
    }


    SignalingPeer.prototype.changeContext = function(){
        console.log("changeContext " + this);
        var self = this;
        self.myInterval = setInterval(function(){
            console.log("Send Staying alive");
            var msg = new inet.NetworkEvent(inet.NetEventType.Log, inet.ConnectionId.INVALID, "stayingAlive");
            self.sendToClient(msg);
        },30000);
    };

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

    SignalingPeer.prototype.getNumberClients = function () {
        return this.mConnections.length
    };

    SignalingPeer.prototype.sendToClient = function (evt) {
        if (this.mState == SignalingConnectionState.Connected&& this.mSocket.readyState == this.mSocket.OPEN) {
            console.log("Out:" + evt.toString());
            var msg = inet.NetworkEvent.toByteArray(evt);
            this.mSocket.send(msg);
        }
    };
    SignalingPeer.prototype.onClose = function (code, error) {
        console.log ("Close socket " +code +" "+ error);
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
            this.stopServer(inet.NetEventMessage.WebsocketClosed);
        }
        this.mState = SignalingConnectionState.Disconnected;
    };
    SignalingPeer.prototype.handleIncomingEvent = function (evt) {
        console.log("handleIncomingEvent "+ evt.Type + "    " + evt.Info);
        var address = evt.Info;
        var newConnectionId = evt.ConnectionId;
        if (evt.Type == inet.NetEventType.NewConnection) {
            console.log("NewConnection "+ address);

            this.tryToConnect(address, evt.ConnectionId);
        }
        else if (evt.Type == inet.NetEventType.ConnectionFailed) {
        }
        else if (evt.Type == inet.NetEventType.Disconnected) {
            console.log("Disconnect peer " + evt.ConnectionId);
            var otherPeerId = evt.ConnectionId;
            this.disconnect(otherPeerId);
        }
        else if (evt.Type == inet.NetEventType.RoomCreated) {
            console.log("Initialized " + evt.Info);
            this.startServer(evt.Info);
        }
        else if (evt.Type == inet.NetEventType.RoomCreateFailed) {
        }
        else if (evt.Type == inet.NetEventType.RoomClosed) {
            this.stopServer(inet.NetEventMessage.HostDisconnected);
        }
        else if (evt.Type == inet.NetEventType.ReliableMessageReceived) {
            this.sendData(evt.ConnectionId, evt.MessageData, true);
        }
        else if (evt.Type == inet.NetEventType.UnreliableMessageReceived) {
            this.sendData(evt.ConnectionId, evt.MessageData, false);
        }
        else if (evt.Type == inet.NetEventType.UserCommand){
            var splittedInfo = evt.Info.split('@');
            console.log("UserCommand " + splittedInfo[0]);

            if (evt.Info == "GetRooms"){
                console.log("GetRooms "+ evt.ConnectionId);
                this.sendToClient(new inet.NetworkEvent(inet.NetEventType.UserCommand, inet.ConnectionId.INVALID, this.mConnectionPool.getRooms()));
            }
			else if (splittedInfo[0]== "SetNumberPlayers"){
                console.log("SetNumberPlayers of room "+ splittedInfo[1] + " to "+ splittedInfo[2] + evt.ConnectionId);
				this.mConnectionPool.mRoomsInfo[splittedInfo[1]] = splittedInfo[2];
            }
            else if (splittedInfo[0] == inet.NetEventMessage.AskIfAllowedToEnter) {
                var peerConnectionId = parseInt(splittedInfo[1]);
                if (splittedInfo[2] == inet.NetEventMessage.GameStarted) {
                    console.log("ConnectionFailed GameStarted " + peerConnectionId);
                    this.mWaitingForConnection[peerConnectionId].sendToClient(new inet.NetworkEvent(inet.NetEventType.RoomJoinFailed, inet.ConnectionId.INVALID, inet.NetEventMessage.GameStarted));
                }
                else if (splittedInfo[2] == inet.NetEventMessage.RoomFull) {
                    console.log("ConnectionFailed RoomFull");
                    this.mWaitingForConnection[peerConnectionId].sendToClient(new inet.NetworkEvent(inet.NetEventType.RoomJoinFailed, inet.ConnectionId.INVALID, inet.NetEventMessage.RoomFull));
                }
                else if (splittedInfo[2] == inet.NetEventMessage.AllowedToEnter) {
                    console.log("Successfully connected " + peerConnectionId);
                    this.mWaitingForConnection[peerConnectionId].connect(this, new inet.ConnectionId(peerConnectionId));
                }
            }
        }
    };
    SignalingPeer.prototype.internalAddIncomingPeer = function (peer,id) {
        console.log("Incoming " + id + "   " + peer);
        this.mConnections[id.id] = peer;
        this.sendToClient(new inet.NetworkEvent(inet.NetEventType.NewConnection, id, inet.NetEventMessage.Incoming));
    };
    SignalingPeer.prototype.internalAddOutgoingPeer = function (peer, id) {
        console.log("Outgoing");
        this.mConnections[id.id] = peer;
        this.sendToClient(new inet.NetworkEvent(inet.NetEventType.NewConnection, id, inet.NetEventMessage.Outgoing));
    };
    SignalingPeer.prototype.internalRemovePeer = function (id) {
        console.log("Remove Peer " + id.id);
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
    SignalingPeer.prototype.tryToConnect = function (address, connectionId) {
        var serverConnections = this.mConnectionPool.getServerConnection(address);
        if (serverConnections == null){
            console.log("ConnectionFailed Room");
            this.sendToClient(new inet.NetworkEvent(inet.NetEventType.RoomJoinFailed, inet.ConnectionId.INVALID, inet.NetEventMessage.RoomDoesntExist));
        }
        else if (serverConnections.length != 1){
            console.log("ConnectionFailed Not1");
            this.sendToClient(new inet.NetworkEvent(inet.NetEventType.RoomJoinFailed, inet.ConnectionId.INVALID, inet.NetEventMessage.ServerConnectionNot1));
        }
        else {
            var newConnectionId = serverConnections[0].nextConnectionId();
            serverConnections[0].mWaitingForConnection[newConnectionId.id] = this;
            console.log("AskPermission for id " + newConnectionId.id );

            serverConnections[0].sendToClient(new inet.NetworkEvent(inet.NetEventType.UserCommand, newConnectionId, inet.NetEventMessage.AskIfAllowedToEnter));
        }
    };

    SignalingPeer.prototype.connect = function (serverPeer, connectionId) {
        serverPeer.internalAddIncomingPeer(this, connectionId);
        this.internalAddOutgoingPeer(serverPeer, connectionId);
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
            this.stopServer(inet.NetEventMessage.OtherConnection);
        if (this.mConnectionPool.isAddressAvailable(address)) {
            this.mServerAddress = address;
            this.mConnectionPool.addServer(this, address);
            this.sendToClient(new inet.NetworkEvent(inet.NetEventType.RoomCreated, inet.ConnectionId.INVALID, address));
        }
        else {
            this.sendToClient(new inet.NetworkEvent(inet.NetEventType.RoomCreateFailed, inet.ConnectionId.INVALID, inet.NetEventMessage.RoomAlreadyExists));
        }
    };
    SignalingPeer.prototype.stopServer = function (message) {
        console.log("Stop Server");
        if (this.mServerAddress != null) {
            this.mConnectionPool.removeClientFromServer(this, this.mServerAddress);
            this.sendToClient(new inet.NetworkEvent(inet.NetEventType.RoomClosed, inet.ConnectionId.INVALID, message));
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

