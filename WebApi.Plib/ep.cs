using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using Common;

namespace WebApi.Plib
{
    /// <summary>
    /// 系统结构
    /// </summary>
    public enum ArchitectureMode
    {
        BS,
        CS
    }

    public class ep
    {
        public static ArchitectureMode ArchMode = ArchitectureMode.BS;//系统结构

        #region 登录相关

        public static Customerlink GetSQLLink(DataConnectType dct)
        {
            Customerlink cl = null;

            try
            {
                if (dct == DataConnectType.UserDBDataService)
                {
                    cl = new Customerlink(UserServerIP, UserDBName, UserServerName, UserServerPassword);

                }
                else if (dct == DataConnectType.ServerDBDataService)
                {
                    cl = new Customerlink(ServerIP, DBName, ServerSQLName, ServerSQLPassword);
                }
                else
                {

                }


            }
            catch (Exception ex)
            {
                cl = null;
            }
            return cl;
        }

        public static string UserServerIP
        {
            get
            {
                if (ArchMode == ArchitectureMode.BS)
                {
                    return WebConfigurationManager.AppSettings["UserServerInfo"].ToString().Split('|')[0].Trim();
                    //return System.Configuration.ConfigurationManager.AppSettings["UserServerIP"];
                }
                else
                {
                    return "";
                }

            }
        }
        /// <summary>
        /// 用户信息数据库名称
        /// </summary>
        public static string UserDBName
        {
            get
            {
                if (ArchMode == ArchitectureMode.BS)
                {

                    return WebConfigurationManager.AppSettings["UserServerInfo"].ToString().Split('|')[1].Trim();

                }
                else
                {
                    return "";
                }
            }
        }


        public static string UserServerName
        {
            get
            {
                CommonServerID cuid = new CommonServerID();

                return cuid.id_main;
                //if (id_main == null)
                //{
                //    SetInfo_Main();
                //}

                //return id_main;
            }
        }
        /// <summary>
        /// 用户信息服务器的密码
        /// </summary>
        public static string UserServerPassword
        {
            get
            {
                CommonServerID cuid = new CommonServerID();
                return cuid.pw_main;
                //if (pw_main == null)
                //{
                //    SetInfo_Main();
                //}

                //return pw_main;
            }
        }

        public static string ServerIP
        {
            get
            {
                if (ArchMode == ArchitectureMode.BS)
                {
                    return WebConfigurationManager.AppSettings["ServerDBInfo"].ToString().Split('|')[0].Trim();
                }
                else
                {

                    return "";

                }
            }
        }

        public static string DBName
        {
            get
            {
                if (ArchMode == ArchitectureMode.BS)
                {

                    return WebConfigurationManager.AppSettings["ServerDBInfo"].ToString().Split('|')[1].Trim();

                }
                else
                {

                    return "";

                }
            }
        }

        
        public static string ServerSQLName
        {
            get
            {
                CommonServerID cuid = new CommonServerID();
                return cuid.id;
                //if (id == null)
                //{
                //    SetInfo();
                //}

                //return id;
            }
        }
        /// <summary>
        /// 分配给用户使用的SQL数据库密码
        /// </summary>
        public static string ServerSQLPassword
        {
            get
            {
                CommonServerID cuid = new CommonServerID();
                return cuid.pw;
                //if (pw == null)
                //{
                //    SetInfo();
                //}

                //return pw;
            }
        }


        #endregion

        public static string LocalDiskDir
        {
            get
            {
                if (ArchMode == ArchitectureMode.BS)
                {

                    return WebConfigurationManager.AppSettings["LocalDiskDir"].ToString().Trim();

                }
                else
                {
                    return "";
                }
            }
        }

        public static string Cache_Redis_Configuration
        {
            get
            {
                if (ArchMode == ArchitectureMode.BS)
                {
                    //Request.RequestUri.Scheme
                    return WebConfigurationManager.AppSettings["Cache_Redis_Configuration"].ToString().Trim();

                }
                else
                {
                    return "";
                }
            }
        }



        /// <summary>
        /// 该用户可以同时上传的文件数
        /// </summary>
        public static int UploadTokenThreadNum
        {
            get
            {
                try
                {
                    if (ArchMode == ArchitectureMode.BS)
                    {

                        return int.Parse(WebConfigurationManager.AppSettings["UploadTokenThreadNum"].ToString().Trim());

                    }
                    else
                    {
                        return 0;
                    }
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 短信用户名
        /// </summary>
        public static string MsgUserName
        {
            get
            {
                if (ArchMode == ArchitectureMode.BS)
                {

                    return WebConfigurationManager.AppSettings["MsgUserName"].ToString().Trim();

                }
                else
                {
                    return "";
                }
            }
        }
        /// <summary>
        /// 短信，密码
        /// </summary>
        public static string MsgPassword
        {
            get
            {

                if (ArchMode == ArchitectureMode.BS)
                {

                    return WebConfigurationManager.AppSettings["MsgPwd"].ToString().Trim();

                }
                else
                {
                    return "";
                }

            }
        }


        #region 远程调用的参数

        /// <summary>
        /// 远程webapi的基址
        /// </summary>
        public static string WebHost
        {
            get
            {
                if (ArchMode == ArchitectureMode.BS)
                {

                    return WebConfigurationManager.AppSettings["WebHost"].ToString().Trim();

                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// 远程webapi的本地用户名
        /// </summary>
        public static string ClientId
        {
            get
            {
                if (ArchMode == ArchitectureMode.BS)
                {

                    return WebConfigurationManager.AppSettings["ClientId"].ToString().Trim();

                }
                else
                {
                    return "";
                }
            }
        }
        /// <summary>
        /// 远程webapi的
        /// </summary>
        public static string ClientSecret
        {
            get
            {
                if (ArchMode == ArchitectureMode.BS)
                {

                    return WebConfigurationManager.AppSettings["ClientSecret"].ToString().Trim();

                }
                else
                {
                    return "";
                }
            }
        }

        #endregion

        #region MyRegion
        /// <summary>
        /// 上传FTP配置
        /// </summary>
        public static List<Common.FtpConnection> FtpConnections
        {
            get
            {
                string value = "";
                if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("FtpConnection"))
                {
                    value = System.Configuration.ConfigurationManager.AppSettings["FtpConnection"];
                }
                List<Common.FtpConnection> ftpConns = new List<Common.FtpConnection>();
                if (!String.IsNullOrEmpty(value))
                {
                    string[] conns = value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (conns.Length > 0)
                    {
                        for (int i = 0; i < conns.Length; i++)
                        {
                            string[] temps = conns[i].Split(new char[] { ';', '；' }, StringSplitOptions.RemoveEmptyEntries);
                            if (temps.Length == 3)
                            {
                                var conn = new Common.FtpConnection
                                {
                                    ServerIp = temps[0].Trim(),
                                    UserName = temps[1].Trim(),
                                    Password = temps[2].Trim()
                                };
                                if (ftpConns.All(u => !String.Equals(u.ToString(), conn.ToString(), StringComparison.OrdinalIgnoreCase))
                                    && !String.IsNullOrEmpty(conn.ToString()))
                                {
                                    ftpConns.Add(conn);
                                }
                            }
                        }
                    }
                }
                return ftpConns;
            }
        }

        /// <summary>
        /// 上传FTP尝试次数，默认3次
        /// </summary>
        public static int FtpTryTimes
        {
            get
            {
                string value = "";
                if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("FtpTryTimes"))
                {
                    value = System.Configuration.ConfigurationManager.AppSettings["FtpTryTimes"];
                }

                int tryTimes;
                if (!int.TryParse(value, out tryTimes))
                {
                    tryTimes = 3;
                }
                if (tryTimes < 0)
                {
                    tryTimes = 1;
                }
                return tryTimes;
            }
        }
        /// <summary>
        /// 返回IP地址列表
        /// </summary>
        public static string IPList
        {
            get
            {
                string value = "";
                if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("IPList"))
                {
                    value = System.Configuration.ConfigurationManager.AppSettings["IPList"];
                }

                return value;
            }
        }
        #endregion
    }
}
