using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Mimp.SeeSharper.Reflection
{
    public static class PropertyInfoExtensions
    {


        public static bool IsStatic(this PropertyInfo property)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));

            return property.GetMethod is not null && property.GetMethod.IsStatic
                || property.SetMethod is not null && property.SetMethod.IsStatic;
        }

        public static bool HasPublicGet(this PropertyInfo property)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));

            return property.GetMethod is not null && property.GetMethod.IsPublic;
        }

        public static bool HasPublicSet(this PropertyInfo property)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));

            return property.SetMethod is not null && property.SetMethod.IsPublic;
        }


        #region GetInstanceAccessDelegate


        public static Delegate GetInstanceAccessDelegate(this PropertyInfo property, Type delegateType)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            var parameterTypes = delegateType.GetDelegateParameterTypes().ToArray();
            if (parameterTypes.Length != 1)
                throw new ArgumentException($@"Delegate ""{delegateType}"" has an incorrect number of parameters", nameof(delegateType));

            var instance = Expression.Parameter(parameterTypes[0], "instance");

            return Expression.Lambda(delegateType,
                Expression.Convert(
                    Expression.MakeMemberAccess(
                        Expression.Convert(instance, property.ReflectedType!),
                        property
                    ),
                    delegateType.GetDelegateReturnType()
                ),
                instance
            ).Compile();
        }

        public static Delegate GetInstanceAccessDelegate(this PropertyInfo property)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));

            return property.GetInstanceAccessDelegate(Expression.GetFuncType(property.ReflectedType!, property.PropertyType));
        }

        public static T GetInstanceAccessDelegate<T>(this PropertyInfo property) where T : Delegate
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));

            return (T)property.GetInstanceAccessDelegate(typeof(T));
        }

        public static Func<object, object?> GetInstanceAccessFunc(this PropertyInfo property)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));

            return property.GetInstanceAccessDelegate<Func<object, object?>>();
        }


        #endregion GetInstanceAccessDelegate


        #region GetInstanceAssignDelegate


        public static Delegate GetInstanceAssignDelegate(this PropertyInfo property, Type delegateType)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            var parameterTypes = delegateType.GetDelegateParameterTypes().ToArray();
            if (parameterTypes.Length != 2)
                throw new ArgumentException($@"Delegate ""{delegateType}"" has an incorrect number of parameters", nameof(delegateType));

            var instance = Expression.Parameter(parameterTypes[0], "instance");
            var value = Expression.Parameter(parameterTypes[1], "value");

            return Expression.Lambda(delegateType,
                Expression.Assign(
                    Expression.Property(Expression.Convert(instance, property.ReflectedType!), property),
                    Expression.Convert(value, property.PropertyType)
                ),
                instance, value
            ).Compile();
        }

        public static Delegate GetInstanceAssignDelegate(this PropertyInfo property)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));

            return property.GetInstanceAssignDelegate(Expression.GetActionType(property.ReflectedType!, property.PropertyType));
        }

        public static T GetInstanceAssignDelegate<T>(this PropertyInfo property) where T : Delegate
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));

            return (T)property.GetInstanceAssignDelegate(typeof(T));
        }

        public static Action<object, object?> GetInstanceAssignAction(this PropertyInfo property)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));

            return property.GetInstanceAssignDelegate<Action<object, object?>>();
        }


        #endregion GetInstanceAssignDelegate


    }
}
