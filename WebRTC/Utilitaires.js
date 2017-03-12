var __extends = this && this.__extends || function(e, t) {
    for (var n in t)
        if (t.hasOwnProperty(n)) e[n] = t[n];
    function i() {
        this.constructor = e
    }
    e.prototype = t === null ? Object.create(t) : (i.prototype = t.prototype, new i)
};

var Queue = function() {
    function e() {
        this.mArr = new Array
    }
    e.prototype.Enqueue = function(e) {
        this.mArr.push(e)
    };
    e.prototype.TryDequeue = function(e) {
        var t = false;
        if (this.mArr.length > 0) {
            e.val = this.mArr.shift();
            t = true
        }
        return t
    };
    e.prototype.Dequeue = function() {
        if (this.mArr.length > 0) {
            return this.mArr.shift()
        } else {
            return null
        }
    };
    e.prototype.Peek = function() {
        if (this.mArr.length > 0) {
            return this.mArr[0]
        } else {
            return null
        }
    };
    e.prototype.Count = function() {
        return this.mArr.length
    };
    return e
}();
var List = function() {
    function e() {
        this.mArr = new Array
    }
    Object.defineProperty(e.prototype, "Internal", {
        get: function() {
            return this.mArr
        },
        enumerable: true,
        configurable: true
    });
    e.prototype.Add = function(e) {
        this.mArr.push(e)
    };
    Object.defineProperty(e.prototype, "Count", {
        get: function() {
            return this.mArr.length
        },
        enumerable: true,
        configurable: true
    });
    return e
}();
var Output = function() {
    function e() {}
    return e
}();
var Debug = function() {
    function e() {}
    e.Log = function(e) {
        if (e == null) {
            console.debug(e)
        }
        console.debug(e)
    };
    e.LogError = function(e) {
        console.debug(e)
    };
    e.LogWarning = function(e) {
        console.debug(e)
    };
    return e
}();
var Encoder = function() {
    function e() {}
    return e
}();
var UTF16Encoding = function(e) {
    __extends(t, e);

    function t() {
        e.call(this)
    }
    t.prototype.GetBytes = function(e) {
        return stringToBuffer(e)
    };
    t.prototype.GetString = function(e) {
        return this.bufferToString(e)
    };
    t.prototype.bufferToString = function(e) {
        var t = new Uint16Array(e.buffer, e.byteOffset, e.byteLength / 2);
        return String.fromCharCode.apply(null, t)
    };
    return t
}(Encoder);
var Encoding = function() {
    function e() {}
    Object.defineProperty(e, "UTF16", {
        get: function() {
            return new UTF16Encoding
        },
        enumerable: true,
        configurable: true
    });
    return e
}();
var Random = function() {
    function e() {}
    e.getRandomInt = function(e, t) {
        e = Math.ceil(e);
        t = Math.floor(t);
        return Math.floor(Math.random() * (t - e)) + e
    };
    return e
}();
var Helper = function() {
    function e() {}
    e.tryParseInt = function(e) {
        try {
            if (/^(\-|\+)?([0-9]+)$/.test(e)) {
                var t = Number(e);
                if (isNaN(t) == false) return t
            }
        } catch (e) {}
        return null
    };
    return e
}();
var SLog = function() {
    function e() {}
    e.L = function(e) {
        console.log(e)
    };
    e.Log = function(e) {
        console.log(e)
    };
    e.LogWarning = function(e) {
        console.warn(e)
    };
    e.LogError = function(e) {
        console.error(e)
    };
    return e
}();

var SignalingInfo = function() {
    function e(e, t, n) {
        this.mConnectionId = e;
        this.mIsIncoming = t;
        this.mCreationTime = n;
        this.mSignalingConnected = true
    }
    e.prototype.IsSignalingConnected = function() {
        return this.mSignalingConnected
    };
    Object.defineProperty(e.prototype, "ConnectionId", {
        get: function() {
            return this.mConnectionId
        },
        enumerable: true,
        configurable: true
    });
    e.prototype.IsIncoming = function() {
        return this.mIsIncoming
    };
    e.prototype.GetCreationTimeMs = function() {
        return Date.now() - this.mCreationTime
    };
    e.prototype.SignalingDisconnected = function() {
        this.mSignalingConnected = false
    };
    return e
}();

var WebRtcPeerState;
(function(e) {
    e[e["Invalid"] = 0] = "Invalid";
    e[e["Created"] = 1] = "Created";
    e[e["Signaling"] = 2] = "Signaling";
    e[e["SignalingFailed"] = 3] = "SignalingFailed";
    e[e["Connected"] = 4] = "Connected";
    e[e["Closing"] = 5] = "Closing";
    e[e["Closed"] = 6] = "Closed"
})(WebRtcPeerState || (WebRtcPeerState = {}));
var WebRtcInternalState;
(function(e) {
    e[e["None"] = 0] = "None";
    e[e["Signaling"] = 1] = "Signaling";
    e[e["SignalingFailed"] = 2] = "SignalingFailed";
    e[e["Connected"] = 3] = "Connected";
    e[e["Closed"] = 4] = "Closed"
})(WebRtcInternalState || (WebRtcInternalState = {}));


var WebsocketConnectionStatus;
(function(e) {
    e[e["Uninitialized"] = 0] = "Uninitialized";
    e[e["NotConnected"] = 1] = "NotConnected";
    e[e["Connecting"] = 2] = "Connecting";
    e[e["Connected"] = 3] = "Connected";
    e[e["Disconnecting"] = 4] = "Disconnecting"
})(WebsocketConnectionStatus || (WebsocketConnectionStatus = {}));
var WebsocketServerStatus;
(function(e) {
    e[e["Offline"] = 0] = "Offline";
    e[e["Starting"] = 1] = "Starting";
    e[e["Online"] = 2] = "Online";
    e[e["ShuttingDown"] = 3] = "ShuttingDown"
})(WebsocketServerStatus || (WebsocketServerStatus = {}));

function bufferToString(e) {
    var t = new Uint16Array(e.buffer, e.byteOffset, e.byteLength / 2);
    return String.fromCharCode.apply(null, t)
}

function stringToBuffer(e) {
    var t = new ArrayBuffer(e.length * 2);
    var n = new Uint16Array(t);
    for (var i = 0, o = e.length; i < o; i++) {
        n[i] = e.charCodeAt(i)
    }
    var r = new Uint8Array(t);
    return r
}