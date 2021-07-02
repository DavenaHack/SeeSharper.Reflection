using System;
using System.Collections.Generic;

namespace Mimp.SeeSharper.Reflection
{
    public static partial class TypeExtensions
    {


        /// <summary>
        /// Return all attributes of <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="type"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this Type type, bool inherit)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            foreach (var a in type.GetCustomAttributes(typeof(TAttribute), inherit))
                yield return (TAttribute)a;
        }

        /// <summary>
        /// Check if <paramref name="type"/> has at least one attribute of <paramref name="attributeType"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="attributeType"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool HasCustomAttribute(this Type type, Type attributeType, bool inherit)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (attributeType is null)
                throw new ArgumentNullException(nameof(attributeType));

            return type.GetCustomAttributes(attributeType, inherit).Length > 0;
        }

        /// <summary>
        /// Check if <paramref name="type"/> has at least one attribute of <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool HasCustomAttribute<TAttribute>(this Type type, bool inherit)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.HasCustomAttribute(typeof(TAttribute), inherit);
        }


    }
}
