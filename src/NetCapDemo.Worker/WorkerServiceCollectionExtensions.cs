using DotNetCore.CAP;
using NetCapDemo.Application;

namespace NetCapDemo.Worker;

public static class WorkerServiceCollectionExtensions
{
    /// <summary>
    /// Registers all classes that implement the ICapSubscribe interface from the assembly where IApplicationMarker is defined,
    /// as transient services in the dependency injection container.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    public static void RegisterApplicationEventHandlers(this IServiceCollection services)
    {
        var applicationTypes = typeof(IApplicationMarker).Assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(ICapSubscribe).IsAssignableFrom(t));

        foreach (var type in applicationTypes)
        {
            services.AddTransient(type);
        }
    }
}