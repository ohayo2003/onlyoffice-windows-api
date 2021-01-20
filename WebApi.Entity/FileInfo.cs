using System;
using Dapper.Contrib.Extensions;

namespace WebApi.Entity
{
    [Table("FileInfo")]
    public class FileInfo
    {
        [Key]
        public int ID { get; set; }
        public DateTime CreateTime { get; set; }
        public int Status { get; set; }
        public int UploadType { get; set; }

        public string Title { get; set; }

        public string BusinessId { get; set; }

        public string ClientId { get; set; }
        public string Uid { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }

        public string OtherMsg { get; set; }
    }


    [Table("VersionInfo")]
    public class VersionInfo
    {
        [Key]
        public int ID { get; set; }
        public DateTime CreateTime { get; set; }
        public int Status { get; set; }
        public int FileID { get; set; }


        public string BusinessId { get; set; }
        /// <summary>
        /// 当前版本对应的key
        /// </summary>
        public long VersionKey { get; set; }

        /// <summary>
        /// 正文的ftp配置
        /// </summary>
        public string FtpConn { get; set; }
        /// <summary>
        /// 正文的path
        /// </summary>
        public string FtpFilePath { get; set; }


        public string ClientId { get; set; }
        public string Uid { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }


        /// <summary>
        /// 记录上一个versionkey
        /// </summary>
        public long PriorVersionKey { get; set; }


        /// <summary>
        /// zip文件的ftp配置
        /// </summary>
        public string FtpConnForChanges { get; set; } //= "";
        /// <summary>
        /// 在线编辑内 版本差异展示需要
        /// </summary>
        public string ChangesUrl { get; set; }
        public string History { get; set; }


        #region 辅助用


        [Computed]
        public int num { get; set; } //= -1;
        #endregion

        public VersionInfo()
        {
            FtpConnForChanges = "";
            num = -1;
        }
    }




    /// <summary>
    /// 保存每个业务的签名密钥
    /// </summary>
    [Table("ClientInfo")]
    public class ClientInfo
    {
        [Key]
        public int ID { get; set; }
        public DateTime CreateTime { get; set; }
        public int Status { get; set; }


        public string ClientId { get; set; }
        /// <summary>
        /// 签名的密钥
        /// </summary>
        public string ClientKey { get; set; }

    }



}
