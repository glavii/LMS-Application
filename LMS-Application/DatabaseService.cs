namespace LMS_Application;

public class DatabaseService
{
    private readonly SQLiteConnection _conn;
    public SQLiteConnection Connection => _conn;

    public DatabaseService()
    {
        var dbName = "FarmData.db";
        //in production
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, dbName);
        //during debugging
        //var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "debug" + dbName);

        if (File.Exists(dbPath) == false)
        {
            using Stream stream = Current.OpenAppPackageFileAsync(dbName).Result;
            using MemoryStream memoryStream = new();
            stream.CopyTo(memoryStream);
            File.WriteAllBytes(dbPath, memoryStream.ToArray());
        }
        _conn = new SQLiteConnection(dbPath);
    }

    public List<Livestock> ReadItems()
    {
        var livestock = new List<Livestock>();
        var lst1 = _conn.Table<Cow>().ToList();
        livestock.AddRange(lst1);
        var lst2 = _conn.Table<Sheep>().ToList();
        livestock.AddRange(lst2);
        return livestock;
    }

    //CRUD
    public int InsertItem(Livestock item)
    {
        return _conn.Insert(item);
    }


    public Livestock? GetItemById(int id)
    {
        var cow = _conn.Table<Cow>().FirstOrDefault(c => c.Id == id);
        if (cow is not null) return cow;

        var sheep = _conn.Table<Sheep>().FirstOrDefault(s => s.Id == id);
        if (sheep is not null) return sheep;

        return null;
    }

    public int DeleteItem(int id)
    {
        var item = GetItemById(id);
        if (item is null) return 0;

        return _conn.Delete(item); // uses your existing DeleteItem(Livestock item) overload
    }
  

    public int UpdateItem(Livestock item)
    {
        return _conn.Update(item);
    }
}
