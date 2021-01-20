using System.ComponentModel.DataAnnotations;

namespace WebApi.Web.ViewModels
{
    public class UserLoginResponseInfo
    {
        public string Token { set; get; }
        public string FileID { set; get; }
    }
    /// <summary>
    /// <remarks>
    /// 登录的参数
    /// </remarks>
    /// </summary>
    public class UserLoginDto
    {
        /// <summary>
        /// 访问reader的业务系统标志
        /// </summary>
        [Required]
        public string client_id { get; set; }
        /// <summary>
        /// 访问reader的业务系统中用户的唯一标识
        /// </summary>
        [Required]
        public string user_key { get; set; }
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
        /// editor模式：
        /// 0为view
        /// 1位edit
        /// 
        /// 组员：user_mode=0 ，其他随意
        /// 
        /// 组长：user_mode=1 user_edit_privilege=1  user_comment_privilege=0      
        /// 
        /// 教师：user_mode=1 user_edit_privilege=0   user_comment_privilege=1
        /// 
        /// </summary>
        [Required]
        public string user_mode { get; set; }

        /// <summary>
        /// 编辑权限
        /// 0为不可读
        /// </summary>
        [Required]
        public string user_edit_privilege { get; set; }

        /// <summary>
        /// 评论权限
        /// 0为不可写
        /// </summary>
        [Required]
        public string user_comment_privilege { get; set; }

        /// <summary>
        /// 业务唯一id  swid
        /// </summary>
        [Required]
        public string business_id { get; set; }

        /// <summary>
        /// 签名用
        /// </summary>
        [Required]
        public string check_code { get; set; }

        /// <summary>
        /// 保持token值;未登录则置为""
        /// </summary>
        public string pin { get; set; }

    }


}