using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//SignalR
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PJ_Source_GV.Hubs;
using PJ_Source_GV.Models;
using PJ_Source_GV.Repositories;
using PJ_Source_GV.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PJ_Source_GV.Controllers
{
    public class TicketMessageController : Controller
    {
        private readonly EmailService _emailService;

        private readonly IHubContext<TicketHub> _hubContext;

        public TicketMessageController(EmailService emailService, IHubContext<TicketHub> hubContext)
        {
            _emailService = emailService;
            _hubContext = hubContext;

        }

        //Load chat

        public IActionResult GetMessages(int ticketId)
        {
            var messages = TicketMessageRes.GetByTicket(ticketId);

            return Json(messages);
        }

        //Send message
        /* [HttpPost]
         public IActionResult SendMessage(int ticketId, string message)
         {
             TicketMessage msg = new TicketMessage
             {
                 TicketId = ticketId,
                 SenderEmail = "staff@gmail.com",
                 SenderName = "IT Support",
                 Message = message,
                 IsStaff = true
             };

             TicketMessageRes.InsertMessage(msg);


             return Ok();
         }*/
        [HttpPost]
        public async Task<IActionResult> SendMessage(int ticketId, string message, List<IFormFile>files)
        {
            TicketMessage msg = new TicketMessage
            {
                TicketId = ticketId,
                SenderEmail = "staff@gmail.com",
                SenderName = "IT Support",
                Message = message,
                IsStaff = true
            };

            //Insert message and get the generated messageId
            int messageId = TicketMessageRes.InsertMessage(msg);

            //=====UPLOAD FILE =====
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        string newFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

                        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads/ticket", newFileName);
                       
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        TicketMessageRes.InsertAttachment(
                            messageId,
                            file.FileName,
                            "/Uploads/ticket/" + newFileName
                        );  

                    }
                }
            }


            // ===== LẤY THÔNG TIN TICKET =====
            var ticket = TicketRes.GetTicketById(ticketId);

            string ticketCode = ticket.TicketCode;
            string userEmail = ticket.CreatedByEmail;

            // ===== LINK CHAT =====
            string link = $"https://localhost:5001/Ticket/Conversation/{ticketId}";

            // ===== EMAIL CONTENT =====
            string subject = $"[TDTU] Staff đã phản hồi ticket {ticketCode}";

            string body = $@"
        Xin chào,<br><br>

        Staff đã phản hồi ticket <b>{ticketCode}</b> của bạn.<br><br>

        Nội dung phản hồi:<br>
        <i>{message}</i><br><br>

        Bạn có thể vào link dưới để trả lời:<br>
        <a href='{link}'>Mở cuộc trò chuyện</a><br><br>

        Trân trọng,<br>
        IT Support
    ";


            await _hubContext.Clients
            .Group(ticketId.ToString())
            .SendAsync("ReceiveMessage");


            await _emailService.SendEmailAsync(
                "khietloi2004.dev.net@gmail.com",
                subject,
                body
            );

            return Ok();

        }


        [HttpPost]
        public async Task<IActionResult> SendMessageUser(int ticketId, string message, string email, List<IFormFile> files )
        {
            TicketMessage msg = new TicketMessage
            {
                TicketId = ticketId,
                SenderEmail = "khietloi2004.dev.net@gmail.com",
                SenderName = "Hell",
                Message = message,
                IsStaff = false
            };
            //Insert message and get the generated messageId
            int messageId = TicketMessageRes.InsertMessage(msg);
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        string newFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

                        string path = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot/Uploads/ticket",
                            newFileName
                        );

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        TicketMessageRes.InsertAttachment(
                            messageId,
                            file.FileName,
                            "/Uploads/ticket/" + newFileName
                        );
                    }
                }
            }

            await _hubContext.Clients
                .Group(ticketId.ToString())
                .SendAsync("ReceiveMessage");
            return Ok();
        }

    }
}
