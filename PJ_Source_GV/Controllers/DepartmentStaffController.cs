using Microsoft.AspNetCore.Mvc;
using PJ_Source_GV.Repositories;
using SSOLibCore;
using System;

namespace PJ_Source_GV.Controllers
{
    public class DepartmentStaffController : PrivateCoreController
    {
        public IActionResult Index()
        {
            return View();
        }
        public ActionResult AddStaff(int departmentId)
        {
            var list = DepartmentStaffRes.GetAvailableStaff(departmentId);

            ViewBag.DepartmentId = departmentId;

            return View(list);
        }

        //Them Staff:
        [HttpPost]
        public ActionResult AddStaffToDepartment(int StaffId, int DepartmentId)
        {
            Console.WriteLine(DepartmentId);
            bool result = DepartmentStaffRes.AddDepartmentStaff(DepartmentId, StaffId);

            if (result)
                TempData["Success"] = "Add staff successfully!";
            else
                TempData["Error"] = "Staff already exists in this department!";
            //Thêm thành công quay về trang chi tiết đơn vị tương ứng
            return RedirectToAction("Details", "SupportDepartment", new { id = DepartmentId });
        }
    }
}
