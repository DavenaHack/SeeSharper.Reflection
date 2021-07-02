using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Mimp.SeeSharper.Reflection
{
    public static class PropertyInfoExtensions
    {


        /// <summary>
        /// Check if property is a static property.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsStatic(this PropertyInfo property)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));

            return property.GetMethod is not null && property.GetMethod.IsStatic
                || property.SetMethod is not null && property.SetMethod.IsStatic;
        }

        /// <summary>
        /// Check if property has a public get method.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool HasPublicGet(this PropertyInfo property)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));

            return property.GetMethod is not null && property.GetMethod.IsPublic;
        }

        /// <summary>
        /// Check if property has a public setter.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool HasPublicSet(this PropertyInfo property)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));

            return property.SetMethod is not null && property.SetMethod.IsPublic;
        }


        #region GetInstanceAccessDelegate


        /// <summary>
        /// Return a compiled delegate to access the property.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="delegateType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate hasn't one parameter for the instance.</exception>
        /// <exception cref="InvalidOperationException">If the property type isn't castable to the delegate return type.</exception>
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

        /// <summary>
        /// Return a compiled delegate to access the property.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate hasn't one parameter for the instance.</exception>
        /// <exception cref="InvalidOperationException">If the property type isn't castable to the delegate return type.</exception>
        public static TDelegate GetInstanceAccessDelegate<TDelegate>(this PropertyInfo property) where TDelegate : Delegate
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));

            return (TDelegate)property.GetInstanceAccessDelegate(typeof(TDelegate));
        }

        /// <summary>
        /// Return a compiled delegate to access the property with a return type of the property type.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Delegate GetInstanceAccessDelegate(this PropertyInfo property)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));

            return property.GetInstanceAccessDelegate(Expression.GetFuncType(property.ReflectedType!, property.PropertyType));
        }


        /// <summary>
        /// Return a compiled function to access the property.
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        public static Func<TInstance, TProperty> GetInstanceAccessFunc<TInstance, TProperty>(this PropertyInfo property)
            where TInstance : notnull
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));

            return property.GetInstanceAccessDelegate<Func<TInstance, TProperty>>();
        }

        /// <summary>
        /// Return a compiled function to access the property.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        public static Func<object, TProperty> GetInstanceAccessFunc<TProperty>(this PropertyInfo property) =>
            property.GetInstanceAccessFunc<object, TProperty>();

        /// <summary>
        /// Return a compiled function to access the property.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static Func<object, object?> GetInstanceAccessFunc(this PropertyInfo property) =>
            property.GetInstanceAccessFunc<object?>();


        #endregion GetInstanceAccessDelegate


        #region GetInstanceAssignDelegate


        /// <summary>
        /// Return a compiled delegate to assign the property.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="delegateType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate hasn't one parameter for the instance and one parameter for the value.</exception>
        /// <exception cref="InvalidOperationException">If the delegate value type isn't castable to the property type.</exception>
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

        /// <summary>
        /// Return a compiled delegate to assign the property.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate hasn't one parameter for the instance and one parameter for the value.</exception>
        /// <exception cref="InvalidOperationException">If the delegate value type isn't castable to the property type.</exception>
        public static TDelegate GetInstanceAssignDelegate<TDelegate>(this PropertyInfo property) where TDelegate : Delegate
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));

            return (TDelegate)property.GetInstanceAssignDelegate(typeof(TDelegate));
        }

        /// <summary>
        /// Return a compiled delegate to assign the property with a parameter of the property type.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Delegate GetInstanceAssignDelegate(this PropertyInfo property)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));

            return property.GetInstanceAssignDelegate(Expression.GetActionType(property.ReflectedType!, property.PropertyType));
        }

        /// <summary>
        /// Return a compield action to assign the property.
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Action<TInstance, TProperty> GetInstanceAssignAction<TInstance, TProperty>(this PropertyInfo property)
            where TInstance : notnull
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));

            return property.GetInstanceAssignDelegate<Action<TInstance, TProperty?>>();
        }

        /// <summary>
        /// Return a compield action to assign the property.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Action<object, TProperty> GetInstanceAssignAction<TProperty>(this PropertyInfo property) =>
            property.GetInstanceAssignAction<object, TProperty>();

        /// <summary>
        /// Return a compield action to assign the property.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Action<object, object?> GetInstanceAssignAction(this PropertyInfo property) =>
            property.GetInstanceAssignAction<object?>();


        #endregion GetInstanceAssignDelegate


    }
}
