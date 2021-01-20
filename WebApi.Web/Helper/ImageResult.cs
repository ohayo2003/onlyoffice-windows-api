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

namespace WebApi.web.Helper
{
    public class ImageResult : IHttpActionResult
    {
        private string _imgPath;
        HttpRequestMessage _request;

        public ImageResult(string  imgPath, HttpRequestMessage request)
        {
            _imgPath = imgPath;
            _request = request;
        }


        public Task<HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
        {
            var imgStream = new MemoryStream(File.ReadAllBytes(_imgPath));
            var resp = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(imgStream),
                RequestMessage = _request
            };
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");

            return Task.FromResult(resp);
        }
    }



}
