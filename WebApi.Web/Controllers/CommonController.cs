using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using BaseCheckLib.Model;
using BaseCheckLib.Parameter;
using BaseCheckUtility;
using Common;
using Dapper;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using WebApi.Plib;
using WebApi.web.Helper;

//using WebApi.Web.Handler;

namespace WebApi.Web.Controllers
{
    /// <summary>
    /// 一些公共的接口
    /// </summary>
    public class CommonController : BaseApiController
    {
               
        [AllowAnonymous]
        [HttpGet]
        public IHttpActionResult GetFileStreamTest2()
        {
            string errorMsg = "出现问题";


            try
            {

                return new FileResult(HostingEnvironment.MapPath("~/bin/20.docx"), "20.docx", this.Request);

            }
            catch (Exception ex)
            {
                KYCX.Logging.Logger.DefaultLogger.Error(ex.Message, ex);
                return new PageResult(Request, false, "数据处理错误");
            }

        }


        /// <summary>
        /// 用户登出
        /// </summary>
        public PageResult Logout()
        {
            string userName = CurrentUserInfo.UserName;
            using (SqlConnection conn = CommonDapper.GetOpenConnection(DataConnectType.ServerDBDataService))
            {

                int result = conn.Execute("delete Session_Current_Details  where UserName=@UserName ", new { UserName = userName });

            }
            return new PageResult(this.Request, true, "");
        }



    }
}
