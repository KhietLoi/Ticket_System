using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PJ_Source_GV.ViewModels
{
    public class TicketCreateVM
    {
        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public int TaskId { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string CreatedByEmail { get; set; }

        public List<IFormFile> Attachments { get; set; }

    }
}
