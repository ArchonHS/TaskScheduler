
using Dapper;
using System.ComponentModel.DataAnnotations;

namespace TaskScheduler.Models
{
    [Table("tasks_log")]
    public class LogScheme
    {
        [System.ComponentModel.DataAnnotations.Key]
        [Column("id_log")]
        public int Id { get; set; }
        [Column("id_task")]
        public int IdTask { get; set; }
        [Column("date_start")]
        public DateTime DateStart { get; set; }
        [Column("date_end")]
        public DateTime DateEnd { get; set; }
        [Column("is_ok")]
        public bool IsOk { get; set; }
        [Column("message")]
        public string? Message { get; set; }
    }
}
