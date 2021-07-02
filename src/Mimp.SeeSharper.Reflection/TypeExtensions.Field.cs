using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mimp.SeeSharper.Reflection
{
    public static partial class TypeExtensions
    {


        /// <summary>
        /// Return all matching fields.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="isPublic"></param>
        /// <param name="isStatic"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<FieldInfo> GetFields(this Type type, string name, bool isPublic, bool isStatic, bool ignoreCase)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            var fields = new List<FieldInfo>();
            foreach (var f in type.GetRuntimeFields())
            {
                if (isPublic && !f.IsPublic)
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
            return fields;
        }

        /// <summary>
        /// Return the matching field.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="isPublic"></param>
        /// <param name="isStatic"></param>
        /// <param name="ignoreCase"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If no or more than one field exsists.</exception>
        /// <returns></returns>
        public static FieldInfo GetField(this Type type, string name, bool isPublic, bool isStatic, bool ignoreCase)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            var fields = type.GetFields(name, isPublic, isStatic, ignoreCase);
            if (fields is null || !fields.Any())
                throw new InvalidOperationException($@"Type ""{type}"" has no field ""{name}""");
            if (fields.Skip(1).Any())
                throw new InvalidOperationException($@"Field ""{name}"" is ambiguous for ""{type}""");
            return fields.First();
        }

        /// <summary>
        /// Return the public instance field.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If no or more than one field exsists.</exception>
        public static FieldInfo GetInstanceField(this Type type, string name, bool ignoreCase)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetField(name, true, false, ignoreCase);
        }

        /// <summary>
        /// Return the public instance field.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If no or more than one field exsists.</exception>
        public static FieldInfo GetInstanceField(this Type type, string name)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetField(name, true, false, true);
        }

        /// <summary>
        /// Return the public static field.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If no or more than one field exsists.</exception>
        public static FieldInfo GetStaticField(this Type type, string name, bool ignoreCase)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetField(name, true, true, ignoreCase);
        }

        /// <summary>
        /// Return the public static field.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If no or more than one field exsists.</exception>
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
