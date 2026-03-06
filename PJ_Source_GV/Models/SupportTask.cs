namespace PJ_Source_GV.Models
{
    public class SupportTask
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; } // Tên của task

        public int DepartmentId { get; set; } // Đơn vị phụ trách task này

        public int MainStaffId { get; set; } // Người phụ trách chính của task này
        public string Description { get; set; } // Mô tả chi tiết về task
    }
}
