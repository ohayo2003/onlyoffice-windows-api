using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace Common
{
    /// <summary>
    /// FTP操作
    /// </summary>
    public class FtpClass
    {
        string ftpServerIP;

        string ftpUserID;

        string ftpPassword;

        FtpWebRequest reqFTP;

        private void Connect(String path)//连接ftp
        {

            // 根据uri创建FtpWebRequest对象

            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(path));

            // 指定数据传输类型

            reqFTP.UseBinary = true;

            // ftp用户名和密码

            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);

            reqFTP.ReadWriteTimeout = 5 * 1000;

            reqFTP.Timeout = 5 * 1000;

        }

        public FtpClass(string ftpServerIP, string ftpUserID, string ftpPassword)
        {

            this.ftpServerIP = ftpServerIP;

            this.ftpUserID = ftpUserID;

            this.ftpPassword = ftpPassword;
        }
        //都调用这个
        private string[] GetFileList(string path, string WRMethods)//上面的代码示例了如何从ftp服务器上获得文件列表
        {
            string[] downloadFiles;

            StringBuilder result = new StringBuilder(); 

            try
            {
                Connect(path);

                reqFTP.Method = WRMethods;

                WebResponse response = reqFTP.GetResponse();

                StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.Default);//中文文件名

                string line = reader.ReadLine();

                while (line != null)
                {
                    result.Append(line);

                    result.Append(" ");

                    line = reader.ReadLine();
                }

                // to remove the trailing '' ''

                result.Remove(result.ToString().LastIndexOf(" "), 1);

                reader.Close();

                response.Close();

                return result.ToString().Split(' ');
            }
            catch
            {
                //System.Windows.Forms.MessageBox.Show(ex.Message);

                downloadFiles = null;

                return downloadFiles;
            }
        }


        private bool CanConnFtp()
        {
            bool fg = true;
            FtpWebRequest req = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP));
            req.Method = WebRequestMethods.Ftp.ListDirectory;
            req.Timeout = 2 * 1000;//超时时间设置为2秒。
            try
            {
                req.GetResponse();
            }
            catch
            {
                fg = false;
            }

            return fg;
        }


        /// <summary>
        /// 是否可以连接上Ftp
        /// </summary>
        /// <returns></returns>
        public bool IsCanConnectFtp()
        {
            return CanConnFtp();
        }


        public string[] GetFileList(string path)//上面的代码示例了如何从ftp服务器上获得文件列表
        {
            return GetFileList("ftp://" + ftpServerIP + "/" + path, WebRequestMethods.Ftp.ListDirectory);
        }

 

        public string[] GetFileList()//上面的代码示例了如何从ftp服务器上获得文件列表
{
            return GetFileList("ftp://" + ftpServerIP + "/", WebRequestMethods.Ftp.ListDirectory);
        }



        public bool Upload(string filename) //上面的代码实现了从ftp服务器上载文件的功能
        {
            bool fg = true;

            //filename = "手工录入.doc";

            FileInfo fileInf = new FileInfo(filename);

            if (fileInf.Exists)
            {

                //string uri = "ftp://" + ftpServerIP + "/" + fileInf.Name;
                string str_Dir = MakeAlldir(filename);

                string uri = "ftp://" + ftpServerIP + "/" + str_Dir + fileInf.Name;

                //if (CanConnFtp())
                //{
                Connect(uri);//连接     

                // 默认为true，连接不会被关闭

                // 在一个命令之后被执行
                reqFTP.KeepAlive = false;

                // 指定执行什么命令
                reqFTP.Method = WebRequestMethods.Ftp.UploadFile;

                // 上传文件时通知服务器文件的大小
                reqFTP.ContentLength = fileInf.Length;

                // 缓冲大小设置为kb
                int buffLength = 2048;

                byte[] buff = new byte[buffLength];

                int contentLen;

                // 打开一个文件流(System.IO.FileStream) 去读上传的文件
                FileStream fs = fileInf.OpenRead();

                try
                {
                    //reqFTP.Timeout = 5 * 1000;
                    // 把上传的文件写入流
                    Stream strm = reqFTP.GetRequestStream();

                    // 每次读文件流的kb
                    contentLen = fs.Read(buff, 0, buffLength);

                    // 流内容没有结束
                    while (contentLen != 0)
                    {
                        // 把内容从file stream 写入upload stream
                        strm.Write(buff, 0, contentLen);

                        contentLen = fs.Read(buff, 0, buffLength);
                    }

                    // 关闭两个流

                    strm.Close();

                    fs.Close();
                }
                catch(Exception ex)
                {
                    //MessageBox.Show(ex.Message, "Upload Error");
                    fg = false;
                }
            }
            else
            {
                fg = false;
            }

            return fg;
        } 
        /// <summary>
        /// 从ftp服务器下载文件的功能
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <param name="errorinfo"></param>
        /// <returns></returns>
        public bool Download(string FTPfilePath, string NewfileFullName, out string errorinfo)
        {
            try
            {
                //String onlyFileName = Path.GetFileName(fileName);

                //string newFileName = filePath + "\\" + onlyFileName;

                if (File.Exists(NewfileFullName))
                {
                    errorinfo = "本地文件{0}已存在,无法下载";

                    return false;
                }

                MakeFtpDownDir(NewfileFullName);

                String onlyFileName = Path.GetFileName(FTPfilePath);

                string FtpDir = GetFtpDownDir(FTPfilePath);


                string url = "ftp://" + ftpServerIP + "/" + FtpDir + "/" + onlyFileName;

                Connect(url);//连接  

                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword); 

                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                
                Stream ftpStream = response.GetResponseStream();

                 long cl = response.ContentLength;

                 int bufferSize = 2048;
                
                int readCount;
                
                byte[] buffer = new byte[bufferSize]; 

                readCount = ftpStream.Read(buffer, 0, bufferSize);

                FileStream outputStream = new FileStream(NewfileFullName, FileMode.Create); 

                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount); 

                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }
                
                ftpStream.Close();
                
                outputStream.Close();
                
                response.Close();

                errorinfo = "";

                return true;
            }
            catch (Exception ex)
            {
                errorinfo = string.Format("因{0},无法下载", ex.Message);

                return false;
            }
        }

        public bool DownloadFile(string FTPfilePath, string NewfileFullName, out string errorinfo)
        {
            try
            {
                //String onlyFileName = Path.GetFileName(fileName);

                //string newFileName = filePath + "\\" + onlyFileName;

                if (File.Exists(NewfileFullName))
                {
                    errorinfo = "本地文件{0}已存在,无法下载";

                    return false;
                }

                MakeFtpDownDir(NewfileFullName);

                String onlyFileName = Path.GetFileName(FTPfilePath);

                string FtpDir = MakeAlldir(FTPfilePath); 


                string url = "ftp://" + ftpServerIP + "/" + FtpDir  + onlyFileName;

                Connect(url);//连接  

                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);

                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();

                Stream ftpStream = response.GetResponseStream();

                long cl = response.ContentLength;

                int bufferSize = 2048;

                int readCount;

                byte[] buffer = new byte[bufferSize];

                readCount = ftpStream.Read(buffer, 0, bufferSize);

                FileStream outputStream = new FileStream(NewfileFullName, FileMode.Create);

                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);

                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }

                ftpStream.Close();

                outputStream.Close();

                response.Close();

                errorinfo = "";

                return true;
            }
            catch (Exception ex)
            {
                errorinfo = string.Format("因{0},无法下载", ex.Message);

                return false;
            }
        }


        //删除文件
        public bool DeleteFileName(string fileName)
        {
            bool fg = true;
            try
            {
                FileInfo fileInf = new FileInfo(fileName);

                string str_Dir = MakeAlldir(fileName);

                string uri = "ftp://" + ftpServerIP + "/" + str_Dir + fileInf.Name;

                //string uri = "ftp://" + ftpServerIP + "/" + fileInf.Name;

                Connect(uri);//连接         

                // 默认为true，连接不会被关闭

                // 在一个命令之后被执行
                reqFTP.KeepAlive = false;



                // 指定执行什么命令
                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;

                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();

                response.Close();
            }
            catch
            {
                fg = false;
                //MessageBox.Show(ex.Message, "删除错误");
            }

            return fg;
        }

        //创建目录
        public bool MakeDir(string dirName)
        {
            bool fg = true;

            try
            {
                string uri = "ftp://" + ftpServerIP + "/" + dirName;

                Connect(uri);//连接      

                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;

                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();

                response.Close();
            }
            catch
            {
                fg = false;
                //MessageBox.Show(ex.Message);
            }

            return fg;
        }

        //删除目录

        public bool delDir(string dirName)
        {
            bool fg = true; 

            try
            {
                string uri = "ftp://" + ftpServerIP + "/" + dirName;

                Connect(uri);//连接      

                reqFTP.Method = WebRequestMethods.Ftp.RemoveDirectory;

                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();

                response.Close();
            }
            catch
            {
                fg = false;
                //MessageBox.Show(ex.Message);
            }

            return fg;
        }

        //获得文件大小
        public long GetFileSize(string filename)
        {
            long fileSize = 0;

            try
            {
                FileInfo fileInf = new FileInfo(filename);

                string uri = "ftp://" + ftpServerIP + "/" + fileInf.Name;

                Connect(uri);//连接      

                reqFTP.Method = WebRequestMethods.Ftp.GetFileSize;

                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();

                fileSize = response.ContentLength;

                response.Close();
            }
            catch
            {
                //MessageBox.Show(ex.Message);
            }

            return fileSize;
        }

        //文件改名
        public bool Rename(string currentFilename, string newFilename)
        {
            bool fg = true; 

            try

            {

                FileInfo fileInf = new FileInfo(currentFilename);

                string uri = "ftp://" + ftpServerIP + "/" + fileInf.Name;

                if (CanConnFtp())
                {
                    Connect(uri);//连接

                    reqFTP.Method = WebRequestMethods.Ftp.Rename;

                    reqFTP.RenameTo = newFilename;
                    
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();

                    //Stream ftpStream = response.GetResponseStream();

                    //ftpStream.Close();

                    response.Close();
                }
                else
                {
                    fg = false;
                }

            }

            catch
            {
                fg = false;
                //MessageBox.Show(ex.Message);

            }

            return fg;
        }

        //获得文件明晰
        public string[] GetFilesDetailList()
        {
            return GetFileList("ftp://" + ftpServerIP + "/", WebRequestMethods.Ftp.ListDirectoryDetails);
        }

        //获得文件明晰
        public string[] GetFilesDetailList(string path)
        {
            return GetFileList("ftp://" + ftpServerIP + "/" + path, WebRequestMethods.Ftp.ListDirectoryDetails);
        }



        public string MakeAlldir(string str_dir)
        {
            // 1/2/3/4/5/6
            string str_Ndir = "";

            string str_tmp = str_dir;

            if (str_tmp.IndexOf(":") > -1)
            {
                str_tmp = str_tmp.Substring(str_tmp.IndexOf(":") + 1);
            }

            if (str_tmp.LastIndexOf("\\") > -1)
            {
                str_tmp = str_tmp.Substring(0, str_tmp.LastIndexOf("\\"));
            }

            str_tmp = str_tmp.Trim('\\');

            try
            {
                string[] ar_dirs = Regex.Split(str_tmp, "\\\\");


                foreach (string str_sdir in ar_dirs)
                {
                    if (!str_sdir.Trim().Equals(""))
                    {
                        str_Ndir += "/" + str_sdir;
                        str_Ndir = str_Ndir.Trim('/');

                        MakeDir(str_Ndir);

                    }

                }
            }
            catch
            {
                str_Ndir = "";
            }

            if (!str_Ndir.Equals(""))
            {
                str_Ndir = str_Ndir + "/";
            }

            return str_Ndir;
        }

        public bool MakeFtpDownDir(string NewFilePath)
        {

            bool fg = true;

            try
            {
                string str_tmp = NewFilePath;

                if (str_tmp.LastIndexOf("\\") > -1)
                {
                    str_tmp = str_tmp.Substring(0, str_tmp.LastIndexOf("\\"));
                }

                str_tmp = str_tmp.Trim('\\');

                if (!System.IO.Directory.Exists(str_tmp))
                {
                    System.IO.Directory.CreateDirectory(str_tmp);
                }
            }
            catch
            {
                fg = false;
            }

            return fg;

        }

        public string GetFtpDownDir(string FtpPath)
        {

            string str_tmp = FtpPath;
            try
            {
                if (str_tmp.IndexOf(":") > -1)
                {
                    str_tmp = str_tmp.Substring(str_tmp.IndexOf(":") + 1);
                }

                if (str_tmp.LastIndexOf("\\") > -1)
                {
                    str_tmp = str_tmp.Substring(0, str_tmp.LastIndexOf("\\"));
                }
            }
            catch
            {
                str_tmp = "";
            }

            str_tmp = str_tmp.Trim('\\');

            return str_tmp;
        }
    }
}
