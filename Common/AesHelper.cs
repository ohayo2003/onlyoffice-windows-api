using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
    /// <summary>
    /// AES 对称加密/解密
    /// </summary>
    public class AesHelper
    {

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="aesText">密文字符串</param>
        /// <param name="aesKey">密钥，密钥长度32</param>
        /// <returns>返回解密后的明文字符串</returns>
        public static string AESDecrypt(string aesText, string aesKey)
        {
            string result = "";
            try
            {
                if (String.IsNullOrEmpty(aesText) || String.IsNullOrEmpty(aesKey))
                {
                    return null;
                }

                byte[] cipherText = Convert.FromBase64String(aesText);
                int length = cipherText.Length;
                SymmetricAlgorithm des = Rijndael.Create();
                byte[] btAesKey = Encoding.UTF8.GetBytes(aesKey);
                byte[] btAesKey32 = new byte[32];
                Buffer.BlockCopy(btAesKey, 0, btAesKey32, 0, Math.Min(btAesKey.Length, btAesKey32.Length));
                des.Key = btAesKey32;
                byte[] iv = new byte[16];
                Buffer.BlockCopy(cipherText, 0, iv, 0, 16);
                des.IV = iv;

                byte[] decryptBytes = new byte[length - 16];
                byte[] passwdText = new byte[length - 16];
                Buffer.BlockCopy(cipherText, 16, passwdText, 0, length - 16);
                using (MemoryStream ms = new MemoryStream(passwdText))
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        cs.Read(decryptBytes, 0, decryptBytes.Length);
                        cs.Close();
                        ms.Close();
                    }
                }

                result = Encoding.UTF8.GetString(decryptBytes).Replace("\0", ""); //将字符串后尾的'\0'去掉
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }

        /// <summary>
        /// AES多组密钥解密
        /// </summary>
        /// <param name="decryptText">密文字符串</param>
        /// <param name="aesKeys">密钥，多组密钥</param>
        /// <param name="reEncrypt">是否需要重新加密</param>
        /// <returns></returns>
        public static string AESDecrypt(string decryptText, List<string> aesKeys, out bool reEncrypt)
        {
            reEncrypt = true;
            if (String.IsNullOrEmpty(decryptText) || aesKeys == null || aesKeys.Count <= 0)
            {
                return null;
            }

            string result = null;
            int index = 0;
            foreach (string key in aesKeys)
            {
                result = AESDecrypt(decryptText, key);
                if (!String.IsNullOrEmpty(result))
                {
                    bool has;
                  KYCX.Common.MyFunction.FilterXmlErrorCode(result, out has);
                    if (!has)
                    {
                        if (index == 0)
                        {
                            reEncrypt = false;
                        }
                        break;
                    }
                }
                index++;
            }
            return result;

        }
    }
}
