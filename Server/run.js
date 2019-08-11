var fs = require('fs');
var http = require('http');
/*
var https = require('https');
var privateKey  = fs.readFileSync('./certs/privkey1.pem');
var certificate = fs.readFileSync('./certs/fullchain1.pem');
var chain = fs.readFileSync('./certs/chain1.pem');
var credentials = {key: privateKey, cert: certificate, ca: chain};
*/
var express = require('express');
var session = require('express-session');
var MongoStore = require('connect-mongo')(session);
var app = express();
var httpServer = http.createServer(app);
//var httpsServer = https.createServer(credentials, app);
var bodyparser = require('body-parser');
var cookieParser = require('cookie-parser');
var cookie = cookieParser('mymonkeystolemybanana');
var _html = __dirname + "/html/";
var serverFunction = require(__dirname + "/mods/serverFunctions.js");
var path = require('path');
/*
function requireHTTPS(req, res, next) {
    if (!req.secure) {
        return res.redirect('https://' + req.get('host') + req.url);
    }
    next();
}
app.use(requireHTTPS);
*/

app.use(express.static(_html + 'files'));
app.use(session({
  	secret: 'mymonkeystolemybanana',
  	name: 'sessID',
  	resave: false,
 	saveUninitialized: true,
  	cookie: { secure: false },
  	store: new MongoStore({ 
  		url: 'mongodb://localhost:27017/r3dmush',
      	autoRemove: 'native' 
      })
}));
function getUser(req, res, next) {
	serverFunction.getUser(req, res, next);
}
//app.use(getUser);
app.use(bodyparser.urlencoded({ extended: false }));
app.use(bodyparser.json());

app.get('/', function(req, res){
	res.json({"status": 200, "loggedIn": true, "sessionId": req.session.id});
	console.log(req.connection.remoteAddress + " accessed the / page.");
});

app.get('/login', function(req, res){
  	res.sendFile(_html +'login.html');
  	console.log(req.connection.remoteAddress + " accessed the Login page.");
});

app.post('/login', function(req, res){
	serverFunction.login(req,res);
});

app.get('/register', function(req, res){
  	res.sendFile(_html +'register.html');
  	console.log(req.connection.remoteAddress + " accessed the Forms page.");
});

app.post('/register', function(req, res){
	serverFunction.register(req,res);
});

app.post('/poi/:poi', function(req, res){
  	serverFunction.poiGet(req,res);
  	console.log(req.connection.remoteAddress + " Interacted with POI: " + req.params.poi);
});

app.get('/user/:user', function(req, res){
	console.log(req.url);
  	serverFunction.getUserStats(req,res);
});

app.post('/invrem', function(req, res){
	console.log(req.url);
  	serverFunction.remInvItem(req,res);
});

app.get('/leader', function(req, res){
  	serverFunction.getLeader(req,res);
});

httpServer.listen(80, function(){
    console.log('listening on *:80');
});
/*
httpsServer.listen(8443, function(){
    console.log('listening on *:8443');
});
*/