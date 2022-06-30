using Tgs.MarketingReport.Scheduler.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.MarketingReport.Scheduler;

public class JobExecutor : IJobExecutor
{
    private readonly ICommandService<ExecuteCommand> _commandService;

    public JobExecutor(
        ICommandService<ExecuteCommand> commandService)
    {
        _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
    }

    public ValueTask ExecuteAsync()
        => _commandService.ExecuteAsync(new ExecuteCommand());
}