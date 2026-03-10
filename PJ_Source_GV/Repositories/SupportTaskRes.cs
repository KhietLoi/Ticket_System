using PJ_Source_GV.Caption;
using PJ_Source_GV.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace PJ_Source_GV.Repositories
{
    public class SupportTaskRes
    {

        public static List<SupportTask> GetTaskByDepartmentId(int departmentId)
        {
            List<SupportTask> list = new List<SupportTask>();

            using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SupportTask_GetByDepartmentId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@DepartmentId", departmentId);

                    conn.Open();

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        SupportTask task = new SupportTask
                        {
                            TaskId = Convert.ToInt32(reader["TaskId"]),
                            TaskName = reader["TaskName"].ToString()
                        };

                        list.Add(task);
                    }
                }
            }

            return list;
        }
        //Truy vấn chi tiết Task theo TaskId
        public static SupportTask GetById(int taskId)
        {
            SupportTask task = null;
            using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SupportTask_GetById", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TaskId", taskId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        task = new SupportTask
                        {
                            TaskId = Convert.ToInt32(reader["TaskId"]),
                            TaskName = reader["TaskName"].ToString(),
                            DepartmentId = Convert.ToInt32(reader["DepartmentId"]),
                            MainStaffId = Convert.ToInt32(reader["MainStaffId"]),

                        };
                    }
                }
            }
            return task;
        }


        //Lấy danh sách tất cả các Task
        public static List<SupportTask> GetAll()
        {
            List<SupportTask> list = new List<SupportTask>();
            using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SupportTask_GetAll", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        SupportTask task = new SupportTask
                        {
                            TaskId = Convert.ToInt32(reader["TaskId"]),
                            TaskName = reader["TaskName"].ToString(),
                            DepartmentId = Convert.ToInt32(reader["DepartmentId"]),
                            MainStaffId = Convert.ToInt32(reader["MainStaffId"]),
                            DepartmentName = reader["DepartmentName"].ToString(),
                            StaffName = reader["StaffName"].ToString()
                        };
                        list.Add(task);
                    }
                }
            }
            return list;
        }

        // Lấy chi tiết Task theo DepartmentId:
        public static SupportTask GetTaskDetail(int taskId)
        {
            SupportTask task = null;

            using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SupportTask_GetDetail", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TaskId", taskId);

                    conn.Open();

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        task = new SupportTask
                        {
                            TaskId = Convert.ToInt32(reader["TaskId"]),
                            TaskName = reader["TaskName"]?.ToString(),
                            DepartmentId = Convert.ToInt32(reader["DepartmentId"]),
                            MainStaffId = Convert.ToInt32(reader["MainStaffId"]),
                            DepartmentName = reader["DepartmentName"]?.ToString(),
                            StaffName = reader["StaffName"]?.ToString(),
                            CreatedDate = reader["CreatedDate"] == DBNull.Value
                              ? DateTime.MinValue
                              : Convert.ToDateTime(reader["CreatedDate"])
                        };
                    }
                }
            }

            return task;
        }

        //Cập nhật thông tin Task:
        public static void Update(SupportTask task)
        {
            using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SupportTask_Update", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@TaskId", task.TaskId);
                    cmd.Parameters.AddWithValue("@TaskName", task.TaskName);
                    cmd.Parameters.AddWithValue("@DepartmentId", task.DepartmentId);
                    cmd.Parameters.AddWithValue("@MainStaffId", task.MainStaffId);


                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        //Xoá Task:
        public static void Delete(int taskId)
        {
            using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SupportTask_Delete", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TaskId", taskId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        //Thêm mới Task:
        public static void Create(SupportTask task)
        {
            using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SupportTask_Create", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@TaskName", task.TaskName);
                    cmd.Parameters.AddWithValue("@DepartmentId", task.DepartmentId);
                    cmd.Parameters.AddWithValue("@MainStaffId", task.MainStaffId);

                    conn.Open();

                    int rows = cmd.ExecuteNonQuery();

                    if (rows == 0)
                    {
                        throw new Exception("Không tạo được Support Task.");
                    }
                }
            }
        }
    }
}