using Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MyTest
{
    class Program
    {
        static WebSocket4Net.WebSocket mSocketClient = null;
        static void Main(string[] args)
        {

            InitNetWork();

            var info = new NetMessage.PB_UserInfo();
            info.Guid = 1000L;
            info.Level = 5;
            info.NickName = "limotao";
            byte[] data = new byte[info.CalculateSize()];
            var buff = new Google.Protobuf.CodedOutputStream(data);
            info.WriteTo(buff);
            Console.ReadKey();
            string message = Convert.ToBase64String(data);
            //mSocketClient.Send(data, 0, data.Length);
            mSocketClient.Send(message);

            Console.ReadKey();
            mSocketClient.Close(0, "logout!");
        }

        private static void InitNetWork()
        {
            var extension = new List<KeyValuePair<string, string>>();
            extension.Add(new KeyValuePair<string, string>("sec-websocket-extensions", "permessage-deflate; client_max_window_bits, x-webkit-deflate-frame"));
            mSocketClient = new WebSocket4Net.WebSocket("ws://127.0.0.1:8000/", subProtocol:"nodejs1", customHeaderItems:extension);
            mSocketClient.Opened += new EventHandler(websocket_Opened);
            mSocketClient.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(websocket_Error);
            mSocketClient.Closed += new EventHandler(websocket_Closed);
            mSocketClient.MessageReceived += new EventHandler<WebSocket4Net.MessageReceivedEventArgs>(websocket_MessageReceived);
            mSocketClient.DataReceived += new EventHandler<WebSocket4Net.DataReceivedEventArgs>(websocket_DataReceived);
            mSocketClient.Open();
        }

        private static void websocket_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("websocket_Opened");
        }
        private static void websocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            Console.WriteLine("websocket_Error:" + e.Exception.Message);
        }
        private static void websocket_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("websocket_Closed");
        }
        private static void websocket_MessageReceived(object sender, WebSocket4Net.MessageReceivedEventArgs e)
        {
            Console.WriteLine("receive:" + e.Message);
            byte[] buffer = Convert.FromBase64String(e.Message);

            var de_info = NetMessage.PB_UserInfo.Parser.ParseFrom(buffer);

            Console.WriteLine("{0}:{1}:{2}", de_info.Guid, de_info.Level, de_info.NickName);
        }
        private static void websocket_DataReceived(object sender, WebSocket4Net.DataReceivedEventArgs e)
        {
            var de_info = NetMessage.PB_UserInfo.Parser.ParseFrom(e.Data);

            Console.WriteLine("{0}:{1}:{2}", de_info.Guid, de_info.Level, de_info.NickName);
        }

        private static void InitDataTables()
        {
            FileStream stream = File.Open("", FileMode.Open, FileAccess.Read);

            //1. Reading from a binary Excel file ('97-2003 format; *.xls)
            //IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);

            //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            //3. DataSet - The result of each spreadsheet will be created in the result.Tables
            //DataSet result = excelReader.AsDataSet();

            //4. DataSet - Create column names from first row
            excelReader.IsFirstRowAsColumnNames = true;
            DataSet result = excelReader.AsDataSet();

            //5. Data Reader methods
            while (excelReader.Read())
            {
                //excelReader.GetInt32(0);
            }

            //6. Free resources (IExcelDataReader is IDisposable)
            excelReader.Close();
        }
    }
}
