using Quartz;

namespace OtherWay.Radio.Scheduler.Services.Jobs;

public class CacheCalendarJob : IJob {
  private readonly OtherwayAPIProxy _proxy;
  private readonly ILogger<CacheCalendarJob> _logger;

  public CacheCalendarJob(OtherwayAPIProxy proxy, ILogger<CacheCalendarJob> logger) {
    _proxy = proxy;
    _logger = logger;
  }

  public async Task Execute(IJobExecutionContext context) {
    var result = await _proxy.MakeGet("/api/cron/cache");
    if (!string.IsNullOrEmpty(result)) {
      _logger.LogError("Error sending show notification: {Reason}", result);
    }
  }
}