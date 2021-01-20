using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

namespace Common
{
    public class Myfunctions
    {

        public static string QueryString(string s)
        {
            string temp = HttpContext.Current.Request.Params[s];

            if (temp != null)
            {
                temp = ClearSQL(temp.Trim());

                return temp;
            }

            return "";
        }

        /// <summary>
        /// 获取URL值 减少过滤字符 如from之类
        /// </summary>
        /// <param name="name"></param>
        /// <param name="escape">是否转义HTML符号</param>
        /// <param name="urlDecode">是否需要URL解码</param>
        /// <returns></returns>
        public static string QueryString2016(string name, bool escape = true, bool urlDecode = false)
        {
            try
            {
                string result = HttpContext.Current.Request.Params[name];
                //throw new Exception("test");
                if (result != null)
                {
                    if (escape)
                    {
                        result = ClearSQL(result);
                    }
                    if (urlDecode && result.Contains("%"))
                    {
                        result = Uri.UnescapeDataString(result); //对应 encodeURIComponent
                    }
                    return result;
                }

                return "";
            }
            catch (Exception ex)
            {
                //string request = Common.JsonConvertEx.ObjectToJson(HttpContext.Current.Request);
                //KYCX.Logging.Logger.DefaultLogger.ErrorFormat("获取URL值异常，QueryString2016(name=\"{0}\", escape={1}, urlDecode{2})，\rHttpRequest:{3}",
                //    ex, name, escape, urlDecode, HttpHelper.GetRequestData(HttpContext.Current.Request));
                return "";
            }
        }



        public static string UrLQueryString(string s)
        {
            string temp = HttpContext.Current.Request.Params[s];

            if (temp != null)
            {
                temp = ClearSQL(Uri.UnescapeDataString(temp.Trim()));

                return temp;
            }

            return "";
        }

        public static string RequestForm(string s)
        {
            string temp = HttpContext.Current.Request.Form[s];

            if (temp != null)
            {
                return ClearSQL(temp.Trim());
            }

            return "";
        }

        private static string ClearSQL(string s)
        {
            s = s.Trim();

            double temp = 0;

            if (IsDate(s) || double.TryParse(s, out temp))
            {
                return s;
            }

            //string StrKeyWord = @"select|insert|delete|from|count\(|drop table|update|truncate|asc\(|mid\(|char\(|xp_cmdshell|exec master|netlocalgroup administrators|:|net user|""|or|and|exec";
            string StrKeyWord = @"select|insert|delete|from|count\(|drop table|update|truncate|asc\(|mid\(|char\(|xp_cmdshell|exec master|netlocalgroup administrators|:|net user|""|exec";
            string StrRegex = @"[%|*|'<]";
            //string StrRegex = @"[-|;|,|/|\(|\)|\[|\]|}|{|%|\@|*|!|']";

            s = Regex.Replace(s, StrKeyWord, "", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, StrRegex, "", RegexOptions.IgnoreCase);

            return s;
        }

        private static bool IsDate(string str)
        {
            if (str == null || str == "") return false;
            return System.Text.RegularExpressions.Regex.IsMatch(str, @"^(?ni:(?=\d)((?'year'((1[6-9])|([2-9]\d))\d\d)(?'sep'[/.-])(?'month'0?[1-9]|1[012])\2(?'day'((?<!(\2((0?[2469])|11)\2))31)|(?<!\2(0?2)\2)(29|30)|((?<=((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|(16|[2468][048]|[3579][26])00)\2\3\2)29)|((0?[1-9])|(1\d)|(2[0-8])))(?:(?=\x20\d)\x20|$))?((?<time>((0?[1-9]|1[012])(:[0-5]\d){0,2}(\x20[AP]M))|([01]\d|2[0-3])(:[0-5]\d){1,2}))?)$");
        }
        /// <summary>
        /// 设置密码
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string SetPassword(string s)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(s.ToLower() + "@check.cnki", "md5").Substring(7, 15).ToLower();
        }

        public static byte[] MakeMd5(string val)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            //BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(str))).Replace("-", null);
            return md5.ComputeHash(Encoding.UTF8.GetBytes(val));
        }

        /// <summary>
        /// 生成Excel文件
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="content">内容</param>
        public static void ExcelDownload(string filename, string content)
        {
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(filename + ".xls", System.Text.Encoding.UTF8));
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            //HttpContext.Current.Response.BinaryWrite(System.Text.Encoding.GetEncoding("big5").GetBytes(content.ToString()));
            HttpContext.Current.Response.BinaryWrite(System.Text.Encoding.Default.GetBytes(content.ToString()));
            //HttpContext.Current.ApplicationInstance.CompleteRequest();
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// 修正firefox导出excel名字乱码的问题
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="agent"></param>
        /// <returns></returns>
        public static string ConvertFileName(string fileName, string agent)
        {
            if (Regex.IsMatch(agent.ToLower(), "firefox"))
            {
                return fileName;
            }
            return Uri.EscapeDataString(fileName);
        }

        /// <summary>
        /// 生成Excel文件，并下载
        /// </summary>
        /// <param name="fileName">Excel文件名，带扩展名</param>
        /// <param name="dtTaskFlow">数据集</param>
        /// <param name="dicColumnNames">旧标题-新标题，并以此确定前后顺序</param>
        /// <param name="isWriteColumnName">是否把DataTable的列名作为第一行写入Excel</param>
        public static void ExcelDownload(string fileName, DataTable dtTaskFlow, Dictionary<string, string> dicColumnNames, bool IsWriteColumnName, string agent = "")
        {

            byte[] btFile = null;
            IWorkbook workbook = null;

            try
            {

                Reform(dicColumnNames, ref dtTaskFlow);

                string extension = Path.GetExtension(fileName);
                if (extension == ".xls")
                {
                    workbook = new HSSFWorkbook();
                }
                else if (extension == ".xlsx")
                {
                    workbook = new XSSFWorkbook();
                }

                int startCellNum_taskflow = 0;

                ISheet sheet = workbook.CreateSheet(Path.GetFileNameWithoutExtension(fileName));
                int rowIndex = 0;
                int cellIndex = 0;
                IRow row;

                for (int kkk = 0; kkk < dtTaskFlow.Rows.Count; kkk++)
                {
                    #region MyRegion


                    if (IsWriteColumnName && rowIndex == 0) //写入DataTable的列名
                    {
                        row = sheet.CreateRow(rowIndex);
                        cellIndex = startCellNum_taskflow;
                        for (int i = 0; i < dtTaskFlow.Columns.Count; i++)
                        {
                            string colName = dtTaskFlow.Columns[i].ColumnName;
                            row.CreateCell(cellIndex).SetCellValue(colName);
                            cellIndex++;
                        }

                        rowIndex++;
                    }

                    row = sheet.CreateRow(rowIndex);
                    cellIndex = startCellNum_taskflow;
                    for (int j = 0; j < dtTaskFlow.Columns.Count; j++)
                    {
                        string cellValue = dtTaskFlow.Rows[kkk][j].ToString();
                        row.CreateCell(cellIndex).SetCellValue(cellValue);
                        cellIndex++;
                    }
                    rowIndex++;

                    #endregion

                }

                KYCX.OfficeNPOI.ExcelOperation.AutoSizeColumn(sheet);

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.Write(stream);
                    btFile = stream.ToArray();
                }


                if (btFile != null)
                {
                    using (MemoryStream stream = new MemoryStream(btFile))
                    {
                        //指定块大小  
                        long chunkSize = 102400;
                        //建立一个100K的缓冲区  
                        byte[] buffer = new byte[chunkSize];
                        //未读的字节数  
                        long dataToRead = stream.Length;

                        //添加Http头  
                        HttpContext.Current.Response.ContentType = "application/octet-stream";
                        HttpContext.Current.Response.AddHeader("Content-Disposition",
                            string.Format("attachment;filename={0}", ConvertFileName(fileName, agent)));
                        HttpContext.Current.Response.AddHeader("Content-Length", dataToRead.ToString());

                        while (dataToRead > 0 && HttpContext.Current.Response.IsClientConnected)
                        {
                            int length = stream.Read(buffer, 0, (int)chunkSize);
                            HttpContext.Current.Response.BinaryWrite(buffer);
                            HttpContext.Current.Response.Flush();
                            HttpContext.Current.Response.Clear();

                            dataToRead -= length;
                        }
                        HttpContext.Current.Response.Close();
                    }
                }
                else
                {
                    throw new Exception("生成Excel文件失败。");
                }


            }
            catch (Exception ex)
            {
                btFile = null;
                //KYCX.Logging.Logger.DefaultLogger.ErrorFormat("ExcelDownload(fileName=\"{0}\", dt=\"{1}\", dicColumnNames=\"{2}\", isWriteColumnName=\"{3}\")生成Excel文件失败。", ex,
                //    fileName ?? "NULL",
                //    dt == null ? "NULL" : JsonConvertEx.ObjectToJson(dt),
                //    dicColumnNames == null ? "NULL" : JsonConvertEx.ObjectToJson(dicColumnNames),
                //    isWriteColumnName.ToString());
            }
            finally
            {
                if (workbook != null)
                {
                    workbook.Close();
                    workbook = null;
                }
            }

        }

        public static bool Reform(Dictionary<string, string> dicColumnNames, ref DataTable dt)
        {
            bool flag = true;
            try
            {
                if (dicColumnNames != null && dicColumnNames.Count > 0)
                {
                    int columnCount = dt.Columns.Count;
                    for (int i = columnCount - 1; i >= 0; i--)
                    {
                        string columnName = dt.Columns[i].ColumnName;
                        if (dicColumnNames.Keys.All(u => u != columnName))
                        {
                            dt.Columns.Remove(columnName);
                        }
                    }
                    var oldColumnNameList = dicColumnNames.Keys.ToList();
                    for (int i = 0; i < oldColumnNameList.Count; i++)
                    {
                        string oldColumnName = oldColumnNameList[i];
                        dt.Columns[oldColumnName].SetOrdinal(i);
                        string newColumnName = dicColumnNames[oldColumnName];
                        if (oldColumnName != newColumnName)
                        {
                            dt.Columns[oldColumnName].ColumnName = newColumnName;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">生成的sheet名</param>
        /// <param name="dtTaskFlow">taskflowinfo列表</param>
        /// <param name="IsWriteColumnName">是否显示表头</param>
        /// <param name="dtInnerCheckList">校内互检列表</param>
        /// <returns></returns>
        public static byte[] MakeInnerCheckExcel(string fileName, DataTable dtTaskFlow, bool IsWriteColumnName,
            List<DataTable> dtInnerCheckList)
        {

            byte[] btFile = null;
            IWorkbook workbook = null;

            try
            {
                string extension = Path.GetExtension(fileName);
                if (extension == ".xls")
                {
                    workbook = new HSSFWorkbook();
                }
                else if (extension == ".xlsx")
                {
                    workbook = new XSSFWorkbook();
                }

                int startCellNum_taskflow = 0;
                //int startCellNum_innercheck = startCellNum_taskflow + blankCount;

                ISheet sheet = workbook.CreateSheet(Path.GetFileNameWithoutExtension(fileName));
                int rowIndex = 0;
                int cellIndex = 0;
                IRow row;

                //填充表头
                if (IsWriteColumnName) //写入DataTable的列名
                {
                    row = sheet.CreateRow(rowIndex);

                    cellIndex = startCellNum_taskflow;
                    for (int i = 0; i < dtTaskFlow.Columns.Count; i++)
                    {
                        string colName = dtTaskFlow.Columns[i].ColumnName;
                        row.CreateCell(cellIndex).SetCellValue(colName);
                        cellIndex++;
                    }

                    for (int i = 0; i < dtInnerCheckList[0].Columns.Count; i++)
                    {
                        string colName = dtInnerCheckList[0].Columns[i].ColumnName;
                        row.CreateCell(cellIndex).SetCellValue(colName);
                        cellIndex++;
                    }

                    rowIndex++;
                    row = sheet.CreateRow(rowIndex);
                }
                else
                {
                    row = sheet.CreateRow(rowIndex);
                }

                for (int kkk = 0; kkk < dtTaskFlow.Rows.Count; kkk++)
                {
                    #region MyRegion

                    cellIndex = startCellNum_taskflow;
                    for (int j = 0; j < dtTaskFlow.Columns.Count; j++)
                    {
                        string cellValue = dtTaskFlow.Rows[kkk][j].ToString();
                        row.CreateCell(cellIndex).SetCellValue(cellValue);
                        cellIndex++;
                    }


                    //开始写入innercheck数据
                    DataTable tmp = dtInnerCheckList[kkk];

                    if (tmp != null && tmp.Rows.Count > 0)//空表则直接略过
                    {
                        foreach (DataRow qqq in tmp.AsEnumerable())
                        {
                            for (int j = 0; j < tmp.Columns.Count; j++)
                            {
                                string cellValue = qqq[j].ToString();
                                row.CreateCell(cellIndex).SetCellValue(cellValue);
                                cellIndex++;
                            }
                            cellIndex -= tmp.Columns.Count;

                            rowIndex++;
                            row = sheet.CreateRow(rowIndex);
                        }
                    }


                    #endregion

                }

                KYCX.OfficeNPOI.ExcelOperation.AutoSizeColumn(sheet);

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.Write(stream);
                    btFile = stream.ToArray();
                }

                return btFile;
            }
            catch (Exception ex)
            {
                throw new Exception("将集合中的数据导入到Excel文件失败。", ex);
            }
            finally
            {
                if (workbook != null)
                {
                    workbook.Close();
                    workbook = null;
                }
            }


        }

        /// <summary>
        /// 解析excel并转换到dt
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static DataTable ExcelExtract(string fileName)
        {
            DataTable dt = null;
            DataSet ds = KYCX.OfficeNPOI.ExcelOperation.ExcelToDataSet(fileName, true);

            if (ds.Tables != null && ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
                int itWholeCols = dt.Columns.Count;
                Preprocess(dt, itWholeCols);
            }

            return dt;
        }

        /// <summary>
        /// 预处理，删除空行
        /// </summary>
        /// <param name="csvsr"></param>
        /// <param name="colCount"></param>
        private static void Preprocess(DataTable csvsr, int colCount)
        {
            for (int i = csvsr.Rows.Count - 1; i >= 0; i--)
            {
                int cc = 0;
                for (int j = 0; j < colCount; j++)
                {
                    cc += csvsr.Rows[i][j].ToString().Trim().Length;
                    if (cc > colCount)
                        break;
                }

                if (cc <= 1)
                {
                    csvsr.Rows.RemoveAt(i);
                }
            }

        }

        /// <summary>
        /// JS脚本提示信息
        /// </summary>
        /// <param name="s">信息</param>
        public static void ShowMessage(string s)
        {
            Page cur = HttpContext.Current.Handler as Page;

            if (cur != null)
            {
                cur.ClientScript.RegisterStartupScript(typeof(System.String), System.Guid.NewGuid().ToString(), string.Format("alert(\"{0}\");", s), true);
            }
        }

        public static void RunJs(string s)
        {
            Page cur = HttpContext.Current.Handler as Page;

            if (cur != null)
            {
                cur.ClientScript.RegisterStartupScript(typeof(System.String), System.Guid.NewGuid().ToString(), s, true);
            }
        }

        /// <summary>
        /// 返回警示图片(短文件)
        /// </summary>
        /// <param name="word">重合字数</param>
        /// <param name="sim">相似结果</param>
        /// <param name="type">1表示同级目录,2表示2级目录</param>
        /// <returns></returns>
        public static string WarningPicture(decimal sim)
        {
            if (sim >= 0)
            {
                if (sim >= 50)
                {
                    return "<div class=\"per_r\">" + sim + "%</div>";
                }
                else if (sim >= 40)
                {
                    return "<div class=\"per_o\">" + sim + "%</div>";
                }
                else if (sim > 0)
                {
                    return "<div class=\"per_y\">" + sim + "%</div>";
                }
                else
                {
                    return "<div class=\"per_g\">" + sim + "%</div>";
                }
            }

            return "";
        }

        public static bool IsMail(string sEmailAddress)
        {
            bool fg = true;

            try
            {
                Regex r = new Regex("^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@([\\w-]+\\.)+\\w{2,5})\\s*$");
                if (!r.IsMatch(sEmailAddress))
                {
                    fg = false;
                }

            }
            catch
            {
                fg = false;
            }
            return fg;
        }
        /// <summary>
        /// 限制IP串转换成int串
        /// </summary>
        /// <param name="IPstring">不同IP或IP段间用;分割，IP段前后用,分割</param>
        /// <returns></returns>
        public static string UserLimitIPString(string IPstring)
        {
            string strOut = "";

            string[] ar_IPstring = IPstring.Split(';');

            foreach (string s_IPstring in ar_IPstring)
            {
                string TempIP = s_IPstring.Trim();
                if (!TempIP.Equals(""))
                {
                    string SingleIpSection = "";

                    string[] ar_TempIP = TempIP.Split(',');
                    if (ar_TempIP.Length < 3)
                    {
                        foreach (string s_TempIP in ar_TempIP)
                        {
                            string Temp_singleIP = s_TempIP.Trim();

                            if (Temp_singleIP.Equals(""))
                            {
                                Temp_singleIP = "255.255.255.255";
                            }
                            string BaseIpString = Common.MyTransform.IPToInt64(Temp_singleIP).ToString();
                            SingleIpSection += BaseIpString + ",";

                        }

                    }
                    SingleIpSection = SingleIpSection.Trim(',');
                    strOut += SingleIpSection + ";";
                }

            }

            strOut = strOut.Trim(';');

            return strOut;
        }

        /// <summary>
        /// 数据库中IP限制int串转换成IP串显示
        /// </summary>
        /// <param name="BaseIpString">int串</param>
        /// <returns>如是ip段用,分割</returns>
        public static string[] UserLimitIPShow(string BaseIpString)
        {
            string strOut = "";

            string[] ar_BaseIPstring = BaseIpString.Split(';');

            foreach (string s_BaseIPstring in ar_BaseIPstring)
            {
                string TempIP = s_BaseIPstring.Trim();
                if (!TempIP.Equals(""))
                {
                    string SingleIpSection = "";

                    string[] ar_TempIP = TempIP.Split(',');

                    if (ar_TempIP.Length == 1)
                    {
                        SingleIpSection = Common.MyTransform.Int64ToIP(ar_TempIP[0]);
                    }
                    else if (ar_TempIP.Length == 2)
                    {
                        SingleIpSection = Common.MyTransform.Int64ToIP(ar_TempIP[0]) + "," + Common.MyTransform.Int64ToIP(ar_TempIP[1]);
                    }
                    else
                    {

                    }

                    strOut += SingleIpSection + ";";
                }

            }

            strOut = strOut.Trim(';');


            return strOut.Split(';');
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

    }
}
