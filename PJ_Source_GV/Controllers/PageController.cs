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
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Localization;
using PJ_Source_GV.Areas.API.Models;
using PJ_Source_GV.Caption;
using PJ_Source_GV.FunctionSupport;
using PJ_Source_GV.Models;
using SSOLibCore;

namespace PJ_Source_GV.Controllers
{
    [BreadCrumb(UseDefaultRouteUrl = true, Order = 0)]
    [LecturerLoginCheck]
    public class PageController : PrivateCoreController
    {
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public PageController(IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _sharedLocalizer = sharedLocalizer;
        }

        [Authorize(Roles = "admin")]
        [BreadCrumb(Order = 1)]
        public ActionResult Index()
        {
            this.InitBreadCrumbTitle("Quản trị", "Danh sách trang");
            //Info Page
            //this.InitBreadCrumbTitle(_sharedLocalizer["PageMenu"], _sharedLocalizer["PageMenu"]);
            var cultureInfo = HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.UICulture;

            DEntity<Page> e = new DEntity<Page>(ConstValue.ConnectionString, Page.getTableName());
            List<Page> lst = e.getAll();
            if (User.IsInRole("ad_view") == false)
            {
                lst = lst.Where(p => p.alias != "ad").ToList();
            }
            return View(lst);
        }

        [BreadCrumb(Order = 1)]
        [Authorize(Roles = "admin")]
        //[Authorize(Roles = "page_insert,page_update")]
        public ActionResult Edit(int id = 0)
        {
            //Info Page
            //this.InitBreadCrumbTitle(_sharedLocalizer["PageMenu"], _sharedLocalizer["Update"]);
            var cultureInfo = HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.UICulture;

            ViewData["Page_id"] = id;
            return View();
        }

    }
}
