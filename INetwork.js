"use strict";
var NetEventType;
(function (NetEventType) {
    NetEventType[NetEventType["Invalid"] = 0] = "Invalid";
    NetEventType[NetEventType["UnreliableMessageReceived"] = 1] = "UnreliableMessageReceived";
    NetEventType[NetEventType["ReliableMessageReceived"] = 2] = "ReliableMessageReceived";
    NetEventType[NetEventType["RoomCreated"] = 3] = "RoomCreated";
    NetEventType[NetEventType["RoomCreateFailed"] = 4] = "RoomCreateFailed";
    NetEventType[NetEventType["RoomJoinFailed"] = 5] = "RoomJoinFailed";
    NetEventType[NetEventType["RoomClosed"] = 6] = "RoomClosed";
    NetEventType[NetEventType["NewConnection"] = 7] = "NewConnection";
    NetEventType[NetEventType["ConnectionFailed"] = 8] = "ConnectionFailed";
    NetEventType[NetEventType["Disconnected"] = 9] = "Disconnected";
    NetEventType[NetEventType["ConnectionToSignalingServerEstablished"] = 10] = "ConnectionToSignalingServerEstablished";
    NetEventType[NetEventType["FatalError"] = 100] = "FatalError";
    NetEventType[NetEventType["Warning"] = 101] = "Warning";
    NetEventType[NetEventType["Log"] = 102] = "Log";
    NetEventType[NetEventType["UserCommand"] = 103] = "UserCommand";
})(NetEventType || (NetEventType = {}));
exports.NetEventType = NetEventType;

var NetEventMessage;
(function (NetEventMessage) {
    NetEventMessage[NetEventMessage["RoomDoesntExist"] = "0"] = "RoomDoesntExist";
    NetEventMessage[NetEventMessage["WebsocketClosed"] = "1"] = "WebsocketClosed";
    NetEventMessage[NetEventMessage["HostDisconnected"] = "2"] = "HostDisconnected";
    NetEventMessage[NetEventMessage["RoomAlreadyExists"] = "3"] = "RoomAlreadyExists";
    NetEventMessage[NetEventMessage["GameStarted"] = "4"] = "GameStarted";
    NetEventMessage[NetEventMessage["RoomFull"] = "5"] = "RoomFull";
    NetEventMessage[NetEventMessage["ServerConnectionNot1"] = "6"] = "ServerConnectionNot1";
    NetEventMessage[NetEventMessage["OtherConnection"] = "7"] = "OtherConnection";
    NetEventMessage[NetEventMessage["Incoming"] = "8"] = "Incoming";
    NetEventMessage[NetEventMessage["Outgoing"] = "9"] = "Outgoing";
    NetEventMessage[NetEventMessage["AskIfAllowedToEnter"] = "10"] = "AskIfAllowedToEnter";
    NetEventMessage[NetEventMessage["AllowedToEnter"] = "11"] = "AllowedToEnter";
    NetEventMessage[NetEventMessage["SetNumberPlayers"] = "12"] = "SetNumberPlayers";
    NetEventMessage[NetEventMessage["WrongPassword"] = "13"] = "WrongPassword";


})(NetEventMessage || (NetEventMessage = {}));
exports.NetEventMessage = NetEventMessage;

var NetEventDataType;
(function (NetEventDataType) {
    NetEventDataType[NetEventDataType["Null"] = 0] = "Null";
    NetEventDataType[NetEventDataType["ByteArray"] = 1] = "ByteArray";
    NetEventDataType[NetEventDataType["UTF16String"] = 2] = "UTF16String";
})(NetEventDataType || (NetEventDataType = {}));
var NetworkEvent = (function () {
    function NetworkEvent(t, conId, data) {
        this.type = t;
        this.connectionId = conId;
        this.data = data;
    }
    Object.defineProperty(NetworkEvent.prototype, "RawData", {
        get: function () {
            return this.data;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(NetworkEvent.prototype, "MessageData", {
        get: function () {
            if (typeof this.data != "string")
                return this.data;
            return null;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(NetworkEvent.prototype, "Info", {
        get: function () {
            if (typeof this.data == "string")
                return this.data;
            return null;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(NetworkEvent.prototype, "Type", {
        get: function () {
            return this.type;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(NetworkEvent.prototype, "ConnectionId", {
        get: function () {
            return this.connectionId;
        },
        enumerable: true,
        configurable: true
    });
    NetworkEvent.prototype.toString = function () {
        var output = "NetworkEvent[";
        output += "NetEventType: (";
        output += NetEventType[this.type];
        output += "), id: (";
        output += this.connectionId.id;
        if (this.data){
            output += "), Data.Length: (";
            output += this.data.length;
            output += ")]";
        }
        return output;
    };
    NetworkEvent.parseFromString = function (str) {
        var values = JSON.parse(str);
        var data;
        if (values.data == null) {
            data = null;
        }
        else if (typeof values.data == "string") {
            data = values.data;
        }
        else if (typeof values.data == "object") {
            var arrayAsObject = values.data;
            var length = 0;
            for (var prop in arrayAsObject) {
                length++;
            }
            var buffer = new Uint8Array(Object.keys(arrayAsObject).length);
            for (var i = 0; i < buffer.length; i++)
                buffer[i] = arrayAsObject[i];
            data = buffer;
        }
        else {
            console.error("data can't be parsed");
        }
        var evt = new NetworkEvent(values.type, values.connectionId, data);
        return evt;
    };
    NetworkEvent.toString = function (evt) {
        return JSON.stringify(evt);
    };
    NetworkEvent.fromByteArray = function (arr) {
        var type = arr[0];
        var dataType = arr[1];
        var id = new Int16Array(arr.buffer, arr.byteOffset + 2, 1)[0];
        var data = null;
        if (dataType == NetEventDataType.ByteArray) {
            var length_1 = new Uint32Array(arr.buffer, arr.byteOffset + 4, 1)[0];
            var byteArray = new Uint8Array(arr.buffer, arr.byteOffset + 8, length_1);
            data = byteArray;
        }
        else if (dataType == NetEventDataType.UTF16String) {
            var length_2 = new Uint32Array(arr.buffer, arr.byteOffset + 4, 1)[0];
            var uint16Arr = new Uint16Array(arr.buffer, arr.byteOffset + 8, length_2);
            var str = "";
            for (var i = 0; i < uint16Arr.length; i++) {
                str += String.fromCharCode(uint16Arr[i]);
            }
            data = str;
        }
        var conId = new ConnectionId(id);
        var result = new NetworkEvent(type, conId, data);
        return result;
    };
    NetworkEvent.toByteArray = function (evt) {
        var dataType;
        var length = 4;
        if (evt.data == null) {
            dataType = NetEventDataType.Null;
        }
        else if (typeof evt.data == "string") {
            dataType = NetEventDataType.UTF16String;
            var str = evt.data;
            length += str.length * 2 + 4;
        }
        else {
            dataType = NetEventDataType.ByteArray;
            var byteArray = evt.data;
            length += 4 + byteArray.length;
        }
        var result = new Uint8Array(length);
        result[0] = evt.type;
        ;
        result[1] = dataType;
        var conIdField = new Int16Array(result.buffer, result.byteOffset + 2, 1);
        conIdField[0] = evt.connectionId.id;
        if (dataType == NetEventDataType.ByteArray) {
            var byteArray = evt.data;
            var lengthField = new Uint32Array(result.buffer, result.byteOffset + 4, 1);
            lengthField[0] = byteArray.length;
            for (var i = 0; i < byteArray.length; i++) {
                result[8 + i] = byteArray[i];
            }
        }
        else if (dataType == NetEventDataType.UTF16String) {
            var str = evt.data;
            var lengthField = new Uint32Array(result.buffer, result.byteOffset + 4, 1);
            lengthField[0] = str.length;
            var dataField = new Uint16Array(result.buffer, result.byteOffset + 8, str.length);
            for (var i = 0; i < dataField.length; i++) {
                dataField[i] = str.charCodeAt(i);
            }
        }
        return result;
    };
    return NetworkEvent;
}());
exports.NetworkEvent = NetworkEvent;
var ConnectionId = (function () {
    function ConnectionId(nid) {
        this.id = nid;
    }
    ConnectionId.INVALID = new ConnectionId(-1);
    return ConnectionId;
}());
exports.ConnectionId = ConnectionId;
