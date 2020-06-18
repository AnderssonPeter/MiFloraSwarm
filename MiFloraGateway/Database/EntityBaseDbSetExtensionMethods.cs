using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MiFloraGateway.Database;

namespace MiFloraGateway.Database
{
    public static class EntityBaseDbSetExtensionMethods
    {
        /// <exception cref="EntityNotFoundException">If no entity with that id is found!</exception>
        public static async Task<T> GetRequiredByIdAsync<T>(this DbSet<T> dbSet, int id, CancellationToken cancellationToken = default) where T : EntityBase =>
            (await dbSet.SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken)) ?? throw new EntityNotFoundException<T>(id);

        public static Task<T?> GetByIdAsync<T>(this DbSet<T> dbSet, int id, CancellationToken cancellationToken = default) where T : EntityBase =>
            dbSet.SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken)!;

        /// <exception cref="EntityNotFoundException">If no entity with that id is found!</exception>
        public static T GetRequiredById<T>(this DbSet<T> dbSet, int id) where T : EntityBase =>
            dbSet.SingleOrDefault(entity => entity.Id == id) ?? throw new EntityNotFoundException<T>(id);

        public static T? GetById<T>(this DbSet<T> dbSet, int id) where T : EntityBase =>
            dbSet.SingleOrDefault(entity => entity.Id == id);


        public static IQueryable<T> GetByIds<T>(this DbSet<T> dbSet, IEnumerable<int> ids) where T : EntityBase =>
            dbSet.Where(entity => ids.Contains(entity.Id));
    }
}
