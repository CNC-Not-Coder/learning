using System;
using System.Collections.Generic;
using System.IO;


namespace MyTest
{
    public class MyDataRow
    {
        private List<string> vals = new List<string>();
        private MyDataTable mTable = null;
        public MyDataRow(MyDataTable table, List<string> _vals)
        {
            if (_vals == null || _vals.Count < 1)
                return;
            vals = _vals;;
            mTable = table;
        }
        public int RowId { get; set; }
        public MyDataTable Table
        {
            get { return mTable; }
            set { mTable = value; }
        }
        public string this[int col]
        {
            get 
            {
                if (vals == null || col < 0 || col > vals.Count - 1)
                    return null;
                return vals[col];
            }
        }
        public string this[string colName]
        {
            get
            {
                if (mTable == null)
                    return string.Empty;
                int index = mTable.GetHeaderIndexByColName(colName);
                if (index < 0)
                    return string.Empty;
                return this[index];
            }
        }
        public int ColNum
        {
            get 
            {
                if (vals != null) 
                    return vals.Count;
                else 
                    return 0;
            }
        }
    }
    public class MyDataTable
    {
        private List<MyDataRow> mData = new List<MyDataRow>();
        private string mTableName;
        private List<string> mHeader = new List<string>();
        public MyDataTable(string data, string tableName)
        {
            if (string.IsNullOrEmpty(data))
                return;
            mTableName = tableName;
            StringReader reader = new StringReader(data);
            try
            {
                int rowId = 0;
                while (true)
                {
                    string line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line)) break;
                    if (line.StartsWith("#")) continue;

                    string[] fields = line.Split(new char[] { '\t' }, StringSplitOptions.None);
                    if (fields == null) break;

                    if (rowId == 0)
                    {//表头
                        mHeader.AddRange(fields);
                    }
                    else
                    {//直接读数据，这里没有数据类型
                        List<string> datas = new List<string>(fields);
                        MyDataRow row = new MyDataRow(this, datas);
                        row.RowId = rowId;
                        mData.Add(row);
                        rowId++;
                    }
                }
            }
            catch(Exception e)
            {
                DataProvider.Instance.Log("Load MyDataTable error: {0}", e.Message);
            }
            finally
            {
                reader.Dispose();
            }
        }
        public string TableName 
        {
            get { return mTableName; }
        }

        public List<MyDataRow> GetData()
        {
            return mData;
        }
        public int GetHeaderIndexByColName(string colName)
        {
            int index = mHeader.FindIndex(p => p == colName);
            return index;
        }
    }
}
