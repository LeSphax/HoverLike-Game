var express = require('express');
var fs = require('fs');
var app = express();
var bodyParser = require('body-parser')
app.use( bodyParser.json() );       // to support JSON-encoded bodies
app.use(bodyParser.urlencoded({     // to support URL-encoded bodies
    extended: true
})); 

var port = process.env.PORT || 3000;

app.post('/log', function (req, res) {
    var user = req.body.user,
        message = req.body.message;
    writeLogLine(user, message)
    res.send('')
    // ...
});

app.get('/', function(req, res){
  writeLogLine(req.query.user, req.query.message)
  res.send('')
});

function writeLogLine(file,line){
	fs.appendFile("log/"+file, line+"\n", function(err) {
	    if(err) {
	        return console.log(err);
	    }

	    console.log(line);
	}); 
}

app.listen(port);
  console.log("Listening on port " + port);
