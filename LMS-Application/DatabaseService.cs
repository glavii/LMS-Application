namespace LMS-Applicatiion;

public class DatabaseService
{
    private readonly SQLiteConnection _conn;
    public DatabaseService()
    {
        var dbName = "FarmData.db";
        //in production
        //var dbPath = Path.Combine(FileSystem.AppDataDirectory, dbName);
        //during debugging
        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "debug"+dbName);
        if (File.Exists(dbPath) == false)
        {
            using Stream stream = Current.OpenAppPackageFileAsync(dbName).Result;
            using MemoryStream memoryStream = new();
            stream.CopyTo(memoryStream);
            File.WriteAllBytes(dbPath, memoryStream.ToArray());
        }
        _conn = new SQLiteConnection(dbPath);
    }
    public List<Store> ReadItems()
    {
        var stores = new List<Store>();
        var lst1 = _conn.Table<Cow>().ToList();
        stores.AddRange(lst1);
        var lst2 = _conn.Table<Sheep>().ToList();
        stores.AddRange(lst2);
        return stores;
    }
    //CRUD
    public int InsertItem(Store item)
    {
        return _conn.Insert(item);
    }
    public int DeleteItem(Store item)
    {
        return _conn.Delete(item);
    }
    public int UpdateItem(Store item)
    {
        return _conn.Update(item);
    }
}
