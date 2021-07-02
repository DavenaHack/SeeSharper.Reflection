using System;
using System.Linq.Expressions;

namespace Mimp.SeeSharper.Reflection
{
    public static partial class TypeExtensions
    {


        /// <summary>
        /// Return a compiled delegate to get the default of <paramref name="type"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="delegateType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If type isn't castable to return type of delegate.</exception>
        public static Delegate GetDefaultDelegate(this Type type, Type delegateType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            return Expression.Lambda(delegateType, Expression.Convert(
                Expression.Default(type), delegateType.GetDelegateReturnType()
            )).Compile();
        }

        /// <summary>
        /// Return a compiled delegate to get the default of <paramref name="type"/>.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If type isn't castable to return type of delegate.</exception>
        public static TDelegate GetDefaultDelegate<TDelegate>(this Type type) where TDelegate : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return (TDelegate)type.GetDefaultDelegate(typeof(TDelegate));
        }

        /// <summary>
        /// Return a compiled delegate to get the default of <paramref name="type"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Delegate GetDefaultDelegate(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.GetDefaultDelegate(Expression.GetFuncType(type));
        }

        /// <summary>
        /// Return a compiled delegate to get the default of <paramref name="type"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If type isn't castable to <typeparamref name="T"/>.</exception>
        public static Func<T> GetDefaultFunc<T>(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.GetDefaultDelegate<Func<T>>();
        }

        /// <summary>
        /// Return a compiled delegate to get the default of <paramref name="type"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Func<object?> GetDefaultFunc(this Type type) =>
            type.GetDefaultFunc<object?>();


        /// <summary>
        /// Return the default of <see cref="type"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object? Default(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.GetDefaultFunc()();
        }


    }
}
