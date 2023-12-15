using DbUp;

namespace NetCapDemo.Application;

public sealed class DatabaseDeploymentService(DbConnection dbConnection)
{
    public void Deploy()
    {
        EnsureDatabase.For.SqlDatabase(dbConnection.ConnectionString);
    }
}