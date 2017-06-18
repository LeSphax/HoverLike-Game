var express = require('express');
var app = express();
var http = require('http').Server(app);
var io = require('socket.io')(http);
var port = process.env.PORT || 3000;
var nodemailer = require('nodemailer');



app.get('/', function (req, res) {
    res.sendFile(__dirname + '/index.html');
});

	io.on('connection', function (socket) {
		//console.log(socket);
		var ip = socket.handshake.headers['x-forwarded-for'] || socket.handshake.address;
		socket.on('log', function (msg) {
			console.log(ip + ": " + msg)
		});
		socket.on('mail',function(msg)){
			// create reusable transporter object using the default SMTP transport
			let transporter = nodemailer.createTransport({
				host: 'smtp.gmail.com',
				port: 587,
				secure: false, // secure:true for port 465, secure:false for port 587
				auth: {
					user: 'sbkerbrat@gmail.com',
					pass: 'Joar3oesiereero'
				}
			});

			// setup email data with unicode symbols
			let mailOptions = {
				from: '"Slideball 👻" <slideball@feedback.com>', // sender address
				to: 'sbkerbrat@gmail.com', // list of receivers
				subject: 'You have some feedback', // Subject line
				text: msg, // plain text body
				html: msg // html body
			};

			// send mail with defined transport object
			transporter.sendMail(mailOptions, (error, info) => {
				if (error) {
					return console.log(error);
				}
				console.log('Mail %s sent: %s', info.messageId, info.response);
			});
		}
});
app.use("/", express.static(__dirname + '/'));

  http.listen(port, function () {
      console.log('listening on *:' + port);
  });