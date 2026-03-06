using PJ_Source_GV.Areas.API.Models;
using PJ_Source_GV.Caption;
using PJ_Source_GV.Models;
using CAIT.SQLHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PJ_Source_GV.Repositories
{
    public class PageRes
    {
        public static List<Page> Page_GetAll()
        {
            List<Page> lstPage = new List<Page>();
            object[] value = null;
            var connection = new SQLCommand(ConstValue.ConnectionString);
            var result = connection.Select("lp_page_GetAll", value);
            if (connection.errorCode == 0 && result.Rows.Count > 0)
            {
                foreach (DataRow dr in result.Rows)
                {
                    Page page = new Page()
                    {
                        id = int.Parse(dr["id"].ToString()),
                        name = dr["name"].ToString(),
                        alias = dr["alias"].ToString(),
                        note = dr["note"].ToString(),
                        permission = int.Parse(dr["permission"].ToString()),
                    };
                    lstPage.Add(page);
                }
            }
            return lstPage;
        }
        public static bool Save(object[] value, ref string[] output, ref int errorCode,
        ref string errorMessage)
        {
            var connection = new SQLCommand(ConstValue.ConnectionString);
            var result = connection.ExecuteData("lp_page_Save", value);
            output = connection.output;
            errorCode = connection.errorCode;
            errorMessage = connection.errorMessage;
            return result;
        }

        public static bool Delete(object[] value, ref string[] output, ref int errorCode,
       ref string errorMessage)
        {
            var connection = new SQLCommand(ConstValue.ConnectionString);
            var result = connection.ExecuteData("lp_page_Delete", value);
            output = connection.output;
            errorCode = connection.errorCode;
            errorMessage = connection.errorMessage;
            return result;
        }
    }
}
