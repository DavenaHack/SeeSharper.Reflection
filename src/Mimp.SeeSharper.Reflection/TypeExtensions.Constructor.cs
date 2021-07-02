using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Mimp.SeeSharper.Reflection
{
    public static partial class TypeExtensions
    {


        /// <summary>
        /// Return a matching constructor or throw a <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="bindingAttr"></param>
        /// <param name="binder"></param>
        /// <param name="callConvention"></param>
        /// <param name="types"></param>
        /// <param name="modifiers"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If no matching constructor exists.</exception>
        public static ConstructorInfo GetConstructorRequired(this Type type, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, IEnumerable<Type> types, IEnumerable<ParameterModifier> modifiers)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (binder is null)
                throw new ArgumentNullException(nameof(binder));
            if (types is null)
                throw new ArgumentNullException(nameof(types));
            if (types.Any(t => t is null))
                throw new ArgumentNullException(nameof(types), "At least one type is null.");
            if (modifiers is null)
                throw new ArgumentNullException(nameof(modifiers));

            return type.GetConstructor(bindingAttr, binder, callConvention, types.ToArray(), modifiers.ToArray()) ?? throw new InvalidOperationException("No matching constructor found");
        }

        /// <summary>
        /// Return a matching constructor or throw a <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="bindingAttr"></param>
        /// <param name="binder"></param>
        /// <param name="types"></param>
        /// <param name="modifiers"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If no matching constructor exists.</exception>
        public static ConstructorInfo GetConstructorRequired(this Type type, BindingFlags bindingAttr, Binder binder, IEnumerable<Type> types, IEnumerable<ParameterModifier> modifiers)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (binder is null)
                throw new ArgumentNullException(nameof(binder));
            if (types is null)
                throw new ArgumentNullException(nameof(types));
            if (types.Any(t => t is null))
                throw new ArgumentNullException(nameof(types), "At least one type is null.");
            if (modifiers is null)
                throw new ArgumentNullException(nameof(modifiers));

            return type.GetConstructor(bindingAttr, binder, types.ToArray(), modifiers.ToArray()) ?? throw new InvalidOperationException("No matching constructor found");
        }

        /// <summary>
        /// Return a matching constructor or throw a <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If no matching constructor exists.</exception>
        public static ConstructorInfo GetConstructorRequired(this Type type, IEnumerable<Type> types)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (types is null)
                throw new ArgumentNullException(nameof(types));
            if (types.Any(t => t is null))
                throw new ArgumentNullException(nameof(types), "At least one type is null.");

            return type.GetConstructor(types.ToArray()) ?? throw new InvalidOperationException("No matching constructor found");
        }

        /// <summary>
        /// Return a matching constructor or throw a <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If no matching constructor exists.</exception>
        public static ConstructorInfo GetConstructorRequired(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.GetConstructorRequired(Type.EmptyTypes);
        }



        /// <summary>
        /// Return a compiled delegate to invoke the constructor with same parameters like <paramref name="delegateType"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="delegateType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If no matching constructor exists.</exception>
        public static Delegate GetNewDelegate(this Type type, Type delegateType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            return type.GetConstructorRequired(delegateType.GetDelegateParameterTypes()).GetDelegate(delegateType);
        }

        /// <summary>
        /// Return a compiled delegate to invoke the constructor with same parameters like <typeparamref name="TDelegate"/>.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If no matching constructor exists.</exception>
        public static TDelegate GetNewDelegate<TDelegate>(this Type type) where TDelegate : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return (TDelegate)type.GetNewDelegate(typeof(TDelegate));
        }

        /// <summary>
        /// Return a compiled function to invoke the default constructor.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If no matching constructor exists.</exception>
        public static Func<object> GetNewFunc(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.GetNewDelegate<Func<object>>();
        }

        /// <summary>
        /// Return a compiled delegate to invoke a constructor - that match <paramref name="types"/> - with a array of parameters.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="types"></param>
        /// <param name="delegateType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">
        /// If no matching constructor exists.
        /// If delegate types isn't castable to constructor types.
        /// </exception>
        public static Delegate GetNewParameterDelegate(this Type type, IEnumerable<Type> types, Type delegateType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (types is null)
                throw new ArgumentNullException(nameof(types));
            if (types.Any(t => t is null))
                throw new ArgumentNullException(nameof(types), "At least on type is null");
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            return type.GetConstructorRequired(types).GetParameterDelegate(delegateType);
        }

        /// <summary>
        /// Return a compiled delegate to invoke a constructor - that match <paramref name="types"/> - with a array of parameters.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="type"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">
        /// If no matching constructor exists.
        /// If delegate types isn't castable to constructor types.
        /// </exception>
        public static TDelegate GetNewParameterDelegate<TDelegate>(this Type type, IEnumerable<Type> types) where TDelegate : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (types is null)
                throw new ArgumentNullException(nameof(types));
            if (types.Any(t => t is null))
                throw new ArgumentNullException(nameof(types), "At least on type is null");

            return (TDelegate)type.GetNewParameterDelegate(types, typeof(TDelegate));
        }

        /// <summary>
        /// Return a compiled delegate to invoke a constructor - that match <paramref name="types"/> - with a array of parameters.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If no matching constructor exists.</exception>
        public static Delegate GetNewParameterDelegate(this Type type, IEnumerable<Type> types)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (types is null)
                throw new ArgumentNullException(nameof(types));
            if (types.Any(t => t is null))
                throw new ArgumentNullException(nameof(types), "At least on type is null");

            return type.GetNewParameterDelegate(types, Expression.GetFuncType(types.Concat(new[] { type }).ToArray()));
        }

        /// <summary>
        /// Return a compiled delegate to invoke a constructor - that match <paramref name="types"/> - with a array of parameters.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If no matching constructor exists.</exception>
        public static Func<object?[], object> GetNewParameterFunc(this Type type, IEnumerable<Type> types)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (types is null)
                throw new ArgumentNullException(nameof(types));
            if (types.Any(t => t is null))
                throw new ArgumentNullException(nameof(types), "At least on type is null");

            return type.GetNewParameterDelegate<Func<object?[], object>>(types);
        }

        /// <summary>
        /// Return a compiled delegate to invoke a constructor - that match <paramref name="types"/> - with a array of parameters.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If no matching constructor exists.</exception>
        public static ParamsFunc GetNewParamsFunc(this Type type, IEnumerable<Type> types)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (types is null)
                throw new ArgumentNullException(nameof(types));
            if (types.Any(t => t is null))
                throw new ArgumentNullException(nameof(types), "At least on type is null");

            return type.GetNewParameterDelegate<ParamsFunc>(types);
        }

        /// <summary>
        /// Call the default constructor.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If no matching constructor exists.</exception>
        public static object New(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.GetNewFunc()();
        }

        /// <summary>
        /// Call the matching constructor with <paramref name="values"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="types"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If no matching constructor exists.</exception>
        public static object New(this Type type, IEnumerable<Type> types, object?[] values)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (types is null)
                throw new ArgumentNullException(nameof(types));
            if (types.Any(t => t is null))
                throw new ArgumentNullException(nameof(types), "At least on type is null");
            if (values is null)
                throw new ArgumentNullException(nameof(values));

            return type.GetNewParameterFunc(types)(values ?? Array.Empty<object?>());
        }


    }
}
