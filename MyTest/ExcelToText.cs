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
        public delegate void ParseCompleteDelegate(List<Tuple<string, string>> header_out);
        private Queue<Tuple<string, string, ParseCompleteDelegate>> mTasks = new Queue<Tuple<string, string, ParseCompleteDelegate>>();
        private object _lock = new object();
        private Thread thread1 = null;
        private Thread thread2 = null;
        private Thread thread3 = null;
        public void Start()
        {//多线程初始化
            thread1 = new Thread(DoTask);
            thread2 = new Thread(DoTask);
            thread3 = new Thread(DoTask);

            thread1.Start();
            thread2.Start();
            thread3.Start();
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
            thread2.Join();
            thread3.Join();
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

                if (!ParseExcel(srcExcel, destFile, callBack))
                {
                    break;
                }
            }
        }
        private bool ParseExcel(string srcExcel, string destFile, ParseCompleteDelegate callBack)
        {
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
            excelReader.IsFirstRowAsColumnNames = true;//第一行不会进入DataSet
            DataSet result = excelReader.AsDataSet();
            
            //先按列名排序
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

            //Free resources (IExcelDataReader is IDisposable)
            result.Dispose();
            excelReader.Close();
            excelReader.Dispose();
            stream.Close();
            stream.Dispose();
            return true;
        }
    }
}
