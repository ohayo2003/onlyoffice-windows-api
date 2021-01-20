using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Web.Handler
{
    /// <summary>
    /// 执行http调用动作的枚举
    /// 仅代表call动作本身的成败
    /// </summary>
    public enum CallActionStatus
    {
        failed = -1,
        process = 0,
        success = 1
    }

    public enum ReturnStatus
    {
        failed = -1,
        process = 0,
        success = 1
    }




    public class DocConfig
    {
        public Dictionary<string, object> editorConfig { get; set; }
        public Dictionary<string, object> document { get; set; }

        public string documentType { get; set; }
        public string lang { get; set; }

        public Dictionary<string, object> events { get; set; } = new Dictionary<string, object>
        {
            { "onRequestHistoryClose","onRequestHistoryClose"},
            { "onRequestHistory","onRequestHistory"},
            { "onRequestHistoryData","onRequestHistoryData"}
        };

        public string token { get; set; }

        Dictionary<string, object> config;

        /// 组员：user_mode=0 ，其他随意
        /// 
        /// 组长：user_mode=1 user_edit_privilege=1  user_comment_privilege=0      
        /// 
        /// 教师：user_mode=1 user_edit_privilege=0   user_comment_privilege=1
        public DocConfig(string id, string name, string key, string title, string url,
            bool userMode = false, bool editPrivilege = false, bool commentPrivilege = false,
            string callbackUrl = "")
        {

            Dictionary<string, object> editorConfig = new Dictionary<string, object>
            {
                { "lang","zh-CN"},
                { "customization",new {
                    chat =false,
                    spellcheck =false,
                    showReviewChanges =false
                } },
                { "user",new { id=id,name=name} }
            };

            if (userMode)
            {
                editorConfig.Add("mode", "edit");
            }
            else
            {
                editorConfig.Add("mode", "view");

            }

            if (!string.IsNullOrEmpty(callbackUrl))
            {
                editorConfig.Add("callbackUrl", callbackUrl);
            }

            Dictionary<string, object> document = new Dictionary<string, object>
            {
                { "fileType","docx"},
                { "key",key},
                { "title",title},
                { "url",url},
                { "permissions",new { download=true,edit=editPrivilege,print=true,review=true,comment=commentPrivilege} }
            };

            //
            config = new Dictionary<string, object>();
            config.Add("editorConfig", editorConfig);
            config.Add("document", document);
            config.Add("documentType", "text");
            config.Add("lang", "zh-CN");

        }

        public Dictionary<string, object> GetPayload()
        {
            return config;
        }

        internal Dictionary<string, object> GetContent()
        {
            config.Add("events", events);
            config.Add("token", token);

            return config;
        }
    }



    public class HistoryDataConfig
    {
        Dictionary<string, object> config;
        public string token { get; set; }

        public HistoryDataConfig(string key, string url, int version, string changesUrl = "", string previousUrl = "", long previousKey = -1)
        {

            //
            config = new Dictionary<string, object>();
            if (!string.IsNullOrWhiteSpace(changesUrl))
            {
                config.Add("changesUrl", changesUrl);
                config.Add("previous", new { key = previousKey.ToString(), url = previousUrl });

            }

            config.Add("key", key);
            config.Add("url", url);
            config.Add("version", version);
        }

        public Dictionary<string, object> GetPayload()
        {
            return config;
        }

        internal Dictionary<string, object> GetContent()
        {
            //config.Add("events", events);
            config.Add("token", token);

            return config;
        }
    }






}
