
var http = require("http");
var url = require("url");
var util = require("util");
var events = require("events");


console.log("Hello World");

/*
var ws = require("nodejs-websocket");

var server = ws.createServer(function (conn) {
    console.log("New connection");
    conn.on("text", function (str) {
        console.log("Received "+str)
        conn.sendText(str)
    });
    conn.on("close", function (code, reason) {
        console.log("Connection closed")
    });
	conn.on("error", function(err){
		console.log("ERROR:" + err);
	});
}).listen(9001);
*/

/*
try
{

var WebSocket = require('faye-websocket');

var server = http.createServer();

server.on('upgrade', function(request, socket, body) {
  if (WebSocket.isWebSocket(request)) {
    var ws = new WebSocket(request, socket, body);

    ws.on('message', function(event) {
      ws.send(event.data);
    });

    ws.on('close', function(event) {
      console.log('close', event.code, event.reason);
      ws = null;
    });
  }
});

server.listen(9001);
}
catch(err)
{
	console.log(err);
}
*/

//typedarray-to-buffer


var Message_Center_Port = 8000;//Center监听端口
var NodeMap = {};//所有连接到Center的其他节点名字和Connection的键值对

var webSocket = require("websocket").server;
var server = new http.Server().listen(Message_Center_Port);

var webServer = new webSocket({
	httpServer : server,
	autoAcceptConnections : false
	});

webServer.on("request", function(request){
	//console.log(util.inspect(request.requestedExtensions[0]));
	var connection = request.accept("servercenter", request.origin);	
});
webServer.on("connect", function(connection){
	NodeMap[connection.protocol] = connection;
	console.log(connection.protocol + "connected!!");
	connection.on("message", function(msg){
		console.log(msg);
		if(msg.type == "utf8")
			connection.sendUTF(msg.utf8Data);
		else
			connection.sendBytes(msg.binaryData);
		connection.send("{\"from\":\"Lobby\", \"data\":\"9999999999\"}");
	});
	connection.on("close", function(reasonCode, description){
		console.log("onclose :" + description);
	});
});



/*
var WebSocketServer = require('web_sock').Server;
io = new WebSocketServer({ port: 9001 });
console.log("Server runing at port: " + 9001 + ".");

io.on('connection', function (socket) {
	socket.on('message', function (arg) {
		console.log(arg);
		socket.send("" + arg);
	});
});
*/



/*
var count = 0;
setInterval(function () {
	count++;
	console.log(count);
}, 1000);
*/

/*
http.createServer(function(req, res){
	res.writeHead(200, {"Content-Type":"text/plain"});
	res.write(util.inspect(url.parse(req.url)));
	res.end("Hello Web!");
	console.log("Handle one http request!");
}).listen(8080);
*/

/*
var webSocket = require("ws").Server;
var httpserver = http.createServer(function(req, res){}).listen(9001);
	
var webServer = new webSocket({
	debug : false,
	server : httpserver
});
webServer.on("connection", function(connection){
	console.log("connected!!");
	connection.on("message", function(msg){
	console.log(msg);
	connection.send(msg);
    });
});
*/

/*
var emitter = new events.EventEmitter();
function test()
{
	emitter.on("myevent", function(){
		console.log("11111");
		emitter.emit("myevent");
		console.log("22222");
	});
}

test();
emitter.emit("myevent");

*/

process.on('uncaughtException', function (err) {
console.log('Caught exception: ' + err);
});