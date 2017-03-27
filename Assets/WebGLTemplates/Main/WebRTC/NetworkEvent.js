



var NetEventType;
(function (e) {
    e[e["Invalid"] = 0] = "Invalid";
    e[e["UnreliableMessageReceived"] = 1] = "UnreliableMessageReceived";
    e[e["ReliableMessageReceived"] = 2] = "ReliableMessageReceived";
    e[e["RoomCreated"] = 3] = "RoomCreated";
    e[e["RoomCreateFailed"] = 4] = "RoomCreateFailed";
    e[e["RoomJoinFailed"] = 5] = "RoomJoinFailed";
    e[e["RoomClosed"] = 6] = "RoomClosed";
    e[e["NewConnection"] = 7] = "NewConnection";
    e[e["ConnectionFailed"] = 8] = "ConnectionFailed";
    e[e["Disconnected"] = 9] = "Disconnected"; 
    e[e["ConnectionToSignalingServerEstablished"] = 10] = "ConnectionToSignalingServerEstablished";
    e[e["FatalError"] = 100] = "FatalError";
    e[e["Warning"] = 101] = "Warning";
    e[e["Log"] = 102] = "Log"
    e[e["UserCommand"] = 103] = "UserCommand"
    e[e["SignalingConnectionFailed"] = 200] = "SignalingConnectionFailed"
})(NetEventType || (NetEventType = {}));
var NetEventDataType;
(function(e) {
    e[e["Null"] = 0] = "Null";
    e[e["ByteArray"] = 1] = "ByteArray";
    e[e["UTF16String"] = 2] = "UTF16String"
})(NetEventDataType || (NetEventDataType = {}));


var NetworkEvent = function() {
    function e(e, t, n) {
        this.type = e;
        this.connectionId = t;
        this.data = n
    }
    Object.defineProperty(e.prototype, "RawData", {
        get: function() {
            return this.data
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(e.prototype, "MessageData", {
        get: function() {
            if (typeof this.data != "string") return this.data;
            return null
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(e.prototype, "Info", {
        get: function() {
            if (typeof this.data == "string") return this.data;
            return null
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(e.prototype, "Type", {
        get: function() {
            return this.type
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(e.prototype, "ConnectionId", {
        get: function() {
            return this.connectionId
        },
        enumerable: true,
        configurable: true
    });
    e.prototype.toString = function() {
        var e = "NetworkEvent[";
        e += "NetEventType: (";
        e += NetEventType[this.type];
        e += "), id: (";
        e += this.connectionId.id;
        e += "), Data: (";
        e += this.data;
        e += ")]";
        return e
    };
    e.parseFromString = function(t) {
        var n = JSON.parse(t);
        var i;
        if (n.data == null) {
            i = null
        } else if (typeof n.data == "string") {
            i = n.data
        } else if (typeof n.data == "object") {
            var o = n.data;
            var r = 0;
            for (var a in o) {
                r++
            }
            var s = new Uint8Array(Object.keys(o).length);
            for (var c = 0; c < s.length; c++) s[c] = o[c];
            i = s
        } else {
            console.error("data can't be parsed")
        }
        var l = new e(n.type, n.connectionId, i);
        return l
    };
    e.toString = function(e) {
        return JSON.stringify(e)
    };
    e.fromByteArray = function(t) {
        var n = t[0];
        var i = t[1];
        var o = new Int16Array(t.buffer, t.byteOffset + 2, 1)[0];
        var r = null;
        if (i == NetEventDataType.ByteArray) {
            var a = new Uint32Array(t.buffer, t.byteOffset + 4, 1)[0];
            var s = new Uint8Array(t.buffer, t.byteOffset + 8, a);
            r = s
        } else if (i == NetEventDataType.UTF16String) {
            var c = new Uint32Array(t.buffer, t.byteOffset + 4, 1)[0];
            var l = new Uint16Array(t.buffer, t.byteOffset + 8, c);
            var u = "";
            for (var g = 0; g < l.length; g++) {
                u += String.fromCharCode(l[g])
            }
            r = u
        }
        var f = new ConnectionId(o);
        var h = new e(n, f, r);
        return h
    };
    e.toByteArray = function(e) {
        var t;
        var n = 4;
        if (e.data == null) {
            t = NetEventDataType.Null
        } else if (typeof e.data == "string") {
            t = NetEventDataType.UTF16String;
            var i = e.data;
            n += i.length * 2 + 4
        } else {
            t = NetEventDataType.ByteArray;
            var o = e.data;
            n += 4 + o.length
        }
        var r = new Uint8Array(n);
        r[0] = e.type;
        r[1] = t;
        var a = new Int16Array(r.buffer, r.byteOffset + 2, 1);
        a[0] = e.connectionId.id;
        if (t == NetEventDataType.ByteArray) {
            var o = e.data;
            var s = new Uint32Array(r.buffer, r.byteOffset + 4, 1);
            s[0] = o.length;
            for (var c = 0; c < o.length; c++) {
                r[8 + c] = o[c]
            }
        } else if (t == NetEventDataType.UTF16String) {
            var i = e.data;
            var s = new Uint32Array(r.buffer, r.byteOffset + 4, 1);
            s[0] = i.length;
            var l = new Uint16Array(r.buffer, r.byteOffset + 8, i.length);
            for (var c = 0; c < l.length; c++) {
                l[c] = i.charCodeAt(c)
            }
        }
        return r
    };
    return e
}();