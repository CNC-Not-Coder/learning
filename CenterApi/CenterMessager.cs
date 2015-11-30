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

        public delegate void OnMessageDelegate(string msg);
        public delegate void LogHandlerDelegate(string format, params object[] p);

        public OnMessageDelegate OnMessage = null;
        public LogHandlerDelegate LogHandler = null;

        private WebSocket mWebSocket = null;
        private bool mIsRunning = false;

        private CenterMessager()
        {

        }

        public void Init(IPAddress centerIp, int centerPort)
        {
            string uri = string.Format("ws://{0}:{1}", centerIp.ToString(), centerPort);
            mWebSocket = new WebSocket4Net.WebSocket(uri);
            mWebSocket.Opened += new EventHandler(websocket_Opened);
            mWebSocket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(websocket_Error);
            mWebSocket.Closed += new EventHandler(websocket_Closed);
            mWebSocket.MessageReceived += new EventHandler<WebSocket4Net.MessageReceivedEventArgs>(websocket_MessageReceived);
            mWebSocket.DataReceived += new EventHandler<WebSocket4Net.DataReceivedEventArgs>(websocket_DataReceived);
            mWebSocket.Open();
        }

        private void websocket_Opened(object sender, EventArgs e)
        {
            mIsRunning = true;
            if(LogHandler != null) LogHandler("websocket_Opened");

            //向Center发送注册消息包
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
            if (LogHandler != null) LogHandler("receive:" + e.Message);
            byte[] buffer = Convert.FromBase64String(e.Message);
        }
        private void websocket_DataReceived(object sender, WebSocket4Net.DataReceivedEventArgs e)
        {
        }

        public void SendMessage(string destName, string msg)
        {
            if (!mIsRunning)
                return;
            mWebSocket.Send(msg);
        }
        public void SendMessage(string destName, byte[] data, int length)
        {
            if (!mIsRunning)
                return;
            if (data == null || data.Length < length)
                return;
            string sendStr = Convert.ToBase64String(data, 0, length);
            SendMessage(destName, sendStr);
        }
    }
}
