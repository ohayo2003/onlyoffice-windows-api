using System;
using Dapper.Contrib.Extensions;

namespace WebApi.Entity
{
    [Table("ExtractResultList")]
    public class ExtractResultList
    {
        [Key]
        public int ID { get; set; }
        public DateTime CreateTime { get; set; }
        public int Status { get; set; }

        public int ExtractType { get; set; }
        public string RequestJson { get; set; }
        public int UserInfoID { get; set; }

        public int ExtractStatus { get; set; }
        public DateTime FinishedTime { get; set; }
        public string ResponseJson { get; set; }
    }
}
