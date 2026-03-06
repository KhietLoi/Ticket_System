using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PJ_Source_GV.Services
{
    public class NhanVienSV
    {
        public static async Task<List<NhanVienSVModel>> GetNhanVienTheoBoPhanID(string PhongBanID)
        {
            var Service = new ServiceReference.Service1Client();
            var listNhanVien = new List<NhanVienSVModel>();
            ServiceReference.getNhanVienTheoBoPhanIDResponse dsNhanVienSv =
                await Service.getNhanVienTheoBoPhanIDAsync("28b57650187c49fa8f51318fa580f973", PhongBanID);
            //ServiceReference.getAllGroupNameByTypeResponse dsPhongBanTuSv = await Service.getAllGroupNameByTypeAsync("ea3cf26fd721a2aa80f067e112e63dfb", "P");

            // Lấy danh sách nhân viên của đơn vị
            if (dsNhanVienSv.getNhanVienTheoBoPhanIDResult != null)
            {
                for (int item = 0; item < dsNhanVienSv.getNhanVienTheoBoPhanIDResult.Length; item++)
                {
                    try
                    {
                        string[] data = dsNhanVienSv.getNhanVienTheoBoPhanIDResult[item].Split("\t");
                        NhanVienSVModel nvTemp = new NhanVienSVModel();
                        if (data.Length > 0)
                        {
                            nvTemp.MaNhanVien = data[0];
                            nvTemp.Email = data[1];
                            nvTemp.Hoten = data[2];
                            nvTemp.MaKhoa = data[3];
                            nvTemp.TenBoPhan = data[4];
                            nvTemp.MaLoaiNV = data[5];
                            nvTemp.TenLoaiNV = data[6];
                        }
                        else
                        {
                            continue;
                        }
                        listNhanVien.Add(nvTemp);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }

            return listNhanVien;
        }

        /// <summary>
        /// lấy tất cả nhân viên
        /// </summary>
        /// <returns></returns>
        public static async Task<List<NhanVienSVModel>> GetAllStaff()
        {
            var staffService = new ServiceReference.Service1Client();
            var listStaff = new List<NhanVienSVModel>();
            ServiceReference.getAllStaffInfoResponse staffFromSv =
                    await staffService.getAllStaffInfoAsync("902f1ac07edfe3fcccbc90a46a73d0ec");
            if (staffFromSv.getAllStaffInfoResult != null)
            {
                foreach (var item in staffFromSv.getAllStaffInfoResult)
                    try
                    {
                        string[] data = item.Split(";");

                        NhanVienSVModel staffTemp = new NhanVienSVModel();
                        if (data.Length > 6)
                        {
                            staffTemp.MaNhanVien = data[0];
                            staffTemp.Hoten = data[1];
                            staffTemp.MaKhoa = data[2];
                            staffTemp.TenBoPhan = data[3];
                            staffTemp.MaLoaiNV = data[4];
                            staffTemp.TenLoaiNV = data[5];
                            staffTemp.Email = data[6];
                        }
                        else
                        {
                            continue;
                        }
                        listStaff.Add(staffTemp);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
            }
            return listStaff;
        }

        public static async Task<List<NhanVienSVModel>> GetNhanVien()
        {
            var Service = new ServiceReference.Service1Client();
            var listNhanVien = new List<NhanVienSVModel>();
            ServiceReference.getAllStaffInfoResponse dsNhanVienSv =
                await Service.getAllStaffInfoAsync("902f1ac07edfe3fcccbc90a46a73d0ec");

            // Lấy danh sách nhân viên của đơn vị
            if (dsNhanVienSv.getAllStaffInfoResult != null)
            {
                foreach (var item in dsNhanVienSv.getAllStaffInfoResult)
                {
                    try
                    {
                        string[] data = item.Split(";");
                        NhanVienSVModel nvTemp = new NhanVienSVModel();
                        if (data.Length > 0)
                        {
                            nvTemp.MaNhanVien = data[0];
                            nvTemp.Hoten = data[1];
                            nvTemp.MaKhoa = data[2];
                            nvTemp.TenBoPhan = data[3];
                            nvTemp.MaLoaiNV = data[4];
                            nvTemp.TenLoaiNV = data[5];
                            nvTemp.Email = data[6];
                        }
                        else
                        {
                            continue;
                        }
                        listNhanVien.Add(nvTemp);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }

            return listNhanVien;
        }
    }
}
