using System;
using System.Collections.Generic;
using System.Threading;
using Excel;
using System.IO;
using System.Data;
using System.Text;

namespace GenerateCMD
{
    public class ExcelToText
    {
        public static ExcelToText Instance = new ExcelToText();
        public delegate void ParseCompleteDelegate(string tableRelativePath, List<string> header, List<string> types, List<string> defVal);//<列名, 类型>
        public string SrcRootPath { get; set; }
        public string DestRootPath { get; set; }
        private Queue<Tuple<string, string, ParseCompleteDelegate>> mTasks = new Queue<Tuple<string, string, ParseCompleteDelegate>>();
        private object _lock = new object();
        private Thread thread1 = null;
        //private Thread thread2 = null;
        //private Thread thread3 = null;
        public void Start()
        {//多线程初始化
            thread1 = new Thread(DoTask);
            //thread2 = new Thread(DoTask);
            //thread3 = new Thread(DoTask);

            thread1.Start();
            //thread2.Start();
            //thread3.Start();
        }
        public void AddTask(string srcExcel, string destFile,ParseCompleteDelegate callBack )
        {
            var task = new Tuple<string, string, ParseCompleteDelegate>(srcExcel, destFile, callBack);
            lock(_lock)
            {
                mTasks.Enqueue(task);
            }
        }
        public void WaitingFinish()
        {
            thread1.Join();
            //thread2.Join();
            //thread3.Join();
        }
        private void DoTask()
        {
            while (true)
            {
                Tuple<string, string, ParseCompleteDelegate> task = null;
                lock (_lock)
                {
                    if (mTasks.Count > 0)
                        task = mTasks.Dequeue();
                }
                if (task == null)
                    break;
                string srcExcel = task.Item1;
                string destFile = task.Item2;
                ParseCompleteDelegate callBack = task.Item3;

                try
                {
                    if (!ParseExcel(srcExcel, destFile, callBack))
                    {
                        break;
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("Exception:{0}, Stack:{1}", e.Message, e.StackTrace);
                    break;
                }
            }
        }
        private bool ParseExcel(string srcRelativeExcel, string destRelativeFile, ParseCompleteDelegate callBack)
        {
            string srcExcel =Path.Combine(SrcRootPath, srcRelativeExcel);
            string destFile = Path.Combine(DestRootPath, destRelativeFile);
            FileStream stream = File.Open(srcExcel, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = null;
            if (srcExcel.EndsWith(".xls"))
            {
                excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
            }
            else if (srcExcel.EndsWith(".xlsx"))
            {
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            }
            if (excelReader == null)
                return false;
            //DataSet - Create column names from first row
            //excelReader.IsFirstRowAsColumnNames = true;
            DataSet result = excelReader.AsDataSet();

            if (result.Tables.Count < 1)
                return false;
            DataTable table = result.Tables[0];
            int row = table.Rows.Count;
            int col = table.Columns.Count;
            if (row < 2 || col < 1)
                return false;
            List<string> header = new List<string>();//第一行
            List<string> types = new List<string>();//第二行
            List<string> defVal = new List<string>();//第三行
            StringBuilder txtContent = new StringBuilder();
            for (int i = 0; i < row; i++)
            {
                bool isDef = false;
                if(i == 1)
                {//注释第2行的类型
                    txtContent.Append("#");
                }
                else if(i == 2)
                {//注释第3行的默认值
                    string firstCol = table.Rows[i][0].ToString();
                    if(firstCol.StartsWith("Def:", StringComparison.OrdinalIgnoreCase))
                    {
                        isDef = true;
                        txtContent.Append("#");
                        table.Rows[i][0] = firstCol.Replace("Def:", "");
                    }
                }

                for (int j = 0; j < col; j++)
                {
                    if(j < col - 1)
                    {
                        txtContent.AppendFormat("{0}\t", table.Rows[i][j].ToString());
                    }
                    else
                    {
                        txtContent.Append(table.Rows[i][j].ToString());
                    }
                    if(i == 0)
                    {
                        header.Add(table.Rows[i][j].ToString());
                    }
                    else if(i == 1)
                    {
                        types.Add(table.Rows[i][j].ToString());
                    }
                    else if(i == 2)
                    {
                        if(isDef)
                        {
                            string def = table.Rows[i][j].ToString();
                            defVal.Add(def);
                        }
                    }
                }
                txtContent.Append("\n");
            }

            if(File.Exists(destFile))
            {
                File.Delete(destFile);
            }
            string dir = Path.GetDirectoryName(destFile);
            if(!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            FileStream output = File.OpenWrite(destFile);
            StreamWriter writer = new StreamWriter(output);
            writer.Write(txtContent.ToString());
            writer.Flush();
            writer.Close();
            writer.Dispose();
            output.Close();
            output.Dispose();

            //Free resources (IExcelDataReader is IDisposable)
            result.Dispose();
            excelReader.Close();
            excelReader.Dispose();
            stream.Close();
            stream.Dispose();

            if(callBack != null)
                callBack(destRelativeFile, header, types, defVal);

            return true;
        }
    }
}
