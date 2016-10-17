"use strict";
var config = require("./config.json");
var http = require('http');
var https = require('https');
var ws = require('ws');
var fs = require('fs');
var wns = require('./WebsocketNetworkServer');
var httpServer = null;
var httpsServer = null;
if (config.httpConfig) {
    httpServer = http.createServer();
    httpServer.listen(config.httpConfig.port, function () { console.log('Listening on ' + httpServer.address().port); });
}
if (config.httpsConfig) {
    httpsServer = https.createServer({
        key: fs.readFileSync(config.httpsConfig.ssl_key_file),
        cert: fs.readFileSync(config.httpsConfig.ssl_cert_file)
    });
    httpsServer.listen(config.httpsConfig.port, function () { console.log('Listening on wala' + httpsServer.address().port); });
}
var websocketSignalingServer = new wns.WebsocketNetworkServer();
for (var _i = 0, _a = config.apps; _i < _a.length; _i++) {
    var app = _a[_i];
    if (httpServer) {
        var webSocket = new ws.Server({ server: httpServer, path: app.path, maxPayload: config.maxPayload, perMessageDeflate: false });
        websocketSignalingServer.addSocketServer(webSocket, app);
    }
    if (httpsServer) {
        var webSocketSecure = new ws.Server({ server: httpsServer, path: app.path, maxPayload: config.maxPayload, perMessageDeflate: false });
        websocketSignalingServer.addSocketServer(webSocketSecure, app);
    }
}
