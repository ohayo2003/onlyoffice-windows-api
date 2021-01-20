using Common;
using Dapper;
using Dapper.Contrib;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
    /// 局域网：本地和业务系统的后台交互
    /// </summary>
    public class LocalOperationController : ApiController
    {

        UsersHandlers uh = new UsersHandlers();
        //public PageResult CheckIPList()
        //{

        //}
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="businessId"></param>
        /// <param name="UID"></param>
        /// <returns></returns>
        [HttpGet]
        public PageResult DelFile(string clientId, string businessId, string UID )
        {
          
            if (!uh.CheckIP())
            {
                return new PageResult(this.Request, (int)ReturnStatus.failed, "没有权限");
            }



            string error = "出现问题";
            
            try
            {
                int result = 0;
                bool insertSuccess = false;

                using (SqlConnection conn = CommonDapper.GetOpenConnection(DataConnectType.ServerDBDataService))
                {
                    IDbTransaction transaction = conn.BeginTransaction();
                    try
                    {
                        result = conn.Execute("update FileInfo set Status=@Status where BusinessId=@BusinessId and ClientId=@ClientId and UID=@UID ", new { ClientId = clientId, BusinessId = businessId, Status = 0, UID = UID },transaction);

                        result = conn.Execute("update VersionInfo set Status=@Status where BusinessId=@BusinessId and ClientId=@ClientId and UID=@UID ", new { ClientId = clientId, BusinessId = businessId, Status = 0, UID = UID },transaction);

                        transaction.Commit();
                        insertSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        insertSuccess = false;

                        string paramStr = string.Format("更新数据库异常，详情：{0}   ", Common.JsonConvertEx.ObjectToJson(new { ClientId = clientId, BusinessId = businessId, Status = 0, UID = UID }));

                        KYCX.Logging.Logger.DefaultLogger.Error(paramStr + ex.Message, ex);

                    }
                }

                if (insertSuccess)
                    return new PageResult(this.Request, (int)ReturnStatus.success, "");
                else
                return new PageResult(this.Request, (int)ReturnStatus.failed, "删除错误");


            }
            catch (Exception ex)
            {
                error = ex.Message;
                KYCX.Logging.Logger.DefaultLogger.Error(ex.Message, ex);
            }

            return new PageResult(this.Request, (int)ReturnStatus.failed, error);
        }


        /// <summary>
        /// 创建新文档
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public PageResult InitDoc([FromBody]InitDocDto dto)
        {
            //验证dto的合法性

            if (!uh.CheckIP())
            {
                return new PageResult(this.Request, (int)ReturnStatus.failed, "没有权限");
            }

            string error = "出现问题";

            try
            {

                List<ClientInfo> list = new List<ClientInfo>();

                using (SqlConnection conn = CommonDapper.GetOpenConnection(DataConnectType.ServerDBDataService))
                {
                    list.AddRange(conn.Query<ClientInfo>("select top 1 * from ClientInfo where ClientId=@ClientId and Status=@Status  ",
                        new { ClientId = dto.client_id, Status = 1 }));
                }

                if (list.Count <= 0)
                    return new PageResult(this.Request, (int)ReturnStatus.failed, "clientid参数无效");

                //验证签名
                #region 生成文件key和加密值的集合


                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("client_id", dto.client_id);
                dic.Add("uid", dto.uid);
                dic.Add("businessId", dto.businessId);

                string check_code = CrypHash.GetCheckCode(dic, list[0].ClientKey);

                if (!string.Equals(check_code, dto.signature, StringComparison.OrdinalIgnoreCase))
                    return new PageResult(this.Request, (int)ReturnStatus.failed, "签名无效");

                #endregion


                //不能重复新建文档
                List<FileInfo> fiList = new List<FileInfo>();

                using (SqlConnection conn = CommonDapper.GetOpenConnection(DataConnectType.ServerDBDataService))
                {
                    fiList.AddRange(conn.Query<FileInfo>("select top 1 * from FileInfo where ClientId=@ClientId and BusinessId=@BusinessId and Status=@Status  ",
                        new { ClientId = dto.client_id, BusinessId = dto.businessId, Status = 1 }));
                }

                if (fiList.Count > 0)
                    return new PageResult(this.Request, (int)ReturnStatus.failed, "已经存在文档无需重复新建");


                //生成当前版本key
                SnowflakeIDcreator.SetWorkerID(list[0].ID);
                long versionKey = SnowflakeIDcreator.nextId();

                string localFileFullName = Plib.ep.LocalDiskDir + "new.docx";
                string ftpFileFullName = GetFtpPath() + Guid.NewGuid().ToString() + ".docx";
                string ftpConnStr = "";

                int UploadType = 1;


                ///在线新建
                if (dto.UploadType == "1")
                {
                    //上传到ftp
                    string message = "";
                 
                    if (!FtpUpDownFiles.UploadFile(localFileFullName, ftpFileFullName, out message, out ftpConnStr))
                    {
                        return new PageResult(this.Request, (int)ReturnStatus.failed, message);
                    }
                }
                else
                {
                    ///本地上传
                    UploadType = 2;
                    ftpConnStr = dto.FtpPath.Split('|')[0];
                    ftpFileFullName = dto.FtpPath.Split('|')[1];
                }

                //成功则事务保存信息到FileInfo和VersionInfo 


                FileInfo fo = new FileInfo()
                {
                    CreateTime = DateTime.Now,
                    Status = 1,
                    UploadType = UploadType,
                    Title = dto.title,
                    BusinessId = dto.businessId,
                    ClientId = dto.client_id,
                    Uid = dto.uid,
                    UserName = dto.user_name,
                    RoleName = dto.user_rolename,
                    OtherMsg = ""
                };

                VersionInfo vo = new VersionInfo()
                {
                    CreateTime = DateTime.Now,
                    Status = 1,
                    BusinessId = dto.businessId,
                    ClientId = dto.client_id,
                    Uid = dto.uid,
                    UserName = dto.user_name,
                    RoleName = dto.user_rolename,
                    VersionKey = versionKey,
                    FtpConn = ftpConnStr,
                    FtpFilePath = ftpFileFullName,
                    PriorVersionKey = -1,
                    FtpConnForChanges="",
                    ChangesUrl = "",
                    History = ""
                };


                bool insertSuccess = false;
                using (SqlConnection conn = CommonDapper.GetOpenConnection(DataConnectType.ServerDBDataService))
                {
                    IDbTransaction transaction = conn.BeginTransaction();
                    try
                    {

                        string strAddFileSql = "insert into FileInfo(BusinessId,ClientId,CreateTime,OtherMsg,RoleName,Status,Title,Uid,UploadType,UserName) values(@BusinessId,@ClientId,@CreateTime,@OtherMsg,@RoleName,@Status,@Title,@Uid,@UploadType,@UserName);select SCOPE_IDENTITY() as FileID";
                        //conn.Execute(strAddFileSql, fo, transaction);
                        int fileID = conn.Query<int>(strAddFileSql, fo,transaction).FirstOrDefault();
                        //conn.Insert(fo, transaction);
                        vo.FileID = fileID;
                        conn.Insert(vo, transaction);

                        transaction.Commit();
                        insertSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        insertSuccess = false;

                        string paramStr = string.Format("插入数据库异常，详情：{0}   ",Common.JsonConvertEx.ObjectToJson(fo));

                        KYCX.Logging.Logger.DefaultLogger.Error(paramStr + ex.Message, ex);

                    }
                }
                if (insertSuccess)
                {
                    return new PageResult(this.Request, (int)ReturnStatus.success, "");
                }
                else
                {
                    return new PageResult(this.Request, (int)ReturnStatus.failed, "数据库操作有问题");
                }


            }
            catch (Exception ex)
            {
                error = ex.Message;
                KYCX.Logging.Logger.DefaultLogger.Error(ex.Message, ex);
            }

            return new PageResult(this.Request, (int)ReturnStatus.failed, error);




            //return new PageResult(this.Request, (int)ReturnStatus.success, dto.businessId);
        }

        private string GetFtpPath()
        {

            string localpath = Plib.ep.LocalDiskDir + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\" + DateTime.Now.ToString("HH") + "\\" + DateTime.Now.ToString("mm") + "\\";

            return localpath;
        }

        /// <summary>
        /// 获取doc信息：
        /// 缺失version参数则返回最新版本
        /// 包括文件名、最后更新时间、最新版本号等
        /// </summary>
        /// <param name="businessId">业务唯一id swid</param>
        /// <returns></returns>
        [HttpGet]
        public PageResult GetDocInfo(string clientId, string businessId, int version = -1)
        {
            if (!uh.CheckIP())
            {
                return new PageResult(this.Request, (int)ReturnStatus.failed, "没有权限");
            }
            string error = "出现问题";

            try
            {
                dynamic result = null;

                using (SqlConnection conn = CommonDapper.GetOpenConnection(DataConnectType.ServerDBDataService))
                {
                    result = conn.Query(@"
select top 1 fi.title,join1.[CreateTime],num versionNum,join1.ClientId,join1.BusinessId
  FROM 
  (   select *,ROW_NUMBER() over( Order BY ID ) as num
  FROM [VersionInfo] 
    where Status=@Status and BusinessId=@BusinessId and ClientId=@ClientId
  ) join1 inner join FileInfo fi on join1.ClientId=fi.ClientId and join1.BusinessId=fi.BusinessId and fi.Status=1
order by num desc

", new { ClientId = clientId, BusinessId = businessId, Status = 1 }).FirstOrDefault(); ;
                }

                if (result == null)
                    return new PageResult(this.Request, (int)ReturnStatus.success, "");

                return new PageResult(this.Request, (int)ReturnStatus.success, "", JsonConvertEx.ObjectToJsonAndDateTimeFormat(result), false);


            }
            catch (Exception ex)
            {
                error = ex.Message;
                KYCX.Logging.Logger.DefaultLogger.Error(ex.Message, ex);
            }

            return new PageResult(this.Request, (int)ReturnStatus.failed, error);
        }

        [HttpGet]
        public PageResult GetFtpPathInfo(string clientId, string businessId)
        {
            if (!uh.CheckIP())
            {
                return new PageResult(this.Request, (int)ReturnStatus.failed, "没有权限");
            }
            string error = "出现问题";

            try
            {
                dynamic result = null;

                using (SqlConnection conn = CommonDapper.GetOpenConnection(DataConnectType.ServerDBDataService))
                {
                    result = conn.Query(@"
select top 1 fi.title,join1.*
  FROM 
  (   select *,ROW_NUMBER() over( Order BY ID ) as versionNum
  FROM [VersionInfo] 
    where Status=@Status and BusinessId=@BusinessId and ClientId=@ClientId
  ) join1 inner join FileInfo fi on join1.ClientId=fi.ClientId and join1.BusinessId=fi.BusinessId and fi.Status=1
order by versionNum desc

", new { ClientId = clientId, BusinessId = businessId, Status = 1 }).FirstOrDefault(); ;
                }

                if (result == null)
                    return new PageResult(this.Request, (int)ReturnStatus.success, "");

                return new PageResult(this.Request, (int)ReturnStatus.success, "", JsonConvertEx.ObjectToJsonAndDateTimeFormat(result), false);


            }
            catch (Exception ex)
            {
                error = ex.Message;
                KYCX.Logging.Logger.DefaultLogger.Error(ex.Message, ex);
            }

            return new PageResult(this.Request, (int)ReturnStatus.failed, error);
        }




        /// <summary>
        /// 返回历史版本列表json
        /// </summary>
        /// <param name="businessId">业务唯一id swid</param>
        /// <returns></returns>
        [NonAction]
        [HttpGet]
        public PageResult GetAllHistory([FromUri]string businessId)
        {
            List<HistoryInfo> result = new List<HistoryInfo>();

            result.Add(new HistoryInfo
            {
                title = "ceshi",
                createTime = DateTime.Now,
                version = 1,
                versionKey = 1444,
                businessId = businessId
            });
            result.Add(new HistoryInfo
            {
                title = "ceshi",
                createTime = DateTime.Now,
                version = 2,
                versionKey = 5555,
                businessId = businessId
            });




            return new PageResult(this.Request, (int)ReturnStatus.success, "", JsonConvertEx.ObjectToJsonAndDateTimeFormat(result), false);
        }
    }

    public class HistoryInfo
    {
        public string title { get; set; }
        public DateTime createTime { get; set; }
        public int version { get; set; }
        public int versionKey { get; set; }
        public string businessId { get; set; }
    }
}
