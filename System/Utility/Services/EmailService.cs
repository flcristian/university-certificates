using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using DotNetEnv;

namespace UniversityCertificates.System.Utility.Services;

public class EmailService
{
    private readonly SmtpClient _smtpClient;

    public EmailService()
    {
        _smtpClient = new SmtpClient(Env.GetString("SMTP_HOST"), Env.GetInt("SMTP_PORT"));
        _smtpClient.Credentials = new NetworkCredential(
            Env.GetString("SMTP_EMAIL"),
            Env.GetString("SMTP_PASSWORD")
        );
        _smtpClient.EnableSsl = true;
    }

    public async Task SendEmail(
        string to,
        string subject,
        string body,
        List<(byte[] fileContents, string fileName)> attachedFiles
    )
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(Env.GetString("SMTP_EMAIL")),
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
        };

        List<Attachment> attachments = attachedFiles
            .Select(file => new Attachment(new MemoryStream(file.fileContents), file.fileName))
            .ToList();
        attachments.ForEach(mailMessage.Attachments.Add);
        mailMessage.To.Add(to);

        await _smtpClient.SendMailAsync(mailMessage);
    }
}
