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
    }
}
