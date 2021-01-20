using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Security.Principal;
using WebApi.Web;
using WebApi.Web.Handler;
using WebApi.Plib;

namespace WebApi.web.Helper
{
    public class RequestAuthorizeAttribute : AuthorizeAttribute
    {
        //重写基类的验证方式，加入自定义的Token验证
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {

            #region v2
            var attributes = actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().OfType<AllowAnonymousAttribute>();
            bool isAnonymous = attributes.Any(a => a is AllowAnonymousAttribute);
            if (isAnonymous)
            {
                base.OnAuthorization(actionContext);
            }
            else
            {
                var authorization = actionContext.Request.Headers.Authorization;
                if ((authorization != null) && (authorization.Parameter != null))
                {
                    //解密用户Token,并校验用户名密码是否匹配
                    var encryptTicket = authorization.Parameter;

                    if (ValidateTicket(encryptTicket))
                    {
                        //new GenericPrincipal()
                        //HttpContext.Current.User=
                        base.IsAuthorized(actionContext);
                    }
                    else
                    {
                        HandleUnauthorizedRequest(actionContext);
                    }
                }
                else
                {
                    HandleUnauthorizedRequest(actionContext);
                }

            }



            #endregion
        }

        private bool ValidateTicket(string encryptTicket)
        {

            try
            {
                UsersHandlers handler = new UsersHandlers();
                UserIdentity userinfo = handler.GetCache(encryptTicket);

                //存在则读取到对应的userinfo;账号过期则为null，相当于此时是重新登录
                if (userinfo != null)
                {
                    HttpContext.Current.User = new GenericPrincipal(userinfo, null);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }


        }



    }
}
