using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using DCSL.DatabaseFactory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PJ_Source_GV.Areas.API.Models;
using PJ_Source_GV.Caption;
using PJ_Source_GV.Models;
using PJ_Source_GV.Repositories;

namespace PJ_Source_GV.Areas.API.Controllers
{
    public class PageController : Controller
    {

        [Route("API/Page/GetAllPage")]
        [Area("API")]
        public ActionResult GetAllPage()
        {
            DEntity<Page> e = new DEntity<Page>(ConstValue.ConnectionString, Page.getTableName());
            return Json(e.getAll());
        }

        [Route("API/Page/GetPage")]
        [Area("API")]
        public ActionResult GetPage(int id)
        {
            DEntity<Page> e = new DEntity<Page>(ConstValue.ConnectionString, Page.getTableName());
            return Json(e.get("id", id));
        }

        [Route("API/Page/InsertPage")]
        [Area("API")]
        [Authorize(Roles ="admin")]
        public ActionResult InsertPage(IFormCollection form)
        {
            int id = Int32.Parse(form["id"].ToString());
            string name = form["name"].ToString();
            string alias = form["alias"].ToString();
            int permission = Convert.ToInt32(form["permission"].ToString());
            string note = WebUtility.HtmlDecode(form["note"].ToString());
            string email = HttpContext.User.Claims.Where(c => c.Type == System.Security.Claims.ClaimsIdentity.DefaultNameClaimType).FirstOrDefault().Value ;

            object[] value = { id, name, alias, permission, note, email };
            var errorCode = 0;
            var errorMessage = "";
            string[] output = { };
            var result = PageRes.Save(value, ref output, ref errorCode, ref errorMessage);
            return Json(result);
        }

        [Route("API/Page/UpdatePage")]
        [Area("API")]
        [Authorize(Roles = "admin")]
        public ActionResult UpdatePage(IFormCollection form)
        {
            int id = Int32.Parse(form["id"].ToString());
            string name = form["name"].ToString();
            string alias = form["alias"].ToString();
            int permission = Convert.ToInt32(form["permission"].ToString());
            string note = WebUtility.HtmlDecode(form["note"].ToString());
            string email = HttpContext.User.Claims.Where(c => c.Type == System.Security.Claims.ClaimsIdentity.DefaultNameClaimType).FirstOrDefault().Value;

            object[] value = { id, name, alias, permission, note , email };
            var errorCode = 0;
            var errorMessage = "";
            string[] output = { };
            var result = PageRes.Save(value, ref output, ref errorCode, ref errorMessage);
            return Json(result);
        }

        [Route("API/Page/DeletePage")]
        [Area("API")]
        [Authorize(Roles = "admin")]
        public ActionResult DeletePage(int id)
        {
            string email = HttpContext.User.Claims.Where(c => c.Type == System.Security.Claims.ClaimsIdentity.DefaultNameClaimType).FirstOrDefault().Value;

            object[] value = { id, email };
            var errorCode = 0;
            var errorMessage = "";
            string[] output = { };
            var result = PageRes.Delete(value, ref output, ref errorCode, ref errorMessage);
            return Json(result);
        }

    }
}