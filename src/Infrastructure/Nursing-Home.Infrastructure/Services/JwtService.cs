using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Nursing_Home.Application.Common.Security;
using Nursing_Home.Application.Services;
using Nursing_Home.Domain.Constants;
using Nursing_Home.Domain.Entities.Identities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Nursing_Home.Infrastructure.Services;
public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;
    private readonly SignInManager<User> _signInManager;
    private readonly IDataProtectionProvider _dataProtectionProvider;
    private readonly IDataProtector _protector;
    private readonly TicketDataFormat _ticketDataFormat;
    public JwtService(
    SignInManager<User> signInManager,
        IDataProtectionProvider dataProtectionProvider,
        IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
        _signInManager = signInManager;
        _dataProtectionProvider = dataProtectionProvider;
        _protector = _dataProtectionProvider.CreateProtector(_jwtSettings.SerectRefreshKey);
        _ticketDataFormat = new TicketDataFormat(_protector);
    }

    public async Task<AccessTokenResponse> GenerateTokenAsync(User user, long? expiresTime = null)
    {
        var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
        var claims = claimsPrincipal.Claims.ToList();
        claims.Add(new Claim(ClaimUser.AvatarUrl, user.Avatar ?? ""));
        claims.Add(new Claim(ClaimUser.FullName, user.FullName ?? ""));
        claims.Add(new Claim(ClaimUser.PhoneNumber, user.PhoneNumber ?? ""));
        claims.Add(new Claim(ClaimUser.UserName, user.UserName ?? ""));
        claims.Add(new Claim(ClaimUser.Id, user.Id.ToString() ?? ""));
        claims.Add(new Claim(ClaimUser.Email, user.Email ?? ""));

        // thiếu add Roles có thể thêm vào ở đây 
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SerectKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var expires = expiresTime ?? _jwtSettings.TokenExpire;

        var token = new JwtSecurityToken(
        claims: claims,
            expires: DateTime.UtcNow.AddSeconds(expires),
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        var response = new AccessTokenResponse
        {
            AccessToken = jwt,
            ExpiresIn = expires,
            RefreshToken = _ticketDataFormat.Protect(CreateRefreshTicket(claimsPrincipal, DateTimeOffset.UtcNow))
        };
        return response;
    }

    public async Task<User> ValidateRefreshTokenAsync(string refreshToken)
    {
        var ticket = _ticketDataFormat.Unprotect(refreshToken);
        if (ticket?.Properties?.ExpiresUtc is not { } expiresUtc ||
            DateTimeOffset.UtcNow >= expiresUtc ||
            await _signInManager.ValidateSecurityStampAsync(ticket.Principal) is not User user
            )
        {
            throw new UnauthorizedAccessException("Refresh token is not valid.");
        }
        return user;
    }
    private AuthenticationTicket CreateRefreshTicket(ClaimsPrincipal user, DateTimeOffset utcNow)
    {
        var refreshProperties = new AuthenticationProperties
        {
            ExpiresUtc = utcNow.AddSeconds(_jwtSettings.RefreshTokenExpire)
        };
        return new AuthenticationTicket(user, refreshProperties, JwtBearerDefaults.AuthenticationScheme);
    }
}