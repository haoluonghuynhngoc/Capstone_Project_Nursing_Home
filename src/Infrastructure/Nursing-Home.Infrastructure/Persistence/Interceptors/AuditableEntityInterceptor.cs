using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Nursing_Home.Domain.Common.Interfaces;

namespace Nursing_Home.Infrastructure.Persistence.Interceptors;
public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string Anonymous = nameof(Anonymous);
    public AuditableEntityInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
    public void UpdateEntities(DbContext? context)
    {
        var CurrentUserId = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? Anonymous;
        if (context == null) return;
        foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = CurrentUserId;
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                entry.Entity.ModifiedBy = CurrentUserId;
                entry.Entity.ModifiedAt = DateTime.UtcNow;
            }
        }
    }
}
public static class Extensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
         entry.References.Any(r =>
             r.TargetEntry != null &&
             r.TargetEntry.Metadata.IsOwned() &&
             (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}
