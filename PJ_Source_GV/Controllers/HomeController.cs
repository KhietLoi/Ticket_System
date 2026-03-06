using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using CAIT.SQLHelper;
using DCSL.DatabaseFactory;
using DNTBreadCrumb.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
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
    public class HomeController : PrivateCoreController
    {
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public HomeController(IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _sharedLocalizer = sharedLocalizer;
        }

        [LecturerLoginCheck]
        public IActionResult Index()
        {
            //Info Page
            this.InitBreadCrumbTitle(_sharedLocalizer["Menu_Home"], _sharedLocalizer["Menu_Home"]);
            var cultureInfo = HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.UICulture;

            return View();
        }

        public new ActionResult Logout()
        {
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            ActionResult actionResult = base.Logout();
            HttpContext.Session.Clear();
            return actionResult;
        }

        [HttpPost]
        public async Task<IActionResult> SetPhongBan(string maPhongBan)
        {
            var identity = (ClaimsIdentity)HttpContext.User.Identity;
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            var listPhongBan = JsonConvert.DeserializeObject<AuthService.KiemNhiemInfo[]>(identity.FindFirst("ListKhoa").Value).ToList();
            var phongBan = listPhongBan.FirstOrDefault(d => d.MaBoPhan == maPhongBan);
            if (phongBan != null)
            {
                identity.RemoveClaim(identity.FindFirst("KhoaID"));
                identity.RemoveClaim(identity.FindFirst("TenKhoa"));
                identity.AddClaim(new Claim("KhoaID", phongBan.MaBoPhan));
                identity.AddClaim(new Claim("TenKhoa", phongBan.TenBoPhan));
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return Json(true);
            }
            return Json(false);
        }

        /// <summary>
        /// set ngon ngu khi thay doi select ngon ngu
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public IActionResult SetLanguage(string culture, string returnUrl = "~/")
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        [HttpPost]
        [Authorize(Roles = "chuyendoitk_view")]
        public async Task<IActionResult> SetNhanVien(string maNhanVien)
        {
            var listNhanVien = await NhanVienSV.GetAllStaff();
            var nhanviens = listNhanVien.Where(d => d.MaNhanVien == maNhanVien).ToList();
            var email = nhanviens.FirstOrDefault().Email;

            //Thong tin co ban
            var nhanvien = nhanviens.FirstOrDefault();

            var lstKiemNhiem = new List<AuthService.KiemNhiemInfo>();
            nhanviens.ForEach(d =>
            {
                lstKiemNhiem.Add(new AuthService.KiemNhiemInfo
                {
                    TenBoPhan = d.TenBoPhan,
                    MaBoPhan = d.MaKhoa
                });
            });

            //thêm thông tin Authorization
            ClaimsIdentity userIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);// claims, "login");
            ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
            string roles = Areas.API.Models.Permission.getPermissionString(nhanvien.Email, nhanvien.MaKhoa);
            string[] lstRole = roles.Split(new Char[] { ',' });
            Claim cl;
            userIdentity.AddClaim(new Claim(ClaimTypes.Email, nhanvien.Email));
            userIdentity.AddClaim(new Claim(ClaimTypes.Name, nhanvien.Hoten));
            userIdentity.AddClaim(new Claim("MNV", nhanvien.MaNhanVien));
            userIdentity.AddClaim(new Claim("KhoaID", nhanvien.MaKhoa));
            userIdentity.AddClaim(new Claim("TenKhoa", nhanvien.TenBoPhan));
            userIdentity.AddClaim(new Claim("MaChucVu", nhanvien.MaLoaiNV));
            userIdentity.AddClaim(new Claim("ListKhoa", JsonConvert.SerializeObject(lstKiemNhiem)));

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Json(true);
        }

        [LecturerLoginCheck]
        [BreadCrumb(Order = 1)]
        public IActionResult About()
        {
            // Info Page
            this.InitBreadCrumbTitle(_sharedLocalizer["Menu_Home"], _sharedLocalizer["ThongTinPhanMem"]);
            var cultureInfo = HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.UICulture;

            return View();
        }

        /// <summary>
        /// Trang error
        /// dev: Tuanlm
        /// </summary>
        /// <returns></returns>
        [BreadCrumb(Title = "Error", Order = 1)]
        public IActionResult Error()
        {
            var ex = HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (ex != null)
            {
                ViewBag.Message = ex.Error.Message;
                ViewBag.StackTrace = ex.Error.StackTrace;
            }

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
