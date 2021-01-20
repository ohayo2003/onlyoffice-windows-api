using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using WebApi.Entity;

namespace WebApi.Plib
{
    public class MsgStruct
    {
        /// <summary>
        /// 
        /// </summary>
        public string file_id { get; set; }
        /// <summary>
        /// 
        /// </summary>ss
        public string check_code { get; set; }

    }

    [Serializable]
    public class UserIdentity : IIdentity
    {
        /// <summary>
        /// 使用批注的业务系统类型
        /// </summary>
        public string SysType { get; set; }

        /// <summary>
        /// 可以是编号或者其他的唯一标识
        /// </summary>
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }


        public string UserMode { get; set; }
        public string EditPrivilege { get; set; }
        public string CommentPrivilege { get; set; }

        public string MyRoles { get; set; }
        /// <summary>
        /// 直接调用返回文件流，省去读取后台webapi的一步
        /// </summary>
        public string BaseCheckInfo { get; set; }
        /// <summary>
        /// 可展示的文件名单
        /// </summary>
        public string BusinessId { get; set; }

        public DateTime CreateTime { get; set; }

        public UserIdentity()
        {

        }


        public string CacheKey { get; set; }




        #region 基本属性



        public string AuthenticationType
        {

            get { return "Form"; }

        }

        public bool IsAuthenticated
        {

            get { return true; }

        }

        public string Name
        {

            get { return UserName; }

        }

        #endregion

    }
}
