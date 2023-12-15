using DotNetCore.CAP;
using NetCapDemo.Application;

var builder = WebApplication.CreateBuilder(args);

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

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

    // Web application will not consume messages
    cap.ConsumerThreadCount = 0;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbDeployment = scope.ServiceProvider.GetRequiredService<DatabaseDeploymentService>();
    dbDeployment.Deploy();
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/api/user", async (ICapPublisher capBus) =>
    {
        var newUser = new NewUserAddedEvent(Guid.NewGuid(), "Joe", "Due");
        await capBus.PublishAsync(nameof(NewUserAddedEvent), newUser);
        return Results.Ok(newUser);
    })
    .WithName("AddUser")
    .WithOpenApi();

app.Run();