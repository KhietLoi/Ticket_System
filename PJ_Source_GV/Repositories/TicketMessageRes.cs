using PJ_Source_GV.Caption;
using PJ_Source_GV.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

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
        //Lấy danh sách tin nhắn và file của một ticket
        public static List<TicketMessage> GetByTicket(int ticketId)
        {
            Dictionary<int, TicketMessage> messages = new Dictionary<int, TicketMessage>();

            using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_TicketMessage_GetByTicket", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@TicketId", ticketId);

                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int messageId = Convert.ToInt32(reader["MessageId"]);

                    if (!messages.ContainsKey(messageId))
                    {
                        messages[messageId] = new TicketMessage
                        {
                            MessageId = messageId,
                            TicketId = Convert.ToInt32(reader["TicketId"]),
                            SenderEmail = reader["SenderEmail"].ToString(),
                            SenderName = reader["SenderName"].ToString(),
                            Message = reader["Message"].ToString(),
                            IsStaff = Convert.ToBoolean(reader["IsStaff"]),
                            CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                            Attachments = new List<TicketMessageAttachment>()
                        };
                    }

                    if (reader["FileName"] != DBNull.Value)
                    {
                        messages[messageId].Attachments.Add(new TicketMessageAttachment
                        {
                            AttachmentId = Convert.ToInt32(reader["AttachmentId"]),
                            MessageId = messageId,
                            FileName = reader["FileName"].ToString(),
                            FilePath = reader["FilePath"].ToString()
                        });
                    }
                }
            }

            return messages.Values.ToList();
        }

        // Gửi file đính kèm cho một tin nhắn
        public static void InsertAttachment(int messageId, string fileName, string filePath)
        {
            using (SqlConnection conn = new SqlConnection(ConstValue.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_TicketMessage_Attachment_Insert", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@MessageId", messageId);
                    cmd.Parameters.AddWithValue("@FileName", fileName);
                    cmd.Parameters.AddWithValue("@FilePath", filePath);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
