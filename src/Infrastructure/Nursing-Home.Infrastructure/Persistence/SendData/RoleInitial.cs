using Nursing_Home.Domain.Constants;
using Nursing_Home.Domain.Entities.Identities;

namespace Nursing_Home.Infrastructure.Persistence.SendData;
public static class RoleInitial
{
    public static IList<Role> Default => new List<Role>()
    {
        new(RoleName.Admin),
        new(RoleName.Director),
        new(RoleName.Manager),
        new(RoleName.Staff),
        new(RoleName.Nurses),
        new(RoleName.User),
        new(RoleName.Guest),
    };
}
