using System;
using System.Collections.Generic;
using System.Linq;
#if NullableAttributes
using System.Diagnostics.CodeAnalysis;
#endif

namespace Mimp.SeeSharper.Reflection
{
    public static partial class TypeExtensions
    {


#if !NET
        /// <summary>
        /// Determines whether the current type can be assigned to a variable of the specified
        /// targetType.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="targetType">The type to compare with the current type.</param>
        /// <returns>
        /// true if any of the following conditions is true: - The current instance and targetType
        /// represent the same type. - The current type is derived either directly or indirectly
        /// from targetType. The current type is derived directly from targetType if it inherits
        /// from targetType; the current type is derived indirectly from targetType if it
        /// inherits from a succession of one or more classes that inherit from targetType.
        /// - targetType is an interface that the current type implements. - The current
        /// type is a generic type parameter, and targetType represents one of the constraints
        /// of the current type. - The current type represents a value type, and targetType
        /// represents Nullable<c> (Nullable(Of c) in Visual Basic). false if none of these
        /// conditions are true, or if targetType or this is null.
        /// </returns>
        public static bool IsAssignableTo(
            this Type? type,
#if NullableAttributes
            [NotNullWhen(true)]
#endif
            Type? targetType)
        {
            return targetType?.IsAssignableFrom(type) ?? false;
        }
#endif


        /// <summary>
        /// Return all inherit types.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
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


        /// <summary>
        /// Return all base types.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<Type> GetBaseTypes(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            while (type.BaseType is not null)
                yield return type = type.BaseType;
        }


        /// <summary>
        /// Check if match all constraints for <paramref name="parameterType"/>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parameterType"></param>
        /// <param name="genericType"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="parameterType"/> isn't a generic parameter.
        /// If <paramref name="genericType"/> isn't the generic type of <paramref name="parameterType"/>.
        /// </exception>
        /// <returns></returns>
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


        /// <summary>
        /// Check if inherit <paramref name="inheritType"/>. 
        /// If <paramref name="type"/> has unspecified generic parameters it will check if all generic parameters are inherit to <paramref name="inheritType"/> generic arguments.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="inheritType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
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

        /// <summary>
        /// Check if inherit <typeparamref name="T"/>. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool Inherit<T>(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.Inherit(typeof(T));
        }

        /// <summary>
        /// Check if inherit or assignable to <paramref name="inheritType"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="inheritType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool InheritOrAssignable(this Type type, Type inheritType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (inheritType is null)
                throw new ArgumentNullException(nameof(inheritType));

            return type.IsAssignableTo(inheritType) || type.Inherit(inheritType);
        }

        /// <summary>
        /// Check if inherit or assignable to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
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
        /// <exception cref="ArgumentNullException"></exception>
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

        private static bool InheritGenericDefinition(this Type type, Type genericTypeDefinition, IEnumerable<Func<int, Type, bool>> genericTypeArgumentConditions)
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
