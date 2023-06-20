namespace TaskScheduler.Data
{
    public class TaskFromApiUpdateDTO
    {
        public string Name { get; set; }
        public string Descr { get; set; }
        public string Link { get; set; }
        public string CronInterval { get; set; }
        public bool IsActive { get; set; }
        public bool Deleted { get; set; }
    }
}
