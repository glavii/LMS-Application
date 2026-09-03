namespace LMS_Application;

public abstract class Livestock
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public double Expense { get; set; }
    public double Weight { get; set; }
    public string Colour { get; set; } = string.Empty;
    [Ignore]
    public string Type => GetType().Name;
}

[Table("Cow")]
public class Cow : Livestock
{
    public double Milk { get; set; }
}

[Table("Sheep")]
public class Sheep : Livestock
{
    public double Wool { get; set; }
}
