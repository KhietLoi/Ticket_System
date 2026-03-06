using PJ_Source_GV.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using PJ_Source_GV.Caption;
using System;

namespace PJ_Source_GV.Repositories
{
    public class DepartmentStaffRes
    {
        //Hiển danh sách nhân viên phụ trách của đơn vị:
        public static List<SupportStaff> GetStaffByDepartmentId(
    int departmentId,
    int page,
    int pageSize)
        {
            List<SupportStaff> list = new List<SupportStaff>();

            using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
            {
                SqlCommand cmd =
                    new SqlCommand("sp_DepartmentStaff_GetByDepartmentId", conn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
                cmd.Parameters.AddWithValue("@Page", page);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new SupportStaff
                    {
                        StaffId = Convert.ToInt32(reader["StaffId"]),
                        FullName = reader["FullName"].ToString(),
                        Email = reader["Email"].ToString(),
                        Position = reader["Position"].ToString()
                    });
                }
            }

            return list;
        }

        //Đếm số nhân viên thuộc đơn vị:
        public static int CountStaffByDepartmentId(int departmentId)
        {
            using (SqlConnection conn =
                new SqlConnection(ConstValue.ConnectionString))
            {
                SqlCommand cmd =
                    new SqlCommand("sp_DepartmentStaff_CountByDepartmentId", conn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DepartmentId", departmentId);

                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        //Xóa nhân viên khỏi đơn vị:
        public static void RemoveStaff(int departmentId, int staffId)
        {
            using (SqlConnection conn =
                new SqlConnection(ConstValue.ConnectionString))
            {
                SqlCommand cmd =
                    new SqlCommand("sp_DepartmentStaff_Remove", conn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
                cmd.Parameters.AddWithValue("@StaffId", staffId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        //Thêm nhân viên phụ trách cho đơn vị:
        public static bool AddDepartmentStaff(int departmentId, int staffId)
        {
            using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_DepartmentStaff_Add", conn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
                cmd.Parameters.AddWithValue("@StaffId", staffId);

                conn.Open();

                object result = cmd.ExecuteScalar();

                if (result != null && Convert.ToInt32(result) == 1)
                    return true;

                return false;
            }
        }
        //Lọc nhân viên chưa thuộc đơn vị:
        public static List<SupportStaff> GetAvailableStaff (int departmentId)
        {
            List<SupportStaff> list = new List<SupportStaff>();

            using (SqlConnection con = new SqlConnection(ConstValue.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_DepartmentStaff_GetAvailableStaff", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@DepartmentId", departmentId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new SupportStaff
                    {
                        StaffId = Convert.ToInt32(reader["StaffId"]),
                        FullName = reader["FullName"].ToString(),
                        Email = reader["Email"].ToString(),
                        CurrentDepartments = reader["CurrentDepartments"].ToString()
                    });
                }
            }

            return list;

        }
    }
}
