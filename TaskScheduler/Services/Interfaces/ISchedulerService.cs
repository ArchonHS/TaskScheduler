using TaskScheduler.Data;
using TaskScheduler.Models;

namespace TaskScheduler.Services.Interfaces
{
    public interface ISchedulerService
    {
        public Task InitializeTasks();
        public Task AddTask(TaskFromApiDTO task);
        public Task RemoveTask(string id);
        public Task ReloadTasks();
        public Task ReloadTask(string id);
    }
}
