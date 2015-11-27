///
/// This file is generated by machine, You shouldn't edit it manual.
///
using System;
using System.Collections.Generic;

namespace MyNamespace
{
    
    public class config : IDataUnit
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
    public class configData : DataInstance<config> { }
    
    public class Equip : IDataUnit
    {
        public int Id;
		public string Desc;
		public string Name;
		public int Hp;
		public int Mp;
		public List<int> ItemList;
		
        public void Load(MyDataRow row)
        {
            Id = DataParser.Parse<int>(row, "Id", -1);
			Desc = DataParser.Parse<string>(row, "Desc", string.Empty);
			Name = DataParser.Parse<string>(row, "Name", string.Empty);
			Hp = DataParser.Parse<int>(row, "Hp", -1);
			Mp = DataParser.Parse<int>(row, "Mp", -1);
			ItemList = DataParser.ParseList<int>(row, "Item", -1);
			
        }
        public int GetId()
        {
            return Id;
        }
    }
    public class EquipData : DataInstance<Equip> { }
    
    public class weapons : IDataUnit
    {
        public int Id;
		public string Desc;
		public string Name;
		public int Hp;
		public int Mp;
		public List<int> ItemList;
		public List<string> ModelList;
		
        public void Load(MyDataRow row)
        {
            Id = DataParser.Parse<int>(row, "Id", -1);
			Desc = DataParser.Parse<string>(row, "Desc", string.Empty);
			Name = DataParser.Parse<string>(row, "Name", string.Empty);
			Hp = DataParser.Parse<int>(row, "Hp", -1);
			Mp = DataParser.Parse<int>(row, "Mp", -1);
			ItemList = DataParser.ParseList<int>(row, "Item", -1);
			ModelList = DataParser.ParseList<string>(row, "Model", string.Empty);
			
        }
        public int GetId()
        {
            return Id;
        }
    }
    public class weaponsData : DataInstance<weapons> { }
    
    public class S_10001 : IDataUnit
    {
        public int Id;
		public string Desc;
		public string Name;
		
        public void Load(MyDataRow row)
        {
            Id = DataParser.Parse<int>(row, "Id", -1);
			Desc = DataParser.Parse<string>(row, "Desc", string.Empty);
			Name = DataParser.Parse<string>(row, "Name", string.Empty);
			
        }
        public int GetId()
        {
            return Id;
        }
    }
    public class S_10001Data : DataInstance<S_10001> { }
    
    public partial class DataProvider
    {
        private void LoadAllData()
        {
            LoadData(configData.Instance, "config.txt");
			LoadData(EquipData.Instance, "Equip.txt");
			LoadData(weaponsData.Instance, "weapons.txt");
			LoadData(S_10001Data.Instance, "Scenes/S_10001.txt");
			
        }
    }
}