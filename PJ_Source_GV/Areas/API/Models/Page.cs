using PJ_Source_GV.Caption;
using PJ_Source_GV.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PJ_Source_GV.Areas.API.Models
{
    public class Page
    {
        public int id { get; set; }
        public string name { get; set; }
        public string alias { get; set; }
        public string note { get; set; }
        public int permission { get; set; }
        public Page() { }
        public Page(int id, string name, string alias, string note, int permission)
        {
            this.id = id;
            this.name = name;
            this.alias = alias;
            this.note = note;
            this.permission = permission;
        }
        public static string getTableName() { return "lp_page"; }
        public static List<Page> getPageUnManager(int group_id)
        {
            SqlConnection conn = new SqlConnection(ConstValue.ConnectionString);
            try
            {
                List<Page> pages = new List<Page>();
                conn.Open();
                string strQuery = "SELECT * FROM lp_page page WHERE page.id NOT IN ";
                strQuery += "(SELECT p.id FROM lp_permission per, lp_group g, lp_page p WHERE p.id = per.page_id and g.id = per.group_id and g.id=@Group_ID)";
                SqlCommand com = new SqlCommand(strQuery, conn);
                com.Parameters.Add(new SqlParameter("@Group_ID", group_id));
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    Page page = new Page();
                    page.id = Int32.Parse(reader["id"].ToString());
                    page.name = reader["name"].ToString();
                    page.alias = reader["alias"].ToString();
                    page.note = reader["note"].ToString();
                    pages.Add(page);
                }
                conn.Close();
                return pages;
            }
            catch (Exception ex)
            {
                conn.Close();
                return null;
            }
        }
    }
}
