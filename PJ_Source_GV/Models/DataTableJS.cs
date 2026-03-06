using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PJ_Source_GV.Models
{
    public class DataTableJS<T>
    {
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public T[] data { get; set; }
    }
}
