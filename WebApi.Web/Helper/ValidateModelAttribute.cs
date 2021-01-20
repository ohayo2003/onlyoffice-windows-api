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
using Common;

namespace WebApi.web.Helper
{
    /// <summary>
    /// 创建时才需要验证，
    /// search则不需要增加此特性
    /// </summary>
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                //返回第一个异常的错误消息

                actionContext.Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(JsonConvertEx.ObjectToJson(new  { Message = string.Format("请求{0}数据未通过验证，原因：{1}",
                        actionContext.ActionDescriptor.ActionName, actionContext.ModelState.Values.First().Errors.First().ErrorMessage) }),
                        System.Text.Encoding.GetEncoding("UTF-8"), "application/json")//new StringContent(string.Format("请求{0}过于频繁", sActionName))

                };

            }
        }




    }
}
