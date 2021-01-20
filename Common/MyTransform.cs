using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic;
using System.Web;
using System.Data;
using System.Text.RegularExpressions;

namespace Common
{
    /// <summary>
    /// 转换方法集合
    /// </summary>
    public class MyTransform
    {
        /// <summary>
        /// 百分比
        /// </summary>
        /// <param name="a">被除数</param>
        /// <param name="b">除数</param>
        /// <param name="leave">小数点后保留位数</param>
        /// <returns></returns>
        public static decimal percentage(int a, int b, int leave)
        {
            if (b > 0)
            {
                //string temp = Math.Round((decimal)(a * 100.0 / b), leave, MidpointRounding.AwayFromZero).ToString();
                decimal temp = Math.Round((decimal)((decimal)(a) * 100 / b), leave, MidpointRounding.AwayFromZero);

                if (temp == 100)
                {
                    return 100;
                }
                else
                {
                    return temp;
                }
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 字符串转换成时间
        /// </summary>
        /// <param name="s">字符串</param>
        /// <param name="f">格式，如yyyy-MM-dd</param>
        /// <returns></returns>
        public static string StringToDateTime(string s, string f)
        {
            DateTime temp = new DateTime();

            if (DateTime.TryParse(s, out temp))
            {
                return temp.ToString(f);
            }

            return "";
        }
        /// <summary>
        /// 字符串转换日期
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="defaulttime">转换失败返回的默认值</param>
        /// <returns></returns>
        public static DateTime StringToDateTime(string source, DateTime defaulttime)
        {
            DateTime temp = new DateTime();

            if (!DateTime.TryParse(source, out temp))
            {
                return defaulttime;
            }

            return temp;
        }
        /// <summary>
        /// 字符串转换浮点型
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="defaultnumber">转换失败返回的默认值</param>
        /// <returns></returns>
        public static float StringToFloat(string source, float defaultnumber)
        {
            float temp = 0;

            if (!float.TryParse(source, out temp))
            {
                return defaultnumber;
            }

            return temp;
        }
        /// <summary>
        /// 字符串转换成double
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="defaultnumber">转换失败返回的默认值</param>
        /// <returns></returns>
        public static double StringToDouble(string source, double defaultnumber)
        {
            double temp = 0d;

            if (!double.TryParse(source, out temp))
            {
                return defaultnumber;
            }

            return temp;
        }

        /// <summary>
        /// 字符串转换整数
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="defaultnumber">转换失败返回的默认值</param>
        /// <returns></returns>
        public static int StringToInt(string source, int defaultnumber)
        {
            int temp = 0;

            if (!int.TryParse(source, out temp))
            {
                return defaultnumber;
            }

            return temp;
        }

        /// <summary>
        /// 字符串转长整型
        /// </summary>
        /// <param name="obj">原数据</param>
        /// <param name="defValue">转换失败返回的默认值</param>
        /// <returns></returns>
        public static long StringToLong(string source, long defValue)
        {
            if (String.IsNullOrEmpty(source))
            {
                return defValue;
            }
            long temp;
            if (long.TryParse(source, out temp))
            {
                return temp;
            }

            return defValue;
        }

        /// <summary>
        /// 字符串转decimal
        /// </summary>
        /// <param name="source"></param>
        /// <param name="defaultnumber"></param>
        /// <returns></returns>
        public static decimal StringToDecimal(string source, decimal defaultnumber)
        {
            decimal temp = 0M;

            if (!decimal.TryParse(source, out temp))
            {
                return defaultnumber;
            }

            return temp;
        }
        /// <summary>
        /// 过滤字符串的特殊字符（“'”、“*”）
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Clear(string s)
        {
            return s.Replace("'", "").Replace("*", "");
        }

        public static string ClearSQL(string s)
        {
            return s.Replace("'", "").Replace("*", "").Replace("?", "");
        }
        /// <summary>
        /// IP地址转换成Int64整数
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static Int64 IPToInt64(string ip)
        {
            try
            {
                string[] temp = ip.Split('.');

                if (temp.Length != 4)
                {
                    return 0;
                }

                Int64 count = 0;

                for (int i = 0; i < temp.Length; i++)
                {
                    Int64 temp_count = 1;

                    for (int j = 3 - i; j > 0; j--)
                    {
                        temp_count = temp_count * 256;
                    }

                    count += temp_count * Int64.Parse(temp[i]);
                }

                return count;
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// Int64整数字符串转换成IP地址
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string Int64ToIP(string num)
        {
            try
            {
                string temp = Int64.Parse(num).ToString("x");

                temp = temp.PadLeft(8, '0');

                string ip = "";

                for (int i = 0; i < 8; i += 2)
                {
                    ip += Int32.Parse(temp.Substring(i, 2), System.Globalization.NumberStyles.AllowHexSpecifier).ToString() + ".";
                }

                return ip.Substring(0, ip.Length - 1);
            }
            catch
            {
                return "";
            }
        }
       
       
        /// <summary>
        /// 获得字节数
        /// </summary>
        /// <param name="strTmp"></param>
        /// <returns></returns>
        public static int GetByteCount(string strTmp)
        {
            int intCharCount = 0;
            for (int i = 0; i < strTmp.Length; i++)
            {
                if (System.Text.UTF8Encoding.UTF8.GetByteCount(strTmp.Substring(i, 1)) == 3)
                {
                    intCharCount = intCharCount + 2;
                }
                else
                {
                    intCharCount = intCharCount + 1;
                }
            }
            return intCharCount;
        }

        public static string Get_S_E_TimeFromString(string str_in)
        {
            string str_out = string.Empty;
            if (!str_in.Trim().Equals(""))
            {

                string tmp = str_in.Trim();

                DateTime dt_Tdate = DateTime.Now;
                if (DateTime.TryParse(tmp, out dt_Tdate))
                {
                    str_out = dt_Tdate.ToString("yyyy-MM-dd");
                }

            }

            return str_out;
        }

        /// <summary>
        /// 将俩个字段转成JASONString格式
        /// </summary>
        /// <param name="dtSourse"></param>
        /// <returns></returns>
        public static string GetDataTableToJasonString(DataTable dtSourse)
        {
            string JasonString = "";
            string graphs = "";

            if (dtSourse.Columns.Count == 2)
            {
                JasonString += "<chart>";
                JasonString += "<series>";
               
                graphs+="<graphs>";
                graphs += "<graph gid='0'>";
                for (int j = 0; j < dtSourse.Rows.Count; j++)
                {
                    JasonString += "<value xid='" + j + "'>"+dtSourse.Rows[j][1].ToString()+" </value>";
                    graphs += "<value xid='"+j+"'>" + dtSourse.Rows[j][0].ToString() + "</value>";
                }
                graphs += "</graph>";
                graphs += "</graphs>";
                JasonString += "</series>";
                JasonString += graphs;
                JasonString += "</chart>";
            }
            return JasonString;
        }

        /// <summary>
        /// 将俩个字段转成柱状多日期JASONString格式
        /// </summary>
        /// <param name="dtSourse"></param>
        /// <returns></returns>
        public static string GetDataTableToJasonStringCloumn(DataTable dtSourse,string[] year,string[] pNum)
        {
            string JasonString = "";
            string graphs = "";
            string series = "";
            for (int n = 0; n < pNum .Length ; n++)
            {
                string value = "";
                for (int m = 0; m < year.Length; m++)
                {
                    if (dtSourse.Columns.Count == 3)
                    {
                        for (int i = 0; i < dtSourse.Rows.Count; i++)
                        {
                            if (dtSourse.Rows[i][1].ToString() == pNum[n].ToString() && dtSourse.Rows[i][2].ToString() == year[m].ToString())
                            {
                                value += "<value xid='" + m + "'>"+dtSourse.Rows[i][0].ToString ()+"</value>";
                            }
                        }

                    }
                    if (!series.Contains (year[m].ToString()))
                    {
                        series += "<value xid='" + m + "'>" + year[m].ToString() + "</value>"; 
                    }
                }

                graphs+="<graph gid='"+n+"'>";
                graphs += value;
                graphs += " </graph>";
            }
            JasonString += "<chart><series>";
            JasonString += series;
            JasonString += "</series>";
            JasonString += "<graphs>";
            JasonString += graphs;
            JasonString += "</graphs></chart>";

            return JasonString;
        }

        /// <summary>
        /// 将俩个字段转成柱状多日期JASONString格式
        /// </summary>
        /// <param name="dtSourse"></param>
        /// <returns></returns>
        public static string GetDataTableToCloumnChart_settings(string[] pNum)
        {
            string[] colors = new string[] { "69102173", "1694259", "07660", "1853113", "244203207", "2313833", "2331945", "2451840", "0161124", "23169144", "10677157", "2159178", "070118", "2286844", "236183201", "170154167", "147123175", "18115689", "154182186", "25523672", "2389958", "183214144", "2156218", "12765150", "1871541", "2532190", "3179161", "104190151", "0166166", "250229228" };
            string graphs = "";
            graphs += " <graphs>";
            for (int i = 0; i < pNum .Length ; i++)
            {
                if (!graphs.Contains (pNum[i].ToString() ))
                {
                    graphs += "<graph gid='" + i + "'>";
                    graphs += "<title>" + pNum[i].ToString() + "</title>";
                    if (i <= 29)
                    {
                        graphs += "<color>" + colors[i].ToString() + "</color>";

                    }
                    else
                    {
                        graphs += "<color>0489e6</color>";
                    }
                    graphs += "</graph>"; 
                }
            }
            graphs += " </graphs>";

            return graphs;

        }

        /// <summary>  
        /// Json特符字符过滤，参见http://www.json.org/  
        /// </summary>  
        /// <param name="sourceStr">要过滤的源字符串</param> 
        /// <returns>返回过滤的字符串</returns>  
        public static string JsonCharFilter(string sourceStr)
        {

            sourceStr = sourceStr.Replace("\\", "\\\\");

            sourceStr = sourceStr.Replace("\b", "\\\b");

            sourceStr = sourceStr.Replace("\t", "\\\t");

            sourceStr = sourceStr.Replace("\n", "\\\n");

            sourceStr = sourceStr.Replace("\n", "\\\n");

            sourceStr = sourceStr.Replace("\f", "\\\f");

            sourceStr = sourceStr.Replace("\r", "\\\r");

            return sourceStr.Replace("\"", "\\\"");

        }

        public static string GetLaiyuan(string WholeContent)
        {
            string sOUT = "";

            //string Patten_ly = @"来源[:：\s]\s*(<a.*?>)*(.*?)[\s+<]";
            string Patten_ly = @"来源[:：]\s*(<.*?>)*(.*?)[\s+<]";

            WholeContent = Regex.Replace(WholeContent, @"<!--.*?-->", "", RegexOptions.Singleline);

            WholeContent = WholeContent.Replace("&nbsp;", " ");

            MatchCollection matches = Regex.Matches(WholeContent, Patten_ly, RegexOptions.IgnoreCase);

            if (matches.Count > 0)
            {
                sOUT = matches[0].Groups[2].Value.ToString().Trim().Replace("）", "").Replace("（", "").Replace("]", "").Replace("[", "");
                sOUT = sOUT.Replace("(", "").Replace(")", "");
                sOUT = sOUT.Replace("【", "").Replace("】", "");

                if (sOUT.Length > 20)
                {
                    sOUT = "";
                }

            }

            return sOUT;
        }


        public static string StringSub(string sInput, int length)
        {
            sInput = sInput.Trim();
            if (sInput.Length > length)
            {
                return sInput.Substring(0, length).Trim();
            }
            else
            {
                return sInput;
            }


        }

        /// <summary>
        /// 转半角的函数(DBC case) 
        /// <para>全角空格为12288，半角空格为32 </para>
        /// <para>其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248</para>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToDBC(string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return "";
            }

            char[] array = input.ToCharArray();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == 12288)
                {
                    array[i] = (char)32;
                    continue;
                }
                if (array[i] > 65280 && array[i] < 65375)
                {
                    array[i] = (char)(array[i] - 65248);
                }
            }
            return new string(array);
        }

    }
}
