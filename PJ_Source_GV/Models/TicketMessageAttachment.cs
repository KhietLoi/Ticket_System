using System;

namespace PJ_Source_GV.Models
{
    public class TicketMessageAttachment
    {
        public int AttachmentId { get; set; }
        public int MessageId { get; set; }
        public string FilePath { get; set; }

        public string FileName { get; set; }
        public DateTime UploadedDate { get; set; }

    }
}
