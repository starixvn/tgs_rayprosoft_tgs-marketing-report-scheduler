using Tgs.MarketingReport.DailyAggregate.Client;
using Tgs.ServiceUtility;

namespace Tgs.MarketingReport.Scheduler.ApplicationServices;

public class ExecuteCommandService : ICommandService<ExecuteCommand>
{
    private readonly IMarketingReportDailyAggregateClient _reportClient;

    public ExecuteCommandService(
        IMarketingReportDailyAggregateClient reportClient)
    {
        _reportClient = reportClient ?? throw new ArgumentNullException(nameof(reportClient));
    }

    public ValueTask ExecuteAsync(ExecuteCommand command, CancellationToken cancellationToken = default)
        => _reportClient.GenerateMarkingDailyAggregateReportByDailyAsync(cancellationToken);
}