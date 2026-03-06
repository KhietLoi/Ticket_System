using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PJ_Source_GV.FunctionSupport
{
    public static class GetDateUpdate
    {
        public static string getRegisterDate()
        {
            var formatDate = "yyyy-MM-dd";
            return DateTime.Now.ToString(formatDate);
        }

        public static string getLastDate()
        {
            var formatDate = "yyyy-MM-dd";
            return DateTime.Now.ToString(formatDate);
        }

        public static string getDataVersion()
        {
            var formatDate = "yyyyMMddHHmmss";
            return DateTime.Now.ToString(formatDate);
        }

        public static DateTime getCurrentDate()
        {
            return DateTime.Now;
        }
        public static string getStringCurrentDate()
        {
            var formatDate = "yyyy-MM-dd";
            return DateTime.Now.ToString(formatDate);
        }
    }
}
