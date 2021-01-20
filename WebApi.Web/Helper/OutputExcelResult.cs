using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Formatting;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using WebApi.Entity;

namespace WebApi.web.Helper
{
    public class OutputExcelResult<T> : IHttpActionResult
    {
        HttpRequestMessage _request;
        private string _fileName;
        private Dictionary<string, string> _dicColumnNames;
        private IEnumerable<T> _source;

        public OutputExcelResult(HttpRequestMessage request, string fileName,
            Dictionary<string, string> dicColumnNames, IEnumerable<T> source)
        {
            _request = request;
            _fileName = fileName + ".xlsx";
            _dicColumnNames = dicColumnNames;
            _source = source;

        }


        public Task<HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
        {

            MediaTypeHeaderValue _mediaType = MediaTypeHeaderValue.Parse("application/octet-stream");//指定文件类型

            string agent = _request.Headers.UserAgent.Count > 0 ? _request.Headers.UserAgent.First().ToString() : _request.Headers.UserAgent.ToString();

            ContentDispositionHeaderValue _disposition = ContentDispositionHeaderValue.Parse("attachment;filename="
                + ConvertFileName(_fileName, agent));//指定文件名称（编码中文）
            try
            {
                int dateTimeColNum = 0;

                List<string> hearder = new List<string>(_dicColumnNames.Values);

                List<MemberInfo> filterList = new List<MemberInfo>();

                int colNum = 1;
                foreach (var item in _dicColumnNames)
                {
                    PropertyInfo info = typeof(RequestObject).GetProperty(item.Key);

                    filterList.Add(info);

                    if (info.PropertyType.FullName.ToLower().Contains("datetime"))
                        dateTimeColNum = colNum;

                    colNum++;
                }

                ExcelPackage package = new ExcelPackage();

                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("sheet1");

                for (int i = 0; i < hearder.Count; i++)
                {
                    worksheet.Cells[1, i + 1].Value = hearder[i];
                }

                using (var range = worksheet.Cells[1, 1, 1, hearder.Count])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                    range.Style.Font.Color.SetColor(Color.White);
                }

                var dataRange = worksheet.Cells["A2"].LoadFromCollection(
                    _source, false,
                    OfficeOpenXml.Table.TableStyles.Medium2,
                    BindingFlags.Instance | BindingFlags.Public,
                    filterList.ToArray());

                if (dateTimeColNum > 0)
                    worksheet.Cells[2, dateTimeColNum, dataRange.End.Row, dateTimeColNum].Style.Numberformat.Format = "mm-dd-yy";

                dataRange.AutoFitColumns();

                var resp = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(package.GetAsByteArray()),
                    RequestMessage = _request
                };

                //HttpResponseMessage resp = _request.CreateResponse();
                //resp.Content = new ByteArrayContent(package.GetAsByteArray());

                resp.Content.Headers.ContentType = _mediaType;
                resp.Content.Headers.ContentDisposition = _disposition;
                resp.Content.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");

                return Task.FromResult(resp);

            }
            catch (Exception ex)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            finally
            {

            }
        }

        /// <summary>
        /// 修正firefox导出excel名字乱码的问题
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="agent"></param>
        /// <returns></returns>
        public string ConvertFileName(string fileName, string agent)
        {
            if (Regex.IsMatch(agent.ToLower(), "firefox"))
            {
                return fileName;
            }
            return Uri.EscapeDataString(fileName);
        }



    }



}
