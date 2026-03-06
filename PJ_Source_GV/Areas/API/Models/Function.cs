using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PJ_Source_GV.Areas.API.Models
{
    public class Function
    {
        public int id { get; set; }
        public string name { get; set; }
        public int code { get; set; }
        public string note { get; set; }
        public Function() { }
        public static string getTableName() { return "lp_function"; }
    }
}
