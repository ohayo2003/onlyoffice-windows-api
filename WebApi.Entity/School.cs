using System;
using Dapper.Contrib.Extensions;

namespace WebApi.Entity
{
    [Table("School")]
    public class School
    {
        [Key]
        public int ID { get; set; }
        public DateTime CreateTime { get; set; }
        public int Status { get; set; }
        public string SchoolName { get; set; }
    }
}
