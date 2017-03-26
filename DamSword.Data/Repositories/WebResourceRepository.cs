using DamSword.Data.Entities;

namespace DamSword.Data.Repositories
{
    public interface IWebResourceRepository : IEntityRepository<WebResource>
    {
    }

    public class WebResourceRepository : EntityRepositoryBase<WebResource>, IWebResourceRepository
    {
        public WebResourceRepository(IEntityContext entityContext, bool asNoTracking)
            : base(entityContext, asNoTracking)
        {
        }
    }
}