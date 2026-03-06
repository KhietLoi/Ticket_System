using DCSL.DatabaseFactory;
using PJ_Source_GV.Models;
using PJ_Source_GV.Caption;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace PJ_Source_GV.Repositories
{
    public class SupportDepartmentRes
    {
        public static List<SupportDepartment> GetAll()
        {
            List<SupportDepartment> list = new List<SupportDepartment>();

            using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SupportDepartment_GetAll", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        list.Add(new SupportDepartment
                        {
                            DepartmentId = Convert.ToInt32(reader["DepartmentId"]),
                            DepartmentCode = reader["DepartmentCode"].ToString(),
                            DepartmentName = reader["DepartmentName"].ToString()
                        });
                    }
                }
            }

            return list;
        }
        public static SupportDepartment GetById(int id)
        {
            SupportDepartment dept = null;

            using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SupportDepartment_GetById", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DepartmentId", id);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        dept = new SupportDepartment
                        {
                            DepartmentId = Convert.ToInt32(reader["DepartmentId"]),
                            DepartmentCode = reader["DepartmentCode"].ToString(),
                            DepartmentName = reader["DepartmentName"].ToString(),
                            CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                        };
                    }
                }
            }

            return dept;
        }

        //Update du lieu:
        public static SupportDepartment UpdateById (int id, SupportDepartment model)
        {
            SupportDepartment updatedDepartment = null;

            using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString) )
            {
                using (SqlCommand cmd = new SqlCommand("sp_UpdateSupportDepartment",conn))
                {
                    cmd.CommandType= CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@DepartmentId", id);
                    cmd.Parameters.AddWithValue("@DepartmentCode", model.DepartmentCode);
                    cmd.Parameters.AddWithValue("@DepartmentName", model.DepartmentName);
                    cmd.Parameters.AddWithValue("@CreatedDate", model.CreatedDate.Date);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            updatedDepartment = new SupportDepartment
                            {
                                DepartmentId = Convert.ToInt32(reader["DepartmentId"]),
                                DepartmentCode = reader["DepartmentCode"].ToString(),
                                DepartmentName = reader["DepartmentName"].ToString(),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                            };
                        }
                    }
                }
            }
            return updatedDepartment;

        }

        public static int  Delete (int id)
        {
            using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Sp_DeleteSupportDepartment", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue ("@DepartmentId", id);
                    conn.Open();

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public static int Insert(SupportDepartment model)
        {
            int result = 0;

            using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_CreateSupportDepartment", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@DepartmentCode", model.DepartmentCode);
                    cmd.Parameters.AddWithValue("@DepartmentName", model.DepartmentName);

                    SqlParameter returnValue = new SqlParameter();
                    returnValue.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(returnValue);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    result = (int)returnValue.Value;
                }
            }

            return result;
        }



    }
}

