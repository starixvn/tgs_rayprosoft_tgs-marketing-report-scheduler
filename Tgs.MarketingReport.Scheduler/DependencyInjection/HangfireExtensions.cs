using System.Runtime.InteropServices;
using Hangfire;

namespace Tgs.MarketingReport.Scheduler.DependencyInjection;

public static class HangfireExtensions
{
    private static readonly string _TimeZoneId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
        ? "China Standard Time"
        : "Asia/Shanghai";

    public static IApplicationBuilder UseJobRegister(this IApplicationBuilder app)
    {
        RecurringJob.AddOrUpdate<JobExecutor>(
            nameof(JobExecutor),
            p => p.ExecuteAsync(),
            Cron.Daily(6, 0),
            TimeZoneInfo.FindSystemTimeZoneById(_TimeZoneId));

        return app;
    }
}