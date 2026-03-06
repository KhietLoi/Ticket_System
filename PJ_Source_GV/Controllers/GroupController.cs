using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DCSL.DatabaseFactory;
using DNTBreadCrumb.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using PJ_Source_GV.Areas.API.Models;
using PJ_Source_GV.Caption;
using PJ_Source_GV.FunctionSupport;
using PJ_Source_GV.Models;
using PJ_Source_GV.Repositories;
using PJ_Source_GV.Services;
using SSOLibCore;

namespace PJ_Source_GV.Controllers
{
    [BreadCrumb(UseDefaultRouteUrl = true, Order = 0)]
    [LecturerLoginCheck]
    public class GroupController : PrivateCoreController
    {
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public GroupController(IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _sharedLocalizer = sharedLocalizer;
        }

        [BreadCrumb(Order = 1)]
        [Authorize(Roles = "admin")]
        public ActionResult Index()
        {
            //Info Page
            //this.InitBreadCrumbTitle(_sharedLocalizer["GroupMenu"], _sharedLocalizer["GroupMenu"]);
            this.InitBreadCrumbTitle("Quản trị", "Phân quyền");
            var cultureInfo = HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.UICulture;

            DEntity<Group> e = new DEntity<Group>(ConstValue.ConnectionString, Group.getTableName());
            List<Group> g = e.getAll();
            return View(g);
            /*    return Content(User.IsInRole("admin").ToString());*/
        }

        [BreadCrumb(Order = 1)]
        //[Authorize(Roles = "group_insert,group_update")]
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int id = 0)
        {
            //Info Page
            //this.InitBreadCrumbTitle(_sharedLocalizer["GroupMenu"], _sharedLocalizer["Update"]);
            var cultureInfo = HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.UICulture;

            ViewData["group_id"] = id;
            return View();
        }

        [BreadCrumb(Order = 1)]
        //[Authorize(Roles = "group_update,phancongtruongdonvi_view,phancongvanthu_view")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Member(int id)
        {
            //Info Page
            //this.InitBreadCrumbTitle(_sharedLocalizer["GroupMenu"], _sharedLocalizer["MemberofGroupForm"]);
            var cultureInfo = HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.UICulture;
            var identity = (ClaimsIdentity)HttpContext.User.Identity;
            if (!User.IsInRole("group_update"))
            {
                bool permission = true;
                if (User.IsInRole("phancongtruongdonvi_view") && id == 3)
                {
                    permission = true;
                }
                else if (User.IsInRole("phancongvanthu_view") && id == 4)
                {
                    permission = true;
                }
                if (!permission)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
            }

            DEntity<Group> eg = new DEntity<Group>(ConstValue.ConnectionString, Group.getTableName());
            ViewData["group_id"] = id;
            ViewData["group_name"] = eg.getAll().FirstOrDefault(d => d.id == id).name;

            DEntity<Member> e = new DEntity<Member>(ConstValue.ConnectionString, Areas.API.Models.Member.getTableName());
            e.setPrimaryKey("id");
            List<Member> lst = e.getList("group_id", id);
            List<NhanVienSVModel> staff = await NhanVienSV.GetAllStaff();
            List<NhanVienSVModel> members = staff.Where(d => lst.Any(x => x.email == d.Email)).ToList();
            members.AddRange(lst.Where(d => !members.Any(x => x.Email == d.email)).Select(d => new NhanVienSVModel
            {
                MaNhanVien = d.MaNhanVien,
                Hoten = d.Hoten,
                MaKhoa = d.MaKhoa,
                TenBoPhan = d.TenBoPhan,
                MaLoaiNV = d.MaLoaiNV,
                TenLoaiNV = d.TenLoaiNV,
                Email = d.email
            }));

            if (!User.IsInRole("group_update") && User.IsInRole("phancongvanthu_view") && !User.IsInRole("phancongvanthu_total") && id == 4)
            {
                members = members.Where(d => d.MaKhoa == identity.FindFirst("KhoaID").Value).ToList();
            }

            return View(members);
        }

        [BreadCrumb(Order = 1)]
        //[Authorize(Roles = "group_update")]
        [Authorize(Roles = "admin")]
        public ActionResult Permission(int id)
        {
            //Info Page
            //this.InitBreadCrumbTitle(_sharedLocalizer["GroupMenu"], _sharedLocalizer["PermissionofGroupForm"]);
            var cultureInfo = HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.UICulture;

            ViewData["group_id"] = id;
            ViewData["functions"] = Areas.API.Models.Permission.getAllFunction();
            ViewData["permissions"] = Areas.API.Models.Permission.getAll(id);
            var lstPage = PageRes.Page_GetAll();
            if (User.IsInRole("subpergroup_view") == false || User.IsInRole("ad_view") == false)
            {
                lstPage = lstPage.Where(p => p.alias != "subpergroup").ToList();
                lstPage = lstPage.Where(p => p.alias != "ad").ToList();
            }
            ViewData["page"] = lstPage;
            return View();
        }

        //[Authorize(Roles = "group_update")]
        [Authorize(Roles = "admin")]
        public JsonResult Insert(int group_id, int page_id)
        {
            var permisstionExists = PermissionRes.Permission_GetAll().Where(x => x.group_id == group_id && x.page_id == page_id).ToList();
            if (permisstionExists.Count != 0)
            {
                return Json("Exists");
            }
            int result = PermissionRes.insert(group_id, page_id, 0);
            return Json(result);
        }
        
        //[Authorize(Roles = "group_update,phancongtruongdonvi_view,phancongvanthu_view")]
        [Authorize(Roles = "admin")]
        public async Task<JsonResult> GetAllMemberUnit(string email)
        {
            var listNhanVien = await NhanVienSV.GetAllStaff();
            var nhanviens = listNhanVien.Where(d => d.Email == email).ToList();

            var lstKiemNhiem = new List<AuthService.KiemNhiemInfo>();
            nhanviens.ForEach(d =>
            {
                lstKiemNhiem.Add(new AuthService.KiemNhiemInfo
                {
                    TenBoPhan = d.TenBoPhan,
                    MaBoPhan = d.MaKhoa
                });                
            });

            return Json(lstKiemNhiem);
        }
    }
}
