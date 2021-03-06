///
/// This file is generated by machine, You shouldn't edit it manual.
///
using System;
using System.Collections.Generic;

namespace GenerateCMD
{
    
    public class Config : IDataUnit
    {
        public int Id;
		public string Desc;
		public string Value;
		
        public void Load(MyDataRow row)
        {
            Id = DataParser.Parse<int>(row, "Id", -1);
			Desc = DataParser.Parse<string>(row, "Desc", string.Empty);
			Value = DataParser.Parse<string>(row, "Value", string.Empty);
			
        }
        public int GetId()
        {
            return Id;
        }
    }
    public class ConfigData : DataInstance<Config> { }
    
    public partial class DataProvider
    {
        private void LoadAllData()
        {
            LoadData(ConfigData.Instance, "Config.txt");
			
        }
    }
}
