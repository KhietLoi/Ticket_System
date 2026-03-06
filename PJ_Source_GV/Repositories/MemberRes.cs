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
    public class MemberRes
    {
        public static bool DeleteMember(object[] value, ref string[] output, ref int errorCode,
        ref string errorMessage)
        {
            var connection = new SQLCommand(ConstValue.ConnectionString);
            var result = connection.ExecuteData("lp_groupuser_DeleteUser", value);
            output = connection.output;
            errorCode = connection.errorCode;
            errorMessage = connection.errorMessage;
            return result;
        }

        public static bool InsertMember(object[] value, ref string[] output, ref int errorCode,
        ref string errorMessage)
        {
            var connection = new SQLCommand(ConstValue.ConnectionString);
            var result = connection.ExecuteData("lp_groupuser_InsertUser", value);
            output = connection.output;
            errorCode = connection.errorCode;
            errorMessage = connection.errorMessage;
            return result;
        }

        public static List<Member> GetAll()
        {
            List<Member> lstMember = new List<Member>();
            object[] value = null;
            var connection = new SQLCommand(ConstValue.ConnectionString);
            var result = connection.Select("lp_groupuser_GetAll", value);
            if(connection.errorCode == 0 && result.Rows.Count > 0)
            {
                foreach(DataRow dr in result.Rows)
                {
                    Member member = new Member()
                    {
                        id = int.Parse(dr["id"].ToString()),
                        group_id = int.Parse(dr["group_id"].ToString()),
                        email = dr["email"].ToString()
                    };
                    lstMember.Add(member);
                }
            }
            return lstMember;
        }
    }
}
