namespace VerticalSliceTest.Orders.Api.Infrastructure.Persistence.Outbox;

internal sealed partial class ProcessOutboxMessagesService : BackgroundService, IDisposable
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly OutboxOptions _outboxOptions;
    private readonly ILogger<ProcessOutboxMessagesService> _logger;
    private readonly PeriodicTimer _timer;

    public ProcessOutboxMessagesService(
        IServiceScopeFactory serviceScopeFactory,
        IOptions<OutboxOptions> outboxOptions,
        ILogger<ProcessOutboxMessagesService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _outboxOptions = outboxOptions.Value;
        _logger = logger;
        _timer = new(TimeSpan.FromSeconds(_outboxOptions.IntervalInSeconds));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        OutboxWorkerLog.ServiceStarted(_logger);

        while (await _timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false) && !stoppingToken.IsCancellationRequested)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            IOutboxMessageProcessor processor = scope.ServiceProvider.GetRequiredService<IOutboxMessageProcessor>();

            await processor.ProcessAsync(_outboxOptions.BatchSize, stoppingToken).ConfigureAwait(false);
        }
    }

    public override void Dispose()
    {
        _timer.Dispose();
        base.Dispose();
    }
}
