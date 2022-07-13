using System.Reflection;

namespace Tgs.MarketingReport.Scheduler.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddScheduler(this IServiceCollection services)
        => services
            .RegisterAllServiceUtilityTypes(
                Assembly.GetExecutingAssembly(),
                ServiceLifetime.Transient);
}