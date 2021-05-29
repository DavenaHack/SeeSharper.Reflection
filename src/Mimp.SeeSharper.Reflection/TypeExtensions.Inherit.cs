using System;
using System.Collections.Generic;
using System.Linq;

namespace Mimp.SeeSharper.Reflection
{
    public static partial class TypeExtensions
    {


#if !NET
        public static bool IsAssignableTo(this Type type, Type targetType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (targetType is null)
                throw new ArgumentNullException(nameof(targetType));

            return targetType.IsAssignableFrom(type);
        }
#endif

        public static IEnumerable<Type> GetInheritTypes(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            var types = new HashSet<Type>();
            foreach (var b in type.GetBaseTypes())
            {
                if (types.Contains(b))
                    continue;
                yield return b;
                types.Add(b);
            }
            foreach (var i in type.GetInterfaces())
            {
                if (types.Contains(i))
                    continue;
                yield return i;
                types.Add(i);
            }
        }


        public static IEnumerable<Type> GetBaseTypes(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            while (type.BaseType is not null)
                yield return type = type.BaseType;
        }


        public static bool InheritGenericParameter(this Type type, Type parameterType, Type genericType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (parameterType is null)
                throw new ArgumentNullException(nameof(parameterType));
            if (!parameterType.IsGenericParameter)
                throw new ArgumentException($"{parameterType} isn't a generic parameter.", nameof(parameterType));
            if (genericType is null)
                throw new ArgumentNullException(nameof(genericType));
            if (!genericType.IsGenericType)
                throw new ArgumentException($"{genericType} isn't a generic type.", nameof(genericType));
            var definition = genericType.GetGenericTypeDefinition();
            if (parameterType.DeclaringType != definition)
                throw new ArgumentException($"{parameterType} isn't a generic parameter of {genericType}.", nameof(genericType));

            return InheritGenericParameter(type, parameterType, genericType.GetGenericArguments());
        }

        private static bool InheritGenericParameter(Type type, Type parameterType, Type[] parameters)
        {
            if (!parameterType.IsGenericParameter)
                return type.Inherit(parameterType);

            foreach (var constraint in parameterType.GetGenericParameterConstraints())
                if (constraint.IsGenericParameter)
                    return InheritGenericParameter(type, parameters[constraint.GenericParameterPosition], parameters);
                else if (!type.Inherit(constraint))
                    return false;

            return true;
        }


        public static bool Inherit(this Type type, Type inheritType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (inheritType is null)
                throw new ArgumentNullException(nameof(inheritType));

            var genericTypeDefinition = inheritType.IsGenericType ? inheritType.GetGenericTypeDefinition() : null;
            return Inherit(type, t =>
            {
                if (t == inheritType)
                    return true;

                if (t.IsGenericType)
                {
                    if (t.GetGenericTypeDefinition() == genericTypeDefinition)
                    {
                        if (inheritType.IsGenericTypeDefinition)
                            return true;
                        var gts = t.GetGenericArguments();
                        var igts = inheritType.GetGenericArguments();

                        for (var i = 0; i < gts.Length; i++)
                        {
                            var gt = gts[i];
                            var igt = igts[i];

                            if (gt.IsGenericParameter)
                            {
                                if (!igt.IsGenericParameter)
                                    return false;

                                if (!gt.Inherit(igt))
                                    return false;
                            }
                            else
                            {
                                if (igt.IsGenericParameter)
                                {
                                    if (!gt.InheritGenericParameter(igt, t))
                                        return false;
                                }
                                else if (gt != igt)
                                    return false;
                            }
                        }
                        return true;
                    }
                }

                return false;
            });
        }

        public static bool Inherit<T>(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.Inherit(typeof(T));
        }


        public static bool InheritOrAssignable(this Type type, Type inheritType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (inheritType is null)
                throw new ArgumentNullException(nameof(inheritType));

            return type.IsAssignableTo(inheritType) || type.Inherit(inheritType);
        }

        public static bool InheritOrAssignable<T>(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.InheritOrAssignable(typeof(T));
        }


        /// <summary>
        /// Return true if <paramref name="type"/> inherit from the <paramref name="genericTypeDefinition"/> with the <paramref name="genericTypeArguments"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="genericTypeDefinition"></param>
        /// <param name="genericTypeArguments">To ignore the type check on a position use null</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If <paramref name="genericTypeArguments"/> size isn't equal to generic arguments of <paramref name="genericTypeDefinition"/></exception>
        public static bool InheritGenericDefinition(this Type type, Type genericTypeDefinition, IEnumerable<Type?> genericTypeArguments)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (genericTypeDefinition is null)
                throw new ArgumentNullException(nameof(genericTypeDefinition));
            if (genericTypeArguments is null)
                throw new ArgumentNullException(nameof(genericTypeArguments));

            return type.InheritGenericDefinition(genericTypeDefinition, genericTypeArguments.Select(t => (Func<int, Type, bool>)((i, r) => t.IsNullOrVoid() || t == r)));
        }

        public static bool InheritGenericDefinition(this Type type, Type genericTypeDefinition, IEnumerable<Func<int, Type, bool>> genericTypeArgumentConditions)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (genericTypeDefinition is null)
                throw new ArgumentNullException(nameof(genericTypeDefinition));
            if (genericTypeArgumentConditions is null)
                throw new ArgumentNullException(nameof(genericTypeArgumentConditions));
            if (genericTypeArgumentConditions.Any(c => c is null))
                throw new ArgumentNullException(nameof(genericTypeArgumentConditions), "At least one condition is null.");

            if (!genericTypeDefinition.IsGenericTypeDefinition)
                throw new ArgumentException($"{nameof(genericTypeDefinition)} isn't a generic type definition", nameof(genericTypeDefinition));
            var typeArgs = genericTypeArgumentConditions?.ToArray();
            if (typeArgs is null || genericTypeDefinition.GetGenericArguments().Length != typeArgs.Length)
                throw new ArgumentException($"The size of {nameof(genericTypeArgumentConditions)} isn't equal to generic arguments of {nameof(genericTypeDefinition)}", nameof(genericTypeArgumentConditions));

            return Inherit(type, t =>
            {
                if (!t.IsGenericType)
                    return false;
                var g = t.GetGenericTypeDefinition();
                if (g != genericTypeDefinition)
                    return false;
                if (typeArgs is null)
                    return true;
                for (var i = 0; i < typeArgs.Length; i++)
                    if (!typeArgs[i](i, t.GenericTypeArguments[i]))
                        return false;
                return true;
            });
        }


        private static bool Inherit(Type type, Predicate<Type> typeMatch)
        {
            if (typeMatch(type))
                return true;
            foreach (var b in type.GetBaseTypes())
                if (typeMatch(b))
                    return true;
            foreach (var i in type.GetInterfaces())
                if (typeMatch(i))
                    return true;
            return false;
        }


    }
}
