using Microsoft.AspNetCore.Mvc;
using PJ_Source_GV.Repositories;

namespace PJ_Source_GV.Controllers
{
    public class SupportTaskController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetTaskByDepartment(int departmentId)
        {
            var tasks = SupportTaskRes.GetTaskByDepartmentId(departmentId);
            return Json(tasks);
        }
    }
}
