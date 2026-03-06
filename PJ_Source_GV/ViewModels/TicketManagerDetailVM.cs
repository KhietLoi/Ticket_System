using PJ_Source_GV.Enums;
using PJ_Source_GV.Models;
using System;

namespace PJ_Source_GV.ViewModels
{
    public class TicketManagerDetailVM
    {
        public int TicketId { get; set; }

        public string TicketCode { get; set; }

        public int DepartmentId { get; set; }

        public int TaskId { get; set; }

        public string Description { get; set; }

        public TicketStatus Status { get; set; }

        public DateTime? ExpectedCompleteDate { get; set; }

        public string CreatedByEmail { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? UpdatedByStaffId { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? AssignedToStaffId { get; set; }
        public string AssignedToStaffName { get;set; }
        //Thêm file
        public TicketAttachment Attachment { get; set; }
    }
}
