using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Nursing_Home.Application.Services;
using Nursing_Home.Domain.Entities.Identities;
using Nursing_Home.Utils.Convert;
using System.Security.Claims;

namespace Nursing_Home.Infrastructure.Services;
public class CurrentUserService : ICurrentUserService
{

    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }
    public ClaimsPrincipal? CurrentUserPrincipal => _httpContextAccessor.HttpContext?.User;
    public string? CurrentUserId => CurrentUserPrincipal?.Identity?.Name;

    public async Task<User> FindCurrentUserAsync()
    {
        if (CurrentUserPrincipal != null &&
            await _userManager.GetUserAsync(CurrentUserPrincipal) is { } user
            )
        {
            return user;
        }
        throw new UnauthorizedAccessException("Authentication required. Please provide valid credentials.");
    }
    public Task<Guid> FindCurrentUserIdAsync()
    {
        if (CurrentUserId == null)
        {
            throw new UnauthorizedAccessException("Authentication required. Please provide valid credentials.");
        }
        return Task.FromResult(CurrentUserId.ConvertToGuid());
    }
}
