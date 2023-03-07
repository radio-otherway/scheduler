using Quartz;

namespace OtherWay.Radio.Scheduler.Services.Jobs;

public static class JobStartup {
  public static IServiceCollection AddJobScheduler(this IServiceCollection services) {
    services.AddSingleton<OtherwayAPIProxy>();
    services.AddSingleton<ScheduleLoader>();
    services.AddQuartz(q => {
      q.SchedulerName = "OtherWay Job Scheduler";
      q.UseMicrosoftDependencyInjectionJobFactory();
      q.UseSimpleTypeLoader();
      q.UseInMemoryStore();
      q.UseDefaultThreadPool(tp => { tp.MaxConcurrency = 10; });

      _scheduleJobs(q);
    });
    services.AddQuartzServer(options => { options.WaitForJobsToComplete = true; });

    services.BuildServiceProvider()
      .GetService<ScheduleLoader>()?
      .LoadSchedules();

    return services;
  }

  private static void _scheduleJobs(IServiceCollectionQuartzConfigurator quartz) {
    var jobKey = new JobKey("CacheCalendarJob");
    quartz.AddJob<CacheCalendarJob>(opts => opts.WithIdentity(jobKey));

    quartz.AddTrigger(opts => opts
      .ForJob(jobKey)
      .WithIdentity("CacheCalendarJob-trigger", "cron-group")
      .StartNow()
      .WithSimpleSchedule(x => x
        .WithIntervalInMinutes(10)
        .RepeatForever())
    );
  }
}