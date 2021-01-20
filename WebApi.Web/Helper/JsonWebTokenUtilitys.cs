using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JWT.Algorithms;
using JWT.Serializers;
using JWT.Exceptions;
using JWT;
using System.Web.Script.Serialization;
using System.Web.Configuration;

namespace WebApi.web.Helper
{
    public class JsonWebTokenUtilitys
    {

        private string _secret { set; get; }

        private IDictionary<string, object> _extraHeaders;


        public JsonWebTokenUtilitys()
        {
            _secret = "cnki";
            //_secret = WebConfigurationManager.AppSettings["UserServerInfo"].ToString();

            //_extraHeaders = new Dictionary<string, object>{{ "userId", "001" },{ "userAccount", "fan" }};
        }

        /// <summary>
        /// 创建token
        /// </summary>
        /// <returns></returns>
        public string CreateJwtToken(IDictionary<string, object> payload)
        {
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            var token = encoder.Encode(payload, _secret);
            return token;
        }


        /// <summary>
        /// 校验解析token
        /// </summary>
        /// <returns></returns>
        public string ValidateJwtToken(string token)
        {
            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtAlgorithm alg = new HMACSHA256Algorithm();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, alg);
                var json = decoder.Decode(token, _secret, true);
                //校验通过，返回解密后的字符串
                return json;
            }
            catch (TokenExpiredException)
            {
                //表示过期
                return "expired";
            }
            catch (SignatureVerificationException)
            {
                //表示验证不通过
                return "invalid";
            }
            catch (Exception)
            {
                return "error";
            }
        }

        /// <summary>
        /// json字符串转Dictionary
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Dictionary<string, object> getValue(string json)
        {
            //解析json对象
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return (Dictionary<string, object>)serializer.DeserializeObject(json);
        }


    }
}
