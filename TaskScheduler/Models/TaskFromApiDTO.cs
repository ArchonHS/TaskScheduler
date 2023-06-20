using Dapper;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using TaskScheduler.Data.Interfaces;

namespace TaskScheduler.Models
{
    [Table("tasks")]
    public class TaskFromApiDTO
    {
        [IgnoreInsert]
        [IgnoreUpdate]
        [Column("id_task")]
        public int IdTask { get; set; }
        [Column("uid_task")]
        [Dapper.Key]
        public Guid UidTask { get; set; }
        [IgnoreUpdate]
        [Column("id_project")]
        public long  IdProject { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("descr")]
        public string Descr { get; set; }
        [Column("link")]
        public string Link { get; set; }
        [Column("cronInterval")]
        public string CronInterval { get; set; }
        [Column("is_active")]
        public bool IsActive { get; set; } = true;
        [Column("is_delete")]
        public bool Deleted { get; set; } 
        public async Task ExecuteTask()
        {            
            HttpClient client = new HttpClient();
            await client.GetAsync(Link);
            Console.WriteLine($"{UidTask}: task executed");
        }
    }
}
