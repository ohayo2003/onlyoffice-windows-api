using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Common
{
    /// <summary>
    /// FTP
    /// </summary>
    public class FtpHelper
    {
        #region 初始化
        /// <summary>
        /// FTP服务器地址
        /// </summary>
        private string _ftpServerIp;
        /// <summary>
        /// FTP用户名
        /// </summary>
        private string _ftpUserId;
        /// <summary>
        /// FTP密码
        /// </summary>
        private string _ftpPassword;
        /// <summary>
        /// FTP客户端
        /// </summary>
        FtpWebRequest _ftpRequest;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ftpServerIp"></param>
        /// <param name="ftpUserId"></param>
        /// <param name="ftpPassword"></param>
        public FtpHelper(string ftpServerIp, string ftpUserId, string ftpPassword)
        {
            if (String.IsNullOrEmpty(ftpServerIp))
            {
                throw new System.ArgumentNullException(string.Format("FtpHelper(_ftpServerIP=\"{0}\")初始化参数为空。", ftpServerIp ?? "null"));
            }
            if (String.IsNullOrEmpty(ftpUserId))
            {
                throw new System.ArgumentNullException(string.Format("FtpHelper(_ftpUserId=\"{0}\")初始化参数为空。", ftpUserId ?? "null"));
            }
            if (String.IsNullOrEmpty(ftpPassword))
            {
                throw new System.ArgumentNullException(string.Format("FtpHelper(_ftpPassword=\"{0}\")初始化参数为空。", ftpPassword ?? "null"));
            }

            this._ftpServerIp = ftpServerIp;
            this._ftpUserId = ftpUserId;
            this._ftpPassword = ftpPassword;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ftpConn"></param>
        public FtpHelper(FtpConnection ftpConn)
        {
            if (ftpConn == null || String.IsNullOrEmpty(ftpConn.ToString()))
            {
                throw new System.ArgumentNullException("ftpConn");
            }
            
            this._ftpServerIp = ftpConn.ServerIp;
            this._ftpUserId = ftpConn.UserName;
            this._ftpPassword = ftpConn.Password;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 连接FTP
        /// </summary>
        /// <param name="uri"></param>
        private void Connect(string uri)
        {
            System.GC.Collect();
            // 根据uri创建FtpWebRequest对象
            this._ftpRequest = (FtpWebRequest)WebRequest.Create(new Uri(uri));
            // 指定数据传输类型
            this._ftpRequest.UseBinary = true;
            this._ftpRequest.KeepAlive = false; //指定连接是应该关闭还是在请求完成之后关闭，默认为true
            // ftp用户名和密码
            this._ftpRequest.Credentials = new NetworkCredential(this._ftpUserId, this._ftpPassword);
            //this._ftpRequest.ReadWriteTimeout = 10 * 1000;
            //this._ftpRequest.Timeout = 10 * 1000;
        }

        /// <summary>
        /// 从ftp服务器上获得文件列表
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="method">发送到FTP服务器的命令</param>
        /// <returns></returns>
        private string[] GetFileList(string uri, string method)//上面的代码示例了如何从ftp服务器上获得文件列表
        {
            StringBuilder result = new StringBuilder();
            WebResponse response = null;
            try
            {
                this.Connect(uri);
                this._ftpRequest.Method = method;
                using (response = this._ftpRequest.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8)) //中文文件名
                        {
                            string line = reader.ReadLine();
                            while (line != null)
                            {
                                result.AppendLine(line);
                                line = reader.ReadLine();
                            }
                        }
                    }
                }

                string strResult = result.ToString() ?? "";
                return strResult.Split(new char[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            }
            catch (Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show(ex.Message);
                //LogHelper.WriteLog("GetFileList从ftp服务器上获得文件列表失败", ex);
                throw new Exception(string.Format("GetFileList(uri=\"{0}\")从ftp服务器上获得文件列表失败。", uri), ex);
                //return null;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
                if (this._ftpRequest != null)
                {
                    this._ftpRequest.Abort();
                    this._ftpRequest = null;
                }
            }
        }

        #endregion

        #region 公有方法
        /// <summary>
        /// 获取FTP服务器上的文件(不包括目录)的简短列表
        /// </summary>
        /// <param name="ftpFilePath">FTP服务器上绝对路径</param>
        /// <returns></returns>
        public string[] GetFileList(string ftpFilePath = "")
        {
            string uri = this.ToFtpUri(ftpFilePath);
            return this.GetFileList(uri, WebRequestMethods.Ftp.ListDirectory);
        }

        /// <summary>
        /// 获取FTP服务器上的文件和目录的详细列表，目录或文件的名称中不能包含空格
        /// </summary>
        /// <param name="ftpFilePath">FTP服务器上绝对路径</param>
        /// <returns></returns>
        public List<FileStruct> GetDetailFileList(string ftpFilePath = "")
        {
            string uri = this.ToFtpUri(ftpFilePath);
            List<FileStruct> fileList = new List<FileStruct>();
            StringBuilder result = new StringBuilder();
            WebResponse response = null;
            try
            {
                this.Connect(uri);
                this._ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                using (response = this._ftpRequest.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8)) //中文文件名
                        {
                            string line = reader.ReadLine();
                            while (line != null)
                            {
                                result.AppendLine(line);
                                line = reader.ReadLine();
                            }
                        }
                    }
                }

                string str = result.ToString() ?? "";
                string[] files = str.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                EFileListFormat format = this.JudgeFileListFormat(files);
                if (!string.IsNullOrEmpty(str) && format != EFileListFormat.Unknown)
                {
                    fileList = this.ParseFileStruct(files, format);
                }
                return fileList;
            }
            catch (Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show(ex.Message);
                //LogHelper.WriteLog("GetFileList从ftp服务器上获得文件列表失败", ex);
                throw new Exception(string.Format("GetFileList(uri=\"{0}\")从ftp服务器上获得文件列表失败。", uri), ex);
                //return null;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
                if (this._ftpRequest != null)
                {
                    this._ftpRequest.Abort();
                    this._ftpRequest = null;
                }
            }
        }

        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="ftpFileFullName">文件在FTP服务器上的绝对路径</param>
        /// <returns></returns>
        public bool IsExists(string ftpFileFullName)
        {
            bool exists = true;
            if (String.IsNullOrEmpty(ftpFileFullName))
            {
                exists = false;
                return exists;
            }
            //string fileName = Path.GetFileName(ftpFileFullName);
            string tempPath = ftpFileFullName;
            //if (Path.IsPathRooted(tempPath))
            //{
            //    tempPath = tempPath.Remove(0, Path.GetPathRoot(tempPath).Length);
            //}
            int index = tempPath.IndexOf(@":\", StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
            {
                tempPath = tempPath.Remove(0, index + 2);
            }
            string[] tempPaths = tempPath.Split(new char[] {'\\', '/'}, StringSplitOptions.RemoveEmptyEntries);
            tempPath = "";
            foreach (string path in tempPaths)
            {
                List<FileStruct> fileList = this.GetDetailFileList(tempPath);
                if (fileList.All(f => !String.Equals(f.Name, path, StringComparison.OrdinalIgnoreCase)))
                {
                    exists = false;
                    break;
                }
                tempPath += "\\" + path;
            }

            return exists;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="localFileFullName">上传本地文件的绝对路径</param>
        /// <param name="ftpFileFullName">保存到FTP服务器上的绝对路径</param>
        /// <returns></returns>
        public bool Upload(string localFileFullName, string ftpFileFullName)
        {
            bool isSuccess = false;
            if (String.IsNullOrEmpty(localFileFullName))
            {
                throw new System.ArgumentNullException(string.Format("Upload(localFileFullName=\"{0}\")上传文件参数为空。", localFileFullName ?? "null"));
                //return isSuccess;
            }
            if (String.IsNullOrEmpty(ftpFileFullName))
            {
                throw new System.ArgumentNullException(string.Format("Upload(ftpFileFullName=\"{0}\")上传文件参数为空。", ftpFileFullName ?? "null"));
                //return isSuccess;
            }

            try
            {
                FileInfo fileInfo = new FileInfo(localFileFullName);
                if (fileInfo.Exists)
                {
                    string dir = this.MakeAllDirectory(ftpFileFullName);
                    string uri = "ftp://" + this._ftpServerIp + "/" + dir + "/" + Path.GetFileName(ftpFileFullName);
                    this.Connect(uri); //连接
                    this._ftpRequest.KeepAlive = false;
                    // 指定执行什么命令
                    this._ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                    // 上传文件时通知服务器文件的大小
                    this._ftpRequest.ContentLength = fileInfo.Length;
                    // 缓冲大小设置为kb
                    int buffLength = 102400;
                    byte[] buff = new byte[buffLength];
                    int contentLen;

                    // 打开一个文件流(System.IO.FileStream) 去读上传的文件
                    using (FileStream fs = fileInfo.OpenRead())
                    {
                        // 每次读文件流的kb
                        contentLen = fs.Read(buff, 0, buffLength);

                        // 把上传的文件写入流
                        using (Stream stream = this._ftpRequest.GetRequestStream())
                        {
                            // 流内容没有结束
                            while (contentLen != 0)
                            {
                                // 把内容从file stream 写入upload stream
                                stream.Write(buff, 0, contentLen);
                                contentLen = fs.Read(buff, 0, buffLength);
                            }
                        }
                    }

                    isSuccess = true;
                }
                else
                {
                    isSuccess = false;
                    throw new FileNotFoundException(string.Format("Upload(localFileFullName=\"{0}\")要上传的本地文件不存在。", localFileFullName));
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                //MessageBox.Show(ex.Message, "Upload Error");
                //LogHelper.WriteLog("Upload上传文件[" + localFileFullName + "]失败", ex);
                throw new Exception(string.Format("Upload(ftpFileFullName=\"{0}\",localFileFullName=\"{1}\")上传文件失败。", ftpFileFullName, localFileFullName), ex);
            }
            finally
            {
                if (this._ftpRequest != null)
                {
                    this._ftpRequest.Abort();
                    this._ftpRequest = null;
                }
            }
            return isSuccess;
        }

        /// <summary>
        /// 下载文件，如果本地已存在则覆盖
        /// </summary>
        /// <param name="ftpFileFullName">文件在FTP服务器上的绝对路径</param>
        /// <param name="localFileFullName">保存到本地的绝对路径</param>
        /// <returns></returns>
        public bool Download(string ftpFileFullName, string localFileFullName)
        {
            bool isSuccess = false;
            try
            {
                if (String.IsNullOrEmpty(ftpFileFullName))
                {
                    throw new System.ArgumentNullException(string.Format("Download(ftpFileFullName=\"{0}\")下载参数为空。", ftpFileFullName ?? "null"));
                    //return isSuccess;
                }
                if (String.IsNullOrEmpty(localFileFullName))
                {
                    throw new System.ArgumentNullException(string.Format("Download(localFileFullName=\"{0}\")下载参数为空。", localFileFullName ?? "null"));
                    //return isSuccess;
                }

                string localPath = Path.GetDirectoryName(localFileFullName) ?? "";
                if (!Directory.Exists(localPath))
                {
                    Directory.CreateDirectory(localPath);
                }

                string uri = this.ToFtpUri(ftpFileFullName);
                this.Connect(uri); //连接  
                this._ftpRequest.Credentials = new NetworkCredential(this._ftpUserId, this._ftpPassword);
                using (FtpWebResponse response = (FtpWebResponse) this._ftpRequest.GetResponse())
                {
                    using (Stream ftpStream = response.GetResponseStream())
                    {
                        //long cl = response.ContentLength;
                        int bufferSize = 102400;
                        int readCount;
                        byte[] buffer = new byte[bufferSize];
                        readCount = ftpStream.Read(buffer, 0, bufferSize);
                        using (FileStream outputStream = new FileStream(localFileFullName, FileMode.Create))
                        {
                            while (readCount > 0)
                            {
                                outputStream.Write(buffer, 0, readCount);
                                readCount = ftpStream.Read(buffer, 0, bufferSize);
                            }
                        }
                    }
                }
                isSuccess = true;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                //LogHelper.WriteLog("Download下载文件[" + ftpFileFullName + "]失败", ex);
                throw new Exception(string.Format("Download(ftpFileFullName=\"{0}\",localFileFullName=\"{1}\")下载文件失败。", ftpFileFullName, localFileFullName), ex);
            }
            finally
            {
                if (this._ftpRequest != null)
                {
                    this._ftpRequest.Abort();
                    this._ftpRequest = null;
                }
            }
            return isSuccess;
        }

        /// <summary>
        /// 删除FTP服务器上的文件
        /// </summary>
        /// <param name="ftpFileFullName">文件在FTP服务器上的绝对路径</param>
        /// <returns></returns>
        public bool DeleteFile(string ftpFileFullName)
        {
            if (String.IsNullOrEmpty(ftpFileFullName))
            {
                throw new ArgumentNullException(string.Format("DeleteFile(ftpFileFullName=\"{0}\")参数异常。", ftpFileFullName ?? "null"));
                //return false;
            }
            bool isSuccess = false;
            try
            {
                string uri = this.ToFtpUri(ftpFileFullName);
                this.Connect(uri); //连接 
                // 指定执行什么命令
                this._ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                using (FtpWebResponse response = (FtpWebResponse) this._ftpRequest.GetResponse())
                {
                }

                isSuccess = true;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                //MessageBox.Show(ex.Message, "删除错误");
                //LogHelper.WriteLog("DeleteFile删除文件[" + fileFullName + "]失败", ex);
                throw new Exception(string.Format("DeleteFile(ftpFileFullName=\"{0}\")删除文件失败。", ftpFileFullName), ex);
            }
            finally
            {
                if (this._ftpRequest != null)
                {
                    this._ftpRequest.Abort();
                    this._ftpRequest = null;
                }
            }

            return isSuccess;
        }

        /// <summary>
        /// 获取FTP服务器上文件的大小
        /// </summary>
        /// <param name="ftpFileFullName">文件在FTP服务器上的绝对路径</param>
        /// <returns></returns>
        public long GetFileSize(string ftpFileFullName)
        {
            if (String.IsNullOrEmpty(ftpFileFullName))
            {
                throw new ArgumentNullException(string.Format("GetFileSize(ftpFileFullName=\"{0}\")参数异常。", ftpFileFullName ?? "null"));
                //return -1;
            }
            long fileSize = 0;
            try
            {
                string uri = this.ToFtpUri(ftpFileFullName);
                this.Connect(uri);//连接 
                this._ftpRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                using (FtpWebResponse response = (FtpWebResponse)this._ftpRequest.GetResponse())
                {
                    fileSize = response.ContentLength;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                //LogHelper.WriteLog("GetFileSize获取文件[" + fileFullName + "]大小失败", ex);
                fileSize = -1;
                throw new Exception(string.Format("GetFileSize(ftpFileFullName=\"{0}\")获取文件大小失败。", ftpFileFullName), ex);
            }
            finally
            {
                if (this._ftpRequest != null)
                {
                    this._ftpRequest.Abort();
                    this._ftpRequest = null;
                }
            }

            return fileSize;
        }

        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="oldFileFullName">文件在FTP服务器上的原文件名</param>
        /// <param name="newFileName">新文件名</param>
        /// <returns></returns>
        public bool Rename(string oldFileFullName, string newFileName)
        {
            bool isSuccess = false;
            if (String.IsNullOrEmpty(oldFileFullName))
            {
                throw new ArgumentNullException(string.Format("Rename(oldFileFullName=\"{0}\")重命名参数异常。", oldFileFullName ?? "null"));
                //return isSuccess;
            }
            if (String.IsNullOrEmpty(newFileName))
            {
                throw new ArgumentNullException(string.Format("Rename(newFileName=\"{0}\")重命名参数异常。", newFileName ?? "null"));
                //return isSuccess;
            }

            try
            {
                string uri = this.ToFtpUri(oldFileFullName);
                this.Connect(uri);//连接
                this._ftpRequest.Method = WebRequestMethods.Ftp.Rename;
                this._ftpRequest.RenameTo = newFileName;
                using (FtpWebResponse response = (FtpWebResponse)this._ftpRequest.GetResponse()) { }
                isSuccess = true;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                //MessageBox.Show(ex.Message);
                //LogHelper.WriteLog(string.Format("Rename重命名失败。oldFileFullName='{0}'，newFileName='{1}'", oldFileFullName, newFileName), ex);
                throw new Exception(string.Format("Rename(oldFileFullName=\"{0}\",newFileName=\"{1}\")重命名失败。", oldFileFullName, newFileName), ex);
            }
            finally
            {
                if (this._ftpRequest != null)
                {
                    this._ftpRequest.Abort();
                    this._ftpRequest = null;
                }
            }

            return isSuccess;
        }

        /// <summary>
        /// 在FTP服务器上创建一级目录
        /// </summary>
        /// <param name="dirName">目录名称</param>
        /// <returns></returns>
        public bool MakeDirectory(string dirName)
        {
            bool isSuccess = false;
            if (String.IsNullOrEmpty(dirName))
            {
                throw new ArgumentNullException(string.Format("MakeDirectory(dirName=\"{0}\")参数异常。", dirName ?? "null"));
                //return isSuccess;
            }

            try
            {
                dirName = dirName.Trim('/', '\\');
                string uri = this.ToFtpUri(dirName);
                this.Connect(uri);//连接 
                this._ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                using (FtpWebResponse response = (FtpWebResponse)this._ftpRequest.GetResponse()) { }
                isSuccess = true;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                //MessageBox.Show(ex.Message);
                //LogHelper.WriteLog("MakeDirectory创建目录[" + dirName + "]失败", ex);
                throw new Exception(string.Format("MakeDirectory(dirName=\"{0}\")创建目录失败。", dirName), ex);
            }
            finally
            {
                if (this._ftpRequest != null)
                {
                    this._ftpRequest.Abort();
                    this._ftpRequest = null;
                }
            }

            return isSuccess;
        }

        /// <summary>
        /// 在FTP服务器上移除最后一级目录
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns></returns>
        public bool RemoveDirectory(string dirName)
        {
            bool isSuccess = false;
            if (String.IsNullOrEmpty(dirName))
            {
                throw new ArgumentNullException(string.Format("RemoveDirectory(dirName=\"{0}\")参数异常。", dirName ?? "null"));
                //return isSuccess;
            }
            try
            {
                string uri = this.ToFtpUri(dirName);
                this.Connect(uri);//连接
                this._ftpRequest.Method = WebRequestMethods.Ftp.RemoveDirectory;
                using (FtpWebResponse response = (FtpWebResponse)this._ftpRequest.GetResponse()) { }
                isSuccess = true;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                //MessageBox.Show(ex.Message);
                //LogHelper.WriteLog("RemoveDirectory删除目录[" + dirName + "]失败", ex);
                throw new Exception(string.Format("RemoveDirectory(dirName=\"{0}\")删除目录失败。", dirName), ex);
            }
            finally
            {
                if (this._ftpRequest != null)
                {
                    this._ftpRequest.Abort();
                    this._ftpRequest = null;
                }
            }

            return isSuccess;
        }

        /// <summary>
        /// 在FTP服务器上创建多级目录
        /// <para>1\2\3\4\5\6</para>
        /// </summary>
        /// <param name="ftpFilePath"></param>
        /// <returns></returns>
        public string MakeAllDirectory(string ftpFilePath)
        {
            // 1/2/3/4/5/6
            string retDir = "";
            if (String.IsNullOrEmpty(ftpFilePath))
            {
                return retDir;
            }

            string strTemp = ftpFilePath;
            if (strTemp.EndsWith("\\") || Path.HasExtension(strTemp))
            {
                strTemp = Path.GetDirectoryName(strTemp);
            }
            int index = strTemp.IndexOf(@":\", StringComparison.OrdinalIgnoreCase);
            if (index > -1)
            {
                strTemp = strTemp.Remove(0, index + 2);
            }
            strTemp = strTemp.Trim('\\');

            try
            {
                string[] alldirs = strTemp.Split(new char[] {'\\', '/'}, StringSplitOptions.RemoveEmptyEntries);
                foreach (string dir in alldirs)
                {
                    if (!dir.Trim().Equals(""))
                    {
                        retDir += "/" + dir;
                        if (!this.IsExists(retDir))
                        {
                            this.MakeDirectory(retDir);
                        }
                    }
                }
                retDir = retDir.Trim('/', ' ');
            }
            catch (Exception ex)
            {
                retDir = "";
                //LogHelper.WriteLog("MakeAllDirectory创建目录[" + filePath + "]失败", ex);
                throw new Exception(string.Format("MakeAllDirectory(ftpFilePath=\"{0}\")创建多级目录失败。", ftpFilePath), ex);
            }

            return retDir;
        }

        /// <summary>
        /// 转换为FTP路径，例如："C:\temp\DirectoryName\file.txt" => "ftp://FtpServerIp/temp/DirectoryName/file.txt"
        /// </summary>
        /// <param name="ftpFileFullName">FTP服务器上的文件绝对路径</param>
        /// <returns></returns>
        public string ToFtpUri(string ftpFileFullName)
        {
            string uri = string.Format("ftp://{0}/", this._ftpServerIp);
            if (String.IsNullOrEmpty(ftpFileFullName))
            {
                return uri;
            }

            try
            {
                string temp = ftpFileFullName;
                int index = temp.IndexOf(@":\", StringComparison.OrdinalIgnoreCase);
                if (index >= 0)
                {
                    temp = temp.Remove(0, index + 2);
                }
                uri += temp.Replace('\\', '/').Trim('/');
            }
            catch (Exception ex)
            {
                uri = "";
                //LogHelper.WriteLog("FilePathToFtpUriPart本地路径[" + fileFullName + "]转换为FTP路径时失败", ex);
                throw new Exception(string.Format("ToFtpUri(ftpFileFullName=\"{0}\")转换为FTP路径失败。", ftpFileFullName), ex);
            }

            return uri;
        }
        #endregion

        #region 解析文件详细列表
        /// <summary>
        /// 解析文件列表信息返回文件列表，目录或文件的名称中不能包含空格
        /// </summary>
        /// <param name="fileList"></param>
        /// <param name="format">文件列表格式</param>
        /// <returns></returns>
        private List<FileStruct> ParseFileStruct(string[] fileList, EFileListFormat format)
        {
            List<FileStruct> list = new List<FileStruct>();
            if (format == EFileListFormat.UnixFormat)
            {
                foreach (string info in fileList)
                {
                    FileStruct fstuct = new FileStruct();
                    fstuct.Origin = info.Trim();
                    fstuct.OriginArr = fstuct.Origin.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (fstuct.OriginArr.Length == 9)
                    {
                        fstuct.Flags = fstuct.OriginArr[0];
                        fstuct.IsDirectory = (fstuct.Flags[0] == 'd');
                        fstuct.Owner = fstuct.OriginArr[2];
                        fstuct.Group = fstuct.OriginArr[3];
                        fstuct.Size = Common.MyTransform.StringToLong(fstuct.OriginArr[4], 0);
                        if (fstuct.OriginArr[7].Contains(":"))
                        {
                            fstuct.OriginArr[7] = DateTime.Now.Year + " " + fstuct.OriginArr[7];
                        }
                        fstuct.UpdateTime = DateTime.Parse(string.Format("{0} {1} {2}", fstuct.OriginArr[5], fstuct.OriginArr[6], fstuct.OriginArr[7]));
                        fstuct.Name = fstuct.OriginArr[8];
                        if (fstuct.Name != "." && fstuct.Name != "..")
                        {
                            list.Add(fstuct);
                        }
                    }

                }
            }
            else if (format == EFileListFormat.WindowsFormat)
            {
                foreach (string info in fileList)
                {
                    FileStruct fstuct = new FileStruct();
                    fstuct.Origin = info.Trim();
                    fstuct.OriginArr = fstuct.Origin.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (fstuct.OriginArr.Length == 4)
                    {
                        DateTimeFormatInfo usDate = new CultureInfo("en-US", false).DateTimeFormat;
                        usDate.ShortTimePattern = "t";
                        fstuct.UpdateTime = DateTime.Parse(fstuct.OriginArr[0] + " " + fstuct.OriginArr[1], usDate);

                        fstuct.IsDirectory = (fstuct.OriginArr[2] == "<DIR>");
                        if (!fstuct.IsDirectory)
                        {
                            fstuct.Size = Common.MyTransform.StringToLong(fstuct.OriginArr[2], 0);
                        }
                        fstuct.Name = fstuct.OriginArr[3];
                        if (fstuct.Name != "." && fstuct.Name != "..")
                        {
                            list.Add(fstuct);
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 判断文件列表的方式Window方式还是Unix方式
        /// </summary>
        /// <param name="fileList">文件信息列表</param>
        /// <returns></returns>
        private EFileListFormat JudgeFileListFormat(string[] fileList)
        {
            foreach (string str in fileList)
            {
                if (str.Length > 10 && Regex.IsMatch(str.Substring(0, 10), "(-|d)(-|r)(-|w)(-|x)(-|r)(-|w)(-|x)(-|r)(-|w)(-|x)"))
                {
                    return EFileListFormat.UnixFormat;
                }
                else if (str.Length > 8 && Regex.IsMatch(str.Substring(0, 8), "[0-9][0-9]-[0-9][0-9]-[0-9][0-9]"))
                {
                    return EFileListFormat.WindowsFormat;
                }
            }
            return EFileListFormat.Unknown;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Record"></param>
        /// <returns></returns>
        private FileStruct ParseFileStructFromWindowsStyleRecord(string Record)
        {
            FileStruct f = new FileStruct();
            string processstr = Record.Trim();
            string dateStr = processstr.Substring(0, 8);
            processstr = (processstr.Substring(8, processstr.Length - 8)).Trim();
            string timeStr = processstr.Substring(0, 7);
            processstr = (processstr.Substring(7, processstr.Length - 7)).Trim();
            DateTimeFormatInfo myDTFI = new CultureInfo("en-US", false).DateTimeFormat;
            myDTFI.ShortTimePattern = "t";
            f.UpdateTime = DateTime.Parse(dateStr + " " + timeStr, myDTFI);
            if (processstr.Substring(0, 5) == "<DIR>")
            {
                f.IsDirectory = true;
                processstr = (processstr.Substring(5, processstr.Length - 5)).Trim();
            }
            else
            {
                string[] strs = processstr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);   // true);
                processstr = strs[1];
                f.IsDirectory = false;
            }
            f.Name = processstr;
            return f;
        }


        /// <summary>
        /// 文件列表格式
        /// </summary>
        public enum EFileListFormat
        {
            /// <summary>
            /// Unix文件格式
            /// </summary>
            UnixFormat,
            /// <summary>
            /// Window文件格式
            /// </summary>
            WindowsFormat,
            /// <summary>
            /// 未知格式
            /// </summary>
            Unknown
        }

        /// <summary>
        /// 文件结构
        /// </summary>
        public struct FileStruct
        {
            /// <summary>
            /// 
            /// </summary>
            public string Origin { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string[] OriginArr { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Flags { get; set; }

            /// <summary>
            /// 所有者
            /// </summary>
            public string Owner { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Group { get; set; }

            /// <summary>
            /// 是否为目录
            /// </summary>
            public bool IsDirectory { get; set; }

            /// <summary>
            /// 文件或目录更新时间
            /// </summary>
            public DateTime UpdateTime { get; set; }

            /// <summary>
            /// 文件或目录名称
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 文件大小(目录始终为0)
            /// </summary>
            public long Size { get; set; }
        }
        #endregion
    }

    /// <summary>
    /// FTP服务器
    /// </summary>
    [Serializable]
    public class FtpConnection
    {
        /// <summary>
        /// 服务器IP
        /// </summary>
        public string ServerIp { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public FtpConnection()
        {
            this.ServerIp = "";
            this.UserName = "";
            this.Password = "";
        }

        /// <summary>
        /// ServerIp;UserName;Password
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string strConn = "";
            if (!String.IsNullOrEmpty(this.ServerIp)
                && !String.IsNullOrEmpty(this.UserName)
                && !String.IsNullOrEmpty(this.Password))
            {
                strConn = string.Format("{0};{1};{2}", this.ServerIp, this.UserName, this.Password);
            }
            return strConn;
        }
    }
}
