using Nursing_Home.Domain.Common.Interfaces;

namespace Nursing_Home.Domain.Common;
public class BaseKeyEntity<TKey> : IEntity<TKey> where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; } = default!;
}

