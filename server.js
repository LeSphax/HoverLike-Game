"use strict";
var config = require("./config.json");
var http = require('http');
var https = require('https');
var ws = require('ws');
var fs = require('fs');
var wns = require('./WebsocketNetworkServer');
var httpServer = null;

console.log("<Server class=json></Server>");
if (config.httpConfig) {
    httpServer = http.createServer();
    httpServer.listen(process.env.PORT || 5000, function () { console.log('Listening on ' + httpServer.address().port); });
}

var websocketSignalingServer = new wns.WebsocketNetworkServer();
for (var _i = 0, _a = config.apps; _i < _a.length; _i++) {
    var app = _a[_i];
    if (httpServer) {
        var webSocket = new ws.Server({ server: httpServer, path: app.path, maxPayload: config.maxPayload, perMessageDeflate: false });
        websocketSignalingServer.addSocketServer(webSocket, app);
    }
}
