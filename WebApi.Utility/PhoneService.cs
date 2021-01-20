using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KYCX.Common;
using KYCX.SMS.Model;

namespace WebApi.Utility
{
    public class PhoneService
    {

        /// <summary>
        /// 验证码
        /// </summary>
        public string VCode { get; set; }
        /// <summary>
        /// 电话号
        /// </summary>
        public string PhoneNum { get; set; }
        /// <summary>
        /// 短信用户名
        /// </summary>
        public string MsgUserName { get; set; }
        /// <summary>
        /// 短信密码
        /// </summary>
        public string MsgPwd { get; set; }




        /// <summary>
        /// 第几次发送
        /// </summary>
        public int Times { get; set; }

        public PhoneService(string phoneNum, int times, string msgUserName, string msgPwd)
        {
            VCode = MakeValidateCode();
            PhoneNum = phoneNum;
            Times = times;
            MsgUserName = msgUserName;
            MsgPwd = msgPwd;
        }

        public Task SendAsync(string contentMsg)
        {
            return Task.Run(() =>
            {
                string res = "";
                Send(out res, contentMsg);
            });
        }


        public bool Send(out string message, string contentMsg)
        {
            System.Net.Http.HttpClient client = null;
            HttpResponseMessage response = null;
            message = "";
            bool Result = false;

            try
            {
                ClientMsg clientMsg = new ClientMsg
                 {
                     UniqueId = Guid.NewGuid(),
                     UserName = MsgUserName,
                     Password = MsgPwd,
                     RequestTime = DateTime.Now.Ticks, //请求时间
                     OutTime = 10, //超时时间（分钟）
                     TelNumber = this.PhoneNum,
                     Times = this.Times, //请求次数，1首次发送，2重新发送……
                     Content = contentMsg
                     //ClientIP = ""
                 };


                PreCheckStateType preCheckState = SendSms(clientMsg, out message);
                if (preCheckState == PreCheckStateType.Success)
                {

                    Result = true;
                }
                else
                {
                    Result = false;
                }
            }
            catch (Exception e)
            {

                message = e.Message;
                Result = false;
            }
            finally
            {
                if (null != response)
                {
                    response.Dispose();
                }
                if (null != client)
                {
                    client.Dispose();
                }
            }
            return Result;
        }
        String generate_md5(String str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text
            md5.ComputeHash(Encoding.GetEncoding("utf-8").GetBytes(str));

            //get hash result after compute it
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }

        string MakeValidateCode()
        {
            Random r = new Random();

            //char[] s = new char[] { '2', '3', '4', '5', '6', '7', '8', '9', 
            //                        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 
            //                        'k', 'm', 'n', 'p', 'q', 's', 'u', 'v', 
            //                        'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 
            //                        'E', 'F', 'G', 'H', 'J', 'U', 'V', 'W', 
            //                        'X', 'Y', 'Z' };//枚举数组

            char[] s = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };//枚举数组
            string num = "";
            for (int i = 0; i < 6; i++)
            {
                num += s[r.Next(0, s.Length)].ToString();
            }
            return num;
        }

        /// <summary>
        /// 调用综合短息服务发送短信
        /// </summary>
        /// <param name="clientMsg"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static PreCheckStateType SendSms(ClientMsg clientMsg, out string message)
        {
            message = "";
            if (clientMsg == null)
            {
                message = PreCheckStateType.ParamError.GetDescription();
                return PreCheckStateType.ParamError;
            }

            PreCheckStateType result;
            MessageServiceReference.SendSmsServiceSoapClient smsServiceClient = null;
            try
            {
                if (String.IsNullOrWhiteSpace(clientMsg.ClientIP))
                {
                    clientMsg.ClientIP = KYCX.Common.IPHelper.GetClientIP();
                }
                using (smsServiceClient = new MessageServiceReference.SendSmsServiceSoapClient())
                {
                    string strClientMsg = KYCX.Common.JsonConvertEx.ObjectToJson(clientMsg);
                    result = (PreCheckStateType)smsServiceClient.SendSms(strClientMsg);
                    message = result.GetDescription();
                }
            }
            catch (Exception ex)
            {
                result = PreCheckStateType.Exception;
                message = "请求综合短信服务发送短信异常。" + ex.ToString();
                //KYCX.Logging.Logger.DefaultLogger.ErrorFormat("请求综合短信服务发送短信异常，SendSmsServiceHandler.SendSms({0})",
                //    ex, KYCX.Common.JsonConvertEx.ObjectToJson(clientMsg));
            }
            finally
            {
                if (smsServiceClient != null)
                {
                    smsServiceClient.Close();
                    smsServiceClient = null;
                }
            }
            return result;
        }
    }
}
