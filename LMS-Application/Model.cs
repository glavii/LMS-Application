namespace LMS-Application;

public class Store
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public double Expense { get; set; }
    public double Weight { get; set; }
    public string Colour { get; set; }
}
[Table("Cow")]
public class Cow : Store
{
    public double Milk { get; set; }
}
[Table("Sheep")]
public class Sheep : Store
{
    public double Wool { get; set; }
}
