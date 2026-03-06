using System;
using System.ComponentModel.DataAnnotations;

namespace PJ_Source_GV.Models
{
    public class SupportStaff
    {
        public int StaffId { get; set; }
        [Required(ErrorMessage = "Họ tên không được bỏ trống")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email không được bỏ trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được bỏ trống")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải 10 số và bắt đầu bằng 0")]
        public string PhoneNumber { get; set; }

        public string Position { get; set; }

        public bool IsActive { get; set; } = true;

        public string AvatarPath { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime CreatedDate { get; set; }

        //Danh sách đơn vị hiện tại
        public string CurrentDepartments { get; set; }
    }
}