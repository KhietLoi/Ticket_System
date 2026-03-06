using Microsoft.AspNetCore.Mvc;
using PJ_Source_GV.Models;
using PJ_Source_GV.Repositories;
using PJ_Source_GV.Services;
using System.Threading.Tasks;

namespace PJ_Source_GV.Controllers
{
    public class TicketMessageController : Controller
    {
        private readonly EmailService _emailService;

        public TicketMessageController(EmailService emailService)
        {
            _emailService = emailService;
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
        public async Task<IActionResult> SendMessage(int ticketId, string message)
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

            await _emailService.SendEmailAsync(
                "khietloi2004.dev.net@gmail.com",
                subject,
                body
            );

            return Ok();
        }


        [HttpPost]
        public IActionResult SendMessageUser(int ticketId, string message, string email)
        {
            TicketMessage msg = new TicketMessage
            {
                TicketId = ticketId,
                SenderEmail = "khietloi2004.dev.net@gmail.com",
                SenderName = "Hell",
                Message = message,
                IsStaff = false
            };

            TicketMessageRes.InsertMessage(msg);

            return Ok();
        }

    }
}
