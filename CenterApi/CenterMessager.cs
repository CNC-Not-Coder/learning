using LitJson;
using System;
using System.Net;
using WebSocket4Net;

namespace CenterApi
{
    enum MessageType
    {
        Register = 0,
        Message = 1,
        UnRegister = 2,
    }
    /// <summary>
    /// 提供通用的通信接口，包括发送消息和接收消息
    /// </summary>
    public class CenterMessager
    {
        public static CenterMessager Instance = new CenterMessager();

        public delegate void OnMessageDelegate(string from, string msg);
        public delegate void LogHandlerDelegate(string format, params object[] p);

        public OnMessageDelegate OnMessage = null;
        public LogHandlerDelegate LogHandler = null;

        private WebSocket mWebSocket = null;
        private bool mIsRunning = false;
        private string mNodeName = string.Empty;

        private CenterMessager()
        {

        }

        public void Init(string centerIp, int centerPort, string nodeName)
        {
            if (string.IsNullOrEmpty(nodeName))
                return;
            mNodeName = nodeName;
            string uri = string.Format("ws://{0}:{1}", centerIp, centerPort);
            mWebSocket = new WebSocket4Net.WebSocket(uri, subProtocol: "servercenter");
            mWebSocket.Opened += new EventHandler(websocket_Opened);
            mWebSocket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(websocket_Error);
            mWebSocket.Closed += new EventHandler(websocket_Closed);
            mWebSocket.MessageReceived += new EventHandler<WebSocket4Net.MessageReceivedEventArgs>(websocket_MessageReceived);
            mWebSocket.DataReceived += new EventHandler<WebSocket4Net.DataReceivedEventArgs>(websocket_DataReceived);
            mWebSocket.Open();
        }

        private void websocket_Opened(object sender, EventArgs e)
        {
            if(LogHandler != null) LogHandler("websocket_Opened");

            //向Center发送注册消息包
            JsonData json = new JsonData();
            json["type"] = (int)MessageType.Register;
            json["name"] = mNodeName;
            mWebSocket.Send(json.ToJson());
        }
        private void websocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            mIsRunning = false;
            if (LogHandler != null) LogHandler("websocket_Error:" + e.Exception.Message);
        }
        private void websocket_Closed(object sender, EventArgs e)
        {
            mIsRunning = false;
            if (LogHandler != null) LogHandler("websocket_Closed");
        }
        private void websocket_MessageReceived(object sender, WebSocket4Net.MessageReceivedEventArgs e)
        {
            JsonData json = JsonMapper.ToObject(e.Message);
            if(mIsRunning)
            {
                string from = null, data = null;
                if (json.Keys.Contains("from"))
                    from = json["from"].ToString();
                if (json.Keys.Contains("data"))
                    data = json["data"].ToString();
                if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(data))
                    return;
                if (OnMessage != null) OnMessage(from, data);
            }
            else
            {
                int type = -1, result = -1;
                if (json.Keys.Contains("type") && json["type"].IsInt)
                    type = Convert.ToInt32(json["type"]);
                if (json.Keys.Contains("result") && json["result"].IsInt)
                    result = Convert.ToInt32(json["result"]);
                if (type == (int)MessageType.Register && result == 1)
                    mIsRunning = true;
            }
        }
        private void websocket_DataReceived(object sender, WebSocket4Net.DataReceivedEventArgs e)
        {
        }

        public void SendMessage(string destName, string msg)
        {
            if (!mIsRunning)
                return;
            JsonData json = new JsonData();
            json["type"] = (int)MessageType.Message;
            json["to"] = destName;
            json["data"] = msg;
            mWebSocket.Send(json.ToJson());
        }
        public void SendMessage(string destName, byte[] data, int offset, int length)
        {
            if (!mIsRunning)
                return;
            if (data == null || data.Length < length)
                return;
            string sendStr = Convert.ToBase64String(data, offset, length);
            SendMessage(destName, sendStr);
        }
    }
}
