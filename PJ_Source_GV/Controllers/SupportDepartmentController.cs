using DNTBreadCrumb.Core;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using PJ_Source_GV.FunctionSupport;
using PJ_Source_GV.Models;
using PJ_Source_GV.Repositories;
using SSOLibCore;
using System;

namespace PJ_Source_GV.Controllers
{
    [BreadCrumb(UseDefaultRouteUrl = true, Order = 0)]
    [LecturerLoginCheck]
    public class SupportDepartmentController : PrivateCoreController
    {

        //Tham chiếu DepartmenStaff respository
        [BreadCrumb(Title = "Danh sách", Order = 1)]
        public IActionResult Index()
        {
            var data = SupportDepartmentRes.GetAll();
            return View(data);
        }

        //API trả về danh sách tất cả đơn vị hỗ trợ
        [HttpGet]
        
        public JsonResult GetAll()
        {
            var list = SupportDepartmentRes.GetAll();

            if (list == null)
                return Json("NULL");

            return Json(list);
        }

        //Trả về thông tin chi tiết của đơn vị hỗ trợ
        [BreadCrumb(Title = "Chi tiết", Order = 1)]
        public IActionResult Details(int id, int page = 1)
        {
            var dept = SupportDepartmentRes.GetById(id);
            if (dept == null) return NotFound();

            int pageSize = 5;

            dept.Staffs =
                DepartmentStaffRes.GetStaffByDepartmentId(id, page, pageSize);

            int totalStaff =
                DepartmentStaffRes.CountStaffByDepartmentId(id);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages =
                (int)Math.Ceiling((double)totalStaff / pageSize);

            return View(dept);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Details")]

        public IActionResult Details(int id, SupportDepartment model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var updated = SupportDepartmentRes.UpdateById(id, model);

            if (updated == null)
                return NotFound();

            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult Delete(int id)
        {
            int result = SupportDepartmentRes.Delete(id);

            if (result == 1)
            {
                TempData["Success"] = "Xóa thành công";
                return RedirectToAction("Index");
            }
            else if (result == -1)
            {
                TempData["Error"] = "Không thể xóa vì có dữ liệu liên quan";

            }
            else
            {
                TempData["Error"] = "Không tìm thấy đơn vị hỗ trợ!";
            }

            return RedirectToAction("Details", new { id = id });
        }

        //Tạo SupportDepartment
        [BreadCrumb(Title = "Tạo mới", Order = 1)]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SupportDepartment model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int result = SupportDepartmentRes.Insert(model);

            if (result == 1)
            {
                TempData["Success"] = "Thêm mới thành công!";
                return RedirectToAction("Index");
            }
            else if (result == -1)
            {
                ModelState.AddModelError("DepartmentCode",
                                         "Mã phòng ban đã tồn tại!");
            }
            else
            {
                ModelState.AddModelError("",
                                         "Thêm mới thất bại!");
            }

            return View(model); 
        }

        //Xóa nhân viên thuộc đơn vị hỗ trợ:
        [HttpPost]
        public IActionResult RemoveStaff(int departmentId, int staffId)
        {
            DepartmentStaffRes.RemoveStaff(departmentId, staffId);

            return RedirectToAction("Details",
                new { id = departmentId });
        }
    }
}

