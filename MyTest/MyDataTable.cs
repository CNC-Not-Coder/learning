using System;
using System.Collections.Generic;
using System.IO;


namespace MyTest
{
    public class MyDataRow
    {
        public int RowId { get; set; }
        public string TableName { get; set; }
        private string[] vals = null;
        public MyDataRow(string[] _vals)
        {
            if (_vals == null || _vals.Length < 1)
                return;
            vals = _vals;;
        }
        public string this[int col]
        {
            get 
            {
                if (vals == null || col < 0 || col > vals.Length - 1)
                    return null;
                return vals[col];
            }
        }
        public int ColNum
        {
            get 
            {
                if (vals != null) 
                    return vals.Length;
                else 
                    return 0;
            }
        }
    }
    public class MyDataTable
    {
        private List<MyDataRow> mData = new List<MyDataRow>();
        private string mTableName;
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

                    //直接读数据，这里没有表头和数据类型
                    string[] fields = line.Split(new char[] { '\t' }, StringSplitOptions.None);
                    if (fields == null) break;

                    MyDataRow row = new MyDataRow(fields);
                    row.RowId = rowId;
                    row.TableName = mTableName;
                    mData.Add(row);
                    rowId++;
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
        public List<MyDataRow> GetData()
        {
            return mData;
        }
    }
}
