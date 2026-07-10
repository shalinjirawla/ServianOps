using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServianOps_Backend.Application.Interfaces;
using ServianOps_Backend.Core.Entities.Base;
using ServianOps_Backend.Core.Entities.Identity;
using ServianOps_Backend.Core.Entities.Saas;
using ServianOps_Backend.Core.Entities.Crm;
using ServianOps_Backend.Core.Entities.Jobs;
using ServianOps_Backend.Core.Interfaces;

namespace ServianOps_Backend.EntityFramework.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ICurrentTenant _currentTenant;
        private readonly ICurrentUser _currentUser;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentTenant currentTenant, ICurrentUser currentUser) 
            : base(options)
        {
            _currentTenant = currentTenant;
            _currentUser = currentUser;
        }

        public DbSet<Plan> Plans { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<CustomerType> CustomerTypes { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerContact> CustomerContacts { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<SiteContact> SiteContacts { get; set; }

        public DbSet<Trade> Trades { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobAttachment> JobAttachments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Global delete behavior for relationships to avoid cascading physical deletes
            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var isAuditEntity = typeof(IAuditEntity).IsAssignableFrom(entityType.ClrType);
                var isMustHaveTenant = typeof(IMustHaveTenant).IsAssignableFrom(entityType.ClrType);
                var isMayHaveTenant = typeof(IMayHaveTenant).IsAssignableFrom(entityType.ClrType);
                
                if (isAuditEntity || isMustHaveTenant || isMayHaveTenant)
                {
                    var method = typeof(ApplicationDbContext)
                        .GetMethod(nameof(ApplyGlobalFilters), BindingFlags.NonPublic | BindingFlags.Instance)
                        ?.MakeGenericMethod(entityType.ClrType);
                    
                    method?.Invoke(this, new object[] { builder, isAuditEntity, isMustHaveTenant, isMayHaveTenant });
                }
            }

        }

        private void ApplyGlobalFilters<T>(ModelBuilder builder, bool hasAudit, bool isMustHaveTenant, bool isMayHaveTenant) where T : class
        {
            Expression<Func<T, bool>> filter = x => true;

            if (hasAudit)
            {
                Expression<Func<T, bool>> auditFilter = x => !((IAuditEntity)x).IsDeleted;
                filter = CombineExpressions(filter, auditFilter);
            }

            if (isMustHaveTenant)
            {
                Expression<Func<T, bool>> tenantFilter = x => _currentTenant.TenantId == null || ((IMustHaveTenant)x).TenantId == _currentTenant.TenantId;
                filter = CombineExpressions(filter, tenantFilter);
            }

            if (isMayHaveTenant)
            {
                Expression<Func<T, bool>> tenantFilter = x => _currentTenant.TenantId == null || ((IMayHaveTenant)x).TenantId == _currentTenant.TenantId;
                filter = CombineExpressions(filter, tenantFilter);
            }

            builder.Entity<T>().HasQueryFilter(filter);
        }

        private Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));
            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);
            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
        }

        class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression node)
            {
                if (node == _oldValue)
                    return _newValue;
                return base.Visit(node);
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            long? currentUserId = _currentUser?.UserId;

            foreach (var entry in ChangeTracker.Entries<IAuditEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreationTime = DateTime.UtcNow;
                        entry.Entity.CreatorUserId = currentUserId;
                        entry.Entity.IsDeleted = false;
                        entry.Entity.IsActive = true;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModificationTime = DateTime.UtcNow;
                        entry.Entity.LastModifierUserId = currentUserId;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedDate = DateTime.UtcNow;
                        entry.Entity.DeletedBy = currentUserId;
                        entry.Entity.IsActive = false;
                        break;
                }
            }

            foreach (var entry in ChangeTracker.Entries<IMustHaveTenant>().Where(e => e.State == EntityState.Added))
            {
                if (_currentTenant != null && _currentTenant.IsAuthenticated && _currentTenant.TenantId.HasValue)
                {
                    entry.Entity.TenantId = _currentTenant.TenantId.Value;
                }
            }

            foreach (var entry in ChangeTracker.Entries<IMayHaveTenant>().Where(e => e.State == EntityState.Added))
            {
                if (_currentTenant != null && _currentTenant.IsAuthenticated && _currentTenant.TenantId.HasValue)
                {
                    entry.Entity.TenantId = _currentTenant.TenantId.Value;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
