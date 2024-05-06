using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nursing_Home.Domain.Entities.Identities;

namespace Nursing_Home.Infrastructure.Persistence.Data;
public class ApplicationDbContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
{
    private const string Prefix = "AspNet";

    //public ApplicationDbContext()
    //{
    //}
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.UseMySQL("server=localhost;user=root;password=root;database=Nursing_Home_DB"); // Localhost 
    //    //optionsBuilder.UseMySQL("server=mysql-server-database.mysql.database.azure.com;user=MySQL_Data01;password=NoP@ssword;database=server_database_demo;SslMode=Required;TlsVersion=TLS 1.2"); // Server Azure Database
    //}
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        //builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            if (tableName != null && tableName.StartsWith(Prefix))
            {
                entityType.SetTableName(tableName.Substring(6));
            }
        }
        builder.Entity<User>(b =>
        {
            b.HasMany(e => e.UserRoles)
                .WithOne()
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
        });

        builder.Entity<UserRole>(b =>
        {
            b.HasOne(e => e.Role)
                .WithMany()
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
        });
    }
}
