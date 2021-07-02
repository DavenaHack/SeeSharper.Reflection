using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Mimp.SeeSharper.Reflection
{
    public static class FieldInfoExtensions
    {


        #region GetInstanceAccessDelegate


        /// <summary>
        /// Return a compiled delegate to access the field.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="delegateType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate hasn't one parameter for the instance.</exception>
        /// <exception cref="InvalidOperationException">If the field type isn't castable to the delegate return type.</exception>
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

        /// <summary>
        /// Return a compiled delegate to access the field.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate hasn't one parameter for the instance.</exception>
        /// <exception cref="InvalidOperationException">If the field type isn't castable to the delegate return type.</exception>
        public static TDelegate GetInstanceAccessDelegate<TDelegate>(this FieldInfo field) where TDelegate : Delegate
        {
            if (field is null)
                throw new ArgumentNullException(nameof(field));

            return (TDelegate)field.GetInstanceAccessDelegate(typeof(TDelegate));
        }

        /// <summary>
        /// Return a compiled delegate to access the field with a return type of the field type.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Delegate GetInstanceAccessDelegate(this FieldInfo field)
        {
            if (field is null)
                throw new ArgumentNullException(nameof(field));

            return field.GetInstanceAccessDelegate(Expression.GetFuncType(field.ReflectedType!, field.FieldType));
        }

        /// <summary>
        /// Return a compiled function to access the field.
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <typeparam name="TField"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        public static Func<TInstance, TField> GetInstanceAccessFunc<TInstance, TField>(this FieldInfo field)
            where TInstance : notnull
        {
            if (field is null)
                throw new ArgumentNullException(nameof(field));

            return field.GetInstanceAccessDelegate<Func<TInstance, TField>>();
        }

        /// <summary>
        /// Return a compiled function to access the field.
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        public static Func<object, TField> GetInstanceAccessFunc<TField>(this FieldInfo field) =>
            field.GetInstanceAccessFunc<object, TField>();

        /// <summary>
        /// Return a compiled function to access the field.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static Func<object, object?> GetInstanceAccessFunc(this FieldInfo field) =>
            field.GetInstanceAccessFunc<object?>();


        #endregion GetInstanceAccessDelegate


        #region GetInstanceAssignDelegate


        /// <summary>
        /// Return a compiled delegate to assign the field.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="delegateType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate hasn't one parameter for the instance and one parameter for the value.</exception>
        /// <exception cref="InvalidOperationException">If the delegate value type isn't castable to the field type.</exception>
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

        /// <summary>
        /// Return a compiled delegate to assign the field.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate hasn't one parameter for the instance and one parameter for the value.</exception>
        /// <exception cref="InvalidOperationException">If the delegate value type isn't castable to the field type.</exception>
        public static TDelegate GetInstanceAssignDelegate<TDelegate>(this FieldInfo field) where TDelegate : Delegate
        {
            if (field is null)
                throw new ArgumentNullException(nameof(field));

            return (TDelegate)field.GetInstanceAssignDelegate(typeof(TDelegate));
        }

        /// <summary>
        /// Return a compiled delegate to assign the field with a parameter of the field type.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Delegate GetInstanceAssignDelegate(this FieldInfo field)
        {
            if (field is null)
                throw new ArgumentNullException(nameof(field));

            return field.GetInstanceAssignDelegate(Expression.GetActionType(field.ReflectedType!, field.FieldType));
        }

        /// <summary>
        /// Return a compield action to assign the field.
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <typeparam name="TField"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Action<TInstance, TField> GetInstanceAssignAction<TInstance, TField>(this FieldInfo field)
            where TInstance : notnull
        {
            if (field is null)
                throw new ArgumentNullException(nameof(field));

            return field.GetInstanceAssignDelegate<Action<TInstance, TField?>>();
        }

        /// <summary>
        /// Return a compield action to assign the field.
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Action<object, TField> GetInstanceAssignAction<TField>(this FieldInfo field) =>
            field.GetInstanceAssignAction<object, TField>();

        /// <summary>
        /// Return a compield action to assign the field.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Action<object, object?> GetInstanceAssignAction(this FieldInfo field) =>
            field.GetInstanceAssignAction<object?>();


        #endregion SetInstanceMemberDelegate


    }
}
