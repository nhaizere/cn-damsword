using System.Collections.Generic;
using System.Linq;
using DamSword.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DamSword.Data
{
    public class EntityContext : DbContext, IEntityContext
    {
        public virtual DbSet<MetaAccount> MetaAccounts { get; set; }
        public virtual DbSet<MetaConnection> MetaConnections { get; set; }
        public virtual DbSet<MetaEmail> MetaEmails { get; set; }
        public virtual DbSet<MetaName> MetaNames { get; set; }
        public virtual DbSet<MetaTimeline> MetaTimelines { get; set; }
        public virtual DbSet<Person> Persons { get; set; }
        public virtual DbSet<PersonMetaProvider> PersonMetaProviders { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<WebResource> WebResources { get; set; }

        public EntityContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MetaConnection>()
                .HasOne(c => c.Person)
                .WithMany(p => p.Connections)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MetaConnection>()
                .HasOne(c => c.ConnectedPerson)
                .WithMany(p => p.ConnectedTo)
                .OnDelete(DeleteBehavior.Restrict);
        }

        void IEntityContext.Add<TEntity>(TEntity entity)
        {
            Set<TEntity>().Add(entity);
        }

        void IEntityContext.Attach<TEntity>(TEntity entity)
        {
            Set<TEntity>().Attach(entity);
        }

        void IEntityContext.Remove<TEntity>(TEntity entity)
        {
            Set<TEntity>().Remove(entity);
        }

        IQueryable<TEntity> IEntityContext.Set<TEntity>(bool asNoTracking)
        {
            var set = Set<TEntity>();
            return asNoTracking ? set.AsNoTracking() : set;
        }
        
        void IEntityContext.SaveChanges()
        {
            SaveChanges();
        }

        IEnumerable<EntityChange<TEntity>> IEntityChangesProvider.GetEntityChanges<TEntity>()
        {
            var entries = ChangeTracker.Entries<TEntity>();
            var changes = new List<EntityChange<TEntity>>();
            foreach (var entry in entries)
            {
                EntityChangeState state;
                switch (entry.State)
                {
                    case EntityState.Added:
                        state = EntityChangeState.Added;
                        break;
                    case EntityState.Deleted:
                        state = EntityChangeState.Deleted;
                        break;
                    case EntityState.Modified:
                        state = EntityChangeState.Modified;
                        break;
                    default:
                        continue;
                }

                changes.Add(new EntityChange<TEntity>
                {
                    Entity = entry.Entity,
                    State = state,
                    ModifiedProperties = state == EntityChangeState.Modified ? GetEntityModifiedPropertiesGenerator(entry).ToArray() : null
                });
            }

            return changes.AsReadOnly();
        }

        private static IEnumerable<EntityModifiedProperty> GetEntityModifiedPropertiesGenerator<TEntity>(EntityEntry<TEntity> entry)
            where TEntity : class, IEntity
        {
            foreach (var property in entry.CurrentValues.Properties)
            {
                var originalValue = entry.OriginalValues.GetValue<object>(property);
                var currentValue = entry.CurrentValues.GetValue<object>(property);

                if (originalValue?.Equals(currentValue) == true)
                {
                    yield return new EntityModifiedProperty
                    {
                        PropertyName = property.Name,
                        OriginalValue = originalValue,
                        CurrentValue = currentValue
                    };
                }
            }
        }
    }
}