///
/// 本文件由工具自动生成，请不要手动修改！
///
using System;
using System.Collections.Generic;

namespace MyTest
{
    
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
    
    public partial class DataProvider
    {
        private void LoadAllData()
        {
            LoadData(weaponsData.Instance, "weapons.txt");
LoadData(EquipData.Instance, "Equip.txt");

        }
    }
}
