using DNTBreadCrumb.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using PJ_Source_GV.Caption;
using PJ_Source_GV.Models;
using PJ_Source_GV.Repositories;
using SSOLibCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Syncfusion.DocIO.DLS;
using AngleSharp.Html.Parser;
using AngleSharp.Html;
using Ganss.XSS;
using HtmlAgilityPack;

namespace PJ_Source_GV.FunctionSupport
{
    public static class FunctionUtility
    {
        /// <summary>
        /// Tính toán số dòng hiển thị trên 1 page
        /// </summary>
        /// <param name="rowInDb"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static int CalcTotalPage(int rowInDb, int pageSize)
        {
            int mod = rowInDb % pageSize;
            if (mod > 0)
            {
                return (rowInDb / pageSize) + 1;
            }
            return (rowInDb / pageSize);
        }

        /// <summary>
        /// Chuyển định dạng ngày thánh thành yyyy-mm-dd
        /// VD: 2018-08-20
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyy-MM-dd");
        }


        /// <summary>
        /// Chuyển định dạng ngày thánh thành yyyy-mm-dd
        /// VD: 2018-08-20
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String ConvertDateTimeToString(DateTime value)
        {
            return value.ToString("yyyyMMdd");
        }

        /// <summary>
        /// mã hóa mật khẩu
        /// </summary>
        /// <param name="toEncrypt">chuỗi cần mã hóa</param>
        /// <returns>chuỗi đã được mã hóa</returns>
        public static string Encrypt(string toEncrypt)
        {
            bool useHashing = true;
            byte[] keyArray;
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(ConstValue.KeyEncrypt));
            }
            else
                keyArray = Encoding.UTF8.GetBytes(ConstValue.KeyEncrypt);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// giải mã mật khẩu
        /// </summary>
        /// <param name="toDecrypt">chuỗi cần giải mã</param>
        /// <returns>chuỗi đã được giải mã</returns>
        public static string Decrypt(string toDecrypt)
        {
            bool useHashing = true;
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(ConstValue.KeyEncrypt));
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(ConstValue.KeyEncrypt);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        /// <summary>
        /// Hàm Dynamic Order cho linq hỗ trợ truyền tên cột và thứ tự sort
        /// </summary>
        /// <param name="query">Lệnh query đang sử dụng để sort</param>
        /// <param name="sortColumn">Tên cột cần sort</param>
        /// <param name="descending">true: desc, false: asc</param>
        /// <returns> Danh sách đã Sort </returns>
        public static IQueryable<t> OrderByDynamic<t>(this IQueryable<t> query, string sortColumn, bool descending)
        {
            // Dynamically creates a call like this: query.OrderBy(p =&gt; p.SortColumn)
            var parameter = Expression.Parameter(typeof(t), "p");

            string command = "OrderBy";

            if (descending)
            {
                command = "OrderByDescending";
            }

            Expression resultExpression = null;

            var property = typeof(t).GetProperty(sortColumn);
            // this is the part p.SortColumn
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);

            // this is the part p =&gt; p.SortColumn
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);

            // finally, call the "OrderBy" / "OrderByDescending" method with the order by lamba expression
            resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { typeof(t), property.PropertyType },
               query.Expression, Expression.Quote(orderByExpression));

            return query.Provider.CreateQuery<t>(resultExpression);
        }

        /// <summary>
        /// Hàm Map properties 2 class tương tự nhau
        /// </summary>
        /// <param name="source">Đối tượng nguồn</param>
        /// <typeparam name="T1">Class đích</typeparam>
        /// <typeparam name="T2">Class nguồn</typeparam>
        /// <returns> Class đích đã map properties </returns>
        public static T1 Map<T1, T2>(T2 source) where T1 : new() where T2 : class
        {
            T1 result = new T1();
            foreach (var pro in typeof(T1).GetProperties())
            {
                string proName = pro.Name;
                var value = source.GetType().GetProperty(proName).GetValue(source, null);
                result.GetType().GetProperty(proName).SetValue(result, value, null);
            }
            return result;
        }

        /// <summary>
        /// Hàm Map properties DataRow vào 1 đối tượng
        /// </summary>
        /// <param name="source">Đối tượng nguồn</param>
        /// <typeparam name="T1">Class đích</typeparam>
        /// <returns> Class đích đã map properties </returns>
        public static T Map<T>(DataRow source) where T : new()
        {
            T result = new T();
            foreach (var pro in typeof(T).GetProperties())
            {
                string proName = pro.Name;
                if (!source.Table.Columns.Contains(proName))
                {
                    continue;
                }
                var value = source[proName];
                if (value is DBNull)
                {
                    value = null;
                }
                result.GetType().GetProperty(proName).SetValue(result, value, null);
            }
            return result;
        }

        /// <summary>
        /// Hàm chuyển byte sang chuỗi
        /// </summary>
        /// <param name="byteCount">Giá trị byte</param>
        /// <returns> Chuỗi quy đổi </returns>
        public static string BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + " " + suf[place];
        }

        /// <summary>
        /// Hàm chuyển đổi html sang text
        /// </summary>
        /// <param name="htmlString"></param>
        /// <returns></returns>
        public static string GetPlainTextFromHtml(string htmlString)
        {
            if (!string.IsNullOrEmpty(htmlString))
            {
                string htmlTagPattern = "<.*?>";
                var regexCss = new Regex("(\\<script(.+?)\\</script\\>)|(\\<style(.+?)\\</style\\>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                htmlString = regexCss.Replace(htmlString, string.Empty);
                htmlString = Regex.Replace(htmlString, htmlTagPattern, string.Empty);
                htmlString = Regex.Replace(htmlString, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
                htmlString = htmlString.Replace("&nbsp;", string.Empty);
            }

            return htmlString;
        }

        /// <summary>
        /// Hàm tạo group cho nhiều select list
        /// </summary>
        /// <param name="selectList"></param>
        /// <returns></returns>
        public static List<SelectListItem> CreateSelectListGroup(params (SelectList items, string groupName)[] selectList)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            foreach (var e in selectList)
            {
                var group = new SelectListGroup { Name = e.groupName };
                foreach (var item in e.items)
                {
                    result.Add(new SelectListItem
                    {
                        Value = item.Value,
                        Text = item.Text,
                        Group = group
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// Hàm mã hóa MD5
        /// </summary>
        /// <param name="str">Chuỗi cần mã hóa</param>
        /// <returns> Chuỗi đã mã hóa </returns>
        public static string ToMD5(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bHash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            StringBuilder sbHash = new StringBuilder();
            foreach (byte b in bHash)
            {
                sbHash.Append(String.Format("{0:x2}", b));
            }
            return sbHash.ToString();
        }

        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    if (prop.PropertyType == typeof(DateTime) && (DateTime)prop.GetValue(item) == new DateTime())
                    {
                        row[prop.Name] = DBNull.Value;
                    }
                    else
                    {
                        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                    }
                }
                    
                table.Rows.Add(row);
            }
            return table;
        }

        public static List<WTable> GetListTable(this WTableCollection tables)
        {
            var lstTable = new List<WTable>();
            foreach (WTable table in tables)
            {
                lstTable.Add(table);
            }
            return lstTable;
        }

        /// <summary>
        /// Gán log cho object
        /// </summary>
        /// <returns></returns>
        public static void InitLogs(this object obj, string lastOperator, string PG_ID)
        {
            string now = DateTime.Now.ToString(ConstValue.StringFormat);
            obj.GetType().GetProperty("LastOperator").SetValue(obj, lastOperator, null);
            if (obj.GetType().GetProperty("RegisterDate").PropertyType == typeof(string))
            {
                obj.GetType().GetProperty("RegisterDate").SetValue(obj, now, null);
            }
            else
            {
                obj.GetType().GetProperty("RegisterDate").SetValue(obj, DateTime.Now, null);
            }
            if (obj.GetType().GetProperty("LastDate").PropertyType == typeof(string))
            {
                obj.GetType().GetProperty("LastDate").SetValue(obj, now, null);
            }
            else
            {
                obj.GetType().GetProperty("LastDate").SetValue(obj, DateTime.Now, null);
            }
            obj.GetType().GetProperty("PG_ID").SetValue(obj, PG_ID, null);
            if (obj.GetType().GetProperty("DataVersion") != null)
            {
                obj.GetType().GetProperty("DataVersion").SetValue(obj, DateTime.Now.ToString("yyyyMMddHHmmssffff"), null);
            }
        }

        /// <summary>
        /// chuyển kiểu chuỗi về kiểu int?
        /// </summary>
        /// <returns></returns>
        public static int? ParseInt(string number)
        {
            try
            {
                return int.Parse(number);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Gán tiêu đề cho BreadCrumb
        /// </summary>
        /// <returns></returns>
        public static void InitBreadCrumbTitle(this PrivateCoreController controller, string titleIndex, string title)
        {
            if (controller.ControllerContext.RouteData.Values["controller"].ToString().ToLower() == "vanbanchiluu")
            {
                controller.GetBreadCrumbs();
                controller.AddBreadCrumb(new BreadCrumb
                {
                    Title = titleIndex,
                    Url = "/vanbanchiluu",
                    Order = 0
                });
            }
            controller.SetBreadCrumbTitle(controller.Url.Action("Index"), titleIndex);
            controller.ViewData["Title"] = title;
            controller.SetCurrentBreadCrumbTitle(title);
        }

        public static string GetCorrectHtml(this string html)
        {
            if (string.IsNullOrEmpty(html))
                return "";

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            HtmlNodeCollection styles = doc.DocumentNode.SelectNodes("//table[@style]");
            if (styles != null)
            {
                foreach (HtmlNode e in styles)
                {
                    var lstValue = e.Attributes["style"].Value.Split(";").ToList();
                    
                    e.Attributes["style"].Value = string.Join(";", lstValue.Where(d => !d.Trim().StartsWith("width")));
                }
            }

            //HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//table//*");
            //foreach (HtmlNode node in nodes)
            //{
            //    node.InnerHtml = RemoveUnwantedHtmlTags(node.InnerHtml, new List<string>() { "p", "span" });
            //}

            HtmlNodeCollection elements = doc.DocumentNode.SelectNodes("//*");         
            foreach (HtmlNode e in elements)
            {
                e.Attributes.Remove("dir");
                //e.Attributes.Remove("style");
                if (e.Attributes["style"] != null)
                {
                    var lstValue = e.Attributes["style"].Value.Split(";").ToList();
                    e.Attributes["style"].Value = string.Join(";", lstValue.Where(d => !d.Trim().StartsWith("line-height") || !d.Trim().StartsWith("font-family")));
                }                
            }

            string output = doc.DocumentNode.OuterHtml;
            output = output.Replace("<p></p>", "");

            return output;
        }

        public static string RemoveUnwantedHtmlTags(this string html, List<string> unwantedTags)
        {
            if (String.IsNullOrEmpty(html))
            {
                return html;
            }

            var document = new HtmlDocument();
            document.LoadHtml(html);

            HtmlNodeCollection tryGetNodes = document.DocumentNode.SelectNodes("./*|./text()");

            if (tryGetNodes == null || !tryGetNodes.Any())
            {
                return html;
            }

            var nodes = new Queue<HtmlNode>(tryGetNodes);

            while (nodes.Count > 0)
            {
                var node = nodes.Dequeue();
                var parentNode = node.ParentNode;

                var childNodes = node.SelectNodes("./*|./text()");

                if (childNodes != null)
                {
                    foreach (var child in childNodes)
                    {
                        nodes.Enqueue(child);
                    }
                }

                if (unwantedTags.Any(tag => tag == node.Name))
                {
                    if (childNodes != null)
                    {
                        foreach (var child in childNodes)
                        {
                            parentNode.InsertBefore(child, node);
                        }
                    }

                    parentNode.RemoveChild(node);

                }
            }

            return document.DocumentNode.InnerHtml;
        }

        public static string ReplaceLinks(this string arg)
        {
            Regex urlregex = new Regex(@"(^|[\n ])(?<url>(www|ftp)\.[^ ,""\s<]*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            arg = urlregex.Replace(arg, " <a href=\"http://${url}\" target=\"_blank\">${url}</a>");
            Regex httpurlregex = new Regex(@"(^|[\n ])(?<url>(http://www\.|http://|https://)[^ ,""\s<]*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            arg = httpurlregex.Replace(arg, " <a href=\"${url}\" target=\"_blank\">${url}</a>");
            Regex emailregex = new Regex(@"(?<url>[a-zA-Z_0-9.-]+\@[a-zA-Z_0-9.-]+\.\w+\s)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            arg = emailregex.Replace(arg, " <a href=\"mailto:${url}\">${url}</a> ");
            return arg;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}
