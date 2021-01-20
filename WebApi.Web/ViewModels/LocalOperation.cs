using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Web.ViewModels
{

    public class InitDocDto
    {
        /// <summary>
        /// 访问editor的业务系统标志
        /// </summary>
        [Required]
        public string client_id { get; set; }
        /// <summary>
        /// 访问editor的业务系统中用户的唯一标识
        /// </summary>
        [Required]
        public string uid { get; set; }
        /// <summary>
        /// 师生name
        /// </summary>
        [Required]
        public string user_name { get; set; }
        /// <summary>
        ///访问者角色名
        /// </summary>
        [Required]
        public string user_rolename { get; set; }

        /// <summary>
        /// 文章标题
        /// </summary>
        [Required]
        public string title { get; set; }

        /// <summary>
        /// 业务唯一id swid
        /// </summary>
        [Required]
        public string businessId { get; set; }

        /// <summary>
        /// UploadType，1在线创建，2本地上传
        /// </summary>
        [Required]
        public string UploadType { get; set; }
        /// <summary>
        /// 本地上的FtpPath路径
        /// </summary>
        [Required]
        public string FtpPath { get; set; }

        /// <summary>
        /// md5签名
        /// </summary>
        [Required]
        public string signature { get; set; }


    }
}