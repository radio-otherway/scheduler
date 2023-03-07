using Quartz;

namespace OtherWay.Radio.Scheduler.Services.Jobs;

public class JobTriggerCallback : IJob {
  private readonly OtherwayAPIProxy _proxy;
  private readonly ILogger<JobTriggerCallback> _logger;

  public JobTriggerCallback(OtherwayAPIProxy proxy, ILogger<JobTriggerCallback> logger) {
    _proxy = proxy;
    _logger = logger;
  }

  public async Task Execute(IJobExecutionContext context) {
    var dataMap = context.JobDetail.JobDataMap;
    var showId = dataMap.GetString("ShowId");
    if (!string.IsNullOrEmpty(showId)) {
      var result = await _proxy.MakePost("/api/cron/notify", new {showId});
      if (!string.IsNullOrEmpty(result)) {
        _logger.LogError("Error sending show notification: {Reason}", result);
      }
    }
  }
}