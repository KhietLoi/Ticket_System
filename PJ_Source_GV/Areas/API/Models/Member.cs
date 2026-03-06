using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PJ_Source_GV.Areas.API.Models
{
    public class Member
    {
        public int id { get; set; }
        public int group_id { get; set; }
        public string email { get; set; }
        public string madonvi { get; set; }
        public string MaNhanVien { get; set; }
        public string Email { get; set; }
        public string Hoten { get; set; }
        public string MaKhoa { get; set; }
        public string TenBoPhan { get; set; }
        public string MaLoaiNV { get; set; }
        public string TenLoaiNV { get; set; }
        public static string getTableName() { return "lp_groupuser"; }
    }
}
