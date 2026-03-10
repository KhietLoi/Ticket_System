using System;

namespace PJ_Source_GV.Models
{
    public class SupportTask
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; } // Tên của task

        public int DepartmentId { get; set; } // Đơn vị phụ trách task này

        public int MainStaffId { get; set; } // Người phụ trách chính của task này
        public string Description { get; set; } // Mô tả chi tiết về task


        //Thêm phần này để hiện thị chi tiết thông tin về task:
        public string  DepartmentName { get;set; } // Tên của đơn vị phụ trách

        public string StaffName { get; set; } // Tên của người phụ trách chính

        //Ngay tạo
        public DateTime CreatedDate { get; set; }


    }
}
