using PJ_Source_GV.Caption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PJ_Source_GV.Services
{
    public class DepartmentSV
    {
        public static async Task<List<DepartmentSVModel>> GetAllDeparments()
        {
            var DeptService = new ServiceReference.Service1Client();
            var listDept = new List<DepartmentSVModel>();
            ServiceReference.getAllGroupNameByTypeResponse DeptFromSv =
                await DeptService.getAllGroupNameByTypeAsync(ConstValue.KeyDeptSv, "ALL");
            if (DeptFromSv.getAllGroupNameByTypeResult != null)
            {
                for (int item = 0; item < DeptFromSv.getAllGroupNameByTypeResult.Length; item++)
                {
                    try
                    {
                        string[] data = DeptFromSv.getAllGroupNameByTypeResult[item].Split("\t");
                        DepartmentSVModel deptTemp = new DepartmentSVModel();
                        if (data.Length > 1)
                        {
                            deptTemp.DeptID = data[0];
                            deptTemp.DeptName = data[1];
                        }
                        else
                        {
                            continue;
                        }
                        listDept.Add(deptTemp);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }

            listDept.Add(new DepartmentSVModel
            {
                DeptID = "X1",
                DeptName = "CĐCS Trường đại học Tôn Đức Thắng"
            });

            listDept.Add(new DepartmentSVModel
            {
                DeptID = "X2",
                DeptName = "Văn phòng Đảng uỷ"
            });

            return listDept;
        }

        /// <summary>
        /// Lấy phòng ban theo mã phòng ban
        /// </summary>
        /// <returns></returns>
        public static async Task<DepartmentSVModel> GetPhongBanByMaPhongBan(string maPhongBan)
        {
            return (await GetAllDeparments()).FirstOrDefault(d => d.DeptID.Equals(maPhongBan));
        }
    }
}
