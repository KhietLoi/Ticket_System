using PJ_Source_GV.Caption;
using PJ_Source_GV.Enums;
using PJ_Source_GV.Models;
using PJ_Source_GV.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace PJ_Source_GV.Repositories
{
    public class TicketRes
    {
        public static int CreateTicket(TicketCreateVM model)
        {
            int ticketId = 0;

            using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_Ticket_Create", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Khai báo rõ kiểu dữ liệu (không dùng AddWithValue)
                    cmd.Parameters.Add("@DepartmentId", SqlDbType.Int)
                        .Value = model.DepartmentId;

                    cmd.Parameters.Add("@TaskId", SqlDbType.Int)
                        .Value = model.TaskId;

                    cmd.Parameters.Add("@Description", SqlDbType.NVarChar)
                        .Value = model.Description ?? (object)DBNull.Value;

                    cmd.Parameters.Add("@CreatedByEmail", SqlDbType.NVarChar, 255)
                        .Value = model.CreatedByEmail ?? (object)DBNull.Value;

                    conn.Open();

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                        ticketId = Convert.ToInt32(result);
                }
            }

            return ticketId;
        }


        // Insert thêm bảng TicketAttachment

        public static void InsertAttachment(int ticketId, string filePath)
        {
            using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_TicketAttachment_Insert", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@TicketId", SqlDbType.Int)
                        .Value = ticketId;

                    cmd.Parameters.Add("@FilePath", SqlDbType.NVarChar, 500)
                        .Value = filePath ?? (object)DBNull.Value;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Trả về danh sách đơn của người dùng theo email
        public static List<TicketListVM> GetTicketByEmail(string email)
        {
            List<TicketListVM> list = new List<TicketListVM>();

            using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_Ticket_GetByEmail", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 255)
                        .Value = email ?? (object)DBNull.Value;

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TicketListVM item = new TicketListVM
                            {
                                TicketId = reader["TicketId"] != DBNull.Value
                                    ? Convert.ToInt32(reader["TicketId"])
                                    : 0,

                                TicketCode = reader["TicketCode"]?.ToString(),

                                DepartmentName = reader["DepartmentName"]?.ToString(),

                                TaskName = reader["TaskName"]?.ToString(),

                                Description = reader["Description"]?.ToString(),

                                Status = reader["Status"]?.ToString(),

                                ExpectedCompleteDate = reader["ExpectedCompleteDate"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["ExpectedCompleteDate"])
                                    : (DateTime?)null,

                                CreatedDate = reader["CreatedDate"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["CreatedDate"])
                                    : (DateTime?)null,

                                UpdatedDate = reader["UpdatedDate"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["UpdatedDate"])
                                    : (DateTime?)null
                            };

                            list.Add(item);
                        }
                    }
                }
            }

            return list;
        }

        //Tra   cứu chi tiết đơn theo TicketId - Dùng cho người dùng xem chi tiết đơn của mình:
        public static TicketDetailVM GetTicketById(int ticketId)
        {
            TicketDetailVM ticket = null;

            using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_Ticket_GetById_user", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TicketId", ticketId);

                    conn.Open();

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        ticket = new TicketDetailVM
                        {
                            TicketId = (int)reader["TicketId"],
                            TicketCode = reader["TicketCode"].ToString(),
                            DepartmentName = reader["DepartmentName"].ToString(),
                            TaskName = reader["TaskName"].ToString(),
                            Description = reader["Description"].ToString(),
                            Status = (TicketStatus)reader["Status"],


                            ExpectedCompleteDate = reader["ExpectedCompleteDate"] as DateTime?,


                            CreatedDate = (DateTime)reader["CreatedDate"]
                        };
                    }
                }
            }

            if (ticket != null)
            {
                ticket.Attachment = GetAttachmentByTicketId(ticketId);
            }

            return ticket;
        }

        // Lấy danh sách file đính kèm theo TicketId
        public static TicketAttachment GetAttachmentByTicketId(int ticketId)
        {
            TicketAttachment attachment = null;

            using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_TicketAttachment_GetByTicketId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TicketId", ticketId);

                    conn.Open();

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        attachment = new TicketAttachment
                        {
                            AttachmentId = (int)reader["AttachmentId"],
                            TicketId = (int)reader["TicketId"],
                            FilePath = reader["FilePath"].ToString()
                        };
                    }
                }
            }

            return attachment;
        }


        /**
         *PHÍA QUẢN TRỊ - XỬ LÝ ĐƠN
         ***/

        // Trả về danh sách đơn của tất cả người dùng (dành cho Ng phụ trách)
        public static List<TicketDetailVM> GetAll()
        {
            List<TicketDetailVM> list = new List<TicketDetailVM>();

            using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_Ticket_GetAll", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new TicketDetailVM
                    {
                        TicketId = Convert.ToInt32(reader["TicketId"]),
                        TicketCode = reader["TicketCode"].ToString(),
                        Description = reader["Description"].ToString(),
                        Status = (TicketStatus)Convert.ToInt32(reader["Status"]),
                        ExpectedCompleteDate = reader["ExpectedCompleteDate"] as DateTime?,
                        CreatedByEmail = reader["CreatedByEmail"].ToString()
                    });
                }
            }

            return list;
        }

        //Chi tiết đơn dành cho người phụ trách xem và xử lý:
        public static TicketManagerDetailVM GetById_Manager(int ticketId)
        {
            TicketManagerDetailVM ticket = null;

            using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_Ticket_GetById_Manager", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@TicketId", ticketId);

                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    ticket = new TicketManagerDetailVM
                    {
                        TicketId = Convert.ToInt32(reader["TicketId"]),
                        TicketCode = reader["TicketCode"].ToString(),
                        DepartmentId = Convert.ToInt32(reader["DepartmentId"]),
                        TaskId = Convert.ToInt32(reader["TaskId"]),
                        Description = reader["Description"].ToString(),
                        Status = (TicketStatus)Convert.ToInt32(reader["Status"]),
                        ExpectedCompleteDate = reader["ExpectedCompleteDate"] as DateTime?,
                        CreatedByEmail = reader["CreatedByEmail"].ToString(),
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                        UpdatedByStaffId = reader["UpdatedByStaffId"] as int?,
                        UpdatedDate = reader["UpdatedDate"] as DateTime?,
                        AssignedToStaffId = reader["AssignedToStaffId"] as int?
                    };
                }
            }
            if (ticket != null)
            {
                ticket.Attachment = GetAttachmentByTicketId(ticketId);
            }

            return ticket;
        }

        // Cập nhật thông tin đơn (dành cho người phụ trách xử lý)
        // -- Phân công staff
        //BEGIN: ASSIGN STAFF
        public static void AssignStaff(int ticketId, int assignedToStaffId, int staffId)
        {
            using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_Ticket_AssignStaff", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@TicketId", ticketId);
                    cmd.Parameters.AddWithValue("@AssignedToStaffId", assignedToStaffId);
                    cmd.Parameters.AddWithValue("@StaffId", staffId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        //END: ASSIGN STAFF

        //-- Cập nhật trạng thái
        //--BEGIN: UPDATE STATUS
        public static void UpdateStatus(int ticketId, int status, int staffId)
        {
            using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_Ticket_UpdateStatus", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@TicketId", ticketId);
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@StaffId", staffId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        //--END: UPDATE STATUS


        // -- Cập nhật ngày hoàn thành dự kiến
        public static void UpdateExpectedDate(int ticketId, DateTime expectedDate, int staffId)
        {
            using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_Ticket_UpdateExpectedDate", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@TicketId", ticketId);
                    cmd.Parameters.AddWithValue("@ExpectedCompleteDate", expectedDate);
                    cmd.Parameters.AddWithValue("@StaffId", staffId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        //Staff đc phân công sẽ nhận được email thông báo về việc được giao đơn mới
        // Cập nhật thông tin đơn (dành cho người phụ trách xử lý)
        //Đơn đc cập nhật sẽ gửi  email thông báo cho người tạo đơn về việc đơn đã được xử lý, trạng thái mới của đơn và ngày hoàn thành dự kiến (nếu có)



        //Hoàn thành xử lí ticket:
        public static void CompleteTicket(int ticketId, int staffId)
        {
            using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_Ticket_Complete", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TicketId", ticketId);
                    cmd.Parameters.AddWithValue("@StaffId", staffId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }


        }
    }
}