using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Common;
using WebApi.Utility;

namespace WebApi.web.Helper
{
    public class LocalFileHelper
    {
        private string _info;

        public string Infomation
        {
            set { _info = value; }
            get { return _info; }
        }


        private string _localdiskdir;

        public LocalFileHelper(string LocalDiskDir)
        {
            _localdiskdir = LocalDiskDir.Trim('\\').Trim();

        }

        /// <summary>
        /// 分块存储扩展名格式{0}当前块{1}总块
        /// </summary>
        private const string ChunkFileExtensionFormat = ".part_{0}.{1}";

        public string GetUploadFileTempLocalDir(string SystemType, string UploadType, string UserID, string FileOriginalName, string guid)
        {
            string TempDir = string.Empty;

            try
            {
                DateTime dt = DateTime.Now;
                //string guid = Guid.NewGuid().ToString();

                if (_localdiskdir.Length > 0)
                {
                    TempDir = _localdiskdir + "\\" + SystemType + "\\" + UploadType + "\\" + dt.Year.ToString() + "\\" + dt.Month.ToString()
                        + "\\" + dt.Day.ToString() + "\\" + dt.Hour.ToString() + "\\" + dt.Minute.ToString() + "\\" + dt.Second.ToString()
                        + "\\" + UserID + "\\" + guid + "\\" + FileOriginalName;
                }
                else
                {
                    this._info = "localdisk is null";
                }

            }
            catch (Exception ex)
            {
                TempDir = string.Empty;
                this._info = ex.Message;
            }

            return TempDir;
        }
        public string GetDownloadCertificateFileLocalDir(string FileOriginalName, string guid)
        {
            string TempDir = string.Empty;

            try
            {
                DateTime dt = DateTime.Now;
                //string guid = Guid.NewGuid().ToString();

                if (_localdiskdir.Length > 0)
                {
                    TempDir = _localdiskdir + "\\" + "CertificateFile" + "\\" + dt.Year.ToString() + "\\" + dt.Month.ToString()
                        + "\\" + dt.Day.ToString() + "\\" + dt.Hour.ToString() + "\\" + dt.Minute.ToString() + "\\" + dt.Second.ToString()
                        + "\\" + guid + "\\" + FileOriginalName;
                }
                else
                {
                    this._info = "localdisk is null";
                }

            }
            catch (Exception ex)
            {
                TempDir = string.Empty;
                this._info = ex.Message;
            }

            return TempDir;
        }


        /// <summary>
        /// //由于商家dname为空，且schoolID为-1 ，需要处理
        /// </summary>
        /// <param name="SchoolID"></param>
        /// <param name="Dname"></param>
        /// <param name="UserInfoID"></param>
        /// <returns></returns>
        public string GetZipFilePath(int SchoolID, string Dname, int UserInfoID)
        {


            string TempDir = string.Empty;

            try
            {
                DateTime dt = DateTime.Now;
                //string guid = Guid.NewGuid().ToString();

                if (_localdiskdir.Length > 0)
                {
                    TempDir = _localdiskdir + "\\" + "ActivityZipWorks" + "\\" +
                        SchoolID.ToString() + "\\" + (string.IsNullOrEmpty(Dname) ? "BM" : Dname)
                              + "\\" + UserInfoID.ToString() + "\\";
                    if (!Directory.Exists(TempDir))
                        Directory.CreateDirectory(TempDir);
                }
                else
                {
                    this._info = "localdisk is null";
                }

            }
            catch (Exception ex)
            {
                TempDir = string.Empty;
                this._info = ex.Message;
            }

            return TempDir;
        }

        public bool SaveFileFromPostFormData(Stream filestream, int chunk, int chunks)
        {
            bool fg = true;

            try
            {
                string tempdir = string.Empty;
                if (_localdiskdir.LastIndexOf("\\") > -1)
                {
                    tempdir = _localdiskdir.Substring(0, _localdiskdir.LastIndexOf("\\"));
                }

                if (!System.IO.Directory.Exists(tempdir))
                {
                    System.IO.Directory.CreateDirectory(tempdir);
                }

                string filepartpath = string.Format(_localdiskdir + ChunkFileExtensionFormat, chunk.ToString(), chunks.ToString());

                using (var fileStream = System.IO.File.Create(filepartpath))
                {
                    filestream.CopyTo(fileStream);
                }

            }
            catch (Exception ex)
            {
                fg = false;
                this._info = ex.Message;
            }

            return fg;
        }

        public bool MergeChunkFiles()
        {
            bool fg = true;

            try
            {
                FilesUtils fu = new FilesUtils();

                string tempfilename = string.Format(_localdiskdir + ChunkFileExtensionFormat, "0", "1");

                fg = fu.MergeFile(tempfilename);


            }
            catch (Exception ex)
            {
                fg = false;
                this._info = ex.Message;
            }

            return fg;
        }

    }
}
