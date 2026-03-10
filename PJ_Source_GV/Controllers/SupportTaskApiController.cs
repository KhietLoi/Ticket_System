using Microsoft.AspNetCore.Mvc;
using PJ_Source_GV.Repositories;

namespace PJ_Source_GV.Controllers
{
    public class SupportTaskApiController : Controller
    {

        //Phụ trợ cho ajax Task
        [HttpGet]
        public IActionResult GetTaskByDepartment(int departmentId)
        {
            var tasks = SupportTaskRes.GetTaskByDepartmentId(departmentId);
            return Json(tasks);
        }
    }
}
