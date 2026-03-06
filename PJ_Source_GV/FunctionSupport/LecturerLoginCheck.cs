using DCSL.DatabaseFactory;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using PJ_Source_GV.Areas.API.Models;
using PJ_Source_GV.Caption;
using SSOLibCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PJ_Source_GV.FunctionSupport
{
    public class LecturerLoginCheck : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
        {
            if ((filterContext.ActionDescriptor as ControllerActionDescriptor).MethodInfo.GetCustomAttributes(typeof(AnonymousLoginCheck), false).Any())
            {
                await next();
                return; 
            }

            PrivateCoreController controller = (PrivateCoreController)filterContext.Controller;
            var actionResult = controller.OnLoad();
            if (actionResult != null)
            {
                filterContext.Result = actionResult;
                return;
            }

            /*   if (controller.CurrentUser == null)
               {
                   filterContext.Result = new RedirectResult(Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(controller.Request));
                   return;
               }*/
            if (controller.CurrentUser == null)
            {
                var returnUrl = UriHelper.GetDisplayUrl(filterContext.HttpContext.Request);

                filterContext.Result = new RedirectResult(
                    $"{ConstValue.SSOLoginUrl}?ReturnUrl={WebUtility.UrlEncode(returnUrl)}"
                );

                return;
            }

            var identity = (ClaimsIdentity)filterContext.HttpContext.User.Identity;
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            string email = "";

            if (identity.FindFirst("KhoaID") == null)
            {
                identity.Claims.ToList().ForEach(d => identity.RemoveClaim(d));
                var user = controller.CurrentUser;
                var list = user.lstKiemNhiemInfo.ToList();

                //Add thông tin nhân viên nếu hết cookie
                identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
                identity.AddClaim(new Claim(ClaimTypes.Name, user.FullName));
                identity.AddClaim(new Claim("MNV", user.MaNhanVien));
                identity.AddClaim(new Claim("KhoaID", user.lstKiemNhiemInfo[0].MaBoPhan));
                identity.AddClaim(new Claim("TenKhoa", user.lstKiemNhiemInfo[0].TenBoPhan));
                identity.AddClaim(new Claim("MaChucVu", user.lstKiemNhiemInfo[0].MaChucVu));
                identity.AddClaim(new Claim("ListKhoa", JsonConvert.SerializeObject(user.lstKiemNhiemInfo)));
                email = user.Email;
            }
            else
            {
                //Xóa quyền để lấy lại
                identity.Claims.Where(c => c.Type == ClaimTypes.Role || c.Type == "GroupID").ToList().ForEach(d => identity.RemoveClaim(d));
                email = identity.FindFirst(ClaimTypes.Email).Value;
            }

            //Add quyền nhân viên mỗi lần load trang
            string roles = Areas.API.Models.Permission.getPermissionString(email, identity.FindFirst("KhoaID").Value);
            string[] lstRole = roles.Split(new Char[] { ',' });
            Claim cl;
            for (int i = 0; i < lstRole.Length; i++)
            {
                cl = new Claim(ClaimTypes.Role, lstRole[i]);
                identity.AddClaim(cl);
            }

           /* //lấy group_id
            DEntity<Member> e = new DEntity<Member>(ConstValue.ConnectionString, Areas.API.Models.Member.getTableName());
            e.setPrimaryKey("id");
            List<Member> lst = e.getList("email", email);
            identity.AddClaim(new Claim("GroupID", String.Join(',', lst.Select(o => o.group_id))));
*/
            await filterContext.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);           


            await next();
        }
    }
}
