using Microsoft.AspNetCore.Identity;

namespace Nursing_Home.Domain.Entities.Identities;
public class Role : IdentityRole<Guid>
{
    public Role()
    {
    }

    public Role(string roleName) : this()
    {
        Name = roleName;
    }
}