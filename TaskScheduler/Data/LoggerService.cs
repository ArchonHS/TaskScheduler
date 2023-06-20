using Dapper;
using Microsoft.Data.SqlClient;
using TaskScheduler.Data.Interfaces;
using TaskScheduler.Models;

namespace TaskScheduler.Data
{
    public class LoggerService : ILoggerService
    {
        private readonly string? _connectionString;
        public LoggerService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<int?> Add(LogScheme item)
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                int? id = await cn.InsertAsync(item);
                cn.Close();
                return id;
            }
        }

        public async Task<bool> Failed100Times(int id)
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                var records = await cn.GetListAsync<LogScheme>(new { IdTask = id });
                cn.Close();
                var taskTries = records.TakeLast(100);
                if (taskTries is null || taskTries.Count() < 100) return false;
                return !taskTries.Any(record => record.IsOk);
            }
        }
    }
}