var fs = require('fs');
var passwordHash = require('password-hash');

var mongodb = require('mongodb');
var MongoClient = mongodb.MongoClient;
var url = "mongodb://localhost:27017/r3dmush";
var assert = require('assert');
var mongo = false;
MongoClient.connect(url, function(err, db) {
	if (err) throw err;
	mongo = db;
});

exports.register = function(req,res) {
	var sess = req.session;
	mongo.collection('users').find({ username: req.body.username }).toArray(function(err, docs) {
		if(docs.length == 0) {
			var hashedpass = passwordHash.generate(req.body.password);
			var user = {
					username: req.body.username,
					password: hashedpass,
					email: req.body.email,
					inventory: [1,1,1,1,1,2,2,2,2,2,3,3,3,3,3,4,4,4,4,4],
					stats: [['Enemies Killed', "0"], ['Damage Done', "0"], ['Enemies Hacked', "0"], ['Shots Fired', "0"]],
					score: 0,
					poilist: []
			};
			mongo.collection('users').insertOne( user , function(err, result) {
				mongo.collection('sessions').updateOne(
				    { "_id" : sess.id },
				    {
				      	'$set' : { username: req.body.username, id: user._id.valueOf()}
				    }, function(err, results) {
			   	});
			   	res.json({"redirect": "/"});
				console.log('Reistered user:' + req.body.username);
			});
		}
		else {
			res.json({"message": "Username Taken."});
		}
	});
};

exports.login = function(req,res) {
	var sess = req.session;
	console.log(req.body.username + "   " + req.body.password);
	mongo.collection('users').find({ username: req.body.username }).toArray(function(err, docs) {
		if(docs.length == 1) {
			var correctpass = passwordHash.verify(req.body.password,docs[0].password);
			if(correctpass) {
				mongo.collection('sessions').updateOne(
				    { "_id" : sess.id },
				    {
				      	'$set' : { username: req.body.username, id: docs[0]._id.valueOf()}
				    }, function(err, results) {
				    	res.json({"status": 200, "username": docs[0].username, "inventory": docs[0].inventory, "stats": docs[0].stats, "message": 'Logged In'});
			   	});
	 		}
	 		else {
	 			res.send({"status": 203,"message": 'Username or password wrong.'});
	 		}
		}
		else {
			res.json({"status": 203,"message": 'Username or password wrong.'});
		}
	});
};

exports.getUser = function(req, res, next) {
	if (req.url != '/login' && req.url != '/register') {
		var sess = req.session;
		mongo.collection('sessions').find({"_id":sess.id}).toArray(function(err, docs) {
			if (docs.length > 0) {
				if (typeof(docs[0].id) !== 'undefined') {
					req.user = docs[0];
					next();
				}
			    else {
			    	res.json({"status": 401, "loggedIn": false, "sessionId": sess.id});
			    }
			}
			else {
				res.json({"status": 401, "loggedIn": false, "sessionId": sess.id});
			}
		});	
	} else {
		next();
	}
};

exports.poiGet = function(req, res) {
	var sess = req.session;
	console.log(req.body.username + "   " + req.body.password);
	mongo.collection('users').find({ username: req.body.username }).toArray(function(err, docs) {
		var poitime = docs[0].poilist.find(checkPOI,req.params.poi);
		if(docs.length == 1) {
			var correctpass = passwordHash.verify(req.body.password,docs[0].password);
			if(correctpass) {
				if (poitime != undefined) {
					var diff = new Date() - poitime[1];
					if (diff >= 3600000) {
						console.log('INTERACT');
						var item = Math.floor((Math.random() * 4) + 1);
						console.log(item);
						mongo.collection('users').updateOne(
						    { "username" : req.body.username },
						   	{
						      	"$push": {
						          "inventory": item
						    	}
						    }, function(err, results) {
						    	res.json({"status": 200, "username": docs[0].username, "poiget": item, "inventory": results.inventory, "stats": docs[0].stats, "message": 'Logged In'});
					   	});

						mongo.collection('users').updateOne(
						    { "username" : req.body.username },
						   	{
						      	"$pull": {
						          "poilist": poitime
						    	}
						    }, function(err, results) {
					   	});

					   	var poi = [req.params.poi, new Date()];
						mongo.collection('users').updateOne(
						    { "username" : req.body.username },
						   	{
						      	"$push": {
						          "poilist": poi
						    	}
						    }, function(err, results) {
					   	});
					} else {
						var left = 3600000 - diff;
						var str = Math.floor(left/60000) + " minutes left."
						console.log('WAIT MORE');
						res.json({"status": 201, "message": str});
					}
				} else {
					var item = Math.floor((Math.random() * 4) + 1);
					console.log(item);
					mongo.collection('users').updateOne(
					    { "username" : req.body.username },
					   	{
					      	"$push": {
					          "inventory": item
					    	}
					    }, function(err, results) {
					    	res.json({"status": 200, "username": docs[0].username, "poiget": item, "inventory": results.inventory, "stats": docs[0].stats, "message": 'Logged In'});
				   	});

					var poi = [req.params.poi, new Date()];
					mongo.collection('users').updateOne(
					    { "username" : req.body.username },
					   	{
					      	"$push": {
					          "poilist": poi
					    	}
					    }, function(err, results) {
				   	});
				}

	 		}
	 		else {
	 			res.send({"status": 203,"message": 'Username or password wrong.'});
	 		}
		}
		else {
			res.json({"status": 203,"message": 'Username or password wrong.'});
		}
	});
}

exports.getLeader = function(req,res) {
	mongo.collection('users').find().toArray(function(err, docs) {
		if(docs.length >= 1) {
			var users = [];
			for (var i = 0; i < docs.length; i++) {
				var x = [docs[i].username, docs[i].score];
				users.push(x);
			}
			res.json({"status": 200, "users": users});
			
		}
		else {
			res.json({"status": 203,"message": 'No users found :('});
		}
	});
}

exports.getUserStats = function(req, res) {
	mongo.collection('users').find({ username: req.params.user }).toArray(function(err, docs) {
		if(docs.length == 1) {
		
			res.json({"status": 200, "username": docs[0].username, "stats": docs[0].stats});
			
		}
		else {
			res.json({"status": 203,"message": 'No users found :('});
		}
	});
}

exports.remInvItem = function(req, res) {
	console.log(req.body.item);
	mongo.collection('users').find({ username: req.body.username }).toArray(function(err, docs) {
		if(docs.length == 1) {
			var correctpass = passwordHash.verify(req.body.password,docs[0].password);
			if(correctpass) {
				var arr = docs[0].inventory;
				var toremove;
				for (var i = 0; i < arr.length; i++) {
					if(arr[i] == parseInt(req.body.item)){
						toremove = i;
					}
				}
				if(toremove != null) {
					arr.splice(toremove, 1);
				}
				mongo.collection('users').updateOne(
						    { "username" : req.body.username },
						   	{
						      	"$set": {
						          "inventory": arr
						    	}
						    }, function(err, results) {
						    	res.send({"status": 200, "message": 'We good!'});
					   	});				
	 		}
	 		else {
	 			res.send({"status": 203,"message": 'Username or password wrong.'});
	 		}
		}
		else {
			res.json({"status": 203,"message": 'Username or password wrong.'});
		}
	});
}

function containsObject(obj, list) {
    var i;
    for (i = 0; i < list.length; i++) {
    	console.log(list[i].id,"  ", obj);
        if (list[i].id.equals(obj)) {
        	console.log('True');
            return true;
        }
    }
    console.log('False');
    return false;
}

function checkPOI(poi) {
	return poi[0] == this;
}




