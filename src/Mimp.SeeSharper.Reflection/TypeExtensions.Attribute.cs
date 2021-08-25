using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Mimp.SeeSharper.Reflection
{
    public static partial class TypeExtensions
    {


        /// <summary>
        /// Check if <paramref name="type"/> is a <see cref="Attribute"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsAttribute(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.IsAssignableTo(typeof(Attribute));
        }


        /// <summary>
        /// Return all attributes of <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="type"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this Type type, bool inherit)
            where TAttribute : Attribute
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

            return type.IsDefined(attributeType, inherit);
        }

        /// <summary>
        /// Check if <paramref name="type"/> has at least one attribute of <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool HasCustomAttribute<TAttribute>(this Type type, bool inherit)
            where TAttribute : Attribute
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.HasCustomAttribute(typeof(TAttribute), inherit);
        }


        /// <summary>
        /// Return the <typeparamref name="TAttribute"/> or throw a <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="inherit"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If no or more than one attribute exists</exception>
        public static TAttribute GetSingleCustomAttribute<TAttribute>(this Type type, bool inherit)
            where TAttribute : Attribute
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            var attrs = type.GetCustomAttributes<TAttribute>(inherit).GetEnumerator();
            if (!attrs.MoveNext())
                throw new InvalidOperationException($"No {typeof(TAttribute)} found.");

            var result = attrs.Current;
            if (attrs.MoveNext())
                throw new InvalidOperationException($"More than one {typeof(TAttribute)} found.");

            return result;
        }


        /// <summary>
        /// Try get only one <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="inherit"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool TryGetSingleCustomAttribute<TAttribute>(this Type type, bool inherit,
#if NullableAttributes
                [NotNullWhen(true)]
#endif
                out TAttribute? attribute)
            where TAttribute : Attribute
        {
            attribute = null;
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            try
            {
                attribute = type.GetSingleCustomAttribute<TAttribute>(inherit);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
