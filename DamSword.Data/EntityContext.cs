using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DamSword.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DamSword.Data
{
    public class EntityContext : DbContext, IEntityContext
    {
        private static readonly MethodInfo GetPropertyValueGenericMethodInfo = typeof(PropertyValues).GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Single(m => m.IsGenericMethod && m.Name == "GetValue" && m.GetParameters()[0].ParameterType == typeof(IProperty));

        public virtual DbSet<MetaDataSnapshot> MetaDataSnapshots { get; set; }
        public virtual DbSet<EventLog> EventLogs { get; set; }
        public virtual DbSet<MetaAccount> MetaAccounts { get; set; }
        public virtual DbSet<MetaConnection> MetaConnections { get; set; }
        public virtual DbSet<MetaEmail> MetaEmails { get; set; }
        public virtual DbSet<MetaName> MetaNames { get; set; }
        public virtual DbSet<Person> Persons { get; set; }
        public virtual DbSet<PersonMetaProvider> PersonMetaProviders { get; set; }
        public virtual DbSet<Session> Sessions { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<WebResource> WebResources { get; set; }

        public EntityContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MetaAccount>()
                .HasOne(a => a.WebResource)
                .WithMany(r => r.MetaAccounts)
                .OnDelete(DeleteBehavior.Restrict);

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
                var getValueMethod = GetPropertyValueGenericMethodInfo.MakeGenericMethod(property.ClrType);
                var originalValue = getValueMethod.Invoke(entry.OriginalValues, new object[] { property });
                var currentValue = getValueMethod.Invoke(entry.CurrentValues, new object[] { property });

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