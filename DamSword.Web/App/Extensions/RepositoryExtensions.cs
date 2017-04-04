using System.Linq;
using DamSword.Data;
using DamSword.Data.Repositories;
using DamSword.Web.DTO;

namespace DamSword.Web
{
    public static class RepositoryExtensions
    {
        public static IQueryable<TEntity> QueryApiListRequest<TEntity>(this IEntityRepository<TEntity> self, GenericListFetch request)
            where TEntity : IEntity
        {
            var timestamp = request.GetTimeStamp();
            var query = (IQueryable<TEntity>)self.Select(e => e.CreatedAt >= timestamp).OrderByDescending(e => e.CreatedAt);
            if (request.Offset.HasValue)
                query = query.Skip(request.Offset.Value);
            if (request.Count.HasValue)
                query = query.Take(request.Count.Value);

            return query;
        }
    }
}