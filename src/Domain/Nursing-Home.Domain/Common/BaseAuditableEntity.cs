using Nursing_Home.Domain.Common.Interfaces;

namespace Nursing_Home.Domain.Common;
public class BaseAuditableEntity<TKey> : BaseKeyEntity<TKey>, IEntity<TKey>, IAuditableEntity where TKey : IEquatable<TKey>
{
    public string? CreatedBy { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTimeOffset? ModifiedAt { get; set; }
    public string? DeletedBy { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
