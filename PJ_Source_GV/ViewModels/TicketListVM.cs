using System;

namespace PJ_Source_GV.ViewModels
{
    public class TicketListVM
    {
        public int TicketId { get; set; }
        public string TicketCode { get; set; }

        public string DepartmentName { get; set; }
        public string TaskName { get; set; }

        public string Description { get; set; }
        public string Status { get; set; }

        public DateTime? ExpectedCompleteDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }


    }
}
