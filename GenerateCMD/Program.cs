using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateCMD
{
    class Program
    {
        public enum ConfigDef
        {
            TemplatePath = 1,
            CsFileOutputDir = 2,
            ExcelInputDir = 3,
            TxtOutputDir = 4,
            CsFileName = 5,
        }
        static void Main(string[] args)
        {
            DataProvider.Instance.Init(delegate(string path) { return File.ReadAllText(Path.Combine("", path)); });

            Config templatePath = ConfigData.Instance.GetDataById((int)ConfigDef.TemplatePath);
            Config csFileOutputDir = ConfigData.Instance.GetDataById((int)ConfigDef.CsFileOutputDir);
            Config excelInputDir = ConfigData.Instance.GetDataById((int)ConfigDef.ExcelInputDir);
            Config txtOutputDir = ConfigData.Instance.GetDataById((int)ConfigDef.TxtOutputDir);
            Config csFileName = ConfigData.Instance.GetDataById((int)ConfigDef.CsFileName);

            HeaderToCS.Instance.TemplateFile = templatePath.Value;//模版文件
            HeaderToCS.Instance.DestRootPath = csFileOutputDir.Value;
            HeaderToCS.Instance.Init();

            ExcelToText.Instance.SrcRootPath = excelInputDir.Value;
            ExcelToText.Instance.DestRootPath = txtOutputDir.Value;

            //扫描excelInputDir下的所有excel文件作为输入
            string[] dirs = Directory.GetFiles(excelInputDir.Value, "*.xls", SearchOption.AllDirectories);
            if(dirs != null)
            {
                int len = dirs.Length;
                for (int i = 0; i < len; i++)
                {
                    string path = dirs[i];
                    path = path.Replace(@"\", "/");
                    if (path.EndsWith(".xls") || path.EndsWith(".xlsx"))
                    {
                        string relativePath = path.Replace(excelInputDir.Value, "");
                        string relativeTxt = relativePath.Substring(0, relativePath.LastIndexOf('.')) + ".txt";
                        ExcelToText.Instance.AddTask(relativePath, relativeTxt, HeaderToCS.Instance.GenerateCS);
                        Console.WriteLine(relativePath + "--" + relativeTxt);
                    }
                }
            }
            
            ExcelToText.Instance.Start();

            ExcelToText.Instance.WaitingFinish();

            HeaderToCS.Instance.Flush(csFileName.Value);
            

            Console.ReadKey();
        }
    }
}
