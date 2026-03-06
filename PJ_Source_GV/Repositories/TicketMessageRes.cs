using PJ_Source_GV.Caption;
using PJ_Source_GV.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace PJ_Source_GV.Repositories
{
    public class TicketMessageRes
    {
        public static int InsertMessage(TicketMessage model)
        {
            int messageId = 0;

            using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_TicketMessage_Insert", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@TicketId", model.TicketId);
                cmd.Parameters.AddWithValue("@SenderEmail", model.SenderEmail);
                cmd.Parameters.AddWithValue("@SenderName", model.SenderName);
                cmd.Parameters.AddWithValue("@Message", model.Message);
                cmd.Parameters.AddWithValue("@IsStaff", model.IsStaff);

                conn.Open();

                messageId = Convert.ToInt32(cmd.ExecuteScalar());
            }

            return messageId;
        }
        //Lấy danh sách tin nhắn của một ticket
        public static List<TicketMessage> GetByTicket(int ticketId)
        {
            List<TicketMessage> list = new List<TicketMessage>();

            using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_TicketMessage_GetByTicket", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@TicketId", ticketId);

                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new TicketMessage
                    {
                        MessageId = Convert.ToInt32(reader["MessageId"]),
                        TicketId = Convert.ToInt32(reader["TicketId"]),
                        SenderEmail = reader["SenderEmail"].ToString(),
                        SenderName = reader["SenderName"].ToString(),
                        Message = reader["Message"].ToString(),
                        IsStaff = Convert.ToBoolean(reader["IsStaff"]),
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                    });
                }
            }

            return list;
        }
    }
}
