using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WebApi.Utility
{
    public class CrypHash
    {
        private string getMd5(string s)
        {
            //初始化MD5对象
            MD5 md5 = MD5.Create();

            //将源字符串转化为byte数组
            Byte[] soucebyte = Encoding.Default.GetBytes(s);

            //soucebyte转化为mf5的byte数组
            Byte[] md5bytes = md5.ComputeHash(soucebyte);

            //将md5的byte数组再转化为MD5数组
            StringBuilder sb = new StringBuilder();
            foreach (Byte b in md5bytes)
            {
                //x表示16进制，2表示2位
                sb.Append(b.ToString("x2"));

            }
            return sb.ToString();

        }

        public string GetPassHash(string s)
        {
            return getMd5(s + "@check.cnki").ToLower();
        }

        public static string GetCheckCode(Dictionary<string, string> _dic, string crypo_key)
        {

            Dictionary<string, string> _dicCopy = _dic.OrderBy(p => p.Key).ToDictionary(p => p.Key, o => o.Value);

            string tmp = "";
            foreach (var item in _dicCopy)
            {
                tmp += item.Value.ToString();

            }

            string check_code = (new CrypHash()).getMd5(tmp + crypo_key);

            return check_code;
        }



    }
}
