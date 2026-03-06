using Microsoft.AspNetCore.Identity;
using PJ_Source_GV.Caption;
using PJ_Source_GV.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PJ_Source_GV.Areas.API.Models
{
    public class Permission
    {
        public int id { get; set; }
        public int group_id { get; set; }
        public string group_name { get; set; }
        public int page_id { get; set; }
        public string page_name { get; set; }
        public string page_alias { get; set; }
        public int permission { get; set; }
        public Permission() { }
        public static List<Permission> getAll(int group_id)
        {
            SqlConnection conn = new SqlConnection(ConstValue.ConnectionString);
            try
            {
                List<Permission> permissions = new List<Permission>();
                conn.Open();
                string strQuery = "select per.id id, g.id group_id, g.name group_name, p.id page_id, p.name page_name, p.alias page_alias, per.permission";
                strQuery += " from lp_group g, lp_permission per, lp_page p";
                strQuery += " where g.id = per.group_id and per.page_id = p.id and g.id = " + group_id;
                strQuery += " order by p.id";
                SqlCommand com = new SqlCommand(strQuery, conn);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    Permission p = new Permission();
                    p.id = Int32.Parse(reader["id"].ToString());
                    p.group_id = Int32.Parse(reader["group_id"].ToString());
                    p.group_name = reader["group_name"].ToString();
                    p.page_id = Int32.Parse(reader["page_id"].ToString());
                    p.page_name = reader["page_name"].ToString();
                    p.page_alias = reader["page_alias"].ToString();
                    p.permission = Int32.Parse(reader["permission"].ToString());
                    permissions.Add(p);
                }
                conn.Close();
                return permissions;
            }
            catch (Exception ex)
            {
                conn.Close();
                return null;
            }
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
        public static int update(int id, int group, int page, int permission)
        {
            SqlConnection conn = new SqlConnection(ConstValue.ConnectionString);
            try
            {
                conn.Open();
                string strQuery = "update lp_permission set group_id = @Group_ID, page_id = @Page_ID, permission = @Permission where id=@ID";
                SqlCommand com = new SqlCommand(strQuery, conn);
                com.Parameters.Add(new SqlParameter("@ID", id));
                com.Parameters.Add(new SqlParameter("@Group_ID", group));
                com.Parameters.Add(new SqlParameter("@Page_ID", page));
                com.Parameters.Add(new SqlParameter("@Permission", permission));
                int result = com.ExecuteNonQuery();
                conn.Close();
                return result;
            }
            catch (Exception ex)
            {
                conn.Close();
                return 0;
            }
        }
        public static Dictionary<string, int> getAllFunction()
        {
            SqlConnection conn = new SqlConnection(ConstValue.ConnectionString);
            try
            {
                Dictionary<string, int> function = new Dictionary<string, int>();
                conn.Open();
                string strQuery = "select name, code from lp_function";
                SqlCommand com = new SqlCommand(strQuery, conn);
                SqlDataReader reader = com.ExecuteReader();
                string name = ""; int code = 0;
                while (reader.Read())
                {
                    name = reader["name"].ToString();
                    code = Int32.Parse(reader["code"].ToString());
                    function[name] = code;
                }
                conn.Close();
                return function;
            }
            catch (Exception ex)
            {
                conn.Close();
                return null;
            }
        }
        private static Dictionary<string, int> getPagePermission(string email, string madonvi)
        {
            SqlConnection conn = new SqlConnection(ConstValue.ConnectionString);
            try
            {
                // group.id = 2 : Quyen nguoi dung co ban
                Dictionary<string, int> permission = new Dictionary<string, int>();
                conn.Open();
                string strQuery = @"select distinct g.name, p.alias, per.permission
                    from lp_group g, lp_groupuser gu, lp_permission per, lp_page p
                    where (g.id = gu.group_id and g.id = per.group_id and per.page_id = p.id
                    and gu.email = '" + email + "') or (g.id = 2 and g.id = per.group_id and g.id = per.group_id and per.page_id = p.id)";
                SqlCommand com = new SqlCommand(strQuery, conn);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    string name = reader[1].ToString();
                    int per = Int32.Parse(reader[2].ToString());
                    if (permission.ContainsKey(name))
                        permission[name] = permission[name] | per;
                    else
                        permission[name] = per;
                }
                conn.Close();
                return permission;
            }
            catch (Exception ex)
            {
                conn.Close();
                return null;
            }
        }

        private static bool checkAdmin(int email)
        {
            SqlConnection conn = new SqlConnection(ConstValue.ConnectionString);
            try
            {
                conn.Open();
                SqlCommand com = new SqlCommand("select * from lp_groupuser where email = @Email and group_id = 1", conn);
                com.Parameters.Add(new SqlParameter("@Email", email));
                SqlDataReader reader = com.ExecuteReader();
                bool result = reader.HasRows;
                conn.Close();
                return result;
            }
            catch (Exception ex)
            {
                conn.Close();
                return false;
            }
        }

        private static bool checkAdmin(string email)
        {
            SqlConnection conn = new SqlConnection(ConstValue.ConnectionString);
            try
            {
                conn.Open();
                SqlCommand com = new SqlCommand("select * from lp_groupuser where email = @Email and group_id = 1", conn);
                com.Parameters.Add(new SqlParameter("@Email", email));
                SqlDataReader reader = com.ExecuteReader();
                bool result = reader.HasRows;
                conn.Close();
                return result;
            }
            catch (Exception ex)
            {
                conn.Close();
                return false;
            }
        }
        public static string getPermissionString(string email, string madonvi)
        {
            string strPermission = "";

            Dictionary<string, int> funs = Permission.getAllFunction();
            Dictionary<string, int> dicPers = Permission.getPagePermission(email, madonvi);
            if (dicPers == null)
                return strPermission;
            foreach (string key in dicPers.Keys)
            {
                int permission = dicPers[key];
                foreach (string keyFunc in funs.Keys)
                {
                    int funCode = funs[keyFunc];
                    if ((int)(permission & funCode) == funCode)
                        strPermission += key + "_" + keyFunc + ",";
                }
            }
            if (Permission.checkAdmin(email))
                strPermission += "admin";
            else
                strPermission = strPermission.Trim(',');
            return strPermission;
        }
    }
}
