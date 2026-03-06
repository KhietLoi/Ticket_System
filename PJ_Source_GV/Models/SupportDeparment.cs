using System;
using System.Collections.Generic;

namespace PJ_Source_GV.Models
{
    public class SupportDepartment
    {
        public int DepartmentId { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }


        public DateTime CreatedDate { get; set; }
        public static string getTableName()
        {
            return "SupportDepartment";
        }

        public List<SupportStaff> Staffs { get; set; }
    }
}
