using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;

namespace Common
{
    public class SMTPSendMail
    {
        private string _infomation;

        public string Infomation
        {
            set
            {
                _infomation = value;
            }
            get
            {
                return _infomation;
            }

        }

        private string _SMTPHost;
        /// <summary>
        /// SMTP服务器名称
        /// </summary>
        public string SMTPHost
        {
            set
            {
                _SMTPHost = value;
            }
            get
            {
                return _SMTPHost;
            }

        }

        private int _Port;
        /// <summary>
        /// SMTP服务器端口（默认25）
        /// </summary>
        public int Port
        {
            set
            {
                _Port = value;
            }
            get
            {
                return _Port;
            }

        }

        private string _SendMailUserName;
        /// <summary>
        /// 邮件发送人：用户名
        /// </summary>
        public string SendMailUserName
        {
            set
            {
                _SendMailUserName = value;
            }
            get
            {
                return _SendMailUserName;
            }

        }

        private string _SendMailDisplayName;
        /// <summary>
        /// 邮件发送人：用户名显示名称
        /// </summary>
        public string SendMailDisplayName
        {
            set
            {
                _SendMailDisplayName = value;
            }
            get
            {
                return _SendMailDisplayName;
            }

        }

        private string _SendMailPassWord;
        /// <summary>
        /// 邮件发送人：用户密码
        /// </summary>
        public string SendMailPassWord
        {
            set
            {
                _SendMailPassWord = value;
            }
            get
            {
                return _SendMailPassWord;
            }

        }

        private string _SendToAddress;
        /// <summary>
        /// 发送至：地址
        /// </summary>
        public string SendToAddress
        {
            set
            {
                _SendToAddress = value;
            }
            get
            {
                return _SendToAddress;
            }

        }

        private string _SendToUid;
        /// <summary>
        /// 发送至：显示用户名
        /// </summary>
        public string SendToUid
        {
            set
            {
                _SendToUid = value;
            }
            get
            {
                return _SendToUid;
            }

        }

        private string _Subject;
        /// <summary>
        /// 邮件主题
        /// </summary>
        public string Subject
        {
            set
            {
                _Subject = value;
            }
            get
            {
                return _Subject;
            }

        }

        private string _MailBody;
        /// <summary>
        /// 邮件内容（HTML）
        /// </summary>
        public string MailBody
        {
            set
            {
                _MailBody = value;
            }
            get
            {
                return _MailBody;
            }

        }

        private string[] _AttachmentList;
        /// <summary>
        /// 附件列表（文件地址）
        /// </summary>
        public string[] AttachmentList
        {
            set
            {
                _AttachmentList = value;
            }
            get
            {
                return _AttachmentList;
            }
        }

        public SMTPSendMail()
        {

            SMTPHost = "smtp.cnki.net";

            _Port = 25;

            _SendMailUserName = "amlcalert@cnki.net";

            _SendMailPassWord = "";

            _SendMailDisplayName = "学术不端行为检测系统";

            _SendToAddress = "";

            _SendToUid = "";

            _Subject = "";

            _MailBody = "";

            _AttachmentList = null;


        }

        public SMTPSendMail(string SendToAddress, string SendToUid, string Subject,string MailBody)
        {
            SMTPHost = "smtp.cnki.net";

            _Port = 25;

            _SendMailUserName = "amlcalert@cnki.net";

            _SendMailPassWord = "";

            _SendMailDisplayName = "学术不端行为检测系统";

            _SendToAddress = SendToAddress;

            _SendToUid = SendToUid;

            _Subject = Subject;

            _MailBody = MailBody;

            _AttachmentList = null;

        }

        public bool SenMail()
        {
            bool fg = true;

            try
            {
                SmtpClient smtp = new SmtpClient();
                //实例化一个SmtpClientsmtp.DeliveryMethod = SmtpDeliveryMethod.Network; 
                //将smtp的出站方式设为 Network
                smtp.EnableSsl = false;
                //smtp服务器是否启用SSL加密
                smtp.Host = _SMTPHost;
                //指定 smtp 服务器地址
                smtp.Port = _Port;
                //指定 smtp 服务器的端口，默认是25，如果采用默认端口，可省去
                //如果你的SMTP服务器不需要身份认证，则使用下面的方式，不过，目前基本没有不需要认证的了
                smtp.UseDefaultCredentials = true;
                //如果需要认证，则用下面的方式
                smtp.Credentials = new NetworkCredential(_SendMailUserName, _SendMailPassWord);
                MailMessage mm = new MailMessage();
                //实例化一个邮件类
                mm.Priority = MailPriority.High;
                //邮件的优先级，分为 Low, Normal, High，通常用 Normal即可
                mm.From = new MailAddress(_SendMailUserName, _SendMailDisplayName, Encoding.UTF8);
                //收件方看到的邮件来源；
                //第一个参数是发信人邮件地址
                //第二参数是发信人显示的名称
                //第三个参数是 第二个参数所使用的编码，如果指定不正确，则对方收到后显示乱码
                //Encoding.GetEncoding(936)
                //936是简体中文的codepage值注：上面的邮件来源，一定要和你登录邮箱的帐号一致，否则会认证失败
                //mm.ReplyTo = new MailAddress("noreply@cnki.net", "禁止回复", Encoding.UTF8);
                //ReplyTo 表示对方回复邮件时默认的接收地址，即：你用一个邮箱发信，但却用另一个来收信
                //上面后两个参数的意义， 同 From 的意义

                mm.To.Add(new MailAddress(_SendToAddress, _SendToUid, Encoding.UTF8));

                mm.Subject = _Subject;
                //邮件标题
                mm.SubjectEncoding = Encoding.UTF8;
                // 这里非常重要，如果你的邮件标题包含中文，这里一定要指定，否则对方收到的极有可能是乱码。
                // 936是简体中文的pagecode，如果是英文标题，这句可以忽略不用
                mm.IsBodyHtml = true;
                //邮件正文是否是HTML格式
                mm.BodyEncoding = Encoding.UTF8;
                //邮件正文的编码， 设置不正确， 接收者会收到乱码
                mm.Body = _MailBody;
                //邮件正文
                //mm.Attachments.Add( new Attachment( @"d:a.doc", System.Net.Mime.MediaTypeNames.Application.Rtf ) );
                //添加附件，第二个参数，表示附件的文件类型，可以不用指定
                //可以添加多个附件
                //mm.Attachments.Add( new Attachment( @"d:b.doc") );
                if (_AttachmentList != null)//添加附件
                {
                    for (int i = 0; i < _AttachmentList.Length; i++)
                    {
                        mm.Attachments.Add(new Attachment(_AttachmentList[i]));
                    }
                }
                
                smtp.Send(mm);
                //发送邮件，如果不返回异常， 则大功告成了。

            }
            catch(Exception ex)
            {
                fg = false;
                _infomation = ex.Message;
            }


            return fg;
        }
    }
}
