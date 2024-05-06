using Nursing_Home.Domain.Entities.Identities;
using System.Security.Claims;

namespace Nursing_Home.Application.Services;
public interface ICurrentUserService
{
    public string? CurrentUserId { get; }

    public ClaimsPrincipal? CurrentUserPrincipal { get; }

    Task<User> FindCurrentUserAsync();
    Task<Guid> FindCurrentUserIdAsync();
}
