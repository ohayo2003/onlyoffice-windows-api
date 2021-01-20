using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Common
{

    public class JsonConvertEx
    {
        #region 序列化
        /// <summary>
        /// 将对象序列化为JSON格式
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>json字符串</returns>
        public static string ObjectToJson(object obj)
        {
            if (obj == null)
            {
                return "";
            }
            IsoDateTimeConverter dtConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd" };
            string json = JsonConvert.SerializeObject(obj,dtConverter);
            return json;
        }
        /// <summary>
        /// 将对象序列化为JSON格式
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>json字符串</returns>
        public static string ObjectToJsonAndDateTimeFormat(object obj)
        {
            if (obj == null)
            {
                return "";
            }
            IsoDateTimeConverter dtConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            string json = JsonConvert.SerializeObject(obj, dtConverter);
            return json;
        }
        #endregion

        #region 反序列化
        /// <summary>
        /// 解析JSON字符串生成对象实体
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json字符串(eg.{"ID":"112","Name":"石子儿"})</param>
        /// <returns>对象实体</returns>
        public static T JsonToObject<T>(string json) where T : class
        {
            if (String.IsNullOrEmpty(json))
            {
                return null;
            }

            

            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            StringReader sr = new StringReader(json);
            object obj = serializer.Deserialize(new JsonTextReader(sr), typeof(T));
            T t = obj as T;
            return t;
        }

        /// <summary>
        /// 解析JSON数组生成对象实体集合
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json数组字符串(eg.[{"ID":"112","Name":"石子儿"}])</param>
        /// <returns>对象实体集合</returns>
        public static List<T> JsonToList<T>(string json) where T : class
        {
            if (String.IsNullOrEmpty(json))
            {
                return null;
            }

            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            StringReader sr = new StringReader(json);
            object obj = serializer.Deserialize(new JsonTextReader(sr), typeof(List<T>));
            List<T> list = obj as List<T>;
            return list;
        }

        /// <summary>
        /// 解析JSON到给定的匿名对象.
        /// </summary>
        /// <typeparam name="T">匿名对象类型</typeparam>
        /// <param name="json">json字符串</param>
        /// <param name="anonymousTypeObject">匿名对象</param>
        /// <returns>匿名对象</returns>
        public static T JsonToAnonymousType<T>(string json, T anonymousTypeObject)
        {
            if (String.IsNullOrEmpty(json))
            {
                return anonymousTypeObject;
            }
            T t = JsonConvert.DeserializeAnonymousType<T>(json, anonymousTypeObject);
            return t;
        }

        /// <summary>
        /// 解析JSON字符串生成对象实体
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Newtonsoft.Json.Linq.JObject JsonToJObject(string json)
        {
            if (String.IsNullOrEmpty(json))
            {
                return null;
            }
            return Newtonsoft.Json.Linq.JObject.Parse(json);
        }

        #endregion

        #region JSON格式消息
        /// <summary>
        /// 返回json格式消息
        /// </summary>
        /// <param name="isSuccess">是否成功</param>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public static string GetJsonMessage(bool isSuccess, string message)
        {
            string strJson = "";
            var data = new { isSuccess = isSuccess, message = message };
            strJson = ObjectToJson(data);
            return strJson;
        }

        /// <summary>
        /// 返回json格式消息
        /// </summary>
        /// <param name="isSuccess">是否成功</param>
        /// <param name="message">消息</param>
        /// <param name="dt">数据集合</param>
        /// <param name="totalCount">总数</param>
        /// <returns></returns>
        public static string GetJsonMessage(bool isSuccess, string message, DataTable dt, int totalCount)
        {
            string strJson = "";
            //string _dataList = ObjectToJson(dt);
            var data = new { isSuccess = isSuccess, message = message, dataList = dt, totalCount = totalCount };
            strJson = ObjectToJson(data);
            return strJson;
        }

        /// <summary>
        /// 返回json格式消息
        /// </summary>
        /// <param name="isSuccess">是否成功</param>
        /// <param name="message">消息</param>
        /// <param name="dt">数据集合</param>
        /// <param name="totalCount">总数</param>
        /// <returns></returns>
        public static string GetJsonMessageAndDateTimeFormat(bool isSuccess, string message, DataTable dt, int totalCount)
        {
            string strJson = "";
            //string _dataList = ObjectToJson(dt);
            var data = new { isSuccess = isSuccess, message = message, dataList = dt, totalCount = totalCount };
            strJson = ObjectToJsonAndDateTimeFormat(data);
            return strJson;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="message"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public static string GetJsonMessage(bool isSuccess, string message, string json)
        {
            string strJson = "";
            var data = new { isSuccess = isSuccess, message = message, json = json };
            strJson = ObjectToJson(data);
            return strJson;
        }

        #endregion



    }
}
/*
类型参数约束，.NET支持的类型参数约束有以下五种：
where T : struct | T必须是一个结构类型
where T : class T必须是一个类（class）类型
where T : new() | T必须要有一个无参构造函数
where T : NameOfBaseClass | T必须继承名为NameOfBaseClass的类
where T : NameOfInterface | T必须实现名为NameOfInterface的接口

*/
