using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http;
using System.Net.Http;
using System.Web;
using System.Net;
using System.Security.Principal;
using Common;
using WebApi.Entity;
using WebApi.Plib;

namespace WebApi.web.Helper
{
    /// <summary>
    /// 执行人是否在该可执行动作允许的角色范围内
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AuthorizedRolesAttribute : ActionFilterAttribute
    {
        public Role Roles { get; set; }
        public override void OnActionExecuting(HttpActionContext actionContext)
        {

            if (HttpContext.Current.User != null)
            {
                UserIdentity user = HttpContext.Current.User.Identity as UserIdentity;
                if (user != null)
                {
                    if (user.MyRoles == Roles.ToString())
                    {
                        return;
                    }
                }
            }

            actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent(
                    JsonConvertEx.ObjectToJson(new
                {
                    Message = "请求未通过验证，当前用户的角色不允许执行才操作。"
                }),
                System.Text.Encoding.GetEncoding("UTF-8"), "application/json")

            };


        }




    }
}
