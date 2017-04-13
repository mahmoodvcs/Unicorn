using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Unicorn.Data.EF
{
    public static class EntityUtilities
    {
        public static void EditEntityFromClone(object entity, object clone)
        {
            var entityType = entity.GetType();
            var cloneType = clone.GetType();
            foreach (var prop in entityType.GetProperties(BindingFlags.Instance| BindingFlags.Public | BindingFlags.SetProperty))
            {
                var cp = cloneType.GetProperty(prop.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.GetField);
                if(cp == null)
                {
                    Log($"Property '{prop.Name}' is not found in clone object");
                    continue;
                }
                CopyProperty(prop, cp, prop.GetValue(entity), cp.GetValue(clone));
            }
        }

        public static void CopyProperty(PropertyInfo entityProp, PropertyInfo cloneProp, 
            object propValue, object cloneValue)
        {
            if (typeof(IEnumerable).IsAssignableFrom(entityProp.PropertyType))
            {
                if (!typeof(IEnumerable).IsAssignableFrom(cloneProp.PropertyType))
                {
                    Log($"Property '{entityProp.Name}' is a collection but in clone object it is of type '{cloneProp.PropertyType.FullName}'");
                    return;
                }
                var t = entityProp.PropertyType;
                var elementType = t.IsGenericType ? t.GetGenericArguments()[0] : t.GetElementType();
                t = cloneProp.PropertyType;
                var cloneElementType = t.IsGenericType ? t.GetGenericArguments()[0] : t.GetElementType();
                if(cloneElementType != elementType)
                {
                    throw new InvalidOperationException($"Invalid property type '{cloneElementType.FullName}'. expected '{elementType.FullName}'");
                }
                var objArray = ((IEnumerable)propValue).Cast<object>().ToArray();
                var cloneArray = ((IEnumerable)cloneValue).Cast<object>().ToArray();
                if (Type.GetTypeCode(elementType) == TypeCode.Object)
                    CopyAssosiation(elementType, objArray, cloneArray);
                else
                {
                    entityProp.SetValue(propValue, cloneArray);
                }
            }

        }
        static void CopyAssosiation(Type type, object[] entityArray, object[] cloneArray)
        {
            var idProp = EntityReflectionHelper.GetIdProperty(type);
        }
        private static void Log(string s)
        {

        }
    }
}
