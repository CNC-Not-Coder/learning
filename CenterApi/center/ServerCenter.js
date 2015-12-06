
var http = require("http");
var hash = require("./hash");
var config = require("./CenterConfig");
var webSocket = require("websocket").server;

console.log("Server Center Starting ...");

var node2socket = new hash.HashTable();//nodeName ~ connection 键值对
var Message_Center_Port = 8000;//Center监听端口
var MessageType = {
	Invalid : -1,
    Register : 0,
    Message : 1,
    UnRegister : 2,
};

if(config) Message_Center_Port = config.listenPort;

var server = new http.Server().listen(Message_Center_Port);

var webServer = new webSocket({
	httpServer : server,
	autoAcceptConnections : false
	});

function JsonParse(jsonStr) {
  try {
    var jsonObj = JSON.parse(jsonStr);
    return jsonObj;
  } catch (ex) {
    console.log("JsonParse exception:"+ex);
  }
  return null;
}
function JsonStringify(jsonObj) {
  try {
    var jsonStr = JSON.stringify(jsonObj);
    return jsonStr;
  } catch (ex) {
    console.log("JsonParse exception:"+ex);
  }
  return null;
}

webServer.on("request", function(request){
	var connection = request.accept("servercenter", request.origin);	
});
webServer.on("connect", function(connection){
	
	console.log("one client connected!!");
	
	connection.on("message", function(msg){
		//只接收文本，不接收二进制
		if(msg.type == "utf8"){
			console.log(msg.utf8Data);
		
			var recv = JsonParse(msg.utf8Data);
			if(!recv){
				return;
			}
			if(recv.type == MessageType.Message)
			{//正常消息
				if(!recv.to) return;
				var _connection = node2socket.getValue(recv.to);
				if(_connection)
				{
					_connection.send(msg.utf8Data);//仅仅转发消息
				}
				else
				{
					node2socket.remove(recv.to);
				}
			}
			else if(recv.type == MessageType.Register)
			{//注册结点，如果已经注册过了，则替换
				if(!recv.from) return;
				if(node2socket.containsKey(recv.from))
					node2socket.remove(recv.from);
				node2socket.add(recv.from, connection);
				//回消息
				var result = {};
				result.type = MessageType.Register;
				result.data = "succeed";
				var ret = JsonStringify(result);
				connection.send(ret);
				
				console.log("Register node[" + recv.from + "] succeed!");
			}
			else if(recv.type == MessageType.UnRegister)
			{//注销结点
				if(!recv.from) return;
				node2socket.remove(recv.from);
				console.log("UnRegister node[" + recv.from + "] succeed!");
			}
			else
			{
				console.log("Warning : unsupported message type : " + recv.type);
			}
		}
	});
	connection.on("close", function(reasonCode, description){
		console.log("onclose :" + description);
	});
});


process.on('uncaughtException', function (err) {
console.log('Caught exception: ' + err);
});

console.log("Done.");
console.log("Running at port : " + Message_Center_Port);
