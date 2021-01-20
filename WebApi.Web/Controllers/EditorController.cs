using Common;
using Dapper;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.Entity;
using WebApi.Plib;
using WebApi.Utility;
using WebApi.web.Helper;
using WebApi.Web.Handler;
using WebApi.Web.ViewModels;

namespace WebApi.Web.Controllers
{
    /// <summary>
    /// 公网：在线编辑和业务系统的交互
    /// </summary>
    public class EditorController : BaseApiController
    {

        /// <summary>
        /// 获取config json
        /// </summary>
        /// <param name="pin"></param>
        /// <returns></returns>
        [HttpGet]
        public PageResult GetConfig()
        {

            try
            {
                
                

                //填充json
                dynamic result = null;

                using (SqlConnection conn = CommonDapper.GetOpenConnection(DataConnectType.ServerDBDataService))
                {
                    result = conn.Query(@"
select top 1 fi.title,join1.*
  FROM 
  (   select *,ROW_NUMBER() over( Order BY ID ) as num
  FROM [VersionInfo] 
    where Status=@Status and BusinessId=@BusinessId and ClientId=@ClientId
  ) join1 inner join FileInfo fi on join1.ClientId=fi.ClientId and join1.BusinessId=fi.BusinessId and fi.Status=1 
order by num desc

", new { ClientId = CurrentUserInfo.SysType, BusinessId = CurrentUserInfo.BusinessId, Status = 1 }).FirstOrDefault(); ;
                }

                if (result == null)
                    return new PageResult(this.Request, (int)ReturnStatus.failed, "不存在对应的文档");



                string url = GetApiUrl() + "/api/Editor/GetDoc?pin=" + CurrentUserInfo.CacheKey + "&key=" + result.VersionKey;


                string callbackUrl = GetApiUrl() + "/api/Editor/SaveDoc?pin=" + CurrentUserInfo.CacheKey;// + "&key=" + result.VersionKey;
                //string callbackUrl = "";


                string versionKey = Convert.ToString(result.VersionKey);
                string title = Convert.ToString(result.title);


                bool userMode = string.Equals(CurrentUserInfo.UserMode, "1");
                bool editPrivilege = string.Equals(CurrentUserInfo.EditPrivilege, "1");
                bool commentPrivilege = string.Equals(CurrentUserInfo.CommentPrivilege, "1");

                DocConfig docConfig = new DocConfig(CurrentUserInfo.UserID, CurrentUserInfo.UserName,
                    versionKey, title, url,
                    userMode, editPrivilege, commentPrivilege,
                    callbackUrl);

                JsonWebTokenUtilitys jwtut = new JsonWebTokenUtilitys();

                string token = jwtut.CreateJwtToken(docConfig.GetPayload());

                docConfig.token = token;



                return new PageResult(this.Request, (int)ReturnStatus.success, "", docConfig.GetContent());

            }
            catch (Exception ex)
            {
                //异常日志
                KYCX.Logging.Logger.DefaultLogger.Error(ex.Message, ex);
                return new PageResult(this.Request, (int)ReturnStatus.failed, "数据处理异常");
            }
        }


        /// <summary>
        /// 版本历史，获取当前用户对应的版本列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public PageResult GetHistoryList()
        {

            try
            {
                List<VersionInfo> list = new List<VersionInfo>();

                using (SqlConnection conn = CommonDapper.GetOpenConnection(DataConnectType.ServerDBDataService))
                {
                    list.AddRange(conn.Query<VersionInfo>(@"select *,ROW_NUMBER() over( Order BY ID ) as num  FROM [VersionInfo]
where ClientId=@ClientId and BusinessId=@BusinessId  and Status=@Status  ",
                        new { ClientId = CurrentUserInfo.SysType, BusinessId = CurrentUserInfo.BusinessId, Status = 1 }));
                }

                if (list.Count <= 0)
                    return new PageResult(this.Request, (int)ReturnStatus.failed, "不存在此文件");

                List<dynamic> result = new List<dynamic>();
                foreach (VersionInfo item in list)
                {
                    if (!string.IsNullOrWhiteSpace(item.History))
                    {
                        JObject jobj = JsonConvertEx.JsonToJObject(item.History);

                        var tmp = new
                        {
                            changes = jobj["changes"],
                            serverVersion = jobj["serverVersion"],
                            created = item.CreateTime,
                            key = item.VersionKey,
                            user = new { id = item.Uid, name = item.UserName },
                            version = item.num
                        };

                        result.Add(tmp);
                    }
                    else
                    {
                        var tmp = new
                        {
                            created = item.CreateTime,
                            key = item.VersionKey,
                            user = new { id = item.Uid, name = item.UserName },
                            version = item.num
                        };

                        result.Add(tmp);
                    }

                }

                return new PageResult(this.Request, (int)ReturnStatus.success, list.Count.ToString(), result);

            }
            catch (Exception ex)
            {
                //异常日志
                KYCX.Logging.Logger.DefaultLogger.Error(ex.Message, ex);
                return new PageResult(this.Request, (int)ReturnStatus.failed, "数据处理异常");
            }

            return new PageResult(this.Request, (int)ReturnStatus.failed, "数据处理异常");
        }


        /// <summary>
        /// 版本历史，获取一个版本的详细数据
        /// </summary>
        /// <param name="varsionNum"></param>
        /// <returns></returns>
        [HttpGet]
        public PageResult GetHistoryData([FromUri]int varsionNum)
        {

            try
            {
                

                List<VersionInfo> list = new List<VersionInfo>();

                using (SqlConnection conn = CommonDapper.GetOpenConnection(DataConnectType.ServerDBDataService))
                {
                    list.AddRange(conn.Query<VersionInfo>(@"

select * from (
    select *,ROW_NUMBER() over( Order BY ID ) as num  FROM [VersionInfo]
    where ClientId=@ClientId and BusinessId=@BusinessId  and Status=@Status 
) join1 where num=@varsionNum

",
                        new { ClientId = CurrentUserInfo.SysType, BusinessId = CurrentUserInfo.BusinessId, Status = 1, varsionNum = varsionNum }));
                }

                if (list.Count <= 0)
                    return new PageResult(this.Request, (int)ReturnStatus.failed, "不存在此文件");

                string url = GetApiUrl() + "/api/Editor/GetDoc?pin=" + CurrentUserInfo.CacheKey + "&key=" + list[0].VersionKey;

                string changesUrl = "";
                string previousUrl = "";

                if (!string.IsNullOrWhiteSpace(list[0].ChangesUrl))
                {
                    changesUrl = GetApiUrl() + "/api/Editor/GetZip?id=" + list[0].ID;
                    previousUrl = GetApiUrl() + "/api/Editor/GetDoc?pin=" + CurrentUserInfo.CacheKey + "&key=" + list[0].PriorVersionKey;
                }



                string key = Convert.ToString(list[0].VersionKey);

                HistoryDataConfig dataConfig = new HistoryDataConfig(key, url, varsionNum, changesUrl, previousUrl, list[0].PriorVersionKey);

                JsonWebTokenUtilitys jwtut = new JsonWebTokenUtilitys();

                string token = jwtut.CreateJwtToken(dataConfig.GetPayload());

                dataConfig.token = token;

                return new PageResult(this.Request, (int)ReturnStatus.success, "", dataConfig.GetContent());

            }
            catch (Exception ex)
            {
                //异常日志
                KYCX.Logging.Logger.DefaultLogger.Error(ex.Message, ex);
                return new PageResult(this.Request, (int)ReturnStatus.failed, "数据处理异常");
            }

            return new PageResult(this.Request, (int)ReturnStatus.failed, "数据处理异常");
        }


        [NonAction]
        [AllowAnonymous]
        public PageResult Test()
        {
            JsonWebTokenUtilitys jwtut = new JsonWebTokenUtilitys();

            //this.Request.RequestUri.Scheme

            string cc = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJlZGl0b3JDb25maWciOnsibGFuZyI6InpoLUNOIiwibW9kZSI6ImVkaXQiLCJjYWxsYmFja1VybCI6Imh0dHA6Ly8xOTIuMTY4LjE2OC43Ny9FZGl0b3JEZWJ1Zy9hcGkvRWRpdG9yL1NhdmVEb2M_cGluPWYxNTU0ZmQyODVlOTQ4ZWE5YzQ4ODI3YjU0MGQ5YjlkIiwiY3VzdG9taXphdGlvbiI6eyJjaGF0IjpmYWxzZSwic3BlbGxjaGVjayI6ZmFsc2UsInNob3dSZXZpZXdDaGFuZ2VzIjpmYWxzZX0sInVzZXIiOnsiaWQiOiIxMTEiLCJuYW1lIjoiemhhbmdzYW4ifX0sImRvY3VtZW50Ijp7ImZpbGVUeXBlIjoiZG9jeCIsImtleSI6IjI2MzU2Nzg2MTYzNjQ3NDg4IiwidGl0bGUiOiLmtYvor5XnlKjkvovvvJoiLCJ1cmwiOiJodHRwOi8vMTkyLjE2OC4xNjguNzcvRWRpdG9yRGVidWcvYXBpL0VkaXRvci9HZXREb2M_cGluPWYxNTU0ZmQyODVlOTQ4ZWE5YzQ4ODI3YjU0MGQ5YjlkJmtleT0yNjM1Njc4NjE2MzY0NzQ4OCIsInBlcm1pc3Npb25zIjp7ImRvd25sb2FkIjp0cnVlLCJlZGl0Ijp0cnVlLCJwcmludCI6dHJ1ZSwicmV2aWV3IjpmYWxzZX19LCJkb2N1bWVudFR5cGUiOiJ0ZXh0IiwibGFuZyI6InpoLUNOIn0.rmSxqL_QO5YaslZpbjFtLq5K2rDRkU-e223fCP5Ftno";


            string json = jwtut.ValidateJwtToken(cc);

            return new PageResult(this.Request, (int)ReturnStatus.failed, "数据处理异常");

        }

        /// <summary>
        /// 获取doc文件流
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public IHttpActionResult GetDoc([FromUri]string pin, [FromUri]string key)
        {


            try
            {
                UsersHandlers handler = new UsersHandlers();
                UserIdentity userinfo = null;

                //通过userinfo获取最新businessid对应的version信息
                if (string.IsNullOrWhiteSpace(pin))
                {
                    return new PageResult(this.Request, (int)ReturnStatus.failed, "token不能为空");
                }

                userinfo = handler.GetCache(pin);

                //存在则读取到对应的userinfo;账号过期则为null，相当于此时是重新登录
                if (userinfo == null)
                {
                    return new PageResult(this.Request, (int)ReturnStatus.failed, "用户已过期，请重新登录");
                }

                //从ftp下载该文件
                List<VersionInfo> list = new List<VersionInfo>();

                using (SqlConnection conn = CommonDapper.GetOpenConnection(DataConnectType.ServerDBDataService))
                {
                    list.AddRange(conn.Query<VersionInfo>(@"select top 1 * from VersionInfo 
where ClientId=@ClientId and BusinessId=@BusinessId and VersionKey=@VersionKey and Status=@Status  ",
                        new { ClientId = userinfo.SysType, BusinessId = userinfo.BusinessId, VersionKey = key, Status = 1 }));
                }

                if (list.Count <= 0)
                    return new PageResult(this.Request, (int)ReturnStatus.failed, "不存在此版本");

                string localFileFullName = GetFtpPath() + Guid.NewGuid().ToString() + ".docx";


                if (!FtpUpDownFiles.FtpDownLoadFile2Local(list[0].FtpConn, list[0].FtpFilePath, localFileFullName))
                {

                    return new PageResult(this.Request, (int)ReturnStatus.failed, "ftp下载失败");
                }

                if (File.Exists(localFileFullName))
                {
                    return new FileResult(localFileFullName, Path.GetFileName(localFileFullName), this.Request);
                }
                else
                {
                    return new PageResult(this.Request, (int)ReturnStatus.failed, "本地下载失败");
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


        /// <summary>
        /// 获取历史版本zip文件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public IHttpActionResult GetZip([FromUri]string id)
        {


            try
            {
                //从ftp下载该文件
                List<VersionInfo> list = new List<VersionInfo>();

                using (SqlConnection conn = CommonDapper.GetOpenConnection(DataConnectType.ServerDBDataService))
                {
                    list.AddRange(conn.Query<VersionInfo>(@"select top 1 * from VersionInfo where id=@id and Status=@Status  ",
                        new { id = id, Status = 1 }));
                }

                if (list.Count <= 0)
                    return new PageResult(this.Request, (int)ReturnStatus.failed, "不存在此版本");

                string localFileFullName = GetFtpPath() + Guid.NewGuid().ToString() + ".zip";


                if (!FtpUpDownFiles.FtpDownLoadFile2Local(list[0].FtpConnForChanges, list[0].ChangesUrl, localFileFullName))
                {

                    return new PageResult(this.Request, (int)ReturnStatus.failed, "下载失败");
                }

                if (File.Exists(localFileFullName))
                {
                    return new FileResult(localFileFullName, Path.GetFileName(localFileFullName), this.Request);
                }
                else
                {
                    return new PageResult(this.Request, (int)ReturnStatus.failed, "下载失败");
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




        private string GetFtpPath()
        {

            string localpath = Plib.ep.LocalDiskDir + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\" + DateTime.Now.ToString("HH") + "\\" + DateTime.Now.ToString("mm") + "\\";

            return localpath;
        }

        /// <summary>
        /// 必须传递用户登录时的jwt，否则无效
        /// </summary>
        /// <param name="paramStr"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public IHttpActionResult SaveDoc([FromBody]dynamic paramStr)
        {

            string logStr = JsonConvertEx.ObjectToJson(paramStr);

            try
            {


                //Request.RequestUri.Query    ? pin = f1554fd285e948ea9c48827b540d9b9d
                string[] qs = Request.RequestUri.Query.Split(new char[] { '=' });

                string pin = "";
                if (qs.Length == 2)
                    pin = qs[1];
                else
                {

                    KYCX.Logging.Logger.DefaultLogger.Error("参数错误！！" + logStr);
                    return Json<dynamic>(new { error = 1 });
                    //return new PageResult(this.Request, (int)ReturnStatus.failed, "参数错误");
                }


                int status = Convert.ToInt32(paramStr.status);

                if (status != 2)
                    return Json<dynamic>(new { error = 0 });


                string key = Convert.ToString(paramStr.key);
                string url = Convert.ToString(paramStr.url);
                string changesurl = Convert.ToString(paramStr.changesurl);
                string history = Convert.ToString(paramStr.history);
                string users = Convert.ToString(paramStr.users);


                //验证并执行保存任务
                UsersHandlers handler = new UsersHandlers();
                UserIdentity userinfo = null;

                //通过userinfo获取最新businessid对应的version信息
                if (string.IsNullOrWhiteSpace(pin))
                {
                    KYCX.Logging.Logger.DefaultLogger.Error("token不能为空！！" + logStr);
                    return Json<dynamic>(new { error = 0 });
                    //return new PageResult(this.Request, (int)ReturnStatus.failed, "token不能为空");
                }

                userinfo = handler.GetCache(pin);

                //存在则读取到对应的userinfo;账号过期则为null，相当于此时是重新登录
                if (userinfo == null)
                {
                    KYCX.Logging.Logger.DefaultLogger.Error("用户已过期！！" + logStr);
                    return Json<dynamic>(new { error = 0 });

                    //return new PageResult(this.Request, (int)ReturnStatus.failed, "用户已过期，请重新登录");
                }

                List<VersionInfo> list = new List<VersionInfo>();

                using (SqlConnection conn = CommonDapper.GetOpenConnection(DataConnectType.ServerDBDataService))
                {
                    list.AddRange(conn.Query<VersionInfo>(@"select top 1 * from VersionInfo 
where ClientId=@ClientId and BusinessId=@BusinessId and VersionKey=@VersionKey and Status=@Status  ",
                        new { ClientId = userinfo.SysType, BusinessId = userinfo.BusinessId, VersionKey = key, Status = 1 }));
                }

                if (list.Count <= 0)
                {
                    KYCX.Logging.Logger.DefaultLogger.Error("不存在此版本！！" + logStr);
                    return Json<dynamic>(new { error = 0 });

                    return new PageResult(this.Request, (int)ReturnStatus.failed, "不存在此版本");
                }


                string PATH_FOR_SAVE = GetFtpPath();

                if (!Directory.Exists(PATH_FOR_SAVE))
                {
                    Directory.CreateDirectory(PATH_FOR_SAVE);
                }
                //保存此版本
                string docxPath = PATH_FOR_SAVE + Guid.NewGuid().ToString() + ".docx";

                var req = WebRequest.Create(url);

                using (var stream = req.GetResponse().GetResponseStream())
                using (var fs = File.Open(docxPath, FileMode.Create))
                {
                    var buffer = new byte[4096];
                    int readed;
                    while ((readed = stream.Read(buffer, 0, 4096)) != 0)
                        fs.Write(buffer, 0, readed);
                }

                //ftp上传和db插入
                string message = "";
                string ftpConnStr = "";
                if (!FtpUpDownFiles.UploadFile(docxPath, docxPath, out message, out ftpConnStr))
                {
                    KYCX.Logging.Logger.DefaultLogger.Error(message + logStr);
                    return Json<dynamic>(new { error = 0 });


                    return new PageResult(this.Request, (int)ReturnStatus.failed, message);
                }

                //保存变更集


                string zipPath = PATH_FOR_SAVE + Guid.NewGuid().ToString() + ".zip";
                var req2 = WebRequest.Create(changesurl);

                using (var stream = req2.GetResponse().GetResponseStream())
                using (var fs = File.Open(zipPath, FileMode.Create))
                {
                    var buffer = new byte[4096];
                    int readed;
                    while ((readed = stream.Read(buffer, 0, 4096)) != 0)
                        fs.Write(buffer, 0, readed);
                }


                string ftpConnStr222 = "";
                if (!FtpUpDownFiles.UploadFile(zipPath, zipPath, out message, out ftpConnStr222))
                {
                    KYCX.Logging.Logger.DefaultLogger.Error(message + logStr);
                    return Json<dynamic>(new { error = 0 });

                    return new PageResult(this.Request, (int)ReturnStatus.failed, message);
                }

                //生成当前版本key
                SnowflakeIDcreator.SetWorkerID(list[0].ID);
                long versionKey = SnowflakeIDcreator.nextId();

                VersionInfo vo = new VersionInfo()
                {
                    CreateTime = DateTime.Now,
                    Status = 1,
                    BusinessId = userinfo.BusinessId,
                    FileID = list[0].FileID,
                    ClientId = userinfo.SysType,
                    Uid = userinfo.UserID,
                    UserName = userinfo.UserName,
                    RoleName = userinfo.RoleName,
                    VersionKey = versionKey,
                    FtpConn = ftpConnStr,
                    FtpFilePath = docxPath,
                    PriorVersionKey = Convert.ToInt64(key),
                    FtpConnForChanges = ftpConnStr222,
                    ChangesUrl = zipPath,
                    History = history
                };

                using (SqlConnection conn = CommonDapper.GetOpenConnection(DataConnectType.ServerDBDataService))
                {
                    conn.Insert(vo);
                }
                return Json<dynamic>(new { error = 0 });
            }
            catch (Exception ex)
            {
                //异常日志
                KYCX.Logging.Logger.DefaultLogger.Error(ex.Message + logStr, ex);

                return Json<dynamic>(new { error = 5001 });

                //return new PageResult(this.Request, (int)ReturnStatus.failed, "数据处理异常");
            }




        }




    }
}
