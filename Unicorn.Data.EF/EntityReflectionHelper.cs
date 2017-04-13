using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Unicorn.Data.EF
{
    public static class EntityReflectionHelper
    {
        public static PropertyInfo GetIdProperty(Type type)
        {
            var prop = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
               .Where(pp => pp.GetCustomAttribute<KeyAttribute>() != null).FirstOrDefault();
            if (prop == null)
            {
                var props = type.GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);
                prop = props.FirstOrDefault(p => p.Name.ToLower() == "id" || p.Name.ToLower() == type.Name + "id");
                if (prop == null)
                    prop = props.FirstOrDefault(p => p.Name.ToLower().EndsWith("id"));
            }
            return prop;
        }

    }
}
