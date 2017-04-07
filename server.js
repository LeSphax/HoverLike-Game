var express = require('express');
var fs = require('fs');
var app = express();

app.get('/', function(req, res){
    console.log(req.query.user);
  console.log(req.query.message);
  writeLogLine(req.query.user, req.query.message)
});

function writeLogLine(file,line){
	fs.appendFile("log/"+file, line+"\n", function(err) {
	    if(err) {
	        return console.log(err);
	    }

	    console.log("The file was saved!");
	}); 
}

app.listen(3000);
  console.log("Listening on port 3000");
