using System;
using System.Linq;
using System.Linq.Expressions;

namespace Mimp.SeeSharper.Reflection
{
    public static partial class TypeExtensions
    {


        /// <summary>
        /// Return a compiled delegate to cast a object of <paramref name="source"/> to <paramref name="destination"/>.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="delegateType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If delegate has one parameter for the instance.</exception>
        /// <exception cref="InvalidOperationException">If source isn't castable to destination.</exception>
        public static Delegate GetCastDelegate(this Type source, Type destination, Type delegateType)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));
            if (destination is null)
                throw new ArgumentNullException(nameof(destination));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            var parameterTypes = delegateType.GetDelegateParameterTypes().ToArray();
            if (parameterTypes.Length != 1)
                throw new ArgumentException($@"Delegate ""{delegateType}"" has an incorrect number of parameters", nameof(delegateType));

            var instance = Expression.Parameter(parameterTypes[0], "instance");

            return Expression.Lambda(delegateType,
                Expression.Convert(
                    Expression.ConvertChecked(
                        Expression.Convert(instance, source),
                        destination
                    ),
                    delegateType.GetDelegateReturnType()
                ),
                instance
            ).Compile();
        }

        /// <summary>
        /// Return a compiled delegate to cast a object of <paramref name="source"/> to <paramref name="destination"/>.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If delegate has one parameter for the instance.</exception>
        /// <exception cref="InvalidOperationException">If source isn't castable to destination.</exception>
        public static Delegate GetCastDelegate(this Type source, Type destination)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));
            if (destination is null)
                throw new ArgumentNullException(nameof(destination));

            return source.GetCastDelegate(destination, Expression.GetFuncType(source, destination));
        }

        /// <summary>
        /// Return a compiled delegate to cast a object of <paramref name="source"/> to <paramref name="destination"/>.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If delegate has one parameter for the instance.</exception>
        /// <exception cref="InvalidOperationException">If source isn't castable to destination.</exception>
        public static TDelegate GetCastDelegate<TDelegate>(this Type source, Type destination) where TDelegate : Delegate
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));
            if (destination is null)
                throw new ArgumentNullException(nameof(destination));

            return (TDelegate)source.GetCastDelegate(destination, typeof(TDelegate));
        }

        /// <summary>
        /// Return a compiled delegate to cast a object of <paramref name="source"/> to <paramref name="destination"/>.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If delegate has one parameter for the instance.</exception>
        /// <exception cref="InvalidOperationException">If source isn't castable to destination.</exception>
        public static TDelegate GetCastDelegate<TDelegate>(this Type source) where TDelegate : Delegate
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            return (TDelegate)source.GetCastDelegate(typeof(TDelegate).GetDelegateReturnType(), typeof(TDelegate));
        }

        /// <summary>
        /// Return a compiled function to cast a object of <paramref name="source"/> to <paramref name="destination"/>.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If source isn't castable to destination.</exception>
        public static Func<object?, object?> GetCastFunc(this Type source, Type destination)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));
            if (destination is null)
                throw new ArgumentNullException(nameof(destination));

            return source.GetCastDelegate<Func<object?, object?>>(destination);
        }

        /// <summary>
        /// Return a compiled function to cast a object of <paramref name="source"/> to <paramref name="destination"/>.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If source isn't castable to destination.</exception>
        public static Func<TSource, TDestination> GetCastFunc<TSource, TDestination>(this Type source)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            return source.GetCastDelegate<Func<TSource, TDestination>>();
        }

        /// <summary>
        /// Return a compiled function to cast a object of <paramref name="source"/> to <paramref name="destination"/>.
        /// </summary>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If source isn't castable to destination.</exception>
        public static Func<object?, TDestination> GetCastFunc<TDestination>(this Type source) =>
            source.GetCastFunc<object?, TDestination>();


    }
}
