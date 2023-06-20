using Dapper;
using Microsoft.Data.SqlClient;
using TaskScheduler.Data.Interfaces;

namespace TaskScheduler.Data
{
    public class DapperService<T> : IDapperService<T> where T : class
    {
        private readonly string? _connectionString;
        public DapperService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<Guid> Add(T item)
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                var id = await cn.InsertAsync<Guid, T>(item);
                cn.Close();
                return id;
            }
        }

        public async Task<T> Get(Guid id)
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                var task = await cn.GetAsync<T>(id);
                cn.Close();
                return task;
            }
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                var tasks = await cn.GetListAsync<T>();
                cn.Close();
                return tasks;
            }
        }

        public async Task Update(T item)
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                await cn.UpdateAsync(item);
                cn.Close();
            }
        }
        public async Task Delete(Guid id)
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                await cn.DeleteAsync<T>(id);
                cn.Close();
            }
        }
    }
}
