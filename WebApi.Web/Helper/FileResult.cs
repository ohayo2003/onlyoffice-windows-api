using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Formatting;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace WebApi.web.Helper
{
    public class FileResult : IHttpActionResult
    {
        private string _filePath;
        private string _fileName;
        HttpRequestMessage _request;

        public FileResult(string filePath, string fileName, HttpRequestMessage request)
        {
            _filePath = filePath;
            _fileName = fileName;
            _request = request;
        }


        public Task<HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
        {

            MediaTypeHeaderValue _mediaType = MediaTypeHeaderValue.Parse("application/octet-stream");//指定文件类型
            //ContentDispositionHeaderValue _disposition = ContentDispositionHeaderValue.Parse("attachment;filename=\""
            //    + ConvertFileName(_fileName, _request.Headers.UserAgent.First().ToString()) + "\"");//指定文件名称（编码中文）

            string agent = _request.Headers.UserAgent.Count > 0 ? _request.Headers.UserAgent.First().ToString() : _request.Headers.UserAgent.ToString();

            ContentDispositionHeaderValue _disposition = ContentDispositionHeaderValue.Parse("attachment;filename=\""
                + ConvertFileName(_fileName, agent) + "\"");//指定文件名称（编码中文）


            try
            {

                if (!File.Exists(_filePath))
                    throw new HttpResponseException(HttpStatusCode.NotFound);

                FileStream fileStream = new FileStream(_filePath, FileMode.Open);
                var resp = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(fileStream),
                    RequestMessage = _request
                };

                resp.Content.Headers.ContentType = _mediaType;
                resp.Content.Headers.ContentDisposition = _disposition;
                return Task.FromResult(resp);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            finally
            {

            }
        }

        /// <summary>
        /// 修正firefox导出excel名字乱码的问题
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="agent"></param>
        /// <returns></returns>
        public string ConvertFileName(string fileName, string agent)
        {
            if (Regex.IsMatch(agent.ToLower(), "firefox"))
            {
                return fileName;
            }
            return Uri.EscapeDataString(fileName);
        }



    }



    public class FileResult2 : IHttpActionResult
    {
        private byte[] _file;
        private string _fileName;
        HttpRequestMessage _request;

        public FileResult2(byte[] file, string fileName, HttpRequestMessage request)
        {
            _file = file;
            _fileName = fileName;
            _request = request;
        }


        public Task<HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
        {

            MediaTypeHeaderValue _mediaType = MediaTypeHeaderValue.Parse("application/octet-stream");//指定文件类型

            string agent = _request.Headers.UserAgent.Count > 0 ? _request.Headers.UserAgent.First().ToString() : _request.Headers.UserAgent.ToString();


            ContentDispositionHeaderValue _disposition = ContentDispositionHeaderValue.Parse("attachment;filename=\""
                + ConvertFileName(_fileName, agent) + "\"");//指定文件名称（编码中文）

            try
            {

                if (_file == null || _file.Length == 0)
                    throw new HttpResponseException(HttpStatusCode.NotFound);

                //MemoryStream ms = new MemoryStream(_file);
                //ms.Position = 0;

                var resp = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    //Content = new StreamContent(ms),
                    Content = new ByteArrayContent(_file),
                    RequestMessage = _request
                };

                resp.Content.Headers.ContentType = _mediaType;
                resp.Content.Headers.ContentDisposition = _disposition;
                return Task.FromResult(resp);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            finally
            {

            }
        }

        /// <summary>
        /// 修正firefox导出excel名字乱码的问题
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="agent"></param>
        /// <returns></returns>
        public string ConvertFileName(string fileName, string agent)
        {
            if (Regex.IsMatch(agent.ToLower(), "firefox"))
            {
                return fileName;
            }
            return Uri.EscapeDataString(fileName);
        }



    }

}
