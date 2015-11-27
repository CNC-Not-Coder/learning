using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace GenerateCMD
{
    public class HeaderToCS
    {
        public static HeaderToCS Instance = new HeaderToCS();
        public string TemplateFile { get; set; }
        public string DestRootPath { get; set; }
        private List<string> mTableLoads = new List<string>();
        private object _lockOfTableLoads = new object();

        private StringBuilder tableOutput = new StringBuilder();
        private StringBuilder loaderOutput = new StringBuilder();
        private string mTemplate = string.Empty;
        private string mTableTemplate = string.Empty;
        public void Init()
        {
            if (string.IsNullOrEmpty(TemplateFile))
                return;
            if (!File.Exists(TemplateFile))
                return;
            string template = File.ReadAllText(TemplateFile);
            if (string.IsNullOrEmpty(template))
                return;
            mTemplate = template;

            string begFlag = "@{RepeatTable-Beg}";
            string endFlag = "@{RepeatTable-End}";
            int indexBeg = template.IndexOf(begFlag);
            int indexEnd = template.IndexOf(endFlag);
            if (indexBeg < 0 || indexEnd < 0 || indexEnd < indexBeg)
                return;
            string tableTemplate = template.Substring(indexBeg + begFlag.Length, indexEnd - indexBeg - endFlag.Length);
            mTableTemplate = tableTemplate;
        }
        public void GenerateCS(string tableRelativePath, List<string> header, List<string> types)
        {
            if (header == null || types == null)
                return;
            if (header.Count != types.Count)
                return;
            string propertyDeclares = "";
            string propertyParsers = "";
            string firstCol = "";//第一列默认为Id
            int len = header.Count;
            if (len > 0)
                firstCol = header[0];
            for (int i = 0; i < len; i++)
            {
                if (string.IsNullOrEmpty(header[i]) || string.IsNullOrEmpty(types[i]))
                    continue;

                string typeOfField = types[i].ToLower();
                string colName = header[i];
                
                string strDeclare, strParser;

                if (IsListCol(colName))
                {//要按数组读取
                    string prefix = GetPrefixAndMarkColumnDisable(header, i);
                    string fieldName = string.Format("{0}List", prefix);
                    strDeclare = string.Format("public List<{0}> {1};", typeOfField, fieldName);
                    strParser = string.Format("{0} = DataParser.ParseList<{1}>(row, \"{2}\", {3});", fieldName, typeOfField, prefix, GetDefaultVal(typeOfField));
                }
                else
                {//仅读取这一列
                    strDeclare = string.Format("public {0} {1};", typeOfField, colName);
                    strParser = string.Format("{0} = DataParser.Parse<{1}>(row, \"{2}\", {3});", colName, typeOfField, colName, GetDefaultVal(typeOfField));
                }
                propertyDeclares += strDeclare + "\r\n\t\t" ;
                propertyParsers += strParser + "\r\n\t\t\t";
            }
            string tableName = Path.GetFileNameWithoutExtension(tableRelativePath);
            string strLoader = string.Format("LoadData({0}Data.Instance, \"{1}\");\r\n\t\t\t", tableName, tableRelativePath);

            OutputToCSBuffer(tableName, firstCol, propertyDeclares, propertyParsers, strLoader);
        }
        private string GetPrefixAndMarkColumnDisable(List<string> cols, int col)
        {//通过列名获得具有相同前缀的列的索引
            string prefix = string.Empty;
            Match m = Regex.Match(cols[col], @"^\D+");
            if(m.Success)
            {
                prefix = m.Value;
                int len = cols.Count;
                for (int i = col + 1; i < len; i++)
                {
                    //前缀匹配
                    bool isMatch = Regex.IsMatch(cols[i], string.Format(@"{0}\d+$", prefix));
                    if (isMatch) cols[i] = null; //表示这一列已经当作数组处理了，不需要再处理
                }
            }
            return prefix;
        }
        private bool IsListCol(string colName)
        {
            //Item0,Item1,Item2, 非数字+数字的组合
            return Regex.IsMatch(colName, @"^\D+\d+$");
        }
        private string GetDefaultVal(string t)
        {
            string def = "";
            switch(t)
            {
                case "int": def = "-1"; break;
                case "float": def = "0f"; break;
                case "string": def = "string.Empty"; break;
                default: throw new Exception("Unsupported Type " + t);
            }
            return def;
        }
        private void OutputToCSBuffer(string tableName, string firstColName, string declares, string parsers, string loader)
        {
            string tableTemplate = mTableTemplate;

            tableTemplate = tableTemplate.Replace("@{TableName}", tableName);
            tableTemplate = tableTemplate.Replace("@{Declares}", declares);
            tableTemplate = tableTemplate.Replace("@{FirstCol}", firstColName);
            tableTemplate = tableTemplate.Replace("@{Parsers}", parsers);
            tableTemplate = tableTemplate.Replace("@{TableProviderName}", string.Format("{0}Data", tableName));

            tableOutput.Append(tableTemplate);
            loaderOutput.Append(loader);
            
        }
        public void Flush(string fileName)
        {
            Regex regex = new Regex(@"@{RepeatTable-Beg}(\w|\W)+@{RepeatTable-End}");
            string output = regex.Replace(mTemplate, tableOutput.ToString());
            output = output.Replace("@{Loaders}", loaderOutput.ToString());
            string destPath = Path.Combine(DestRootPath, fileName);
            if (File.Exists(destPath))
                File.Delete(destPath);
            FileStream stream = File.OpenWrite(destPath);
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(output);
            writer.Flush();
            writer.Dispose();
            stream.Close();
            stream.Dispose();
        }
    }
}
