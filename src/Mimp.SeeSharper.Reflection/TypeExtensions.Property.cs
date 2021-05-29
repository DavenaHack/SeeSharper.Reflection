using System;
using System.Collections.Generic;
using System.Reflection;

namespace Mimp.SeeSharper.Reflection
{
    public static partial class TypeExtensions
    {


        public static PropertyInfo[] GetProperties(this Type type, string name, bool hasPublicGet, bool hasPublicSet, bool isStatic, bool ignoreCase)
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

                    if (isStatic != p.IsStatic())
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
            return properties.ToArray();
        }


        public static PropertyInfo GetProperty(this Type type, string name, bool hasPublicGet, bool hasPublicSet, bool isStatic, bool ignoreCase)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            var properties = type.GetProperties(name, hasPublicGet, hasPublicSet, isStatic, ignoreCase);
            if (properties is null || properties.Length == 0)
                throw new InvalidOperationException($@"Type ""{type}"" has no property ""{name}""");
            if (properties.Length != 1)
                throw new InvalidOperationException($@"Property ""{name}"" is ambiguous for ""{type}""");
            return properties[0];
        }


        public static PropertyInfo GetInstanceProperty(this Type type, string name, bool hasPublicGet, bool hasPublicSet, bool ignoreCase)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetProperty(name, hasPublicGet, hasPublicSet, false, ignoreCase);
        }

        public static PropertyInfo GetInstanceProperty(this Type type, string name, bool ignoreCase)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetProperty(name, true, false, false, ignoreCase);
        }

        public static PropertyInfo GetInstanceProperty(this Type type, string name)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetProperty(name, true, false, false, true);
        }

        public static PropertyInfo GetStaticProperty(this Type type, string name, bool hasPublicGet, bool hasPublicSet, bool ignoreCase)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetProperty(name, hasPublicGet, hasPublicSet, true, ignoreCase);
        }

        public static PropertyInfo GetStaticProperty(this Type type, string name, bool ignoreCase)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetProperty(name, true, false, true, ignoreCase);
        }

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
