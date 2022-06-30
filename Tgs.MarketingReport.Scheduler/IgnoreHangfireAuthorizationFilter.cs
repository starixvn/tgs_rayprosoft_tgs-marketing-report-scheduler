using Hangfire.Dashboard;

namespace Tgs.MarketingReport.Scheduler;

public class IgnoreHangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context) => true;
}