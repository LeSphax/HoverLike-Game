var express = require('express');
var fs = require('fs');
var app = express();

var port = process.env.PORT || 3000;

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
