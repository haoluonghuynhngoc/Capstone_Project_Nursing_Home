using Microsoft.AspNetCore.Identity;
using Nursing_Home.Domain.Common.Interfaces;

namespace Nursing_Home.Domain.Entities.Identities;
public class User : IdentityUser<Guid>, IAuditableEntity
{
    public string? Avatar { get; set; }
    public string? FullName { get; set; }
    public string? DateOfBirth { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTimeOffset? ModifiedAt { get; set; }
    public string? DeletedBy { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public virtual ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();

}
