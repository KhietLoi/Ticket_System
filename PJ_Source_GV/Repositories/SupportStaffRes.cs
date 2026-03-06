using Microsoft.AspNetCore.Server.Kestrel.Internal.System.Collections.Sequences;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using PJ_Source_GV.Caption;
using PJ_Source_GV.Models;
using PJ_Source_GV.Services;
using PJ_Source_GV.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace PJ_Source_GV.Repositories
{
    public class SupportStaffRes
    {
        /**Lấy dữ liệu **/
        //Lấy tất cả:
       
            public static List<SupportStaff> GetAll()
            {
                List<SupportStaff> list = new List<SupportStaff>();

                using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_SupportStaff_GetAll", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();

                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            SupportStaff staff = new SupportStaff
                            {
                                StaffId = Convert.ToInt32(reader["StaffId"]),
                                FullName = reader["FullName"].ToString(),
                                Email = reader["Email"].ToString(),
                                Position = reader["Position"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"])
                            };

                            list.Add(staff);
                        }
                    }
                }

                return list;
            }
        //Lấy theo cá nhân:
        public static SupportStaff GetById(int id)
        {
            SupportStaff staff = null;

            using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SupportStaff_GetById", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@StaffId", id);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            staff = new SupportStaff
                            {
                                StaffId = Convert.ToInt32(reader["StaffId"]),
                                FullName = reader["FullName"].ToString(),
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                Position = reader["Position"].ToString(),
                                AvatarPath = reader["AvatarPath"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                DateOfBirth = reader["DateOfBirth"] == DBNull.Value
                                                ? (DateTime?)null
                                                : Convert.ToDateTime(reader["DateOfBirth"]),
                                CurrentDepartments = reader["CurrentDepartments"] == DBNull.Value
                                                ? ""
                                                : reader["CurrentDepartments"].ToString()

                            };
                        }
                    }
                }
            }

            return staff;
        }
        // Sửa thông tin nhân viên:
        public static SupportStaff UpdateById(int id, SupportStaff model)
        {
            SupportStaff updatedStaff = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_SupportStaff_Update", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@StaffId", id);
                        cmd.Parameters.AddWithValue("@FullName", model.FullName);
                        cmd.Parameters.AddWithValue("@Email", model.Email);
                        cmd.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);
                        cmd.Parameters.AddWithValue("@Position", model.Position);

                        cmd.Parameters.AddWithValue("@AvatarPath", model.AvatarPath ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DateOfBirth", model.DateOfBirth ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsActive", model.IsActive);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                return GetById(id);
            }
            catch (SqlException ex)
            {
                if(ex.Message.Contains("EMAIL_EXISTS"))
                {
                    throw new Exception("EMAIL_EXISTS");
                }
                if(ex.Message.Contains("PHONE_EXISTS"))
                {
                    throw new Exception("PHONE_EXISTS");
                }
                throw;
            }
            
        }

        //Tạo nhân viên mới:
        public static int Insert(SupportStaff model)
        {
            int newId = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_SupportStaff_Create", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@FullName", model.FullName);
                        cmd.Parameters.AddWithValue("@Email", model.Email);
                        cmd.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);
                        cmd.Parameters.AddWithValue("@Position", model.Position);
                        cmd.Parameters.AddWithValue("@DateOfBirth",
                            model.DateOfBirth ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@AvatarPath",
                            model.AvatarPath ?? (object)DBNull.Value);

                        conn.Open();
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                            newId = Convert.ToInt32(result);
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("EMAIL_EXISTS"))
                    throw new Exception("EMAIL_EXISTS");

                if (ex.Message.Contains("PHONE_EXISTS"))
                    throw new Exception("PHONE_EXISTS");

                throw;
            }

            return newId;
        }

        //Xóa nhân viên:

        public static int Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SupportStaff_Delete", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StaffId", id);
                    conn.Open();

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        //Lấy nhân viên theo phòng ban phục vụ cho Assign Ticket:
        public static List<StaffAssignVM> GetStaffForAssign(int departmentId)
        {
            List<StaffAssignVM> list = new List<StaffAssignVM>();

            using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_DepartmentStaff_GetStaffForAssign", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@DepartmentId", departmentId);

                    conn.Open();

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        StaffAssignVM staff = new StaffAssignVM
                        {
                            StaffId = (int)reader["StaffId"],
                            FullName = reader["FullName"].ToString(),
                            Email = reader["Email"].ToString()
                        };

                        list.Add(staff);
                    }
                }
            }

            return list;
        }


    }
}
