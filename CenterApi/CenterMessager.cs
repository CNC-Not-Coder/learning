using LitJson;
using System;
using System.Net;
using WebSocket4Net;

namespace CenterApi
{
    /// <summary>
    /// 提供通用的通信接口，包括发送消息和接收消息
    /// </summary>
    public class CenterMessager
    {
        public static CenterMessager Instance = new CenterMessager();

        public delegate void OnMessageDelegate(int nodeType, string from, string msg);
        public delegate void LogHandlerDelegate(string format, params object[] p);

        public OnMessageDelegate OnMessage = null;
        public LogHandlerDelegate LogHandler = null;

        private WebSocket mWebSocket = null;
        private bool mIsRunning = false;
        private string mNodeName = string.Empty;
        private int mNodeType = -1;
        private string mIp = string.Empty;
        private int mPort = 0;

        enum MessageType
        {
            Invalid = -1,
            Register = 0,
            Message = 1,
            UnRegister = 2,
        }

        class MessageData
        {
            public MessageType MsgType = MessageType.Invalid;
            public int NodeType = -1;
            public string From = string.Empty;
            public string To = string.Empty;
            public string Data = string.Empty;
        }

        private CenterMessager()
        {

        }
        /// <summary>
        /// 初始化Socket
        /// </summary>
        /// <param name="centerIp">ServerCenter的IP</param>
        /// <param name="centerPort">ServerCenter的端口</param>
        /// <param name="nodeName">本结点的名字，不能和别的结点重复</param>
        /// <param name="nodeType">本结点的类型，用于接收消息时区分发送者</param>
        public void Init(string centerIp, int centerPort, string nodeName, int nodeType, bool connectNow = true)
        {
            if (string.IsNullOrEmpty(nodeName))
                return;
            mIsRunning = false;
            mWebSocket = null;
            mIp = centerIp;
            mPort = centerPort;
            mNodeName = nodeName;
            mNodeType = nodeType;

            if(connectNow) Connect();
        }
        public void Connect()
        {
            mWebSocket = null;
            mIsRunning = false;

            string uri = string.Format("ws://{0}:{1}", mIp, mPort);
            mWebSocket = new WebSocket4Net.WebSocket(uri, subProtocol: "servercenter");
            mWebSocket.Opened += new EventHandler(websocket_Opened);
            mWebSocket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(websocket_Error);
            mWebSocket.Closed += new EventHandler(websocket_Closed);
            mWebSocket.MessageReceived += new EventHandler<WebSocket4Net.MessageReceivedEventArgs>(websocket_MessageReceived);
            mWebSocket.DataReceived += new EventHandler<WebSocket4Net.DataReceivedEventArgs>(websocket_DataReceived);
            mWebSocket.Open();

            if (LogHandler != null) LogHandler("Current nodeName : {0}", mNodeName);
        }
        public void ShutDown()
        {
            if (!mIsRunning)
                return;
            mIsRunning = false;
            //注销结点
            MessageData data = new MessageData();
            data.MsgType = MessageType.UnRegister;
            data.NodeType = mNodeType;
            data.From = mNodeName;
            data.To = "Center";
            data.Data = string.Empty;

            mWebSocket.Send(BuildMessage(data));

            mWebSocket.Close("Unregister!");
        }
        public bool IsRuning
        {
            get { return mIsRunning; }
        }

        private void websocket_Opened(object sender, EventArgs e)
        {
            if(LogHandler != null) LogHandler("websocket_Opened");

            //向Center发送注册消息包
            MessageData data = new MessageData();
            data.MsgType = MessageType.Register;
            data.NodeType = mNodeType;
            data.From = mNodeName;
            data.To = "Center";
            data.Data = string.Empty;
            
            mWebSocket.Send(BuildMessage(data));
        }
        private void websocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            mIsRunning = false;
            mWebSocket = null;
            if (LogHandler != null) LogHandler("websocket_Error:" + e.Exception.Message);
        }
        private void websocket_Closed(object sender, EventArgs e)
        {
            mIsRunning = false;
            mWebSocket = null;
            if (LogHandler != null) LogHandler("websocket_Closed");
        }
        private void websocket_MessageReceived(object sender, WebSocket4Net.MessageReceivedEventArgs e)
        {
            if(mIsRunning)
            {//正常消息
                MessageData msgData = ParseMessage(e.Message);
                MessageType msgType = msgData.MsgType;
                string from = msgData.From;
                string to = msgData.To;
                string data = msgData.Data;
                int nodeType = msgData.NodeType;
                if (to != mNodeName)
                    return;
                if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(data))
                    return;
                if (OnMessage != null) OnMessage(nodeType, from, data);
            }
            else
            {//注册成功，返回
                MessageData msgData = ParseMessage(e.Message);
                MessageType type = msgData.MsgType;
                string result = msgData.Data;

                if (type == (int)MessageType.Register && result == "succeed")
                {
                    mIsRunning = true;
                    if (LogHandler != null) LogHandler("Register node[{0}] succeed!", mNodeName);
                }
            }
        }
        private void websocket_DataReceived(object sender, WebSocket4Net.DataReceivedEventArgs e)
        {
            if (LogHandler != null) LogHandler("websocket_DataReceived, You shouldn't send message by binary!");
        }

        public void SendMessage(string destName, string msg)
        {
            if (!mIsRunning)
                return;
            MessageData data = new MessageData();
            data.MsgType = MessageType.Message;
            data.From = mNodeName;
            data.To = destName;
            data.NodeType = mNodeType;
            data.Data = msg;
            mWebSocket.Send(BuildMessage(data));
        }
        private string BuildMessage(MessageData data)
        {
            JsonData json = new JsonData();
            json["type"] = (int)data.MsgType;
            json["from"] = data.From;
            json["to"] = data.To;
            json["nodetype"] = data.NodeType;
            json["data"] = data.Data;
            return json.ToJson();
        }
        private MessageData ParseMessage(string jsonStr)
        {
            JsonData json = JsonMapper.ToObject(jsonStr);
            MessageData rData = new MessageData();
            if (json.Keys.Contains("type") && json["type"].IsInt)
                rData.MsgType = (MessageType)Convert.ToInt32(json["type"].ToString());
            if (json.Keys.Contains("from"))
                rData.From = json["from"].ToString();
            if (json.Keys.Contains("to"))
                rData.To = json["to"].ToString();
            if (json.Keys.Contains("nodetype") && json["nodetype"].IsInt)
                rData.NodeType = Convert.ToInt32(json["nodetype"].ToString());
            if (json.Keys.Contains("data"))
                rData.Data = json["data"].ToString();
            return rData;
        }
    }
}
