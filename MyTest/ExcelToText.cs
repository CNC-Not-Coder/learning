using System;
using System.Collections.Generic;
using System.Threading;
using Excel;
using System.IO;
using System.Data;

namespace MyTest
{
    public class ExcelToText
    {
        public static ExcelToText Instance = new ExcelToText();
        public delegate void ParseCompleteDelegate(string tableRelativePath, List<string> header, List<string> types);//<列名, 类型>
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
            List<string> header = new List<string>();
            List<string> types = new List<string>();
            string txtContent = string.Empty;
            for (int i = 0; i < row; i++)
            {
                if(i == 1)
                {//注释第二行的类型
                    txtContent += "#";
                }
                for (int j = 0; j < col; j++)
                {
                    if(j < col - 1)
                    {
                        txtContent += string.Format("{0}\t", table.Rows[i][j].ToString());
                    }
                    else
                    {
                        txtContent += table.Rows[i][j].ToString();
                    }
                    if(i == 0)
                    {
                        header.Add(table.Rows[i][j].ToString());
                    }
                    else if(i == 1)
                    {
                        types.Add(table.Rows[i][j].ToString());
                    }
                }
                txtContent += "\n";
            }

            if(File.Exists(destFile))
            {
                File.Delete(destFile);
            }
            FileStream output = File.OpenWrite(destFile);
            StreamWriter writer = new StreamWriter(output);
            writer.Write(txtContent);
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
                callBack(destRelativeFile, header, types);

            return true;
        }
    }
}
