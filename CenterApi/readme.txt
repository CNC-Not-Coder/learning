
文件列表：

	中心结点:

	Center/ServerCenter.js 中心结点脚本，所有需要通信的端都需要连接大次结点
	Center/CenterConfig.js 中心结点配置文件，配置监听Ip
	
	接口:
	基于WebSocket实现的连接接口，提供连接到中心结点以及接收数据的接口
	
	(js版)Center/CenterMessager.js 依赖库 : npm install websocket
	(c#版)CenterMessager.cs 依赖库 : LitJson.dll  WebSocket4Net.dll
	

	Center/ClientNode.js 连接到中心结点的端的示例脚本，需要提供参数1：配置文件路径

WebSocket库(js) :  https://github.com/theturtle32/WebSocket-Node
WebSocket库(c#) :  https://github.com/kerryjiang/WebSocket4Net