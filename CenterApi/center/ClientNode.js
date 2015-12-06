//args
var arguments = process.argv.splice(2);
if(!arguments[0]) {
	console.log("missing argc : the config file!");
	process.exit(0);
}
var configFile = arguments[0];

var CenterMessager = require("./CenterMessager");
var Config = require(configFile);
var NodeType = {
	Invalid : -1,
    Lobby : 0,
    ClientNode : 1,
    RoomServer : 2,
}
var Message_Center_Port = 8000;
var Message_Center_IP = "127.0.0.1";
var MyNodeName = "clientnode1";

if(Config)
{
	Message_Center_IP = Config.centerIP;
	Message_Center_Port = Config.centerPort;
	MyNodeName = Config.mynodename;
}

var messager = new CenterMessager();

messager.init(Message_Center_IP, Message_Center_Port, MyNodeName, NodeType.ClientNode);

messager.on("onmessage", function(nodetype, from, msg){
	
	console.log("onmessage: " + nodetype + from + msg);
});

messager.connect();

function sendNumber() {
	if (messager.running) {
		var number = Math.round(Math.random() * 0xFFFFFF);
		messager.sendMessage("lobby", number.toString());
	}
	setTimeout(sendNumber, 1000);
}

sendNumber();

console.log("CenterIp : " + Message_Center_IP);
console.log("CenterPort : " + Message_Center_Port);
console.log("Current NodeName : " + MyNodeName);
