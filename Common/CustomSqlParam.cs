using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Common
{
    public class CustomSqlParam
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// 参数类别
        /// </summary>
        public SqlDbType Type { get; set; }

        /// <summary>
        /// 参数值
        /// </summary>
        public object Value { get; set; }

        public CustomSqlParam()
        {
            Name = string.Empty;
            Type = SqlDbType.Char;
            Value = null;
        }
        public CustomSqlParam(string sName,SqlDbType pType,object oValue)
        {
            Name = sName;
            Type = pType;
            Value = oValue;
        }
    }
}
