using CrystalQuartz.Core.Utils;
using OtherWay.Radio.Scheduler.Models;
using Quartz;
using Quartz.Impl.Matchers;

namespace OtherWay.Radio.Scheduler.Services.Extensions;

public static class JobExtensions {
  public static async Task<List<JobInfoModel>> GetAllJobs(this IScheduler scheduler) {
    var jobGroups = await scheduler.GetJobGroupNames();

    var result = jobGroups.Select(group => new JobInfoModel {
      Group = group,
      Jobs = scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupContains(group)).Result.Select(jobKey =>
        new JobInfoModelJobs {
          Detail = scheduler.GetJobDetail(jobKey).Result?.Description,
          JobName = jobKey.Name,
          Triggers = scheduler.GetTriggersOfJob(jobKey).Result.Select(trigger => new JobInfoModelTriggers {
            Name = trigger.Key.Name,
            Group = trigger.Key.Group,
            Type = trigger.GetType().Name,
            State = scheduler.GetTriggerState(trigger.Key).Result,
            NextFireTime = trigger.GetNextFireTimeUtc(),
            NextFireTimeHuman = trigger.GetNextFireTimeUtc().ToDateTime()?.ToLongDateTimeString(),
            PreviousFireTime = trigger.GetPreviousFireTimeUtc(),
            PreviousFireTimeHuman = trigger.GetPreviousFireTimeUtc().ToDateTime()?.ToLongDateTimeString(),
          }).ToList()
        }).ToList()
    });
    return result.ToList();
  }
}