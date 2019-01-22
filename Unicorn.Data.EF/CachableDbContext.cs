using EFSecondLevelCache;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Unicorn.Data.EF
{
    public class CachableDbContext : DbContext
    {
        public CachableDbContext() { }
        public CachableDbContext( string nameOrConnectionString) : base(nameOrConnectionString) { }
        public CachableDbContext( string nameOrConnectionString, DbCompiledModel model) : base(nameOrConnectionString, model) { }
        public CachableDbContext(DbCompiledModel model) : base(model) { }
        public CachableDbContext(DbConnection connection, DbCompiledModel model, bool contextOwnsConnection)
            : base(connection, model, contextOwnsConnection) { }
        public CachableDbContext(DbConnection connection, bool contextOwnsConnection)
            : base(connection, contextOwnsConnection) { }
        public CachableDbContext(ObjectContext context, bool contextOwnsConnection)
            : base(context, contextOwnsConnection) { }


        public static event EventHandler<EntitySaveEventArgs> SavingChanges;
        public static event EventHandler<EntitySaveEventArgs> SavedChanges;
        public override int SaveChanges()
        {
            try
            {
                return SaveAllChanges(invalidateCacheDependencies: true);
            }
            catch (DbEntityValidationException ex)
            {
                var sb = new StringBuilder();

                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                throw new DbEntityValidationException(
                    "Entity Validation Failed - errors follow:\n" +
                    sb.ToString(), ex
                    ); // Add the original exception as the innerException
            }
        }

        private string[] getChangedEntityNames()
        {
            return this.ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added ||
                            x.State == EntityState.Modified ||
                            x.State == EntityState.Deleted)
                .Select(x => ObjectContext.GetObjectType(x.Entity.GetType()).FullName)
                .Distinct()
                .ToArray();
        }
        int SaveAllChanges(bool invalidateCacheDependencies = true)
        {
            SavingChanges?.Invoke(this, new EntitySaveEventArgs(this));
            var changedEntityNames = getChangedEntityNames();
            var result = base.SaveChanges();
            if (invalidateCacheDependencies)
            {
                new EFCacheServiceProvider().InvalidateCacheDependencies(changedEntityNames);
            }
            SavedChanges?.Invoke(this, new EntitySaveEventArgs(this));
            return result;
        }


        public T[] GetAll<T>()
            where T : class
        {
            return this.Set<T>().AsQueryable<T>().Cacheable().ToArray();
        }
        public object[] GetAll(Type t)
        {
            var myMethod = typeof(EFSecondLevelCache.EFCachedQueryExtension)
                         .GetMethods()
                         .Where(m => m.Name == "Cacheable")
                         .Select(m => new
                         {
                             Method = m,
                             Params = m.GetParameters(),
                             Args = m.GetGenericArguments()
                         })
                         .Where(x => x.Params.Length == 1
                                     //&& x.Args.Length == 1
                                     && x.Params[0].ParameterType.Name.StartsWith("IQueryable")
                                     )
                         .Select(x => x.Method).First();

            //MethodInfo method = typeof(EFCachedQueryExtension)
            //    .GetMethod("Cacheable", new Type[] { typeof(IQueryable) })
            //    .MakeGenericMethod(new Type[] { t });
            var queriable = ((IQueryable)myMethod.MakeGenericMethod(new Type[] { t })
                .Invoke(null, new object[] { this.Set(t) }));

            MethodInfo method = typeof(Queryable).GetMethod("OfType");
            MethodInfo generic = method.MakeGenericMethod(new Type[] { t });
            var result = (IEnumerable<object>)generic.Invoke(null, new object[] { queriable });
            return result.ToArray();
        }
        public T[] GetAll<T>(Expression<Func<T, bool>> predicate)
            where T : class
        {
            return this.Set<T>().Where(predicate).Cacheable().ToArray();
        }

    }
}
