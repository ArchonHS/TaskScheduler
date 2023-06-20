namespace TaskScheduler.Data.Interfaces
{
    public interface IDapperService<T> where T : class
    {
        public Task<IEnumerable<T>> GetAll();
        public Task<T> Get(Guid id);
        public Task<Guid> Add(T item);
        public Task Update(T item);
        public Task Delete(Guid id);
    }
}
