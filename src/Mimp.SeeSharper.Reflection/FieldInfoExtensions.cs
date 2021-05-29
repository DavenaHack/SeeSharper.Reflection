using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Mimp.SeeSharper.Reflection
{
    public static class FieldInfoExtensions
    {


        #region GetInstanceAccessDelegate


        public static Delegate GetInstanceAccessDelegate(this FieldInfo field, Type delegateType)
        {
            if (field is null)
                throw new ArgumentNullException(nameof(field));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            var parameterTypes = delegateType.GetDelegateParameterTypes().ToArray();
            if (parameterTypes.Length != 1)
                throw new ArgumentException($@"Delegate ""{delegateType}"" has an incorrect number of parameters", nameof(delegateType));

            var instance = Expression.Parameter(parameterTypes[0], "instance");

            return Expression.Lambda(delegateType,
                Expression.Convert(
                    Expression.MakeMemberAccess(
                        Expression.Convert(instance, field.ReflectedType!),
                        field
                    ),
                    delegateType.GetDelegateReturnType()
                ),
                instance
            ).Compile();
        }

        public static Delegate GetInstanceAccessDelegate(this FieldInfo field)
        {
            if (field is null)
                throw new ArgumentNullException(nameof(field));

            return field.GetInstanceAccessDelegate(Expression.GetFuncType(field.ReflectedType!, field.FieldType));
        }

        public static T GetInstanceAccessDelegate<T>(this FieldInfo field) where T : Delegate
        {
            if (field is null)
                throw new ArgumentNullException(nameof(field));

            return (T)field.GetInstanceAccessDelegate(typeof(T));
        }

        public static Func<object, object?> GetInstanceAccessFunc(this FieldInfo field)
        {
            if (field is null)
                throw new ArgumentNullException(nameof(field));

            return field.GetInstanceAccessDelegate<Func<object, object?>>();
        }


        #endregion GetInstanceAccessDelegate


        #region GetInstanceAssignDelegate


        public static Delegate GetInstanceAssignDelegate(this FieldInfo field, Type delegateType)
        {
            if (field is null)
                throw new ArgumentNullException(nameof(field));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            var parameterTypes = delegateType.GetDelegateParameterTypes().ToArray();
            if (parameterTypes.Length != 2)
                throw new ArgumentException($@"Delegate ""{delegateType}"" has an incorrect number of parameters", nameof(delegateType));

            var instance = Expression.Parameter(parameterTypes[0], "instance");
            var value = Expression.Parameter(parameterTypes[1], "value");

            return Expression.Lambda(delegateType,
                Expression.Assign(
                    Expression.Field(Expression.Convert(instance, field.ReflectedType!), field),
                    Expression.Convert(value, field.FieldType)
                ),
                instance, value
            ).Compile();
        }

        public static Delegate GetInstanceAssignDelegate(this FieldInfo field)
        {
            if (field is null)
                throw new ArgumentNullException(nameof(field));

            return field.GetInstanceAssignDelegate(Expression.GetActionType(field.ReflectedType!, field.FieldType));
        }

        public static T GetInstanceAssignDelegate<T>(this FieldInfo field) where T : Delegate
        {
            if (field is null)
                throw new ArgumentNullException(nameof(field));

            return (T)field.GetInstanceAssignDelegate(typeof(T));
        }

        public static Action<object, object?> GetInstanceAssignAction(this FieldInfo field)
        {
            if (field is null)
                throw new ArgumentNullException(nameof(field));

            return field.GetInstanceAssignDelegate<Action<object, object?>>();
        }


        #endregion SetInstanceMemberDelegate


    }
}
