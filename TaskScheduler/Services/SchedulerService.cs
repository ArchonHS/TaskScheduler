using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Event;
using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using TaskScheduler.Data;
using TaskScheduler.Data.Interfaces;
using TaskScheduler.Models;
using TaskScheduler.Services.Interfaces;

namespace TaskScheduler.Services
{
    public class SchedulerService : ISchedulerService
    {
        private readonly IScheduler _scheduler;
        private readonly IServiceScopeFactory _factory;
        public SchedulerService(IScheduler scheduler, IServiceScopeFactory factory)
        {
            _scheduler = scheduler;
            _factory = factory;
        }
        public async Task AddTask(TaskFromApiDTO task)
        {
            using (var scope = _factory.CreateAsyncScope())
            {
                var logs = scope.ServiceProvider.GetRequiredService<ILoggerService>();
                var tasks = scope.ServiceProvider.GetRequiredService<IDapperService<TaskFromApiDTO>>();
                var scheduledEvent = _scheduler.ScheduleAsync(
                        async () =>
                        {
                            var dateStart = DateTime.Now;
                            Stopwatch stopwatch = Stopwatch.StartNew();
                            try
                            {
                                await task.ExecuteTask();
                                stopwatch.Stop();
                                await logs.Add(new LogScheme
                                {
                                    IdTask = task.IdTask,
                                    DateStart = dateStart,
                                    DateEnd = dateStart + stopwatch.Elapsed,
                                    IsOk = true
                                });
                            }
                            catch (Exception ex)
                            {
                                stopwatch.Stop();
                                var failed100Times = await logs.Failed100Times(task.IdTask);
                                if (failed100Times)
                                {
                                    Console.WriteLine("Task failed too many times!");
                                    await RemoveTask(task.UidTask.ToString());
                                    task.IsActive = false;
                                    await tasks.Update(task);
                                }
                                else
                                {
                                    await logs.Add(new LogScheme
                                    {
                                        IdTask = task.IdTask,
                                        DateStart = dateStart,
                                        DateEnd = dateStart + stopwatch.Elapsed,
                                        IsOk = false,
                                        Message = (ex.Message.Length >= 1023) ? ex.Message.Substring(0, 1023) : ex.Message
                                    });
                                }
                            }
                        }
                        )
                .Cron(task.CronInterval);

                if (scheduledEvent != null) (scheduledEvent as ScheduledEvent)
                        .AssignUniqueIndentifier(task.UidTask.ToString());
            }
            Console.WriteLine($"{task.UidTask}: scheduled for recurring execution");
        }

        public async Task InitializeTasks()
        {
            using (var scope = _factory.CreateAsyncScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<IDapperService<TaskFromApiDTO>>();
                foreach (var task in await db.GetAll())
                {
                    if (task.IsActive && !task.Deleted) await AddTask(task);
                }
            }
        }

        public async Task RemoveTask(string id)
        {
            if (_scheduler is not null) (_scheduler as Scheduler).TryUnschedule(id);
        }

        public async Task ReloadTasks()
        {
            using (var scope = _factory.CreateAsyncScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<IDapperService<TaskFromApiDTO>>();
                foreach (var task in await db.GetAll())
                {
                    await RemoveTask(task.UidTask.ToString());
                    Console.WriteLine($"{task.UidTask}: task unscheduled");
                }
            }
            await InitializeTasks();
        }

        public async Task ReloadTask(string id)
        {
            using (var scope = _factory.CreateAsyncScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<IDapperService<TaskFromApiDTO>>();
                await RemoveTask(id);
                var taskForReloading = await db.Get(Guid.Parse(id));
                if (!taskForReloading.Deleted && taskForReloading.IsActive) await AddTask(taskForReloading);
                Console.WriteLine($"{id}: task reloaded");
            }
        }
    }
}
