using System.Text.Json;
using OtherWay.Radio.Scheduler.Models;
using OtherWay.Radio.Scheduler.Services.Extensions;
using OtherWay.Radio.Scheduler.Services.Jobs;
using Quartz;

namespace OtherWay.Radio.Scheduler.Services;

public class ScheduleLoader {
  private readonly IHttpClientFactory _httpClientFactory;
  private readonly ISchedulerFactory _schedulerFactory;
  private readonly ILogger<ScheduleLoader> _logger;

  public ScheduleLoader(IHttpClientFactory httpClientFactory, ISchedulerFactory schedulerFactory,
    ILogger<ScheduleLoader> logger) {
    _httpClientFactory = httpClientFactory;
    _schedulerFactory = schedulerFactory;
    _logger = logger;
  }

  private async Task ScheduleJob(IScheduler scheduler, string jobKey, string showId, DateTime date) {
    var jobData = new JobDataMap {
      ["ShowId"] = showId,
      ["date"] = date
    };
    var triggerId = $"{jobKey}-trigger";
    var group = "show-alert-triggers";

    // await scheduler.DeleteJob(new JobKey(jobKey));

    var job = JobBuilder.Create<JobTriggerCallback>()
      .WithIdentity(jobKey, group)
      .UsingJobData(jobData)
      .Build();

    var trigger = TriggerBuilder.Create()
      .WithIdentity(triggerId, group)
      .StartAt(date)
      .Build();

    _logger.LogDebug("Schedule loaded for {Show} as {JobKey} @ {ScheduleDate}",
      showId,
      jobKey,
      date.ToLongDateTimeString());
    var result = await scheduler.UnscheduleJob(new TriggerKey(triggerId, group));
    _logger.LogDebug(
      result
        ? "Successfully unscheduled trigger {Trigger} in group {Group}"
        : "Failed to unschedule trigger {Trigger} in group {Group}", triggerId, group);

    await scheduler.ScheduleJob(job, trigger);
  }

  public async Task LoadSchedules() {
    using var client = _httpClientFactory.CreateClient("otherway");
    var response = await client.GetStreamAsync("/api/shows/notifications");
    var schedules = await JsonSerializer.DeserializeAsync<List<NotificationSchedule>>(response,
      new JsonSerializerOptions {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
      });

    if (schedules is null) {
      return;
    }

    await CreateJobsForSchedules(schedules);
  }

  public async Task CreateJobsForSchedules(List<NotificationSchedule> schedules) {
    var scheduler = await _schedulerFactory.GetScheduler();
    foreach (var schedule in schedules) {
      short i = 1;
      foreach (var slot in schedule.ScheduleTimes.Where(r => r.ScheduleTime >= DateTime.Now)
                 .OrderBy(r => r.ScheduleTime)) {
        Console.WriteLine(
          $"New schedule for {schedule.ShowId} - {slot.ScheduleName} - scheduled at {slot.ScheduleTime.ToLongDateTimeString()}");
        var jobKey = $"{slot.ScheduleName}-{i++}";
        await ScheduleJob(scheduler, jobKey, schedule.ShowId, slot.ScheduleTime);
      }
    }
  }
}