namespace Nursing_Home.Domain.Common.Interfaces;
public interface IEntity<TKey> where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; }
}
