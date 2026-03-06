using PJ_Source_GV.Enums;
using PJ_Source_GV.Models;
using System;

namespace PJ_Source_GV.ViewModels
{
    public class TicketDetailVM
    {
        public int TicketId { get; set; }

        public string TicketCode { get; set; }

        public string DepartmentName { get; set; }

        public string TaskName { get; set; }

        public string Description { get; set; }

        public TicketStatus Status { get; set; }


        public DateTime? ExpectedCompleteDate { get; set; }

        public string CreatedByEmail { get; set; }

        public DateTime CreatedDate { get; set; }

        
        public TicketAttachment Attachment { get; set; }

    }
}
