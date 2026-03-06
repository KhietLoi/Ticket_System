using PJ_Source_GV.Enums;
using System;

namespace PJ_Source_GV.Models
{
    public class Ticket
    {
        public int TicketId { get; set; }
        //Thêm TicketCode:
        public string TicketCode { get; set; }

        public int DepartmentId { get; set; }
        public int TaskId { get; set; }
        public string Description { get; set; }

        public TicketStatus Status { get; set; }

        public int? AssignedToStaffId { get; set; } //Người phụ trách task này
        public DateTime? ExpectedCompleteDate { get; set; } 

        public string CreatedByEmail { get; set; }

        public DateTime CreatedDate { get; set; }
        
        
        public int? UpdatedByStaffId { get; set; } // Người chỉnh sửa, cập nhật


        public DateTime? UpdatedDate { get; set; }


    }
}
