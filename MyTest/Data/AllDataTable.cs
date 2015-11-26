
    public class weapons : IDataUnit
    {
        public int Id;
public string Desc;
public string Name;
public int Hp;
public int Mp;

        public void Load(MyDataRow row)
        {
            Id = DataParser.Parse<int>(row, "Id", -1);
Desc = DataParser.Parse<string>(row, "Desc", string.Empty);
Name = DataParser.Parse<string>(row, "Name", string.Empty);
Hp = DataParser.Parse<int>(row, "Hp", -1);
Mp = DataParser.Parse<int>(row, "Mp", -1);

        }
        public int GetId()
        {
            return Id;
        }
    }
    public class weaponsData : DataInstance<weapons> { }
    