
var util = require('util');
var EventEmitter = require('events').EventEmitter;
var WebSocketClient = require('websocket').client;

var Message_Center_Port = 8000;
var MessageType = {
	Invalid : -1,
    Register : 0,
    Message : 1,
    UnRegister : 2,
};
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



var CenterMessager = function CenterMessager(_centerIp, _centerPort, _nodeName, _nodeType)
{
	this.running = false;
	this.centerIp = _centerIp;
	this.centerPort = _centerPort;
	this.nodeName = _nodeName;
	this.nodeType = _nodeType;
	this.socketClient = new WebSocketClient();
	
	this.socketClient.on("connect", function(connection){
		this.socketConnetion = connection;
		connection.on('error', function(error) {
			this.running = false;
			console.log("Connection Error: " + error.toString());
		});
		connection.on('close', function() {
			this.running = false;
			console.log('Connection Closed');
		});
		connection.on('message', function(message) {
			if (message.type === 'utf8') {
				console.log("Received: '" + message.utf8Data + "'");
				var jsonObj = JsonParse(message);
				if(!jsonObj) return;
				if(this.running)
				{//normal message
					if(jsonObj.to != this.nodeName) return;
					if(!jsonObj.from || !jsonObj.data || jsonObj.nodetype) return;
					this.emit('onmessage', jsonObj.nodetype, jsonObj.from, jsonObj.data);
				}
				else
				{//register callback
					if(jsonObj.nodetype == MessageType.Register && jsonObj.data == "succeed")
					{
						this.running = true;
						console.log("Register node[" + this.nodeName +"] succeed!");
					}
				}
			}
		});
	});
	this.socketClient.on('connectFailed', function(error) {
		this.running = false;
		console.log('Connect Error: ' + error.toString());
	});
}

CenterMessager.prototype.connect()
{
	var uri = "ws://" + this.centerIp + ":" + this.centerPort;
	this.socketClient.connect(uri, "servercenter");
}

util.inherits(CenterMessager, EventEmitter);

CenterMessager.prototype.sendMessage(destName, msg)
{
	if(!this.running) return;
	
	var jsonObj = {};
	jsonObj.type = MessageType.Message;
	jsonObj.from = this.nodeName;
	jsonObj.to = destName;
	jsonObj.nodetype = this.nodeType;
	jsonObj.data = msg;
	if(this.socketConnetion && this.socketConnetion.connected)
	{
		this.socketConnetion.send(JsonStringify(jsonObj));
	}
}

module.exports = CenterMessager;











