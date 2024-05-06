using Nursing_Home.Application.Services;
using System.Net;
using System.Net.Mail;

namespace Nursing_Home.Infrastructure.Services;
public class EmailService : IEmailService
{
    public async Task SendEmailAsync(string email, string subject, string htmlMessage, CancellationToken cancellationToken = default)
    {
        var client = new SmtpClient
        {
            Port = 587,
            Host = "smtp.gmail.com",
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential("haoluonghuynh2001@gmail.com", "bddg jwrd icyx qphb")
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress("haoluonghuynh2001@gmail.com"),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };

        mailMessage.To.Add(email);

        await client.SendMailAsync(mailMessage, cancellationToken);
    }

}
