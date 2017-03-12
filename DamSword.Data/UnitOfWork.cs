namespace DamSword.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IEntityContext _entityContext;

        public UnitOfWork(IEntityContext entityContext)
        {
            _entityContext = entityContext;
        }

        public void Commit()
        {
            _entityContext.SaveChanges();
        }
    }
}