namespace Tgs.MarketingReport.Scheduler;

public interface IJobExecutor
{
    public ValueTask ExecuteAsync();
}