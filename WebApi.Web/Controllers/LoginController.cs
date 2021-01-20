using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Http;
using Dapper;
using WebApi.Entity;
using WebApi.Plib;
using WebApi.Web.Handler;
using WebApi.Web.ViewModels;
using Dapper.Contrib.Extensions;
using WebApi.Utility;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading;
using System.Web;
using Common;
using Common.Logging;
using Newtonsoft.Json.Linq;
using CacheManager.Core;
using WebApi.web.Helper;

/**
 * 登录操作返回值举例
 * 成功 返回fileid和对应的pin
 * 失败 返回errorMsg
 * process 则不返回信息，需要用户重新执行登录？
 * 
 */
namespace WebApi.Web.Controllers
{
    //[RoutePrefix("api/Accounts")]
    /// <summary>
    /// 登录相关接口
    /// </summary>
    public class LoginController : ApiController
    {


        /// <summary>
        /// 先查看dto内token是否有值
        /// 有则验证成功后追加到userinfo中，返回token和要打开的fileid
        /// 无则验证成功后，返回新的token和要打开的fileid
        /// 失败则返回错误信息
        /// 等待中呢？
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [ValidateModel]
        public PageResult Post([FromBody]UserLoginDto dto)
        {
            UsersHandlers handler = new UsersHandlers();
            UserIdentity userinfo = null;

            try
            {

                if (!string.IsNullOrWhiteSpace(dto.pin) && dto.pin.ToLower() != "null")
                {
                    userinfo = handler.GetCache(dto.pin);

                    //存在则读取到对应的userinfo;账号过期则为null，相当于此时是重新登录
                    if (userinfo == null)
                    {
                        return new PageResult(this.Request, (int)ReturnStatus.failed, "用户已过期，请重新登录");
                    }

                    //用户切换时要空置
                    if (userinfo != null &&
                        (dto.user_key != userinfo.UserID || dto.client_id != userinfo.SysType || dto.business_id != userinfo.BusinessId
                        || dto.user_mode != userinfo.UserMode
                        || dto.user_edit_privilege != userinfo.EditPrivilege
                        || dto.user_comment_privilege != userinfo.CommentPrivilege))
                        userinfo = null;
                    else
                    {
                        return new PageResult(this.Request, (int)ReturnStatus.success, dto.pin);
                    }

                }

                List<ClientInfo> list = new List<ClientInfo>();

                using (SqlConnection conn = CommonDapper.GetOpenConnection(DataConnectType.ServerDBDataService))
                {
                    list.AddRange(conn.Query<ClientInfo>("select top 1 * from ClientInfo where ClientId=@ClientId and Status=@Status  ",
                        new { ClientId = dto.client_id, Status = 1 }));
                }

                if (list.Count <= 0)
                    return new PageResult(this.Request, (int)ReturnStatus.failed, "clientid参数无效");

                //验证签名
                #region 生成文件key和加密值的集合


                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("client_id", dto.client_id);
                dic.Add("user_key", dto.user_key);
                dic.Add("business_id", dto.business_id);

                string check_code = CrypHash.GetCheckCode(dic, list[0].ClientKey);

                if (!string.Equals(check_code, dto.check_code, StringComparison.OrdinalIgnoreCase))
                    return new PageResult(this.Request, (int)ReturnStatus.failed, "签名无效");

                #endregion
                //建立新的session 并返回pin 否则返回失败


                //判断dto的值合理性



                //成功则生成新的token
                userinfo = new UserIdentity()
                {
                    SysType = dto.client_id,
                    UserID = dto.user_key,
                    UserName = dto.user_name,
                    RoleName = dto.user_rolename,
                    UserMode = dto.user_mode,
                    EditPrivilege = dto.user_edit_privilege,
                    CommentPrivilege = dto.user_comment_privilege,
                    BusinessId = dto.business_id

                };

                string key = handler.SetCache(userinfo);
                if (string.IsNullOrEmpty(key))
                {
                    return new PageResult(this.Request, (int)ReturnStatus.success, "缓存处理异常");
                }
                else
                {
                    return new PageResult(this.Request, (int)ReturnStatus.success, key);
                }


            }

            catch (Exception ex)
            {
                //异常日志
                KYCX.Logging.Logger.DefaultLogger.Error(ex.Message, ex);
                return new PageResult(this.Request, (int)ReturnStatus.failed, "数据处理异常");
            }


            //return new PageResult(this.Request, (int)ReturnStatus.failed, "数据处理异常");
        }

    }
}
