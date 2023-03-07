using Microsoft.AspNetCore.Mvc;
using OtherWay.Radio.Scheduler.Models;
using OtherWay.Radio.Scheduler.Services;
using OtherWay.Radio.Scheduler.Services.Extensions;
using Quartz;

namespace OtherWay.Radio.Scheduler.Controllers;

[ApiController]
[Route("[controller]")]
public class JobController : ControllerBase {
  private readonly ISchedulerFactory _schedulerFactory;
  private readonly ScheduleLoader _scheduleLoader;

  public JobController(ISchedulerFactory schedulerFactory, ScheduleLoader scheduleLoader) {
    _schedulerFactory = schedulerFactory;
    _scheduleLoader = scheduleLoader;
  }

  [HttpGet]
  public async Task<ActionResult<List<JobInfoModel>>> GetAllSchedules() {
    var scheduler = await _schedulerFactory.GetScheduler();
    var executingJobs = await scheduler.GetAllJobs();
    return Ok(executingJobs);
  }

  [HttpPost("schedule")]
  public async Task<IActionResult> ScheduleJobs(List<NotificationSchedule> schedules) {
    await _scheduleLoader.CreateJobsForSchedules(schedules);
    return Ok();
  }

  [HttpPost("reload")]
  public async Task<IActionResult> ReloadSchedules() {
    await _scheduleLoader.LoadSchedules();
    return Ok();
  }

  [HttpPost("trigger")]
  public async Task<OkObjectResult> Run(string jobName) {
    var scheduler = await _schedulerFactory.GetScheduler();
    await scheduler.TriggerJob(new JobKey(jobName));
    return Ok("OK");
  }
}