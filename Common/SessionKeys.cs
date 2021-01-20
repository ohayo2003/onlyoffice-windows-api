using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Common
{
    public class SessionKeys
    {


        private static readonly string ServerTokenKey = "1a2s3d4f5g6h7j8k";//16位服务器端密钥 
        private static readonly string ServerIv = "27hhc76abvz54eqb";//16位IV向量

        public SessionKeys()
        {

        }

        public static string GenerateAesToken(string sMobileNum)
        {

            try
            {
               
                return AesCrypto.Encrypt(sMobileNum, ServerTokenKey, ServerIv);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string DecryptAesToken(string AesToken)
        {
            try
            {
                return AesCrypto.Decrypt(AesToken, ServerTokenKey, ServerIv);
            }
            catch
            {
                return string.Empty;
            }
        }



    }
}
