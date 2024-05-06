using Nursing_Home.Application.Common.Security;
using Nursing_Home.Domain.Entities.Identities;

namespace Nursing_Home.Application.Services;
public interface IJwtService
{
    public Task<AccessTokenResponse> GenerateTokenAsync(User users, long? expiresTime = null);
    public Task<User> ValidateRefreshTokenAsync(string refreshToken);
}
