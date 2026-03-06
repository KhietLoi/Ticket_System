using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using PJ_Source_GV.FunctionSupport;
using PJ_Source_GV.Repositories;
using SSOLibCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PJ_Source_GV.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return Redirect("/");
        }
    }
}