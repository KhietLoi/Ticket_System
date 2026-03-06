using System;

namespace PJ_Source_GV.Models
{
    public class TicketAttachment
    {
        public int AttachmentId { get; set; }

        public int TicketId { get; set; }

        public string FilePath { get; set; }

        public DateTime UploadedDate { get; set; }
    }
}
