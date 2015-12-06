
start "ServerCenter" node.exe ServerCenter.js

start "ClientNode1" node.exe ClientNode.js ./ClientConfig1

start "ClientNode2" node.exe ClientNode.js ./ClientConfig2