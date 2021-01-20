using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WebApi.Plib;

namespace WebApi.web.Helper
{
    public class FtpUpDownFiles
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="localPath"></param>
        /// <param name="ftpPath"></param>
        /// <param name="message"></param>
        /// <param name="ftpConnStr"></param>
        /// <returns></returns>
        public static bool UploadFile(string localPath, string ftpPath, out string message, out string ftpConnStr)
        {
            bool success = false;
            message = "";
            ftpConnStr = "";
            try
            {
                if (string.IsNullOrWhiteSpace(localPath) || !File.Exists(localPath))
                {
                    message = "上传失败，文件不存在";
                    return false;
                }

                var ftpConns = ep.FtpConnections.Copy();
                int ftpTryTimes = ep.FtpTryTimes;

                if (ftpConns.Count <= 0)
                {
                    message = "上传失败，配置错误(F)";
                    return false;
                }
                Random rdm = new Random();
                for (int i = 0; i < 2; i++) //如果第一个ftp失败了，则换一个重试一次
                {
                    int index = rdm.Next(0, ftpConns.Count);
                    var curFtpConn = ftpConns[index];
                    FtpHelper ftpHelper = new FtpHelper(curFtpConn);
                    for (int t = 0; t < ftpTryTimes; t++)
                    {
                        success = ftpHelper.Upload(localPath, ftpPath);
                        if (success)
                        {
                            ftpConnStr = curFtpConn.ToString();
                            break;
                        }
                        else
                        {
                            message = "上传失败，保存文件到服务器失败(" + t + "/" + i + ")";
                            KYCX.Logging.Logger.DefaultLogger.ErrorFormat(message + "。当前FTP：{0}，文件：{1}", curFtpConn.ServerIp, localPath);
                        }
                    }
                    if (success)
                    {
                        break;
                    }
                    else
                    {
                        ftpConns.RemoveAt(index);
                        if (ftpConns.Count <= 0)
                        {
                            break;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                success = false;
                message = "上传失败，保存文件失败(E)";
                KYCX.Logging.Logger.DefaultLogger.ErrorFormat(message + "。文件：{0}", ex, localPath);
            }
            return success;
        }

        /// <summary>
        /// 根据ftpInfo是否为emptty，来判断使用哪种形式的ftp
        /// </summary>
        /// <param name="ftpConn"></param>
        /// <param name="ftpPath"></param>
        /// <param name="localPath"></param>
        /// <returns></returns>
        public static bool FtpDownLoadFile2Local(string ftpConn, string ftpPath, string localPath)
        {

            bool fg = false;
            string ErorMess = "";
            try
            {
                string[] ar_ftpstring = ftpConn.Split(';');

                if (ar_ftpstring.Length.Equals(3))
                {
                    string ftpip = ar_ftpstring[0].ToString();

                    string ftpuser = ar_ftpstring[1].ToString();

                    string ftpuserpwd = ar_ftpstring[2].ToString();

                    FtpClass ftpdel = new FtpClass(ftpip, ftpuser, ftpuserpwd);

                    fg = ftpdel.Download(ftpPath, localPath, out ErorMess);


                }
            }
            catch (Exception ex)
            {

                KYCX.Logging.Logger.DefaultLogger.Error(ex.Message, ex);

                fg = false;
            }


            return fg;


        }
    }
}
