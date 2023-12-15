using DotNetCore.CAP;

namespace NetCapDemo.Application;

public sealed record NewUserAddedEvent(Guid Id, string Name, string Email);

public sealed class NewUserAddedEventHandler : ICapSubscribe
{
    [CapSubscribe(nameof(NewUserAddedEvent))]
    public void Handle(NewUserAddedEvent user)
    {
        Console.WriteLine($"New user added: Id: {user.Id} | Name: {user.Name} | Email: {user.Email}");
    }
}