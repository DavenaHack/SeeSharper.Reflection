using System;
using System.Collections.Generic;
using System.Linq;

namespace Mimp.SeeSharper.Reflection
{
    public static partial class TypeExtensions
    {


        /// <summary>
        /// Check if type has any generic types unspecified type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool HasGenericParameters(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.IsGenericTypeDefinition || type.ContainsGenericParameters;
        }

        /// <summary>
        /// Return all generic arguments of <paramref name="genericTypeDefinition"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="genericTypeDefinition"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If <paramref name="genericTypeDefinition"/> isn't a generic type definition.</exception>
        public static IEnumerable<IEnumerable<Type>> GetAllGenericArguments(this Type type, Type genericTypeDefinition)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (genericTypeDefinition is null)
                throw new ArgumentNullException(nameof(genericTypeDefinition));

            if (!genericTypeDefinition.IsGenericTypeDefinition)
                throw new ArgumentException($"{genericTypeDefinition} isn't a generic type definition.", nameof(genericTypeDefinition));

            static bool TryGet(Type type, Type genericTypeDefinition, out Type[]? types)
            {
                if (type == genericTypeDefinition)
                {
                    types = Type.EmptyTypes;
                    return true;
                }
                if (genericTypeDefinition.IsGenericTypeDefinition &&
                    type.IsGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition)
                {
                    types = type.GetGenericArguments();
                    return true;
                }
                types = null;
                return false;
            }

            if (TryGet(type, genericTypeDefinition, out var types))
                yield return types!;
            foreach (var i in type.GetInheritTypes())
                if (TryGet(i, genericTypeDefinition, out types))
                    yield return types!;
        }

        /// <summary>
        /// Return generic arguments of <paramref name="genericTypeDefinition"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="genericTypeDefinition"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If <paramref name="genericTypeDefinition"/> isn't a generic type definition.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="type"/> inherit no or more than one <paramref name="genericTypeDefinition"/>.</exception>
        public static IEnumerable<Type> GetGenericArguments(this Type type, Type genericTypeDefinition)
        {
            IEnumerable<Type>? result = null;

            foreach (var t in type.GetAllGenericArguments(genericTypeDefinition))
            {
                if (result is not null)
                    throw new InvalidOperationException($@"""{type}"" has ambiguous generic types for ""{genericTypeDefinition}"": {string.Join(", ", result)} - {string.Join(", ", t)}");
                result = t;
            }

            if (result is null)
                throw new InvalidOperationException($@"""{type}"" isn't a ""{genericTypeDefinition}"".");

            return result;
        }

        /// <summary>
        /// Return all inherit types of <paramref name="genericTypeDefinition"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="genericTypeDefinition"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If <paramref name="genericTypeDefinition"/> isn't a generic type definition.</exception>
        public static IEnumerable<Type> GetGenericTypes(this Type type, Type genericTypeDefinition)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (genericTypeDefinition is null)
                throw new ArgumentNullException(nameof(genericTypeDefinition));

            if (!genericTypeDefinition.IsGenericTypeDefinition)
                throw new ArgumentException($"{genericTypeDefinition} isn't a generic type definition.", nameof(genericTypeDefinition));

            foreach (var types in type.GetAllGenericArguments(genericTypeDefinition))
                yield return types.Any() ? genericTypeDefinition.MakeGenericType(types.ToArray()) : genericTypeDefinition;
        }

        /// <summary>
        /// Return inherit type of <paramref name="genericTypeDefinition"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="genericTypeDefinition"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If <paramref name="genericTypeDefinition"/> isn't a generic type definition.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="type"/> inherit no or more than one <paramref name="genericTypeDefinition"/>.</exception>
        public static Type GetGenericType(this Type type, Type genericTypeDefinition)
        {
            Type? result = null;

            foreach (var t in type.GetGenericTypes(genericTypeDefinition))
            {
                if (result is not null)
                    throw new InvalidOperationException($@"""{type}"" has ambiguous generic types for ""{genericTypeDefinition}"": {result} - {t}");
                result = t;
            }

            if (result is null)
                throw new InvalidOperationException($@"""{type}"" isn't a ""{genericTypeDefinition}"".");

            return result;
        }

        /// <summary>
        /// Resolve all possible combinations with generics of <paramref name="inheritGenericType"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="inheritGenericType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If <paramref name="type"/> or <paramref name="inheritGenericType"/> isn't a generic type definition.</exception>
        /// <exception cref="InvalidOperationException">If a generic argument has ambigous resolving types.</exception>
        public static IEnumerable<Type> ResolveInheritGenericTypes(this Type type, Type inheritGenericType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (inheritGenericType is null)
                throw new ArgumentNullException(nameof(inheritGenericType));
            if (!type.IsGenericType)
                throw new ArgumentException($"{type} isn't a generic type.", nameof(type));
            if (!type.IsGenericType)
                throw new ArgumentException($"{inheritGenericType} isn't a generic type.", nameof(inheritGenericType));

            var genericTypeDefinition = type.GetGenericTypeDefinition();
            var args = type.GetGenericArguments();
            var inheritArgs = inheritGenericType.GetGenericArguments();

            foreach (var genericType in type.GetGenericTypes(inheritGenericType.GetGenericTypeDefinition()))
                if (genericType.HasGenericParameters())
                {
                    var genericArgs = genericType.GetGenericArguments();
                    var genericTypes = new Type[args.Length];

                    for (int i = 0; i < args.Length; i++)
                    {
                        var arg = args[i];

                        for (int j = 0; j < genericArgs.Length; j++)
                            if (genericArgs[j] == arg)
                            {
                                var t = genericTypes[i];
                                var it = inheritArgs[j];
                                if (t is null)
                                    genericTypes[i] = it;
                                else if (t != it)
                                    throw new InvalidOperationException($@"{arg} has ambiguous types for resolving ""{type}"" to ""{inheritGenericType}"": {t} - {it}");
                            }

                        if (genericTypes[i] is null)
                            genericTypes[i] = arg;
                    }

                    yield return genericTypeDefinition.MakeGenericType(genericTypes);
                }
        }

        /// <summary>
        /// Resolve generics of <paramref name="inheritGenericType"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="inheritGenericType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If <paramref name="type"/> or <paramref name="inheritGenericType"/> isn't a generic type definition.</exception>
        /// <exception cref="InvalidOperationException">
        /// If exist no or more than one resolves types.
        /// If a generic argument has ambigous resolving types.
        /// </exception>
        public static Type ResolveInheritGenericType(this Type type, Type inheritGenericType)
        {
            Type? result = null;

            foreach (var t in type.ResolveInheritGenericTypes(inheritGenericType))
            {
                if (result is not null)
                    throw new InvalidOperationException($@"""{type}"" has ambiguous generic types for ""{inheritGenericType}"": {result} - {t}");
                result = t;
            }

            if (result is null)
                throw new InvalidOperationException($@"""{type}"" isn't a ""{inheritGenericType}"".");

            return result;
        }


    }
}