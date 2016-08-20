using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicorn.Data.EF
{
    public class EntityEventArgs<TEntity>
    {
        public EntityEventArgs(TEntity entity, EntityState state)
        {
            Entity = entity;
            EntityState = state;
        }
        public TEntity Entity { get; }
        public EntityState EntityState { get; }
    }
}
