using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicorn.Data.EF
{
    public class EntitySaveEventArgs
    {
        public EntitySaveEventArgs(DbContext db)
        {
            this.DbContext = db;
        }
        public readonly DbContext DbContext;
        public IEnumerable<DbEntityEntry<T>> GetChanges<T>()
            where T : class
        {
            return DbContext.ChangeTracker.Entries<T>();
        }
        public IEnumerable<T> GetChanges<T>(EntityState state)
            where T : class
        {
            return DbContext.ChangeTracker.Entries<T>().Where(e => e.State == state).Select(e => e.Entity);
        }
    }
}
