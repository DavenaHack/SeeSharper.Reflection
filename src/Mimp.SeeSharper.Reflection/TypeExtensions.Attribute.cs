using System;
using System.Collections.Generic;

namespace Mimp.SeeSharper.Reflection
{
    public static partial class TypeExtensions
    {


        public static IEnumerable<T> GetCustomAttributes<T>(this Type type, bool inherit = true)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            foreach (var a in type.GetCustomAttributes(typeof(T), inherit))
                yield return (T)a;
        }

        public static bool HasCustomAttribute(this Type type, Type attributeType, bool inherit = true)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (attributeType is null)
                throw new ArgumentNullException(nameof(attributeType));

            return type.GetCustomAttributes(attributeType, inherit).Length > 0;
        }

        public static bool HasCustomAttribute<T>(this Type type, bool inherit = true)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.HasCustomAttribute(typeof(T), inherit);
        }


    }
}
