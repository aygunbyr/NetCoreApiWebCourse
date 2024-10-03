using App.Repositories;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace App.Services.Interceptors
{
    public class AuditDbContextInterceptor : SaveChangesInterceptor
    {
        private static readonly Dictionary<EntityState, Action<DbContext, IAuditEntity>> Behaviors = new()
        {
            {EntityState.Added, AddedBehavior },
            {EntityState.Modified, ModifiedBehavior }
        };
        private static void AddedBehavior(DbContext context, IAuditEntity auditEntity)
        {
            auditEntity.Created = DateTime.UtcNow;
            context.Entry(auditEntity).Property(x => x.Updated).IsModified = false;
        }

        private static void ModifiedBehavior(DbContext context, IAuditEntity auditEntity)
        {
            context.Entry(auditEntity).Property(x => x.Created).IsModified = false;
            auditEntity.Updated = DateTime.UtcNow;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            foreach(var entityEntry in eventData.Context!.ChangeTracker.Entries().ToList())
            {
                if (entityEntry.Entity is not IAuditEntity auditEntity)
                {
                    continue;
                }

                Behaviors[entityEntry.State](eventData.Context, auditEntity);

                //switch (entityEntry.State)
                //{
                //    case EntityState.Added:
                //        AddedBehavior(eventData.Context, auditEntity);
                //        break;

                //    case EntityState.Modified:
                //        ModifiedBehavior(eventData.Context, auditEntity);
                //        break;
                //}
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
