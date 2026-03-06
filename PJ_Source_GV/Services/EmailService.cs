using Microsoft.Extensions.Options;
using PJ_Source_GV.Models;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
namespace PJ_Source_GV.Services
{
    public class EmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        /*public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using (var smtp = new SmtpClient(_settings.SmtpHost))
            {
                smtp.Port = _settings.SmtpPort;
                smtp.Credentials = new NetworkCredential(
                    _settings.SmtpUser,
                    _settings.SmtpPass
                );
                smtp.EnableSsl = _settings.EnableSsl;

                var mail = new MailMessage
                {
                    From = new MailAddress(
                        _settings.FromEmail,
                        _settings.DisplayName
                    ),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mail.To.Add(toEmail);

                await smtp.SendMailAsync(mail);
            }
        }*/
        public async Task SendEmailAsync(
    string toEmail,
    string subject,
    string body,
    List<string> attachmentPaths = null
)
        {
            using (var smtp = new SmtpClient(_settings.SmtpHost))
            {
                smtp.Port = _settings.SmtpPort;
                smtp.Credentials = new NetworkCredential(
                    _settings.SmtpUser,
                    _settings.SmtpPass
                );
                smtp.EnableSsl = _settings.EnableSsl;

                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress(
                        _settings.FromEmail,
                        _settings.DisplayName
                    );

                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;
                    mail.To.Add(toEmail);

                    // 👉 Attach file nếu có
                    if (attachmentPaths != null)
                    {
                        foreach (var path in attachmentPaths)
                        {
                            if (System.IO.File.Exists(path))
                            {
                                mail.Attachments.Add(new Attachment(path));
                            }
                        }
                    }

                    await smtp.SendMailAsync(mail);
                }
            }
        }


    }
}
