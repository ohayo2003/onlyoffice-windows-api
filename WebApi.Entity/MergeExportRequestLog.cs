using System;
using Dapper.Contrib.Extensions;

namespace WebApi.Entity
{
    [Table("MergeExportRequestLog")]
    public class MergeExportRequestLog
    {
        [Key]
        public int ID { get; set; }
        public DateTime CreateTime { get; set; }
        public int Status { get; set; }
        public string FileID { get; set; }
        
        public string HandleResult { get; set; }
        public DateTime UpdateTime { get; set; }
        
        public string ErrorMessage { get; set; }

    }
}
