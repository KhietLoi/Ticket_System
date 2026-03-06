using PJ_Source_GV.Areas.API.Models;
using PJ_Source_GV.Caption;
using PJ_Source_GV.Models;
using CAIT.SQLHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PJ_Source_GV.Repositories
{
    public class PermissionRes
    {
        public static List<Permission> Permission_GetAll()
        {
            List<Permission> lstPermission = new List<Permission>();
            object[] value = null;
            var connection = new SQLCommand(ConstValue.ConnectionString);
            var result = connection.Select("lp_pemrission_GetAll", value);
            if (connection.errorCode == 0 && result.Rows.Count > 0)
            {
                foreach (DataRow dr in result.Rows)
                {
                    Permission permission = new Permission()
                    {
                        id = int.Parse(dr["id"].ToString()),
                        group_id = int.Parse(dr["group_id"].ToString()),
                        page_id = int.Parse(dr["page_id"].ToString()),
                        permission = int.Parse(dr["permission"].ToString()),
                    };
                    lstPermission.Add(permission);
                }
            }
            return lstPermission;
        }
        public static int insert(int group, int page, int permission)
        {
            SqlConnection conn = new SqlConnection(ConstValue.ConnectionString);
            try
            {
                conn.Open();
                SqlCommand com = new SqlCommand("INSERT INTO lp_permission OUTPUT inserted.id VALUES (@Group_ID, @Page_ID, @Permission)", conn);
                com.Parameters.Add(new SqlParameter("@Group_ID", group));
                com.Parameters.Add(new SqlParameter("@Page_ID", page));
                com.Parameters.Add(new SqlParameter("@Permission", permission));
                int result = (int)com.ExecuteScalar();
                conn.Close();
                return result;
            }
            catch (Exception ex)
            {
                conn.Close();
                return 0;
            }
        }
    }
}
