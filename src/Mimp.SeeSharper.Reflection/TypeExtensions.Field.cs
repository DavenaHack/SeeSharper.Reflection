using System;
using System.Collections.Generic;
using System.Reflection;

namespace Mimp.SeeSharper.Reflection
{
    public static partial class TypeExtensions
    {


        public static FieldInfo[] GetFields(this Type type, string name, bool hasPublic, bool isStatic, bool ignoreCase)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            var fields = new List<FieldInfo>();
            foreach (var f in type.GetRuntimeFields())
            {
                if (hasPublic && !f.IsPublic)
                    continue;

                if (isStatic != f.IsStatic)
                    continue;

                if (!string.Equals(f.Name, name, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture))
                {
                    var i = f.Name.LastIndexOf('.');
                    if (i < 0)
                        continue;
                    var n = f.Name.Substring(i + 1);
                    if (!string.Equals(n, name, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture))
                        continue;
                }

                fields.Add(f);
            }
            return fields.ToArray();
        }

        public static FieldInfo GetField(this Type type, string name, bool hasPublic, bool isStatic, bool ignoreCase)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            var fields = type.GetFields(name, hasPublic, isStatic, ignoreCase);
            if (fields is null || fields.Length == 0)
                throw new InvalidOperationException($@"Type ""{type}"" has no property ""{name}""");
            if (fields.Length != 1)
                throw new InvalidOperationException($@"Field ""{name}"" is ambiguous for ""{type}""");
            return fields[0];
        }


        public static FieldInfo GetInstanceField(this Type type, string name, bool ignoreCase)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetField(name, true, false, ignoreCase);
        }

        public static FieldInfo GetInstanceField(this Type type, string name)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetField(name, true, false, true);
        }

        public static FieldInfo GetStaticField(this Type type, string name, bool ignoreCase)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetField(name, true, true, ignoreCase);
        }

        public static FieldInfo GetStaticField(this Type type, string name)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetField(name, true, true, true);
        }


    }
}
