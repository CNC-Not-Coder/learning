
var util = require('util');
var EventEmitter = require('events').EventEmitter;
var WebSocketClient = require('websocket').client;

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



var CenterMessager = function CenterMessager(){};

util.inherits(CenterMessager, EventEmitter);

CenterMessager.prototype.init = function(_centerIp, _centerPort, _nodeName, _nodeType)
{
	this.running = false;
	this.centerIp = _centerIp;
	this.centerPort = _centerPort;
	this.nodeName = _nodeName;
	this.nodeType = _nodeType;
	this.socketClient = new WebSocketClient();
}

CenterMessager.prototype.connect = function()
{
	var messger = this;
	this.socketClient.on("connect", function(connection){
		
		messger.socketConnetion = connection;
		
		connection.on('error', function(error) {
			messger.running = false;
			console.log("Connection Error: " + error.toString());
		});
		
		connection.on('close', function() {
			messger.running = false;
			console.log('Connection Closed');
		});
		
		connection.on('message', function(msg) {
			if (msg.type === 'utf8') {
				var jsonObj = JsonParse(msg.utf8Data);
				if(!jsonObj) return;
				if(messger.running)
				{//normal message
					if(jsonObj.to != messger.nodeName) return;
					if(!jsonObj.from || !jsonObj.data) return;
					
					messger.emit('onmessage', jsonObj.nodetype, jsonObj.from, jsonObj.data);
				}
				else
				{//register callback
					if(jsonObj.type == MessageType.Register && jsonObj.data == "succeed")
					{
						messger.running = true;
						console.log("Register node[" + messger.nodeName +"] succeed!");
					}
				}
			}
		});
		//Register
		var jsonObj = {};
		jsonObj.type = MessageType.Register;
		jsonObj.from = messger.nodeName;
		jsonObj.to = "Center";
		jsonObj.nodetype = messger.nodeType;
		jsonObj.data = "";
		connection.send(JsonStringify(jsonObj));
	});
	this.socketClient.on('connectFailed', function(error) {
		messger.running = false;
		console.log('Connect Error: ' + error.toString());
	});
	
	var uri = "ws://" + this.centerIp + ":" + this.centerPort;
	this.socketClient.connect(uri, "servercenter");
	
	console.log("current nodeName : " + this.nodeName);
}

CenterMessager.prototype.shutDown = function()
{
	this.running = false;
	
	//UnRegister
	var jsonObj = {};
	jsonObj.type = MessageType.UnRegister;
	jsonObj.from = this.nodeName;
	jsonObj.to = "Center";
	jsonObj.nodetype = this.nodeType;
	jsonObj.data = "";
	
	if(this.socketConnetion && this.socketConnetion.connected)
	{
		this.socketConnetion.send(JsonStringify(jsonObj));
	}
}

CenterMessager.prototype.sendMessage = function(destName, msg)
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











