using NetCapDemo.Application;
using NetCapDemo.Worker;

var builder = Host.CreateApplicationBuilder(args);

var dbConnection = builder.Configuration.GetConnectionString("DbConnection");
if (string.IsNullOrEmpty(dbConnection)) throw new ArgumentNullException("DbConnection");

var azureServiceBusConnection = builder.Configuration.GetSection("AzureServiceBus:Connection");
if (string.IsNullOrEmpty(azureServiceBusConnection.Value))
{
    throw new ArgumentNullException("AzureServiceBus:ConnectionString");
}

var azureServiceBusTopic = builder.Configuration.GetSection("AzureServiceBus:Topic");
if (string.IsNullOrEmpty(azureServiceBusTopic.Value))
{
    throw new ArgumentNullException("AzureServiceBus:Topic");
}

builder.Services.AddSingleton(new DbConnection(dbConnection));
builder.Services.AddScoped<DatabaseDeploymentService>();

builder.Services.AddCap(cap =>
{
    // Use SQL Server as the storage
    cap.UseSqlServer(dbConnection);

    // Use Azure Service Bus as the message queue
    cap.UseAzureServiceBus(opt =>
    {
        opt.ConnectionString = azureServiceBusConnection.Value;
        opt.TopicPath = azureServiceBusTopic.Value;
    });
});

builder.Services.RegisterApplicationEventHandlers();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var dbDeployment = scope.ServiceProvider.GetRequiredService<DatabaseDeploymentService>();
    dbDeployment.Deploy();
}

host.Run();