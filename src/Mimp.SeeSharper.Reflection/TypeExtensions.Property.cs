using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mimp.SeeSharper.Reflection
{
    public static partial class TypeExtensions
    {


        /// <summary>
        /// Return all matching properties.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="hasPublicGet"></param>
        /// <param name="hasPublicSet"></param>
        /// <param name="isStatic"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<PropertyInfo> GetProperties(this Type type, string name, bool hasPublicGet, bool hasPublicSet, bool isStatic, bool ignoreCase)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            var properties = new List<PropertyInfo>();
            foreach (var p in type.GetRuntimeProperties())
            {
                if (hasPublicGet && !p.HasPublicGet())
                    continue;

                if (hasPublicSet && !p.HasPublicSet())
                    continue;

                if (isStatic != p.IsStatic())
                    continue;

                if (p.GetIndexParameters().Length > 0)
                    continue;

                if (!string.Equals(p.Name, name, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture))
                {
                    var i = p.Name.LastIndexOf('.');
                    if (i < 0)
                        continue;
                    var n = p.Name.Substring(i + 1);
                    if (!string.Equals(n, name, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture))
                        continue;
                }

                properties.Add(p);
            }
            return properties;
        }

        /// <summary>
        /// Return the matching property.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="hasPublicGet"></param>
        /// <param name="hasPublicSet"></param>
        /// <param name="isStatic"></param>
        /// <param name="ignoreCase"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If no or more than one property exsists.</exception>
        /// <returns></returns>
        public static PropertyInfo GetProperty(this Type type, string name, bool hasPublicGet, bool hasPublicSet, bool isStatic, bool ignoreCase)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            var properties = type.GetProperties(name, hasPublicGet, hasPublicSet, isStatic, ignoreCase);
            if (properties is null || !properties.Any())
                throw new InvalidOperationException($@"Type ""{type}"" has no property ""{name}""");
            if (properties.Skip(1).Any())
                throw new InvalidOperationException($@"Property ""{name}"" is ambiguous for ""{type}""");
            return properties.First();
        }

        /// <summary>
        /// Return the instance property.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="hasPublicGet"></param>
        /// <param name="hasPublicSet"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If no or more than one property exsists.</exception>
        public static PropertyInfo GetInstanceProperty(this Type type, string name, bool hasPublicGet, bool hasPublicSet, bool ignoreCase)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetProperty(name, hasPublicGet, hasPublicSet, false, ignoreCase);
        }

        /// <summary>
        /// Return the public instance property.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If no or more than one property exsists.</exception>
        public static PropertyInfo GetInstanceProperty(this Type type, string name, bool ignoreCase)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetProperty(name, true, false, false, ignoreCase);
        }

        /// <summary>
        /// Return the public instance property.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If no or more than one property exsists.</exception>
        public static PropertyInfo GetInstanceProperty(this Type type, string name)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetProperty(name, true, false, false, true);
        }

        /// <summary>
        /// Return the static property.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="hasPublicGet"></param>
        /// <param name="hasPublicSet"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If no or more than one property exsists.</exception>
        public static PropertyInfo GetStaticProperty(this Type type, string name, bool hasPublicGet, bool hasPublicSet, bool ignoreCase)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetProperty(name, hasPublicGet, hasPublicSet, true, ignoreCase);
        }

        /// <summary>
        /// Return the public static property.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If no or more than one property exsists.</exception>
        public static PropertyInfo GetStaticProperty(this Type type, string name, bool ignoreCase)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetProperty(name, true, false, true, ignoreCase);
        }

        /// <summary>
        /// Return the public static property.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If no or more than one property exsists.</exception>
        public static PropertyInfo GetStaticProperty(this Type type, string name)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetProperty(name, true, false, true, true);
        }


    }
}
