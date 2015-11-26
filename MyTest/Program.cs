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
            //GenerateTable();

            //InitDataTables();

            InitDataFromTxt();

            InitNetWork();

            LoadTables();

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
            FileStream stream = File.Open("../../Data/DataTables/weapons.xls", FileMode.Open, FileAccess.Read);

            //1. Reading from a binary Excel file ('97-2003 format; *.xls)
            IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);

            //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
            //IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            //3. DataSet - The result of each spreadsheet will be created in the result.Tables
            //DataSet result = excelReader.AsDataSet();

            //4. DataSet - Create column names from first row
            excelReader.IsFirstRowAsColumnNames = true;
            DataSet result = excelReader.AsDataSet();

            DataTable table = result.Tables[0];
            int row = table.Rows.Count;
            int col = table.Columns.Count;
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    Console.Write(table.Rows[i][j] + "\t");
                }
                Console.Write("\n");
            }

            //5. Data Reader methods
            //while (excelReader.Read())
            //{
                //excelReader.GetInt32(0);
            //}

            //6. Free resources (IExcelDataReader is IDisposable)
            excelReader.Close();
            excelReader.Dispose();
            stream.Close();
            stream.Dispose();
        }

        //read data from txt file
        public static void InitDataFromTxt()
        {
            StreamReader reader = File.OpenText("../../Data/DataTables/weapons.txt");
            while(true)
            {
                string line = reader.ReadLine();
                if (string.IsNullOrEmpty(line))
                    break;
                string[] cols = line.Split(new char[] { '\t' }, StringSplitOptions.None);
                int len = cols.Length;
                for (int i = 0; i < len; i++)
                {
                    Console.Write(cols[i] + "\t");
                }
                Console.Write("\n");
            }
            reader.Close();
            reader.Dispose();
        }

        public static void LoadTables()
        {
            //加载并解析表格
            DataProvider.Instance.Init(delegate(string path) 
            {
                string text = File.ReadAllText(Path.Combine("../../Data/DataTables/", path));
                return text;
            });
            Equip e = EquipData.Instance.GetDataById(2);
            if(e != null)
            {
                Console.WriteLine(e.ItemList[2]);
            }
            weapons w = weaponsData.Instance.GetDataById(2);
            Console.WriteLine(w.ModelList[2]);
        }

        public static void GenerateTable()
        {
            //将excel表格转换成txt，然后生成对应的cs文件

            HeaderToCS.Instance.TemplateFile = "../../Data/template.txt";//模版文件
            HeaderToCS.Instance.DestRootPath = "../../Data/";

            ExcelToText.Instance.SrcRootPath = "../../Data/DataTables/";
            ExcelToText.Instance.DestRootPath = "../../Data/DataTables/";

            ExcelToText.Instance.AddTask("weapons.xls", "weapons.txt", HeaderToCS.Instance.GenerateCS);
            ExcelToText.Instance.AddTask("Equip.xls", "Equip.txt", HeaderToCS.Instance.GenerateCS);

            ExcelToText.Instance.Start();

            ExcelToText.Instance.WaitingFinish();

            HeaderToCS.Instance.Flush("AllDataTable.cs");
        }
    }
}
