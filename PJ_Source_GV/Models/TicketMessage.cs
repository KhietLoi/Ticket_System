using System;
using System.Collections.Generic;

namespace PJ_Source_GV.Models
{
    public class TicketMessage
    {
        public int MessageId { get; set; }

        public int TicketId { get; set; }

        public string SenderEmail { get; set; }

        public string SenderName { get; set; }

        public bool IsStaff { get; set; } //để phát triển phân quyền sau này.

        public string Message { get; set; }

        public DateTime CreatedDate { get; set; }



        public List<TicketMessageAttachment> Attachments { get; set; }


    }
}
