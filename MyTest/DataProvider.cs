using System;
using System.Collections.Generic;

namespace MyTest
{
    public interface IDataUnit
    {
        void Load(MyDataRow row);
        int GetId();
    }
    public class DataInstance<T> where T : IDataUnit, new()
    {
        public static DataInstance<T> Instance = new DataInstance<T>();
        private Dictionary<int, T> mData = new Dictionary<int, T>();
        public void Load(MyDataTable table)
        {
            var rows = table.GetData();
            int len = rows.Count;
            for (int i = 0; i < len; i++)
            {
                T unit = new T();
                unit.Load(rows[i]);
                int id = unit.GetId();
                if (id != -1 && !mData.ContainsKey(id))
                {
                    mData.Add(id, unit);
                }
            }
        }
        public T GetDataById(int id)
        {
            if (mData.ContainsKey(id))
                return mData[id];
            return default(T);
        }
        public Dictionary<int, T> GetData()
        {
            return mData;
        }
    }
    public class DataParser
    {
        public static T Parse<T>(MyDataRow row, string colName, T defaultVal)
        {
            T val = defaultVal;
            if (row == null || string.IsNullOrEmpty(colName))
                return val;

            try
            {
                if (!string.IsNullOrEmpty(row[colName]))
                    val = (T)Convert.ChangeType(row[colName], typeof(T));
            }
            catch (Exception e)
            {
                DataProvider.Instance.Log("DataParser.Parse<{0}> at Table: {1}({2}, {3}), error: {4}", typeof(T).ToString(), row.Table.TableName, row.RowId, colName, e.Message);
            }
            return val;
        }
        public static List<T> ParseList<T>(MyDataRow row, string prefix, T defaultVal)
        {
            List<T> results = new List<T>();
            //下标从0开始,最大到19
            int maxIndex = 20;
            for (int i = 0; i < maxIndex; i++)
            {
                string colName = string.Format("{0}{1}", prefix, i);
                int index = row.Table.GetHeaderIndexByColName(colName);
                if (index < 0)
                    break;
                results.Add(Parse<T>(row, colName, defaultVal));
            }
            return results;
        }
        public static T Parse<T>(MyDataRow row, int col, T defaultVal)
        {
            T val = defaultVal;
            if (row == null || col > row.ColNum - 1)
                return val;
            
            try
            {
                if (!string.IsNullOrEmpty(row[col]))
                    val = (T)Convert.ChangeType(row[col], typeof(T));
            }
            catch(Exception e)
            {
                DataProvider.Instance.Log("DataParser.Parse<{0}> at Table: {1}({2}, {3}), error: {4}", typeof(T).ToString(), row.Table.TableName, row.RowId, col, e.Message);
            }
            return val;
        }
        public static T[] ParseArry<T>(MyDataRow row, int beg_col, int end_col, T defaultVal)
        {
            if (beg_col >= end_col)
                return default(T[]);
            T[] vals = new T[end_col - beg_col + 1];//包括end_col
            int len = vals.Length;
            for (int i = 0; i < len; i++)
            {
                vals[i] = defaultVal;
                vals[i] = Parse<T>(row, beg_col + i, defaultVal);
            }
            return vals;
        }
    }
    /// <summary>
    /// 负责从文本读取数据，新建MyDataTable
    /// </summary>
    public partial class DataProvider
    {
        public delegate string ReadAllTextDelegate(string relative_path);
        public delegate void LogDelegate(string format, params object[] p);
        private ReadAllTextDelegate readAllTextHandler = null;
        private LogDelegate logHandler = null;
        private static DataProvider _Instance = new DataProvider();
        public static DataProvider Instance
        {
            get { return _Instance; }
        }
        public void Init(ReadAllTextDelegate handler)
        {//Call By Outer
            if (handler == null)
                return;
            readAllTextHandler = handler;
            LoadAllData();
        }
        private void LoadData<T>(DataInstance<T> container, string relativePath) where T : IDataUnit, new()
        {
            string data = ReadAllText(relativePath);
            if (string.IsNullOrEmpty(data))
                return;
            MyDataTable table = new MyDataTable(data, relativePath);
            container.Load(table);
        }
        public void Log(string format, params object[] p)
        {
            if (logHandler != null)
                logHandler(format, p);
        }
        public string ReadAllText(string relativePath)
        {
            try
            {
                if (readAllTextHandler == null)
                    return string.Empty;
                string data = readAllTextHandler(relativePath);
                if (string.IsNullOrEmpty(data))
                    return string.Empty;
                return data;
            }
            catch(Exception e)
            {
                Log("Read All Text ERROR : {0}", e.Message);
            }
            return string.Empty;
        }
    }
}
