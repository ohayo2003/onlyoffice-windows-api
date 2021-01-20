using System;
using Dapper.Contrib.Extensions;

namespace WebApi.Entity
{
    [Table("Session_Current_Details")]
    public class Session_Current_Details
    {
        [Key]
        public int ID { get; set; }
        public int Status { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime ExpireTime { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public int TimeOut { get; set; }


        public string SysType { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
        public string UserInfomations { get; set; }

    }
}
