using Microsoft.AspNetCore.Identity;

namespace PJ_Source_GV.Areas.API.Models
{
    public class Group: IdentityRole
    {
        public int id { get; set; }
        public string name { get; set; }
        public string note { get; set; }
        public Group() { }
        public Group(int id, string name, string note)
        {
            this.id = id;
            this.name = name;
            this.note = note;
        }
        public static string getTableName() { return "lp_group"; }
    }
}
