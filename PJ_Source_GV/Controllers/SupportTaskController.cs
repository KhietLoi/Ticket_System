using DNTBreadCrumb.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PJ_Source_GV.FunctionSupport;
using PJ_Source_GV.Models;
using PJ_Source_GV.Repositories;
using PJ_Source_GV.Services;
using SSOLibCore;
using System;
using System.Threading.Tasks;

namespace PJ_Source_GV.Controllers
{
    [BreadCrumb(UseDefaultRouteUrl = true, Order = 0)]
    [LecturerLoginCheck]
    public class SupportTaskController : PrivateCoreController
    {
        public IActionResult Index()
        {
            var tasks = SupportTaskRes.GetAll();

            return View(tasks);
        }

        
        /*[HttpGet]
        [AllowAnonymous]
        public JsonResult GetTaskByDepartment(int departmentId)
        {
            var tasks = SupportTaskRes.GetTaskByDepartmentId(departmentId);
            return Json(tasks);
        }*/


        //Chi tiết Task
        
        public IActionResult Detail(int id)
        {
            var task = SupportTaskRes.GetTaskDetail(id);

            if (task == null)
            {
                return NotFound();
            }
            //  Load danh sách Department
            ViewBag.Departments = SupportDepartmentRes.GetAll();

            //  Nếu đã chọn Department thì load Task
            // Load Staff theo Department của Task
            if (task.DepartmentId > 0)
            {
                ViewBag.Staffs =
                    SupportStaffRes.GetStaffForAssign(task.DepartmentId);

                ViewBag.SelectedDepartment = task.DepartmentId;
            }


            return View(task);
        }

        
        
        [HttpPost]
        public IActionResult Details(SupportTask model)
        {
            var task = SupportTaskRes.GetTaskDetail(model.TaskId);

            if (task == null)
            {
                TempData["Error"] = "Task không tồn tại.";
                return RedirectToAction("Index");
            }

            var staff = SupportStaffRes.GetById(model.MainStaffId);

          

            SupportTaskRes.Update(model);

            TempData["Success"] = "Cập nhật Task thành công.";

            return RedirectToAction("Detail", new { id = model.TaskId });
        }



        //Xóa Task:
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var task = SupportTaskRes.GetTaskDetail(id);
            if (task == null)
            {
                TempData["Error"] = "Task không tồn tại.";
                return RedirectToAction("Index");
            }
            SupportTaskRes.Delete(id);
            TempData["Success"] = "Xóa Task thành công.";
            return RedirectToAction("Index");
        }

       
        // Hiển thị form tạo Task
        public IActionResult Create()
        {
            ViewBag.Departments = SupportDepartmentRes.GetAll();
            return View();
        }

        //Tạo Task mới:
        [HttpPost]
        public IActionResult Create(SupportTask model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Departments = SupportDepartmentRes.GetAll();
                    return View(model);
                }

                SupportTaskRes.Create(model);

                TempData["Success"] = "Tạo Task thành công";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Departments = SupportDepartmentRes.GetAll();

                TempData["Error"] = ex.Message;
                return RedirectToAction("Create");
            }
        }


    }
}
