using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Formatting;
using System.Net;
using Newtonsoft.Json.Converters;

namespace WebApi.web.Helper
{
    public class PageResult : IHttpActionResult
    {
        object _value;
        HttpRequestMessage _request;
        HttpStatusCode _scode;
        private bool _dateTimeFlag;


        /// <summary>
        /// 返回消息需要标准格式化
        /// </summary>
        /// <param name="isSuccess">true or false </param>
        /// <param name="message">成功则返回对应的消息或者结果实体列表，失败则返回失败的msg</param>
        /// <param name="request"></param>
        public PageResult(HttpRequestMessage request, bool result, string message, object info = null, bool dateTimeFlag = true)
        {
            if (info != null)
            {
                _value = new { result = result, message = message, info = info };
            }
            else
            {
                _value = new { result = result, message = message };
            }

            _request = request;
            _scode = HttpStatusCode.OK;
            _dateTimeFlag = dateTimeFlag;
        }


        public PageResult(HttpRequestMessage request, int returnStatus, string msg, object infoObject = null, bool dateTimeFlag = true)
        {
            _value = new { returnStatus = returnStatus, msg = msg, infoObject = infoObject };
            _request = request;
            _scode = HttpStatusCode.OK;
            _dateTimeFlag = dateTimeFlag;
        }



        public PageResult(HttpStatusCode code, object value, HttpRequestMessage request, bool dateTimeFlag = true)
        {
            _value = value;
            _request = request;
            _scode = code;
            _dateTimeFlag = dateTimeFlag;
        }

        public Task<HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
        {
            var jsonFormatter = new JsonMediaTypeFormatter();
            var settings = jsonFormatter.SerializerSettings;

            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            //这里使用自定义日期格式
            if (_dateTimeFlag)
            {
                timeConverter.DateTimeFormat = "yyyy'-'MM'-'dd";
            }
            else
            {
                timeConverter.DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";
            }
            settings.Converters.Add(timeConverter);

            var response = new HttpResponseMessage(_scode)
            {
                Content = new ObjectContent(typeof(object), _value, jsonFormatter),
                RequestMessage = _request
            };
            return Task.FromResult(response);
        }

        void GetFormatter()
        {

        }
    }





}
