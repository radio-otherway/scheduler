using Quartz;

namespace OtherWay.Radio.Scheduler.Models;

public class JobInfoModel {
  public string Group { get; set; }
  public List<JobInfoModelJobs> Jobs { get; set; }
}

public class JobInfoModelJobs {
  public string? Detail { get; set; }
  public string JobName { get; set; }
  public List<JobInfoModelTriggers> Triggers { get; set; }
}

public class JobInfoModelTriggers {
  public string Name { get; set; }
  public string Group { get; set; }
  public string Type { get; set; }
  public TriggerState State { get; set; }
  public DateTimeOffset? NextFireTime { get; set; }
  public string? NextFireTimeHuman { get; set; }
  public DateTimeOffset? PreviousFireTime { get; set; }
  public string? PreviousFireTimeHuman { get; set; }
}