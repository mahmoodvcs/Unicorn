using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Unicorn.Data.EF.DataAnnotations
{
    public class DataColumnAttribute : Attribute
    {
        public DataColumnAttribute()
        {
            Display = true;
            Editable = true;
            Sortable = true;
        }
        public bool Display { get; set; }
        public bool Editable { get; set; }
        public bool Sortable { get; set; }
        public Type FilterDelegateType { get; set; }
        public string MappedFromColumn { get; set; }
        /// <summary>
        /// Name of the method or delegate whi this syntax: <code>Func<IQueryable, string, IQueryable></code>
        /// example: <code>public static IQueryable FilterBank(IQueryable requests, string text)</code>
        /// </summary>
        public string FilterDelegate { get; set; }

        public IQueryable FilterUsingDelegate(IQueryable data, string text)
        {
            if (FilterDelegateType == null || FilterDelegate == null)
                throw new InvalidOperationException("To use filtering for non standard columns you have to set bothe 'FilterDelegateType' and 'FilterDelegate'.");
            var filterMethod = FilterDelegateType.GetMethod(FilterDelegate, new Type[] {
                typeof(IQueryable), typeof(string)
            });
            if (filterMethod == null || filterMethod.ReflectedType != typeof(IQueryable))
                throw new InvalidOperationException($"Invalid method signature for '{FilterDelegate}'. The correct is 'public static IQueryable {FilterDelegate}(IQueryable data, string text)'");
            return (IQueryable)filterMethod.Invoke(null, new object[] { data, text });
        }
    }
}
