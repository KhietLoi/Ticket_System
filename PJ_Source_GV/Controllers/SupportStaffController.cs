using DNTBreadCrumb.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PJ_Source_GV.FunctionSupport;
using PJ_Source_GV.Models;
using PJ_Source_GV.Repositories;
using SSOLibCore;
using System;
using System.Collections.Generic;
using System.IO;

namespace PJ_Source_GV.Controllers
{
    [BreadCrumb(UseDefaultRouteUrl = true, Order = 0)]
    [LecturerLoginCheck]
    public class SupportStaffController : PrivateCoreController
    {
        [BreadCrumb(Title = "Danh sách", Order = 1)]
        public IActionResult Index()
        {
            List<SupportStaff> list = SupportStaffRes.GetAll();
            return View(list);
        }
        [BreadCrumb(Title = "Chi tiết", Order = 1)]
        public IActionResult Details(int id)
        {
            var staff = SupportStaffRes.GetById(id);

            if (staff == null)
                return NotFound();

            return View(staff);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details(int id, SupportStaff model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var updated = SupportStaffRes.UpdateById(id, model);

                if (updated == null)
                    return NotFound();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("EMAIL_EXISTS"))
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại!");
                }
                else if (ex.Message.Contains("PHONE_EXISTS"))
                {
                    ModelState.AddModelError("PhoneNumber", "Số điện thoại đã tồn tại!");
                }
                else
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật!");
                }

                return View(model);
            }
        }

        //Khởi tạo nhân viên mới:
        [BreadCrumb(Title = "Tạo mới", Order = 1)]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SupportStaff model, IFormFile Avatar)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                if (Avatar != null && Avatar.Length > 0)
                {
                    string uploadsFolder = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/uploads/avatar"
                    );

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName =
                        Guid.NewGuid().ToString() + "_" + Avatar.FileName;

                    string filePath =
                        Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        Avatar.CopyTo(fileStream);
                    }

                    //  KHÔNG CÓ /
                    model.AvatarPath = "uploads/avatar/" + uniqueFileName;
                }
                else
                {
                    // ĐỒNG BỘ SP
                    model.AvatarPath = "images/avt.jpg";
                }

                int newId = SupportStaffRes.Insert(model);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("EMAIL_EXISTS"))
                    ModelState.AddModelError("Email", "Email đã tồn tại!");
                else if (ex.Message.Contains("PHONE_EXISTS"))
                    ModelState.AddModelError("PhoneNumber", "SĐT đã tồn tại!");
                else
                    ModelState.AddModelError("", "Có lỗi xảy ra!");

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            int result = SupportStaffRes.Delete(id);

            switch (result)
            {
                case 1:
                    TempData["Success"] = "Xóa thành công!";
                    return RedirectToAction("Index");

                case -1:
                    TempData["Error"] = "Không thể xóa vì tài khoản đang hoạt động!";
                    return RedirectToAction("Details", new { id = id });

                case 0:
                    TempData["Error"] = "Không tìm thấy nhân viên hỗ trợ!";
                    return RedirectToAction("Details", new { id = id });

                default:
                    TempData["Error"] = "Xóa thất bại!";
                    return RedirectToAction("Details", new { id = id });
            }
        }


        //Trả về các nhân viên thuộc một đơn vị hỗ trợ, dùng cho Assign Ticket:
        [HttpGet]
        public IActionResult GetStaffForAssign(int departmentId)
        {
            var staffList = SupportStaffRes.GetStaffForAssign(departmentId);
            return Json(staffList);
        }
    }
}
