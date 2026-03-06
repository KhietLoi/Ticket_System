using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PJ_Source_GV.Models
{
    public class ServiceModel
    {
        #region Hệ đào tạo
        public string AcademicID { get; set; }
        public string AcademicName { get; set; }
        #endregion

        #region Đợn vị quản lý sinh viên
        public string FacultyID { get; set; }
        public string FacultyName { get; set; }
        #endregion
    }
}
