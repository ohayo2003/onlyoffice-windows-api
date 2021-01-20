using System.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;

//using WebApi.Component.Filters;

namespace WebApi.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //开启自定义路由启用[routeprefix route]
            config.MapHttpAttributeRoutes();

            //跨域配置可以从webconfig里配置读取初始化
            //config.EnableCors(new EnableCorsAttribute("*", "*", "*"));

            var allowOrigins = ConfigurationManager.AppSettings["cors_allowOrigins"];
            var allowHeaders = ConfigurationManager.AppSettings["cors_allowHeaders"];
            var allowMethods = ConfigurationManager.AppSettings["cors_allowMethods"];
            var geduCors = new EnableCorsAttribute(allowOrigins, allowHeaders, allowMethods)
            {
                SupportsCredentials = true

            };
            config.EnableCors(geduCors);


            config.Routes.MapHttpRoute(
                  name: "DefaultApi",
                  //routeTemplate: "api/{controller}/{action}/{id}",
                  routeTemplate: "api/{controller}/{action}/{id}",
                  defaults: new { id = RouteParameter.Optional }
            );

            //config.Filters.Add(new ValidateModelAttribute());
        }
    }
}
