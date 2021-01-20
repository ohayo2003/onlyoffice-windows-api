using System;
using Dapper.Contrib.Extensions;

namespace WebApi.Entity
{
    [Table("Annotation")]
    public class Annotation
    {
        [Key]
        public int ID { get; set; }
        public DateTime CreateTime { get; set; }
        public int Status { get; set; }
        public string FileID { get; set; }
        public string Content { get; set; }
        public string SourcesID { get; set; }
        public string PageNumber { get; set; }
        public string UserInfoStr { get; set; }
        public string Comment { get; set; }
        public string OriginalText { get; set; }
    }

    /// <summary>
    /// 中心保存
    /// </summary>
    [Serializable]
    public class AccountInfo
    {
        public string user_key { get; set; }
        public string user_name { get; set; }
        public string user_rolename { get; set; }
        public int user_read_privilege { get; set; }
        public int user_write_privilege { get; set; }
        public string basecheckinfo { get; set; }
    }

    public enum PrivilegeType
    {
        /// <summary>
        /// 禁止读or写
        /// </summary>
        forbid = 0,
        /// <summary>
        /// 只能读写本人的批注
        /// </summary>
        myself = 1,

        /// <summary>
        /// 所有人的都可读写
        /// </summary>
        all = 2
    }
}
