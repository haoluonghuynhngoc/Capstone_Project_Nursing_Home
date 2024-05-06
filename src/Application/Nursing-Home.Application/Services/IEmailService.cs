namespace Nursing_Home.Application.Services;
public interface IEmailService
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage, CancellationToken cancellationToken = default);
}
