namespace PJ_Source_GV.Models
{
    public class EmailSettings
    {
        public string FromEmail { get; set; }
        public string DisplayName { get; set; }

        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUser { get; set; }
        public string SmtpPass { get; set; }

        public bool EnableSsl { get; set; }

    }
}
