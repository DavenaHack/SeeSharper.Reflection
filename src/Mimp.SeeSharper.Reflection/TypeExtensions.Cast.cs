using System;
using System.Linq;
using System.Linq.Expressions;

namespace Mimp.SeeSharper.Reflection
{
    public static partial class TypeExtensions
    {


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

        public static Delegate GetCastDelegate(this Type source, Type destination)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));
            if (destination is null)
                throw new ArgumentNullException(nameof(destination));

            return source.GetCastDelegate(destination, Expression.GetFuncType(source, destination));
        }

        public static T GetCastDelegate<T>(this Type source, Type destination) where T : Delegate
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));
            if (destination is null)
                throw new ArgumentNullException(nameof(destination));

            return (T)source.GetCastDelegate(destination, typeof(T));
        }

        public static T GetCastDelegate<T>(this Type source) where T : Delegate
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            return (T)source.GetCastDelegate(typeof(T).GetDelegateReturnType(), typeof(T));
        }

        public static Func<object, object?> GetCastFunc(this Type source, Type destination)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));
            if (destination is null)
                throw new ArgumentNullException(nameof(destination));

            return source.GetCastDelegate<Func<object, object?>>(destination);
        }

        public static Func<object, T> GetCastFunc<T>(this Type source)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            return source.GetCastDelegate<Func<object, T>>();
        }


    }
}
