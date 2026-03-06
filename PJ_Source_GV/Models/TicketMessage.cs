using System;

namespace PJ_Source_GV.Models
{
    public class TicketMessage
    {
        public int MessageId { get; set; }

        public int TicketId { get; set; }

        public string SenderEmail { get; set; }

        public string SenderName { get; set; }

        public bool IsStaff { get; set; }

        public string Message { get; set; }

        public DateTime CreatedDate { get; set; }


    }
}
