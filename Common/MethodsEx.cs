using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Common
{
    /// <summary>
    /// 方法扩展
    /// </summary>
    public static class MethodsEx
    {
        #region List<T>
        /// <summary>
        /// 深度复制
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<T> Copy<T>(this List<T> source)
        {
            if (source == null)
            {
                return null;
            }

            using (Stream objectStream = new MemoryStream())
            {
                //利用 System.Runtime.Serialization序列化与反序列化完成引用对象的复制  
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, source);
                objectStream.Seek(0, SeekOrigin.Begin);
                return (List<T>)formatter.Deserialize(objectStream);
            }
        }

        /// <summary>
        /// 转换为字符串，以指定字符分割
        /// </summary>
        /// <param name="source"></param>
        /// <param name="splitChar">分隔符，默认为','</param>
        /// <returns></returns>
        public static string ToStringWithChar(this List<int> source, string splitChar = ",")
        {
            if (source == null || source.Count <= 0)
            {
                return "";
            }
            string result = source.Aggregate("", (current, next) => current + next.ToString() + splitChar);
            return result.TrimEnd(splitChar.Length > 0 ? splitChar[0] : ' ');
        }

        #endregion

        #region T
        /// <summary>
        /// 深度复制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T DeepCopy<T>(this T source)
        {
            if (source == null)
            {
                return default(T);
            }

            using (Stream objectStream = new MemoryStream())
            {
                //利用 System.Runtime.Serialization序列化与反序列化完成引用对象的复制  
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, source);
                objectStream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(objectStream);
            }
        }
        #endregion

        #region Enum
        /// <summary>
        /// 返回枚举项的描述信息。
        /// </summary>
        /// <param name="value">要获取描述信息的枚举项。</param>
        /// <returns>枚举想的描述信息。</returns>
        public static string GetDescription(this Enum value)
        {
            Type enumType = value.GetType();
            DescriptionAttribute attr = null;

            // 获取枚举常数名称。
            string name = Enum.GetName(enumType, value);
            if (name != null)
            {
                // 获取枚举字段。
                FieldInfo fieldInfo = enumType.GetField(name);
                if (fieldInfo != null)
                {
                    // 获取描述的属性。
                    attr = Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute), false) as DescriptionAttribute;
                }
            }


            if (attr != null)
            {
                return attr.Description;
            }
            else
            {
                return "";
            }
        }
        #endregion

        #region string
        /// <summary>
        /// 字符串是否为空
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this String value)
        {
            if (String.IsNullOrEmpty(value) || value.Trim().Length <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 转换为字符串，以指定字符分割
        /// </summary>
        /// <param name="source"></param>
        /// <param name="splitChar">分隔符，默认为','</param>
        /// <returns></returns>
        public static string ToArrayJoin(this IEnumerable<string> source, string splitChar = ",")
        {
            if (source == null || source.Count() <= 0)
            {
                return "";
            }
            string result = "";
            if (source.Count() == 1)
            {
                result = source.FirstOrDefault();
            }
            else
            {
                result = source.Aggregate("", (current, next) => current + next + splitChar);
                result = result.Substring(0, result.Length - 1);
            }
            return result;
        }

        /// <summary>
        /// Model反射
        /// </summary>
        private static List<Type> ModelTypeList { get; set; }

        /// <summary>
        /// 未完成
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string ReplaceModel(this string value)
        {
            if (String.IsNullOrEmpty(value) || !value.Contains("{{") || !value.Contains("}}"))
            {
                return value;
            }

            try
            {
                if (ModelTypeList == null)
                {
                    ModelTypeList = new List<Type>();
                    string localPath = AppDomain.CurrentDomain.BaseDirectory;
                    KYCX.Logging.Logger.DefaultLogger.Debug("localPath: " + localPath);
                    localPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().FullName);
                    string fileName = Path.Combine(localPath ?? "", "model.dll");
                    if (!File.Exists(fileName))
                    {
                        throw new FileNotFoundException("", fileName);
                    }
                    Type[] types = Assembly.LoadFrom(fileName).GetTypes();
                    foreach (Type type in types)
                    {
                        if (type.BaseType == typeof(DataSet))
                        {
                            ModelTypeList.Add(type);
                        }
                    }
                }

                //todo:替换Model


                return null;
            }
            catch (Exception ex)
            {
                KYCX.Logging.Logger.DefaultLogger.ErrorFormat("ReplaceModel(value=\"{0}\")替换异常。", ex, value ?? "NULL");
                return null;
            }
        }
        #endregion
    }
}
