using Microsoft.AspNetCore.Identity;

namespace Nursing_Home.Domain.Entities.Identities;
public class UserRole : IdentityUserRole<Guid>
{
    public virtual Role Role { get; set; } = default!;
}
