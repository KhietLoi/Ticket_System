namespace PJ_Source_GV.Models
{
    public class ThongTinNhanVien
    {
        /// <summary>
        /// ID tự động tăng.
        /// </summary>
        /// <value>The identifier.</value>
        public int ID { get; set; }

        /// <summary>
        /// Mã Nhân Viên
        /// </summary>
        /// <value>The ma nhan vien.</value>
        public string MaNhanVien { get; set; }

        /// <summary>
        /// Tên Nhân Viên
        /// </summary>
        /// <value>The ten nhan vien.</value>
        public string TenNhanVien { get; set; }

        /// <summary>
        /// Mã Phòng Ban
        /// </summary>
        /// <value>The ma phong ban.</value>
        public string MaPhongBan { get; set; }

        /// <summary>
        /// Tên Phòng Ban
        /// </summary>
        /// <value>The ten phong ban.</value>
        public string TenPhongBan { get; set; }

        /// <summary>
        /// Mã Chức Vụ
        /// </summary>
        /// <value>The ma phong ban.</value>
        public string MaChucVu { get; set; }

        /// <summary>
        /// Tên Chức Vụ
        /// </summary>
        /// <value>The ten phong ban.</value>
        public string TenChucVu { get; set; }

        /// <summary>
        /// Mã Chức Danh
        /// </summary>
        /// <value>The ma chuc danh.</value>
        public string MaChucDanh { get; set; }

        /// <summary>
        /// Tên Chức Danh
        /// </summary>
        /// <value>The ten chuc danh.</value>
        public string TenChucDanh { get; set; }

        /// <summary>
        /// EmailGets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        public string Email { get; set; }

        /// <summary>
        /// Quyền Admin
        /// </summary>
        /// <value>The quyen admin.</value>
        public string QuyenAdmin { get; set; }

        /// <summary>
        /// Mã PIN
        /// </summary>
        /// <value>The quyen admin.</value>
        public string MaPin { get; set; }


    }
}
