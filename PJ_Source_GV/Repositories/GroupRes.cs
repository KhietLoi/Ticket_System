using PJ_Source_GV.Caption;
using CAIT.SQLHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PJ_Source_GV.Repositories
{
    public class GroupRes
    {
        public static bool Group_Save(object[] value, ref string[] output, ref int errorCode,
        ref string errorMessage)
        {
            var connection = new SQLCommand(ConstValue.ConnectionString);
            var result = connection.ExecuteData("lp_group_Save", value);
            output = connection.output;
            errorCode = connection.errorCode;
            errorMessage = connection.errorMessage;
            return result;
        }

        public static bool Group_Delete(object[] value, ref string[] output, ref int errorCode,
        ref string errorMessage)
        {
            var connection = new SQLCommand(ConstValue.ConnectionString);
            var result = connection.ExecuteData("lp_group_Delete", value);
            output = connection.output;
            errorCode = connection.errorCode;
            errorMessage = connection.errorMessage;
            return result;
        }
    }
}
