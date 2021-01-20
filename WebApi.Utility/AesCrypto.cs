using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WebApi.Utility
{
    /// <summary>
    /// AES加密解密
    /// </summary>
    public static class AesCrypto
    {

        //对称加密和分组加密中的四种模式(ECB、CBC、CFB、OFB),这三种的区别，主要来自于密钥的长度，16位密钥=128位，24位密钥=192位，32位密钥=256位。
        //更多参考：http://www.cnblogs.com/happyhippy/archive/2006/12/23/601353.html

        /// <summary>
        /// 检验密钥是否有效长度【16|24|32】
        /// </summary>
        /// <param name="key">密钥</param>
        /// <returns>bool</returns>
        private static bool CheckKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;
            if (16.Equals(key.Length) || 24.Equals(key.Length) || 32.Equals(key.Length))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 检验向量是否有效长度【16】
        /// </summary>
        /// <param name="iv">向量</param>
        /// <returns>bool</returns>
        private static bool CheckIv(string iv)
        {
            if (string.IsNullOrWhiteSpace(iv))
                return false;
            if (16.Equals(iv.Length))
                return true;
            else
                return false;
        }

        #region 参数是string类型的
        /// <summary>
        ///  加密 参数：string
        /// </summary>
        /// <param name="palinData">明文</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">向量</param>
        /// <param name="encodingType">编码方式</param>
        /// <returns>string：密文</returns>
        public static string Encrypt(string palinData, string key, string iv, EncodingStrOrByte.EncodingType encodingType = EncodingStrOrByte.EncodingType.UTF8)
        {
            if (string.IsNullOrWhiteSpace(palinData)) return null;
            if (!(CheckKey(key) && CheckIv(iv))) return palinData;
            byte[] toEncryptArray = EncodingStrOrByte.GetBytes(palinData, encodingType);
            var rm = new RijndaelManaged
            {
                IV = EncodingStrOrByte.GetBytes(iv, encodingType),
                Key = EncodingStrOrByte.GetBytes(key, encodingType),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform cTransform = rm.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        ///  解密 参数：string
        /// </summary>
        /// <param name="encryptedData">密文</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">向量</param>
        /// <param name="encodingType">编码方式</param>
        /// <returns>string：明文</returns>
        public static string Decrypt(string encryptedData, string key, string iv, EncodingStrOrByte.EncodingType encodingType = EncodingStrOrByte.EncodingType.UTF8)
        {
            if (string.IsNullOrWhiteSpace(encryptedData)) return null;
            if (!(CheckKey(key) && CheckIv(iv))) return encryptedData;
            byte[] toEncryptArray = Convert.FromBase64String(encryptedData);
            var rm = new RijndaelManaged
            {
                IV = EncodingStrOrByte.GetBytes(iv, encodingType),
                Key = EncodingStrOrByte.GetBytes(key, encodingType),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform cTransform = rm.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Encoding.UTF8.GetString(resultArray);
        }
        #endregion

        #region 参数是byte[]类型的
        /// <summary>  
        /// 加密 参数：byte[] 
        /// </summary>  
        /// <param name="palinData">明文</param>  
        /// <param name="key">密钥</param>  
        /// <param name="iv">向量</param>  
        /// <returns>密文</returns>  
        public static byte[] Encrypt(byte[] palinData, string key, string iv, EncodingStrOrByte.EncodingType encodingType = EncodingStrOrByte.EncodingType.UTF8)
        {
            if (palinData == null) return null;
            if (!(CheckKey(key) && CheckIv(iv))) return palinData;
            byte[] bKey = new byte[32];
            Array.Copy(EncodingStrOrByte.GetBytes(key.PadRight(bKey.Length), encodingType), bKey, bKey.Length);
            byte[] bVector = new byte[16];
            Array.Copy(EncodingStrOrByte.GetBytes(iv.PadRight(bVector.Length), encodingType), bVector, bVector.Length);
            byte[] cryptograph = null; // 加密后的密文  
            Rijndael Aes = Rijndael.Create();
            // 开辟一块内存流  
            using (MemoryStream Memory = new MemoryStream())
            {
                // 把内存流对象包装成加密流对象  
                using (CryptoStream Encryptor = new CryptoStream(Memory, Aes.CreateEncryptor(bKey, bVector), CryptoStreamMode.Write))
                {
                    // 明文数据写入加密流  
                    Encryptor.Write(palinData, 0, palinData.Length);
                    Encryptor.FlushFinalBlock();
                    cryptograph = Memory.ToArray();
                }
            }
            return cryptograph;
        }

        /// <summary>  
        /// 解密  参数：byte[] 
        /// </summary>  
        /// <param name="encryptedData">被解密的密文</param>  
        /// <param name="key">密钥</param>  
        /// <param name="iv">向量</param>  
        /// <returns>明文</returns>  
        public static byte[] Decrypt(byte[] encryptedData, string key, string iv, EncodingStrOrByte.EncodingType encodingType = EncodingStrOrByte.EncodingType.UTF8)
        {
            if (encryptedData == null) return null;
            if (!(CheckKey(key) && CheckIv(iv))) return encryptedData;
            byte[] bKey = new byte[32];
            Array.Copy(EncodingStrOrByte.GetBytes(key.PadRight(bKey.Length), encodingType), bKey, bKey.Length);
            byte[] bVector = new byte[16];
            Array.Copy(EncodingStrOrByte.GetBytes(iv.PadRight(bVector.Length), encodingType), bVector, bVector.Length);
            byte[] original = null; // 解密后的明文  
            Rijndael Aes = Rijndael.Create();
            // 开辟一块内存流，存储密文  
            using (MemoryStream Memory = new MemoryStream(encryptedData))
            {
                // 把内存流对象包装成加密流对象  
                using (CryptoStream Decryptor = new CryptoStream(Memory, Aes.CreateDecryptor(bKey, bVector), CryptoStreamMode.Read))
                {
                    // 明文存储区  
                    using (MemoryStream originalMemory = new MemoryStream())
                    {
                        byte[] Buffer = new byte[1024];
                        int readBytes = 0;
                        while ((readBytes = Decryptor.Read(Buffer, 0, Buffer.Length)) > 0)
                        {
                            originalMemory.Write(Buffer, 0, readBytes);
                        }
                        original = originalMemory.ToArray();
                    }
                }
            }
            return original;
        }
        #endregion
    }

    /// <summary>
    /// 处理编码字符串或字符串
    /// </summary>
    public static class EncodingStrOrByte
    {
        /// <summary>
        /// 编码方式
        /// </summary>
        public enum EncodingType { UTF7, UTF8, UTF32, Unicode, BigEndianUnicode, ASCII, GB2312 };
        /// <summary>
        /// 处理指定编码的字符串，转换字节数组
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encodingType"></param>
        /// <returns></returns>
        public static byte[] GetBytes(string str, EncodingType encodingType)
        {
            byte[] bytes = null;
            switch (encodingType)
            {
                //将要加密的字符串转换为指定编码的字节数组
                case EncodingType.UTF7:
                    bytes = Encoding.UTF7.GetBytes(str);
                    break;
                case EncodingType.UTF8:
                    bytes = Encoding.UTF8.GetBytes(str);
                    break;
                case EncodingType.UTF32:
                    bytes = Encoding.UTF32.GetBytes(str);
                    break;
                case EncodingType.Unicode:
                    bytes = Encoding.Unicode.GetBytes(str);
                    break;
                case EncodingType.BigEndianUnicode:
                    bytes = Encoding.BigEndianUnicode.GetBytes(str);
                    break;
                case EncodingType.ASCII:
                    bytes = Encoding.ASCII.GetBytes(str);
                    break;
                case EncodingType.GB2312:
                    bytes = Encoding.Default.GetBytes(str);
                    break;
            }
            return bytes;
        }

        /// <summary>
        /// 处理指定编码的字节数组，转换字符串
        /// </summary>
        /// <param name="myByte"></param>
        /// <param name="encodingType"></param>
        /// <returns></returns>
        public static string GetString(byte[] myByte, EncodingType encodingType)
        {
            string str = null;
            switch (encodingType)
            {
                //将要加密的字符串转换为指定编码的字节数组
                case EncodingType.UTF7:
                    str = Encoding.UTF7.GetString(myByte);
                    break;
                case EncodingType.UTF8:
                    str = Encoding.UTF8.GetString(myByte);
                    break;
                case EncodingType.UTF32:
                    str = Encoding.UTF32.GetString(myByte);
                    break;
                case EncodingType.Unicode:
                    str = Encoding.Unicode.GetString(myByte);
                    break;
                case EncodingType.BigEndianUnicode:
                    str = Encoding.BigEndianUnicode.GetString(myByte);
                    break;
                case EncodingType.ASCII:
                    str = Encoding.ASCII.GetString(myByte);
                    break;
                case EncodingType.GB2312:
                    str = Encoding.Default.GetString(myByte);
                    break;
            }
            return str;
        }
    }

}