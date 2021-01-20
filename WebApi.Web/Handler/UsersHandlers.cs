using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using AutoMapper;
using Dapper;
using WebApi.web.Helper;
using WebApi.Entity;
using WebApi.Plib;
using WebApi.Utility;
using WebApi.Web.ViewModels;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections.Generic;
using CacheManager.Core;

namespace WebApi.Web.Handler
{
    public class UsersHandlers
    {

        ///检测用户IP是否合法
        public  bool CheckIP()
        {
            string curIP = GetWebUserIP();
            string[] arrryIP = ep.IPList.Split('|');
            if (arrryIP.Contains(curIP) || curIP=="127.0.0.1"|| curIP == "::1")
                return true;
            else return false;
        }
        #region 获取IP
        /// <summary>
        /// 获取用户IP
        /// </summary>
        /// <returns></returns>
        public string GetClientIP()
        {

            return GetWebUserIP();

            //string result = String.Empty;

            //try
            //{
            //    result = HttpContext.Current.Request.UserHostAddress;

            //    if (null == result || result == String.Empty)
            //    {
            //        result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            //    }
            //    if (null == result || result == String.Empty)
            //    {
            //        result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            //        if (result != null && result != String.Empty)
            //        {
            //            //可能有代理 
            //            if (result.IndexOf(".") == -1)    //没有“.”肯定是非IPv4格式 
            //                result = "nocatch";
            //            else
            //            {
            //                if (result.IndexOf(",") != -1)
            //                {
            //                    //有“,”，估计多个代理。取第一个不是内网的IP。 
            //                    result = result.Replace(" ", "").Replace("'", "");
            //                    string[] temparyip = result.Split(",;".ToCharArray());
            //                    for (int i = 0; i < temparyip.Length; i++)
            //                    {
            //                        if (IsIPAddress(temparyip[i])
            //                            && temparyip[i].Substring(0, 3) != "10."
            //                            && temparyip[i].Substring(0, 7) != "192.168"
            //                            && temparyip[i].Substring(0, 7) != "172.16.")
            //                        {
            //                            result = temparyip[i];    //找到不是内网的地址 
            //                        }
            //                    }
            //                }
            //                else if (IsIPAddress(result)) //代理即是IP格式 
            //                { 

            //                }
            //                else
            //                {
            //                    result = "nocatch";    //代理中的内容 非IP，取IP 
            //                }
            //            }

            //        }
            //    }
            //}
            //catch
            //{
            //    result = "nocatch";
            //}

            //return result;
        }

        /// <summary>
        /// 得到用户外网IP
        /// 默认取第一个IP
        /// </summary>
        /// <returns></returns>
        public string GetWebUserIP()
        {
            string user_IP = GetWebUserIPs();
            List<string> ipList = FilterLocalIP(user_IP);
            if (ipList != null && ipList.Count > 0)
            {
                user_IP = ipList[0];
            }
            return user_IP;
        }

        /// <summary>
        /// 过滤内网ip
        /// </summary>
        /// <param name="ips"></param>
        /// <returns></returns>
        public List<string> FilterLocalIP(string ips)
        {
            if (string.IsNullOrEmpty(ips))
            {
                return null;
            }
            string[] ipArray = ips.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> ipList = new List<string>(ipArray);
            foreach (string s in ipArray)
            {
                if (GetIPType(s) != 2)
                {
                    ipList.Remove(s);
                }
            }
            return ipList;
        }

        public string GetWebUserIPs()
        {
            string user_IP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] == null ? "" : HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            if (string.IsNullOrEmpty(user_IP))
            {
                user_IP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] == null ? "" : HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
                if (string.IsNullOrEmpty(user_IP))
                {
                    user_IP = HttpContext.Current.Request.UserHostAddress;
                }
            }
            return user_IP;
        }

        //common.GetIPType
        public int GetIPType(string ipAddress)
        {
            //ABC类外的IP地址为广域网IP
            //A类:10.0.0.0~10.255.255.255
            //B类:172.16.0.0~172.31.255.255
            //C类:192.168.0.0~192.168.255.255

            string[] ipAddressList = ipAddress.Split('.');
            int ipAddressTemp;

            //检查IP地址是否有效
            if (ipAddressList.Length != 4)
            {
                return 0;
            }

            if (!(int.TryParse(ipAddressList[0], out ipAddressTemp) && int.TryParse(ipAddressList[1], out ipAddressTemp)
                && int.TryParse(ipAddressList[2], out ipAddressTemp) && int.TryParse(ipAddressList[3], out ipAddressTemp)))
            {
                return 0;
            }

            if (!(int.Parse(ipAddressList[0]) >= 0 && int.Parse(ipAddressList[0]) <= 255
                    && int.Parse(ipAddressList[1]) >= 0 && int.Parse(ipAddressList[1]) <= 255
                    && int.Parse(ipAddressList[2]) >= 0 && int.Parse(ipAddressList[2]) <= 255
                    && int.Parse(ipAddressList[3]) >= 0 && int.Parse(ipAddressList[3]) <= 255))
            {
                return 0;
            }

            //局域网IP
            if (ipAddress == "127.0.0.1")
            {
                return 1;
            }

            if (int.Parse(ipAddressList[0]) == 10
                    || (int.Parse(ipAddressList[0]) == 172 && int.Parse(ipAddressList[1]) >= 16 && int.Parse(ipAddressList[1]) <= 31)
                    || (int.Parse(ipAddressList[0]) == 192 && int.Parse(ipAddressList[1]) == 168))
            {
                return 1;
            }


            return 2;
        }


        public string GetALLIP()
        {
            string result = String.Empty;

            try
            {
                string temp1 = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                string temp2 = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                string temp3 = HttpContext.Current.Request.UserHostAddress;

                string temp = "";

                if (!string.IsNullOrEmpty(temp1))
                {
                    temp = temp1;
                }
                if (!string.IsNullOrEmpty(temp2))
                {
                    temp += "," + temp2;
                }
                if (!string.IsNullOrEmpty(temp3))
                {
                    temp += "," + temp3;
                }

                result = temp.Trim(',');
            }
            catch
            {
                result = "nocatch";
            }

            return result;
        }

        public string GetServerIP()
        {
            try
            {
                System.Net.IPHostEntry ips = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());

                return FliterIP(ips.AddressList);
            }
            catch
            {
                return "";
            }
        }

        public string FliterIP(System.Net.IPAddress[] iplist)
        {
            foreach (System.Net.IPAddress temp in iplist)
            {
                string temp_ip = temp.ToString();

                if (GetIPType(temp_ip) == 1)
                {
                    return temp_ip;
                }
            }

            return "";
        }

        /// <summary>
        /// 判断是否是IP地址格式 0.0.0.0
        /// </summary>
        /// <param name="str1">待判断的IP地址</param>
        /// <returns>true or false</returns>
        public static bool IsIPAddress(string str1)
        {
            if (str1 == null || str1 == string.Empty || str1.Length < 7 || str1.Length > 15) return false;

            string regformat = @"^\d{1,3}[\.]\d{1,3}[\.]\d{1,3}[\.]\d{1,3}$";

            Regex regex = new Regex(regformat, RegexOptions.IgnoreCase);
            return regex.IsMatch(str1);
        }


        #endregion

        #region 缓存相关

        /// <summary>
        /// 通过pin获取user
        /// </summary>
        /// <param name="pin"></param>
        /// <returns></returns>
        public UserIdentity GetCache(string pin)
        {
            UserIdentity userinfo = null;

            var item = WebApiApplication.redisCache.GetCacheItem(pin);
            if (item != null)
            {
                userinfo = item.Value;
                //获取即刷新
                WebApiApplication.redisCache.Put(item.WithDefaultExpiration());
            }

            return userinfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userinfo"></param>
        /// <returns></returns>
        public string SetCache(UserIdentity userinfo)
        {

            string key = Guid.NewGuid().ToString("N");

            userinfo.CacheKey = key;
            var item = new CacheItem<UserIdentity>(
                key,
                userinfo,
                ExpirationMode.Sliding,
                TimeSpan.FromDays(2));


            if (WebApiApplication.redisCache.Add(item))
            {
                return key;
            }

            return "";
        }



        #endregion
    }
}