namespace OtherWay.Radio.Scheduler.Models;

public class Schedule {
  public string ScheduleName { get; set; }
  public DateTime ScheduleTime { get; set; }
}

public class NotificationSchedule {
  public string ShowId { get; set; }
  public List<Schedule> ScheduleTimes { get; set; }
}