namespace Nursing_Home.Infrastructure.ThirdParty;
public sealed class EmailOptions
{
    public static readonly string Section = "Email";

    public int Port { get; set; }
    public string Host { get; set; } = default!;
    public bool EnableSsl { get; set; }
    public bool UseDefaultCredentials { get; set; }
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public bool IsBodyHtml { get; set; }
    public string From { get; set; } = default!;
}