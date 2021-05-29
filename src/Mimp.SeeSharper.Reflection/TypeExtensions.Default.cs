using System;
using System.Linq.Expressions;

namespace Mimp.SeeSharper.Reflection
{
    public static partial class TypeExtensions
    {


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

        public static Delegate GetDefaultDelegate(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.GetDefaultDelegate(Expression.GetFuncType(type));
        }

        public static T GetDefaultDelegate<T>(this Type type) where T : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return (T)type.GetDefaultDelegate(typeof(T));
        }

        public static Func<object> GetDefaultFunc(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.GetDefaultDelegate<Func<object>>();
        }

        public static object Default(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.GetDefaultFunc()();
        }


    }
}
