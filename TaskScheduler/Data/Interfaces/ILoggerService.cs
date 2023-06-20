using TaskScheduler.Models;

namespace TaskScheduler.Data.Interfaces
{
    public interface ILoggerService
    {
        public Task<int?> Add(LogScheme item);
        public Task<bool> Failed100Times(int id);
    }
}
