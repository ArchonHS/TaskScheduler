using Dapper;
using System.Numerics;

namespace TaskScheduler.Models
{
    public class TaskFromApiCreateDTO
    {
        [Column("id_project")]
        public long IdProject { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("descr")]
        public string Descr { get; set; }
        [Column("link")]
        public string Link { get; set; }
        [Column("cronInterval")]
        public string CronInterval { get; set; }
    }
}
