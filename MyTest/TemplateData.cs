///
/// 本文件由工具自动生成，请不要手动修改！
///
using System;
using System.Collections.Generic;

namespace MyTest
{
    public class TemplateUnit : IDataUnit
    {
        public int Id;
        public int[] Skills;
        public List<int> Items;
        public void Load(MyDataRow row)
        {
            Id = DataParser.Parse<int>(row, 0, -1);
            Skills = DataParser.ParseArry<int>(row, 1, 4, -1);
            Items = DataParser.ParseList<int>(row, "Item", -1);
        }
        public int GetId()
        {
            return Id;
        }
    }
    public class TemplateData : DataInstance<TemplateUnit> { }

    public partial class DataProvider
    {
        private void LoadAllData()
        {
            LoadData(TemplateData.Instance, "Template.txt");
        }
    }
}
