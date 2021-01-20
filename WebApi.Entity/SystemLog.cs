using System;
using Dapper.Contrib.Extensions;

namespace WebApi.Entity
{
    [Table("SystemLog")]
    public class SystemLog
    {
        [Key]
        public int ID { get; set; }
        public int RoleType { get; set; }
        public int UserID { get; set; }
        public DateTime BeginTime { get; set; }
        public Decimal TimeSpan { get; set; }
        public int TableKey { get; set; }
        public string IPAddress { get; set; }
        public int Type { get; set; }
        public string LogContent { get; set; }
        public string Comment { get; set; }

        public string UserName { get; set; }
    }
}
