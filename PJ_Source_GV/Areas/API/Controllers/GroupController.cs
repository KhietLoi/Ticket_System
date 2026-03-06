using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    
    public class GroupController : Controller
    {
        [Route("API/Group/GetAllGroup")]
        [Area("API")]
        public ActionResult GetAllGroup()
        {
            DEntity<Group> e = new DEntity<Group>(ConstValue.ConnectionString, Group.getTableName());
            return Json(e.getAll());
        }
        [Route("API/Group/GetGroup")]
        [Area("API")]
        public ActionResult GetGroup(int id)
        {
            DEntity<Group> e = new DEntity<Group>(ConstValue.ConnectionString, Group.getTableName());
            return Json(e.get("id", id));
        }

        [Route("API/Group/InsertGroup")]
        [Area("API")]
        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult InsertGroup(IFormCollection form)
        {
            int id = Int32.Parse(form["id"].ToString());
            string name = form["name"].ToString();
            string note = WebUtility.HtmlDecode(form["note"].ToString());
            // Group g = new Group(id, name, note);
            string email = HttpContext.User.Claims.Where(c => c.Type == System.Security.Claims.ClaimsIdentity.DefaultNameClaimType).FirstOrDefault().Value;

            object[] value = { id, name, note, email};
            var errorCode = 0;
            var errorMessage = "";
            string[] output = { };
            var result = GroupRes.Group_Save(value, ref output, ref errorCode, ref errorMessage);
            return Json(result);
        }
        [Route("API/Group/UpdateGroup")]
        [Area("API")]
        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult UpdateGroup(IFormCollection form)
        {
            int id = Int32.Parse(form["id"].ToString());
            string name = form["name"].ToString();
            string note = WebUtility.HtmlDecode(form["note"].ToString());
            //Group g = new Group(id, name, note);
            string email = HttpContext.User.Claims.Where(c => c.Type == System.Security.Claims.ClaimsIdentity.DefaultNameClaimType).FirstOrDefault().Value;

            object[] value = { id,name, note, email };
            var errorCode = 0;
            var errorMessage = "";
            string[] output = { };
            var result = GroupRes.Group_Save(value, ref output, ref errorCode, ref errorMessage);
            return Json(result);
        }
        [Route("API/Group/DeleteGroup")]
        [Area("API")]
        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult DeleteGroup(int id)
        {
            // Group g = new Group(id, name, note);
            string email = HttpContext.User.Claims.Where(c => c.Type == System.Security.Claims.ClaimsIdentity.DefaultNameClaimType).FirstOrDefault().Value;

            object[] value = { id, email };
            var errorCode = 0;
            var errorMessage = "";
            string[] output = { };
            var result = GroupRes.Group_Delete(value, ref output, ref errorCode, ref errorMessage);
            return Json(result);
        }

        [Route("API/Group/DeleteMember")]
        [Area("API")]
        //[Authorize(Roles = "group_update,phancongtruongdonvi_view,phancongvanthu_view")]
        [Authorize(Roles = "admin")]
        public JsonResult DeleteMember(int group_id, string email, string madonvi)
        {
            if (!User.IsInRole("group_update"))
            {
                bool permission = true;
                if (User.IsInRole("phancongtruongdonvi_view") && group_id == 3)
                {
                    permission = true;
                }
                else if (User.IsInRole("phancongvanthu_view") && group_id == 4)
                {
                    permission = true;
                }
                if (!permission)
                {
                    return Json(false);
                }
            }

            object[] value = { group_id, email };
            var errorCode = 0;
            var errorMessage = "";
            string[] output = { };
            var result = MemberRes.DeleteMember(value, ref output, ref errorCode, ref errorMessage);
            return Json(result);
        }

        [Route("API/Group/InsertMember")]
        [Area("API")]
        //[Authorize(Roles = "group_update,phancongtruongdonvi_view,phancongvanthu_view")]
        [Authorize(Roles = "admin")]
        public JsonResult InsertMember(int group_id, string email, string madonvi)
        {
            if (!User.IsInRole("group_update"))
            {
                bool permission = true;
                if (User.IsInRole("phancongtruongdonvi_view") && group_id == 3)
                {
                    permission = true;
                }
                else if (User.IsInRole("phancongvanthu_view") && group_id == 4)
                {
                    permission = true;
                }
                if (!permission)
                {
                    return Json(false);
                }
            }

            var memberExists = MemberRes.GetAll().Where(x => x.group_id == group_id && x.email == email).ToList();
            if (memberExists.Count != 0)
            {
                return Json("Exists");
            }
            object[] value = { group_id, email };
            var errorCode = 0;
            var errorMessage = "";
            string[] output = { };
            var result = MemberRes.InsertMember(value, ref output, ref errorCode, ref errorMessage);
            return Json(result);
        }


        public ActionResult GetPermission(int id)
        {
            return Json(Permission.getAll(id));
        }

        [Area("API")]
        [Authorize(Roles = "admin")]
        public ActionResult InsertPermission(FormCollection form)
        {
            int group_id = Int32.Parse(form["group_id"].ToString());
            int page_id = Int32.Parse(form["page_id"].ToString());
            if (Permission.insert(group_id, page_id, 0) > 0)
                return Json(new { success = true });
            return Json(new { success = false });
        }

        [Route("API/Group/UpdatePermission")]
        [Area("API")]
        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult UpdatePermission(IFormCollection form)
        {
            int id = Int32.Parse(form["id"].ToString());
            int group_id = Int32.Parse(form["group_id"].ToString());
            int page_id = Int32.Parse(form["page_id"].ToString());
            int permission = Int32.Parse(form["permission"].ToString());
            if (Permission.update(id, group_id, page_id, permission) > 0)
                return Json(new { success = true });
            return Json(new { success = false });
        }

        public ActionResult GetAllFunction()
        {
            DEntity<Function> e = new DEntity<Function>(ConstValue.ConnectionString, Function.getTableName());
            e.setPrimaryKey("id");
            return Json(e.getAll());
        }

        public ActionResult GetAllPageFunction()
        {
            DEntity<Page> e = new DEntity<Page>(ConstValue.ConnectionString, Page.getTableName());
            return Json(e.getAll());
        }

        public ActionResult GetPageUnMarnage(int id)
        {
            return Json(Page.getPageUnManager(id));
        }

        [Route("API/Group/GetListMember/{id}")]
        [Area("API")]
        public ActionResult GetListMember(int id)
        {
            DEntity<Member> e = new DEntity<Member>(ConstValue.ConnectionString, Member.getTableName());
            e.setPrimaryKey("id");
            return Json(e.getList("group_id", id));
        }
        /*
        public ActionResult InsertMember(FormCollection form)
        {
            DEntity<Member> e = new DEntity<Member>(ConstValue.ConnectionString, Member.getTableName());
            e.setPrimaryKey("id");
            int group_id = Int32.Parse(form["group_id"].ToString());
            string user_id = form["email"].ToString();
            Member m = new Member();
            m.group_id = group_id;
            m.email = user_id;
            if (e.insert(m) > 0)
                return Json(new { success = true });
            return Json(new { success = false });
        }

        public ActionResult DeleteMember(int id)
        {
            DEntity<Member> e = new DEntity<Member>(ConstValue.ConnectionString, Member.getTableName());
            e.setPrimaryKey("id");
            if (e.delete(new DCondition("id", id)) > 0)
                return Json(new { success = true });
            else
                return Json(new { success = false });
        }*/
    }
}