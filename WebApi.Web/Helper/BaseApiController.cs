using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Security.Principal;
using WebApi.Plib;

namespace WebApi.web.Helper
{


    [RequestAuthorize]
    //[ValidateModel]
    public class BaseApiController : ApiController
    {

        [AllowAnonymous]
        public string GetApiUrl()
        {
            return this.Request.RequestUri.Scheme + "://" + this.Request.RequestUri.Authority + "/" + this.Request.RequestUri.Segments[1];
        }

        protected UserIdentity CurrentUserInfo
        {
            get
            {
                try
                {
                    var princ = (GenericPrincipal)System.Web.HttpContext.Current.User;
                    if (princ != null)
                    {
                        return (UserIdentity)princ.Identity;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
