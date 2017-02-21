var WebSocketConnection = function () {
    function e(parent, url) {
        this.serverConnection = parent;
        this.mUrl = url;
        this.status = WebsocketConnectionStatus.NotConnected;
        this.mSocket = null;
    };
    e.prototype.WebsocketConnect = function () {
        var e = this;
        this.status = WebsocketConnectionStatus.Connecting;
        this.mSocket = new WebSocket(this.mUrl);
        this.mSocket.binaryType = "arraybuffer";
        this.mSocket.onopen = function () {
            e.OnWebsocketOnOpen()
        };
        this.mSocket.onerror = function (t) {
            e.OnWebsocketOnError(t)
        };
        this.mSocket.onmessage = function (t) {
            e.OnWebsocketOnMessage(t)
        };
        this.mSocket.onclose = function (t) {
            e.OnWebsocketOnClose(t)
        }
    };
    e.prototype.Cleanup = function () {
        this.mSocket.onopen = null;
        this.mSocket.onerror = null;
        this.mSocket.onmessage = null;
        this.mSocket.onclose = null;
        if (this.mSocket.readyState == this.mSocket.OPEN || this.mSocket.readyState == this.mSocket.CONNECTING) {
            this.mSocket.close()
        }
        this.mSocket = null
    };
    e.prototype.EnsureServerConnection = function () {
        if (this.status == WebsocketConnectionStatus.NotConnected) {
            this.WebsocketConnect()
        }
    };

    e.prototype.OnWebsocketOnOpen = function () {
        console.log("onWebsocketOnOpen");
        this.status = WebsocketConnectionStatus.Connected
    };
    e.prototype.OnWebsocketOnClose = function (e) {
        console.log("Closed: " + JSON.stringify(e));
        if (this.status == WebsocketConnectionStatus.Disconnecting || this.status == WebsocketConnectionStatus.NotConnected) return;
        this.Cleanup();
        this.status = WebsocketConnectionStatus.NotConnected
    };
    e.prototype.OnWebsocketOnMessage = function (e) {
        if (this.status == WebsocketConnectionStatus.Disconnecting || this.status == WebsocketConnectionStatus.NotConnected) return;
        var t = NetworkEvent.fromByteArray(new Uint8Array(e.data));
        this.serverConnection.HandleIncomingEvent(t)
    };
    e.prototype.OnWebsocketOnError = function (e) {
        if (this.status == WebsocketConnectionStatus.Disconnecting || this.status == WebsocketConnectionStatus.NotConnected) return;
        console.log("WebSocket Error " + e)
    };

    e.prototype.Send = function (event) {
        var t = NetworkEvent.toByteArray(event);
        this.mSocket.send(t);
    };
    return e;
}();

