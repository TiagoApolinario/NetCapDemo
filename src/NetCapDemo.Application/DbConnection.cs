namespace NetCapDemo.Application;

public sealed class DbConnection(string connectionString)
{
    public readonly string ConnectionString = connectionString;
}