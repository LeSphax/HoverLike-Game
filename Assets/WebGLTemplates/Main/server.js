var express = require('express');
var app = express();
var http = require('http').Server(app);
var io = require('socket.io')(http);
var port = process.env.PORT || 3000;



app.get('/', function (req, res) {
    res.sendFile(__dirname + '/index.html');
});

io.on('connection', function (socket) {
    console.log(socket);
    var ip = socket.handshake.headers['x-forwarded-for'] || socket.handshake.address;
    socket.on('log', function (msg) {
        console.log(ip + ": " + msg)
    });
});
app.use("/", express.static(__dirname + '/'));

  http.listen(port, function () {
      console.log('listening on *:' + port);
  });