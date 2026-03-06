using DNTBreadCrumb.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using PJ_Source_GV.Caption;
using PJ_Source_GV.Enums;
using PJ_Source_GV.FunctionSupport;
using PJ_Source_GV.Repositories;
using PJ_Source_GV.Services;
using PJ_Source_GV.ViewModels;
using SSOLibCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PJ_Source_GV.Controllers
{
    [BreadCrumb(UseDefaultRouteUrl = true, Order = 0)]
    [LecturerLoginCheck]
    public class TicketController : PrivateCoreController
    {
        private readonly EmailService _emailService;

        public TicketController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [BreadCrumb(Order = 1)]
        public IActionResult Index()
        {
            // Lấy email từ SSO
            var identity = (ClaimsIdentity)User.Identity;
            var email = identity
                .FindFirst(ClaimsIdentity.DefaultNameClaimType)
                ?.Value;
            var list = TicketRes.GetTicketByEmail(email);

            return View(list);
        }
        [BreadCrumb(Order = 1)]
        public IActionResult Create(int? departmentId)
        {
            // Lấy email từ SSO
            var identity = (ClaimsIdentity)User.Identity;

            var email = identity
                .FindFirst(ClaimsIdentity.DefaultNameClaimType)
                ?.Value;

            ViewBag.Email = email;

            //  Load danh sách Department
            ViewBag.Departments = SupportDepartmentRes.GetAll();

            //  Nếu đã chọn Department thì load Task
            if (departmentId.HasValue && departmentId.Value > 0)
            {
                ViewBag.Tasks =
                    SupportTaskRes.GetTaskByDepartmentId(departmentId.Value);

                ViewBag.SelectedDepartment = departmentId.Value;
            }

            return View();
        }
        //Tạo Ticker mới:
        [HttpPost]
        public async Task<IActionResult> Create(TicketCreateVM model, List<IFormFile> files)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = SupportDepartmentRes.GetAll();
                ViewBag.Email = model.CreatedByEmail;

                TempData["Error"] = "Vui lòng điền đầy đủ thông tin.";
                return View("Create", model);
            }



            int ticketId = TicketRes.CreateTicket(model);

            string ticketCode = $"TKT-{ticketId:0000}";
            string status = "Đã tiếp nhận";
            string createdTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            //  Lấy TaskName
            var task = SupportTaskRes.GetById(model.TaskId);
            string taskName = task?.TaskName ?? "N/A";

            List<string> fullAttachmentPaths = new List<string>();

            // Upload file
            if (files != null && files.Count > 0)
            {
                string uploadPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/uploads"
                );

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        string fileName = Guid.NewGuid().ToString()
                                        + Path.GetExtension(file.FileName);

                        string fullPath = Path.Combine(uploadPath, fileName);

                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        TicketRes.InsertAttachment(ticketId, "/uploads/" + fileName);

                        fullAttachmentPaths.Add(fullPath); //  lưu path gửi mail
                    }
                }
            }

            // Đọc template HTML
            string templatePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Templates/Emails/TicketCreated.html"
            );

            string body = await System.IO.File.ReadAllTextAsync(templatePath);

            // Replace placeholder
            body = body.Replace("{{TicketCode}}", ticketCode)
                       .Replace("{{TaskName}}", taskName)
                       .Replace("{{CreatedTime}}", createdTime)
                       .Replace("{{Status}}", status);
            string subject = $"[TDTU] Ticket {ticketCode} đã được tạo thành công";

            await _emailService.SendEmailAsync(
            "khietloi2004.dev.net@gmail.com",   // test trước
            subject,
            body,
            fullAttachmentPaths   //  gửi kèm file
        );

            TempData["Success"] = "Yêu cầu đã được tạo thành công";
            return RedirectToAction("Create");
        }

        //Xem chi tiết ticket:
        public IActionResult Detail(int id)
        {
            var ticket = TicketRes.GetTicketById(id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // Trang quản lý ticket cho Manager
        public IActionResult Index_Manager()
        {
            var tickets = TicketRes.GetAll();
            return View(tickets);
        }
        // Chi tiết ticket cho Manager:
        public IActionResult Detail_Manager(int id)
        {
            var ticket = TicketRes.GetById_Manager(id);
            if (ticket == null)
            {
                return NotFound();
            }
            // Lấy danh sách Staff để hiển thị dropdown
            // Chỉ load staff list khi chưa assign
            if (ticket.AssignedToStaffId == null)
            {
                ViewBag.StaffList = SupportStaffRes.GetStaffForAssign(ticket.DepartmentId);
            }
            else
            {
                ViewBag.AssignedStaff = SupportStaffRes.GetById(ticket.AssignedToStaffId.Value);
            }


            return View(ticket);


        }

        //Xử lý khi phân công ticket cho nhân viên, và cập nhật trạng thái, hiện ngày dự kiến:
        /*        [HttpPost]
                public IActionResult AssignStaff(int ticketId, int assignedStaffId, DateTime expectedDate)
                {
                    try
                    {
                        int managerId = 1; // Giả định staff khác phân cho sau này sửa lại thành chính staff đó, để tự nhận

                        // 1 Assign Staff
                        TicketRes.AssignStaff(ticketId, assignedStaffId, managerId);

                        // 2 Update Expected Date
                        TicketRes.UpdateExpectedDate(ticketId, expectedDate, managerId);

                        // 3 Update Status (Processing)
                        TicketRes.UpdateStatus(ticketId, 1, managerId);

                        TempData["Success"] = "Phân công staff thành công.";
                    }
                    catch
                    {
                        TempData["Error"] = "Có lỗi xảy ra khi phân công.";
                    }

                    return RedirectToAction("Detail_Manager", new { id = ticketId });
                }*/
        // Xử lý khi phân công ticket cho nhân viên
        [HttpPost]
        public async Task<IActionResult> AssignStaff(int ticketId, int? assignedStaffId, DateTime ExpectedCompleteDate)
        {
            Console.WriteLine(ticketId);
            Console.WriteLine(assignedStaffId);

            if (assignedStaffId == null)    
            {
                TempData["Error"] = "Vui lòng chọn staff.";
                return RedirectToAction("Detail_Manager", new { id = ticketId });
            }

            int managerId = 1;

            TicketRes.AssignStaff(ticketId, assignedStaffId.Value, managerId);
            TicketRes.UpdateExpectedDate(ticketId, ExpectedCompleteDate, managerId);
            TicketRes.UpdateStatus(ticketId, 1, managerId);
            //Thêm gửi email đến người dùng để cập nhật trạng thái:
            /**Lấy thông tin ticket**/
            var ticket = TicketRes.GetTicketById(ticketId);
            string ticketCode = ticket.TicketCode;
            string createdEmail = ticket.CreatedByEmail;
            string expectedDate = ExpectedCompleteDate.ToString("dd/MM/yyyy");
            /**Lấy staff name**/
            var staff = SupportStaffRes.GetById(assignedStaffId.Value);
            string staffName = staff?.FullName?? "Support Team";

            /**Đọc template Name**/
           // Đọc template HTML
            string templatePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Templates/Emails/TicketAssigned.html"
            );
            string body = await System.IO.File.ReadAllTextAsync(templatePath);

            // Replace placeholder
            body = body.Replace("{{TicketCode}}", ticketCode)
                       .Replace("{{StaffName}}", staffName)
                       .Replace("{{ExpectedDate}}", expectedDate);

            string subject = $"[TDTU] Ticket {ticketCode} đã được tiếp nhận";

            await _emailService.SendEmailAsync(
                "khietloi2004.dev.net@gmail.com",   // email người tạo ticket
                subject,
                body
            );


            TempData["Success"] = "Phân công staff thành công.";

            return RedirectToAction("Detail_Manager", new { id = ticketId });
        }

    }
}

