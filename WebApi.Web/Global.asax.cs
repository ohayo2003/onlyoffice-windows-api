//using System;
//using System.Collections.Generic;
//using System.Linq;

//using System.Web.Http;
//using System.Web.Mvc;
//using System.Web.Routing;
//using System.Web.SessionState;
//using Newtonsoft.Json.Serialization;

using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using CacheManager.Core;
using Newtonsoft.Json.Serialization;
using WebApi.Web.Automapper;
using CacheManager.Redis;
using WebApi.Plib;
using System.Text.RegularExpressions;

namespace WebApi.Web
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801
    public class WebApiApplication : System.Web.HttpApplication
    {
        //public override void Init()
        //{
        //    //开启session
        //    this.PostAuthenticateRequest += (sender, e) => HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
        //    base.Init();
        //}


        public static ICacheManager<UserIdentity> redisCache = null;

        protected void Application_Start()
        {
            string con = ep.Cache_Redis_Configuration;
            string[] tmp = Regex.Split(con, ":");
            string ip = tmp[0];
            int port = Convert.ToInt32(tmp[1]);

            if (redisCache == null)
                redisCache = CacheFactory.Build<UserIdentity>("cache", settings =>
                {
                    settings
                        .WithMaxRetries(100)
                        .WithRetryTimeout(1000)
                        .WithRedisConfiguration("redisCache", config =>
                        {
                            config.WithAllowAdmin()
                                .WithDatabase(5)
                                .WithEndpoint("192.168.168.77", 6379);
                        })
                        .WithRedisBackplane("redisCache")
                        .WithRedisCacheHandle("redisCache", true)
                        .EnableStatistics();
                });
            else
            {
                KYCX.Logging.Logger.DefaultLogger.Error("redisCache failed");
            }


            //客户端无法发送put delete时，可以通过此在header增加X-HTTP-Method-Override，实现模拟
            GlobalConfiguration.Configuration.MessageHandlers.Add(new HttpMethodOverrideHandler());

            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            //WebApiConfig.Register(GlobalConfiguration.Configuration);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
                new DefaultContractResolver { IgnoreSerializableAttribute = true };

            Configuration.Configure();
        }

        public override void Init()
        {
            //注册事件
            //this.AuthenticateRequest += WebApiApplication_AuthenticateRequest;
            base.Init();
        }

        //开启session支持
        //void WebApiApplication_AuthenticateRequest(object sender, EventArgs e)
        //{
        //    //启用 webapi 支持session 会话
        //    HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
        //}
    }
}