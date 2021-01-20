using System;
using Dapper.Contrib.Extensions;

namespace WebApi.Entity
{
    [Table("RequestObject")]
    public class RequestObject
    {
        [Key]
        public int ID { get; set; }
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 有效还是无效
        /// </summary>
        public int Status { get; set; }
        public string RequesterName { get; set; }
        public string RequesterPhoneNum { get; set; }
        public string RequesterOrganisation { get; set; }
        public string ArticleName { get; set; }
        public string Author { get; set; }
        public string Publication { get; set; }
        public DateTime? PublishTime { get; set; }
        /// <summary>
        /// 当前申请的进度状态
        /// </summary>
        public int CheckStatus { get; set; }
        public string Comment { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
