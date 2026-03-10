using Microsoft.AspNetCore.Mvc;
using PJ_Source_GV.Repositories;

namespace PJ_Source_GV.Controllers
{
    public class SupportStaffApiController : Controller
    {
        [HttpGet]
        public IActionResult GetStaffByDepartment(int departmentId)
        {
            var staffs = SupportStaffRes.GetStaffForAssign(departmentId);
            return Json(staffs);
        }
    }
}
