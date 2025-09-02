using AutService.JwAuthLogin.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace AutService.JwAuthLogin.Infrastructure.Context
{
    public class DatabaseContext : IdentityDbContext<AppUser>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectString = "";

                optionsBuilder.UseNpgsql(connectString);
            }

            base.OnConfiguring(optionsBuilder);
        }

protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder);

    builder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);

    builder.Entity<AppUser>(entity => { entity.ToTable(name: "Users"); });
    builder.Entity<IdentityRole>(entity => { entity.ToTable(name: "Roles"); });
    builder.Entity<IdentityUserRole<string>>(entity => { entity.ToTable("UserRoles"); });
    builder.Entity<IdentityUserClaim<string>>(entity => { entity.ToTable("UserClaims"); });
    builder.Entity<IdentityUserLogin<string>>(entity => { entity.ToTable("UserLogins"); });
    builder.Entity<IdentityUserToken<string>>(entity => { entity.ToTable("UserTokens"); });
    builder.Entity<IdentityRoleClaim<string>>(entity => { entity.ToTable("RoleClaims"); });
}
    }
}