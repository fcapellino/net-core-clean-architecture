namespace MainProject.Infrastructure.DataBaseContext
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Domain.Common;
    using Domain.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid,
        IdentityUserClaim<Guid>,
        ApplicationUserRole,
        IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>,
        IdentityUserToken<Guid>>,
        IDbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUserRole>(userRole =>
            {
                userRole
                    .HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole
                    .HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });
            builder.ApplyGlobalFilters<IEntity>(e => !e.IsDeleted);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }
    }

    public static class EntityFrameworkExtensions
    {
        public static void ApplyGlobalFilters<TInterface>(this ModelBuilder builder, Expression<Func<TInterface, bool>> expression)
        {
            builder.Model.GetEntityTypes()
                .Where(e => e.ClrType.GetInterface(typeof(TInterface).Name) != null)
                .Select(e => e.ClrType).ToList().ForEach(entity =>
                {
                    var parameter = Expression.Parameter(entity);
                    var body = ReplacingExpressionVisitor.Replace(expression.Parameters.Single(), parameter, expression.Body);
                    builder.Entity(entity).HasQueryFilter(Expression.Lambda(body, parameter));
                });
        }
    }
}
