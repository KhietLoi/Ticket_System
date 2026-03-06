using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PJ_Source_GV.Caption;
using PJ_Source_GV.Models;
using ServiceReference;

namespace PJ_Source_GV.Services
{
    public class StaffSv
    {
        /// <summary>
        /// Lấy thông tin nhân viên
        /// </summary>
        /// <returns></returns>
        public static async Task<List<ThongTinNhanVien>> GetAllStaff()
        {
            var staffService = new ServiceReference.Service1Client();
            var listStaff = new List<ThongTinNhanVien>();
            ServiceReference.getAllStaffInfoResponse staffFromSv =
                await staffService.getAllStaffInfoAsync(ConstValue.KeyStaffSv);
            if (staffFromSv.getAllStaffInfoResult != null)
            {
                foreach (var item in staffFromSv.getAllStaffInfoResult)
                {
                    string[] data = item.Split(";");
                    ThongTinNhanVien staffTemp = new ThongTinNhanVien();
                    if (data.Length > 6)
                    {
                        staffTemp.MaNhanVien = data[0];
                        staffTemp.TenNhanVien = data[1];
                        staffTemp.MaPhongBan = data[2];
                        staffTemp.TenPhongBan = data[3];
                        staffTemp.MaChucDanh = data[4];
                        staffTemp.TenChucDanh = data[5];
                        staffTemp.Email = data[6];
                        if (staffTemp.MaNhanVien.Equals("04GN2024"))
                        {
                            staffTemp.QuyenAdmin = "1";
                        }
                        else
                        {
                            staffTemp.QuyenAdmin = "0";
                        }
                    }
                    else
                    {
                        continue;
                    }
                    listStaff.Add(staffTemp);
                }
            }
            return listStaff;
        }
        
        /// <summary>
        /// Lấy chức vụ theo mã nhân viên
        /// </summary>
        /// <param name="maNhanVien"></param>
        /// <returns></returns>
        public static async Task<List<string>> GetNhanVienChucVuTheoMaNv(string maNhanVien)
        {
            var staffService = new ServiceReference.Service1Client();
            var listChucVu = new List<String>();
            ServiceReference.getNhanVienChucVuTheoMaNVResponse statff =
                await staffService.getNhanVienChucVuTheoMaNVAsync(ConstValue.NhanVienChucVuTheoMaNv,
                    maNhanVien);
            if (statff.getNhanVienChucVuTheoMaNVResult != null)
            {
                foreach (var item in statff.getNhanVienChucVuTheoMaNVResult)
                {
                    listChucVu.Add(item);
                }
            }
            return listChucVu;
        }

        /// <summary>
        /// Lấy chức vụ 
        /// </summary>
        /// <returns></returns>
        public static async Task<List<string>> GetDanhMucChucVu()
        {
            var staffService = new ServiceReference.Service1Client();
            var listChucVu = new List<String>();
            ServiceReference.getDanhMucChucVuResponse chucVu =
                await staffService.getDanhMucChucVuAsync(ConstValue.KeyChucVu);
            if (chucVu.getDanhMucChucVuResult != null)
            {
                foreach (var item in chucVu.getDanhMucChucVuResult)
                {
                    listChucVu.Add(item);
                }
            }
            return listChucVu;
        }

        /// <summary>
        /// Lấy mã chức vụ theo mã nhân viên và phòng ban
        /// </summary>
        /// <param name="maNhanVien"></param>
        /// <param name="maPhongBan"></param>
        /// <returns></returns>
        public static async Task<string> GetNhanVienChucVuTheoMaNv(string maNhanVien, string maPhongBan)
        {
            var staffService = new Service1Client();
            var chucVu = "";
            var statff = await staffService.getNhanVienChucVuTheoMaNVAsync(ConstValue.NhanVienChucVuTheoMaNv, maNhanVien);
            if (statff.getNhanVienChucVuTheoMaNVResult != null)
            {
                foreach (var item in statff.getNhanVienChucVuTheoMaNVResult)
                {
                    var split = item.Split('_');
                    if (split[1].Equals(maPhongBan))
                    {
                        chucVu = split[3] + "#" + split[4];
                    }
                }
            }
            return chucVu;
        }
    }
}
