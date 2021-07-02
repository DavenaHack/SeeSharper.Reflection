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
        /// Return a public instance field or property.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If no or more than one field or property exists.</exception>
        public static MemberInfo GetInstancePropertyOrField(this Type type, string name, bool ignoreCase)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            var members = new List<MemberInfo>();
            members.AddRange(type.GetProperties(name, true, false, false, ignoreCase));
            members.AddRange(type.GetFields(name, true, false, ignoreCase));
            if (members.Count == 0)
                throw new InvalidOperationException($@"Type ""{type}"" has no field or property ""{name}""");
            if (members.Count != 1)
                throw new InvalidOperationException($@"Field or property ""{name}"" is ambiguous for ""{type}""");
            return members[0];
        }

        /// <summary>
        /// Return a public instance field or property.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If no or more than one field or property exists.</exception>
        public static MemberInfo GetInstancePropertyOrField(this Type type, string name)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetInstancePropertyOrField(name, true);
        }

        /// <summary>
        /// Return a public static field or property.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If no or more than one field or property exists.</exception>
        public static MemberInfo GetStaticPropertyOrField(this Type type, string name, bool ignoreCase)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            var members = new List<MemberInfo>();
            members.AddRange(type.GetProperties(name, true, false, true, ignoreCase));
            members.AddRange(type.GetFields(name, true, true, ignoreCase));
            if (members.Count == 0)
                throw new InvalidOperationException($@"Type ""{type}"" has no field or property ""{name}""");
            if (members.Count != 1)
                throw new InvalidOperationException($@"Field or property ""{name}"" is ambiguous for ""{type}""");
            return members[0];
        }

        /// <summary>
        /// Return a public static field or property.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If no or more than one field or property exists.</exception>
        public static MemberInfo GetStaticPropertyOrField(this Type type, string name)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetStaticPropertyOrField(name, true);
        }


        #region GetInstanceMemberAccessDelegate

        /// <summary>
        /// Return a compiled delegate to access the member.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="delegateType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one field or property exists.
        /// If the member type isn't castable to the delegate return type.
        /// </exception>
        public static Delegate GetInstanceMemberAccessDelegate(this Type type, string name, Type delegateType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            var member = type.GetInstancePropertyOrField(name);
            return member is PropertyInfo prop
                ? prop.GetInstanceAccessDelegate(delegateType)
                : ((FieldInfo)member).GetInstanceAccessDelegate(delegateType);
        }

        /// <summary>
        /// Return a compiled delegate to access the member.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one field or property exists.
        /// If the member type isn't castable to the delegate return type.
        /// </exception>
        public static TDelegate GetInstanceMemberAccessDelegate<TDelegate>(this Type type, string name) where TDelegate : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (TDelegate)type.GetInstanceMemberAccessDelegate(name, typeof(TDelegate));
        }

        /// <summary>
        /// Return a compiled delegate to access the member.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="delegateType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one field or property exists.
        /// </exception>
        public static Delegate GetInstanceMemberAccessDelegate(this Type type, string name)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            var member = type.GetInstancePropertyOrField(name);
            return member is PropertyInfo prop
                ? prop.GetInstanceAccessDelegate()
                : ((FieldInfo)member).GetInstanceAccessDelegate();
        }

        /// <summary>
        /// Return a compiled delegate to access the member.
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <typeparam name="TMember"></typeparam>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one field or property exists.
        /// If the member type isn't castable to the delegate return type.
        /// </exception>
        public static Func<TInstance, TMember> GetInstanceMemberAccessFunc<TInstance, TMember>(this Type type, string name)
            where TInstance : notnull
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetInstanceMemberAccessDelegate<Func<TInstance, TMember>>(name);
        }

        /// <summary>
        /// Return a compiled delegate to access the member.
        /// </summary>
        /// <typeparam name="TMember"></typeparam>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one field or property exists.
        /// If the member type isn't castable to the delegate return type.
        /// </exception>
        public static Func<object, TMember> GetInstanceMemberAccessFunc<TMember>(this Type type, string name) =>
            type.GetInstanceMemberAccessFunc<object, TMember>(name);

        /// <summary>
        /// Return a compiled delegate to access the member.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one field or property exists.
        /// If the member type isn't castable to the delegate return type.
        /// </exception>
        public static Func<object, object?> GetInstanceMemberAccessFunc(this Type type, string name) =>
            type.GetInstanceMemberAccessFunc<object?>(name);


        #endregion GetInstanceMemberAccessDelegate


        #region GetInstanceMemberAssignDelegate


        /// <summary>
        /// Return a compiled delegate to assign the member.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="delegateType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one field or property exists.
        /// If the delegate value type isn't castable to the member type.
        /// </exception>
        public static Delegate GetInstanceMemberAssignDelegate(this Type type, string name, Type delegateType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            var member = type.GetInstancePropertyOrField(name);
            return member is PropertyInfo prop
                ? prop.GetInstanceAssignDelegate(delegateType)
                : ((FieldInfo)member).GetInstanceAssignDelegate(delegateType);
        }

        /// <summary>
        /// Return a compiled delegate to assign the member.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one field or property exists.
        /// If the delegate value type isn't castable to the member type.
        /// </exception>
        public static TDelegate GetInstanceMemberAssignDelegate<TDelegate>(this Type type, string name) where TDelegate : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (TDelegate)type.GetInstanceMemberAssignDelegate(name, typeof(TDelegate));
        }

        /// <summary>
        /// Return a compiled delegate to assign the member.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="delegateType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one field or property exists.
        /// </exception>
        public static Delegate GetInstanceMemberAssignDelegate(this Type type, string name)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            var member = type.GetInstancePropertyOrField(name);
            return member is PropertyInfo prop
                ? prop.GetInstanceAssignDelegate()
                : ((FieldInfo)member).GetInstanceAssignDelegate();
        }

        /// <summary>
        /// Return a compiled delegate to assign the member.
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <typeparam name="TMember"></typeparam>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one field or property exists.
        /// If the delegate value type isn't castable to the member type.
        /// </exception>
        public static Action<TInstance, TMember> GetInstanceMemberAssignAction<TInstance, TMember>(this Type type, string name)
            where TInstance : notnull
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetInstanceMemberAssignDelegate<Action<TInstance, TMember>>(name);
        }

        /// <summary>
        /// Return a compiled delegate to assign the member.
        /// </summary>
        /// <typeparam name="TMember"></typeparam>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one field or property exists.
        /// If the delegate value type isn't castable to the member type.
        /// </exception>
        public static Action<object, TMember> GetInstanceMemberAssignAction<TMember>(this Type type, string name) =>
            type.GetInstanceMemberAssignAction<object, TMember>(name);

        /// <summary>
        /// Return a compiled delegate to assign the member.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one field or property exists.
        /// If the delegate value type isn't castable to the member type.
        /// </exception>
        public static Action<object, object?> GetInstanceMemberAssignAction(this Type type, string name) =>
            type.GetInstanceMemberAssignAction<object?>(name);


        #endregion GetInstanceMemberAssignDelegate


        #region GetInstanceMemberInvokeDelegate


        /// <summary>
        /// Return a delegate to invoke the method.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="ignoreCase"></param>
        /// <param name="genericParameters"></param>
        /// <param name="parameterTypes"></param>
        /// <param name="delegateType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate has no parameter for instance or the delegate hasn't equal parameter as the method.</exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one mehtod exists.
        /// If the delegate types or instance type isn't castable to the method types.
        /// </exception>
        public static Delegate GetInstanceMemberInvokeDelegate(this Type type, string name, bool ignoreCase, IEnumerable<Type>? genericParameters, IEnumerable<Type>? parameterTypes, Type delegateType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            return type.GetInstanceMethod(name, ignoreCase, genericParameters, parameterTypes).GetInstanceDelegate(delegateType);
        }

        /// <summary>
        /// Return a delegate to invoke the method.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="genericParameters"></param>
        /// <param name="parameterTypes"></param>
        /// <param name="delegateType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate has no parameter for instance or the delegate hasn't equal parameter as the method.</exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one mehtod exists.
        /// If the delegate types or instance type isn't castable to the method types.
        /// </exception>
        public static Delegate GetInstanceMemberInvokeDelegate(this Type type, string name, IEnumerable<Type>? genericParameters, IEnumerable<Type>? parameterTypes, Type delegateType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            return type.GetInstanceMethod(name, genericParameters, parameterTypes).GetInstanceDelegate(delegateType);
        }

        /// <summary>
        /// Return a delegate to invoke the method.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="ignoreCase"></param>
        /// <param name="genericParameters"></param>
        /// <param name="parameterTypes"></param>
        /// <param name="delegateType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate has no parameter for instance or the delegate hasn't equal parameter as the method.</exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one mehtod exists.
        /// If the delegate types or instance type isn't castable to the method types.
        /// </exception>
        public static Delegate GetInstanceMemberInvokeDelegate(this Type type, string name, bool ignoreCase, int genericParameters, IEnumerable<Type>? parameterTypes, Type delegateType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            return type.GetInstanceMethod(name, ignoreCase, genericParameters, parameterTypes).GetInstanceDelegate(delegateType);
        }

        /// <summary>
        /// Return a delegate to invoke the method.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="genericParameters"></param>
        /// <param name="parameterTypes"></param>
        /// <param name="delegateType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate has no parameter for instance or the delegate hasn't equal parameter as the method.</exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one mehtod exists.
        /// If the delegate types or instance type isn't castable to the method types.
        /// </exception>
        public static Delegate GetInstanceMemberInvokeDelegate(this Type type, string name, int genericParameters, IEnumerable<Type>? parameterTypes, Type delegateType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            return type.GetInstanceMethod(name, genericParameters, parameterTypes).GetInstanceDelegate(delegateType);
        }

        /// <summary>
        /// Return a delegate to invoke the method.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="ignoreCase"></param>
        /// <param name="genericParameters"></param>
        /// <param name="parameterTypes"></param>
        /// <param name="delegateType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate has no parameter for instance or the delegate hasn't equal parameter as the method.</exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one mehtod exists.
        /// If the delegate types or instance type isn't castable to the method types.
        /// </exception>
        public static Delegate GetInstanceMemberInvokeDelegate(this Type type, string name, bool ignoreCase, int genericParameters, int parameterTypes, Type delegateType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            return type.GetInstanceMethod(name, ignoreCase, genericParameters, parameterTypes).GetInstanceDelegate(delegateType);
        }

        /// <summary>
        /// Return a delegate to invoke the method.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="genericParameters"></param>
        /// <param name="parameterTypes"></param>
        /// <param name="delegateType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate has no parameter for instance or the delegate hasn't equal parameter as the method.</exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one mehtod exists.
        /// If the delegate types or instance type isn't castable to the method types.
        /// </exception>
        public static Delegate GetInstanceMemberInvokeDelegate(this Type type, string name, int genericParameters, int parameterTypes, Type delegateType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            return type.GetInstanceMethod(name, genericParameters, parameterTypes).GetInstanceDelegate(delegateType);
        }

        /// <summary>
        /// Return a delegate to invoke the method.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="ignoreCase"></param>
        /// <param name="parameterTypes"></param>
        /// <param name="delegateType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate has no parameter for instance or the delegate hasn't equal parameter as the method.</exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one mehtod exists.
        /// If the delegate types or instance type isn't castable to the method types.
        /// </exception>
        public static Delegate GetInstanceMemberInvokeDelegate(this Type type, string name, bool ignoreCase, IEnumerable<Type>? parameterTypes, Type delegateType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            return type.GetInstanceMethod(name, ignoreCase, parameterTypes).GetInstanceDelegate(delegateType);
        }

        /// <summary>
        /// Return a delegate to invoke the method.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="parameterTypes"></param>
        /// <param name="delegateType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate has no parameter for instance or the delegate hasn't equal parameter as the method.</exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one mehtod exists.
        /// If the delegate types or instance type isn't castable to the method types.
        /// </exception>
        public static Delegate GetInstanceMemberInvokeDelegate(this Type type, string name, IEnumerable<Type>? parameterTypes, Type delegateType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            return type.GetInstanceMethod(name, parameterTypes).GetInstanceDelegate(delegateType);
        }

        /// <summary>
        /// Return a delegate to invoke the method.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="ignoreCase"></param>
        /// <param name="parameterTypes"></param>
        /// <param name="delegateType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate has no parameter for instance or the delegate hasn't equal parameter as the method.</exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one mehtod exists.
        /// If the delegate types or instance type isn't castable to the method types.
        /// </exception>
        public static Delegate GetInstanceMemberInvokeDelegate(this Type type, string name, bool ignoreCase, int parameterTypes, Type delegateType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            return type.GetInstanceMethod(name, ignoreCase, parameterTypes).GetInstanceDelegate(delegateType);
        }

        /// <summary>
        /// Return a delegate to invoke the method.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="parameterTypes"></param>
        /// <param name="delegateType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate has no parameter for instance or the delegate hasn't equal parameter as the method.</exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one mehtod exists.
        /// If the delegate types or instance type isn't castable to the method types.
        /// </exception>
        public static Delegate GetInstanceMemberInvokeDelegate(this Type type, string name, int parameterTypes, Type delegateType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            return type.GetInstanceMethod(name, parameterTypes).GetInstanceDelegate(delegateType);
        }

        /// <summary>
        /// Return a delegate to invoke the method.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="ignoreCase"></param>
        /// <param name="delegateType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate has no parameter for instance or the delegate hasn't equal parameter as the method.</exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one mehtod exists.
        /// If the delegate types or instance type isn't castable to the method types.
        /// </exception>
        public static Delegate GetInstanceMemberInvokeDelegate(this Type type, string name, bool ignoreCase, Type delegateType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            return type.GetInstanceMethod(name, ignoreCase).GetInstanceDelegate(delegateType);
        }

        /// <summary>
        /// Return a delegate to invoke the method.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="delegateType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate has no parameter for instance or the delegate hasn't equal parameter as the method.</exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one mehtod exists.
        /// If the delegate types or instance type isn't castable to the method types.
        /// </exception>
        public static Delegate GetInstanceMemberInvokeDelegate(this Type type, string name, Type delegateType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            return type.GetInstanceMethod(name).GetInstanceDelegate(delegateType);
        }

        /// <summary>
        /// Return a delegate to invoke the method.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="ignoreCase"></param>
        /// <param name="genericParameters"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate has no parameter for instance or the delegate hasn't equal parameter as the method.</exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one mehtod exists.
        /// If the delegate types or instance type isn't castable to the method types.
        /// </exception>
        public static TDelegate GetInstanceMemberInvokeDelegate<TDelegate>(this Type type, string name, bool ignoreCase, IEnumerable<Type>? genericParameters, IEnumerable<Type>? parameterTypes) where TDelegate : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (TDelegate)type.GetInstanceMemberInvokeDelegate(name, ignoreCase, genericParameters, parameterTypes, typeof(TDelegate));
        }

        /// <summary>
        /// Return a delegate to invoke the method.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="genericParameters"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate has no parameter for instance or the delegate hasn't equal parameter as the method.</exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one mehtod exists.
        /// If the delegate types or instance type isn't castable to the method types.
        /// </exception>
        public static TDelegate GetInstanceMemberInvokeDelegate<TDelegate>(this Type type, string name, IEnumerable<Type>? genericParameters, IEnumerable<Type>? parameterTypes) where TDelegate : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (TDelegate)type.GetInstanceMemberInvokeDelegate(name, genericParameters, parameterTypes, typeof(TDelegate));
        }

        /// <summary>
        /// Return a delegate to invoke the method.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="ignoreCase"></param>
        /// <param name="genericParameters"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate has no parameter for instance or the delegate hasn't equal parameter as the method.</exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one mehtod exists.
        /// If the delegate types or instance type isn't castable to the method types.
        /// </exception>
        public static TDelegate GetInstanceMemberInvokeDelegate<TDelegate>(this Type type, string name, bool ignoreCase, int genericParameters, IEnumerable<Type>? parameterTypes) where TDelegate : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (TDelegate)type.GetInstanceMemberInvokeDelegate(name, ignoreCase, genericParameters, parameterTypes, typeof(TDelegate));
        }

        /// <summary>
        /// Return a delegate to invoke the method.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="genericParameters"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate has no parameter for instance or the delegate hasn't equal parameter as the method.</exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one mehtod exists.
        /// If the delegate types or instance type isn't castable to the method types.
        /// </exception>
        public static TDelegate GetInstanceMemberInvokeDelegate<TDelegate>(this Type type, string name, int genericParameters, IEnumerable<Type>? parameterTypes) where TDelegate : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (TDelegate)type.GetInstanceMemberInvokeDelegate(name, genericParameters, parameterTypes, typeof(TDelegate));
        }

        /// <summary>
        /// Return a delegate to invoke the method.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="ignoreCase"></param>
        /// <param name="genericParameters"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate has no parameter for instance or the delegate hasn't equal parameter as the method.</exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one mehtod exists.
        /// If the delegate types or instance type isn't castable to the method types.
        /// </exception>
        public static TDelegate GetInstanceMemberInvokeDelegate<TDelegate>(this Type type, string name, bool ignoreCase, int genericParameters, int parameterTypes) where TDelegate : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (TDelegate)type.GetInstanceMemberInvokeDelegate(name, ignoreCase, genericParameters, parameterTypes, typeof(TDelegate));
        }

        /// <summary>
        /// Return a delegate to invoke the method.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="genericParameters"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate has no parameter for instance or the delegate hasn't equal parameter as the method.</exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one mehtod exists.
        /// If the delegate types or instance type isn't castable to the method types.
        /// </exception>
        public static TDelegate GetInstanceMemberInvokeDelegate<TDelegate>(this Type type, string name, int genericParameters, int parameterTypes) where TDelegate : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (TDelegate)type.GetInstanceMemberInvokeDelegate(name, genericParameters, parameterTypes, typeof(TDelegate));
        }

        /// <summary>
        /// Return a delegate to invoke the method.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="ignoreCase"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate has no parameter for instance or the delegate hasn't equal parameter as the method.</exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one mehtod exists.
        /// If the delegate types or instance type isn't castable to the method types.
        /// </exception>
        public static TDelegate GetInstanceMemberInvokeDelegate<TDelegate>(this Type type, string name, bool ignoreCase, IEnumerable<Type>? parameterTypes) where TDelegate : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (TDelegate)type.GetInstanceMemberInvokeDelegate(name, ignoreCase, parameterTypes, typeof(TDelegate));
        }

        /// <summary>
        /// Return a delegate to invoke the method.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate has no parameter for instance or the delegate hasn't equal parameter as the method.</exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one mehtod exists.
        /// If the delegate types or instance type isn't castable to the method types.
        /// </exception>
        public static TDelegate GetInstanceMemberInvokeDelegate<TDelegate>(this Type type, string name, IEnumerable<Type>? parameterTypes) where TDelegate : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (TDelegate)type.GetInstanceMemberInvokeDelegate(name, parameterTypes, typeof(TDelegate));
        }

        /// <summary>
        /// Return a delegate to invoke the method.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="ignoreCase"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate has no parameter for instance or the delegate hasn't equal parameter as the method.</exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one mehtod exists.
        /// If the delegate types or instance type isn't castable to the method types.
        /// </exception>
        public static TDelegate GetInstanceMemberInvokeDelegate<TDelegate>(this Type type, string name, bool ignoreCase, int parameterTypes) where TDelegate : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (TDelegate)type.GetInstanceMemberInvokeDelegate(name, ignoreCase, parameterTypes, typeof(TDelegate));
        }

        /// <summary>
        /// Return a delegate to invoke the method.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate has no parameter for instance or the delegate hasn't equal parameter as the method.</exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one mehtod exists.
        /// If the delegate types or instance type isn't castable to the method types.
        /// </exception>
        public static TDelegate GetInstanceMemberInvokeDelegate<TDelegate>(this Type type, string name, int parameterTypes) where TDelegate : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (TDelegate)type.GetInstanceMemberInvokeDelegate(name, parameterTypes, typeof(TDelegate));
        }

        /// <summary>
        /// Return a delegate to invoke the method.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate has no parameter for instance or the delegate hasn't equal parameter as the method.</exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one mehtod exists.
        /// If the delegate types or instance type isn't castable to the method types.
        /// </exception>
        public static TDelegate GetInstanceMemberInvokeDelegate<TDelegate>(this Type type, string name, bool ignoreCase) where TDelegate : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (TDelegate)type.GetInstanceMemberInvokeDelegate(name, ignoreCase, typeof(TDelegate));
        }

        /// <summary>
        /// Return a delegate to invoke the method.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate has no parameter for instance or the delegate hasn't equal parameter as the method.</exception>
        /// <exception cref="InvalidOperationException">
        /// If no or more than one mehtod exists.
        /// If the delegate types or instance type isn't castable to the method types.
        /// </exception>
        public static TDelegate GetInstanceMemberInvokeDelegate<TDelegate>(this Type type, string name) where TDelegate : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (TDelegate)type.GetInstanceMemberInvokeDelegate(name, typeof(TDelegate));
        }


        #endregion GetInstanceMemberInvokeDelegate


        #region GetDynamicInstanceMemberAccessDelegate


        /// <summary>
        /// Return a compiled delegate to access dynamic the member.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="delegateType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Delegate GetDynamicInstanceMemberAccessDelegate(this Type type, string name, Type delegateType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            var parameterTypes = delegateType.GetDelegateParameterTypes().ToArray();
            if (parameterTypes.Length != 1)
                throw new ArgumentException($@"Delegate ""{delegateType}"" has an incorrect number of parameters", nameof(delegateType));

            var instance = Expression.Parameter(parameterTypes[0], "instance");

            return Expression.Lambda(delegateType,
                Expression.Convert(
                    Expression.Dynamic(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(0, name, type, new[] { Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(0, null) }),
                        typeof(object),
                        Expression.Convert(instance, type)
                    ),
                    delegateType.GetDelegateReturnType()
                ),
                instance
            ).Compile();
        }

        /// <summary>
        /// Return a compiled delegate to access dynamic the member.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static TDelegate GetDynamicInstanceMemberAccessDelegate<TDelegate>(this Type type, string name) where TDelegate : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (TDelegate)type.GetDynamicInstanceMemberAccessDelegate(name, typeof(TDelegate));
        }

        /// <summary>
        /// Return a compiled delegate to access dynamic the member.
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <typeparam name="TMember"></typeparam>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Func<TInstance, TMember> GetDynamicInstanceMemberAccessFunc<TInstance, TMember>(this Type type, string name)
            where TInstance : notnull
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetDynamicInstanceMemberAccessDelegate<Func<TInstance, TMember>>(name);
        }

        /// <summary>
        /// Return a compiled delegate to access dynamic the member.
        /// </summary>
        /// <typeparam name="TMember"></typeparam>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Func<object, TMember> GetDynamicInstanceMemberAccessFunc<TMember>(this Type type, string name) =>
            type.GetDynamicInstanceMemberAccessFunc<object, TMember>(name);

        /// <summary>
        /// Return a compiled delegate to access dynamic the member.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Func<object, object?> GetDynamicInstanceMemberAccessFunc(this Type type, string name) =>
            type.GetDynamicInstanceMemberAccessFunc<object?>(name);


        #endregion GetDynamicInstanceMemberAccessDelegate


        #region GetDynamicInstanceMemberAssignDelegate


        /// <summary>
        /// Return a compiled delegate to assign dynamic the member.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="delegateType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Delegate GetDynamicInstanceMemberAssignDelegate(this Type type, string name, Type delegateType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            var parameterTypes = delegateType.GetDelegateParameterTypes().ToArray();
            if (parameterTypes.Length != 2)
                throw new ArgumentException($@"Delegate ""{delegateType}"" has an incorrect number of parameters", nameof(delegateType));

            var instance = Expression.Parameter(parameterTypes[0], "instance");
            var value = Expression.Parameter(parameterTypes[1], "value");

            return Expression.Lambda(delegateType,
                Expression.Dynamic(Microsoft.CSharp.RuntimeBinder.Binder.SetMember(0, name, type, new[] { Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(0, null), Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(0, value.Name) }),
                    typeof(void),
                    Expression.Convert(instance, type),
                    value
                ),
                instance, value
            ).Compile();
        }

        /// <summary>
        /// Return a compiled delegate to assign dynamic the member.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static TDelegate GetDynamicInstanceMemberAssignDelegate<TDelegate>(this Type type, string name) where TDelegate : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (TDelegate)type.GetDynamicInstanceMemberAssignDelegate(name, typeof(TDelegate));
        }

        /// <summary>
        /// Return a compiled delegate to assign dynamic the member.
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <typeparam name="TMember"></typeparam>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Action<TInstance, TMember> GetDynamicInstanceMemberAssignAction<TInstance, TMember>(this Type type, string name)
            where TInstance : notnull
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetDynamicInstanceMemberAssignDelegate<Action<TInstance, TMember>>(name);
        }

        /// <summary>
        /// Return a compiled delegate to assign dynamic the member.
        /// </summary>
        /// <typeparam name="TMember"></typeparam>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Action<object, TMember> GetDynamicInstanceMemberAssignAction<TMember>(this Type type, string name) =>
            type.GetDynamicInstanceMemberAssignAction<object, TMember>(name);

        /// <summary>
        /// Return a compiled delegate to assign dynamic the member.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Action<object, object?> GetDynamicInstanceMemberAssignAction(this Type type, string name) =>
            type.GetDynamicInstanceMemberAssignAction<object?>(name);


        #endregion GetDynamicInstanceMemberAssignDelegate


        #region GetDynamicInstanceMemberInvokeDelegate


        //public static Delegate GetDynamicInstanceMemberInvokeDelegate(this Type type, string name, Type delegateType)    // TODO add generics
        //{
        //    if (type is null)
        //        throw new ArgumentNullException(nameof(type));
        //    if (name is null)
        //        throw new ArgumentNullException(nameof(name));
        //    if (delegateType is null)
        //        throw new ArgumentNullException(nameof(delegateType));

        //    var parameterTypes = delegateType.GetDelegateParameterTypes().ToArray();
        //    if (parameterTypes.Length < 1)
        //        throw new ArgumentException($@"Delegate ""{delegateType}"" has an incorrect number of parameters", nameof(delegateType));

        //    var instance = Expression.Parameter(parameterTypes[0], "instance");

        //    var bindArguments = new Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[parameterTypes.Length];
        //    var parameters = new ParameterExpression[parameterTypes.Length];
        //    var arguments = new Expression[parameterTypes.Length];
        //    bindArguments[0] = Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(0, null);
        //    parameters[0] = instance;
        //    arguments[0] = Expression.Convert(instance, type);
        //    for (var i = 1; i < parameterTypes.Length; i++)
        //    {
        //        var argName = $"arg{i - 1}";
        //        bindArguments[i] = Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(0, argName);
        //        parameters[i] = Expression.Parameter(parameterTypes[i], argName);
        //        arguments[i] = parameters[i];
        //    }

        //    var returnType = delegateType.GetDelegateReturnType();
        //    var isAction = returnType == typeof(void);
        //    Expression dyn = Expression.Dynamic(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(
        //            isAction ? Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags.ResultDiscarded : 0, name, Type.EmptyTypes /* TODO add generics */, type, bindArguments),
        //        isAction ? typeof(void) : typeof(object),
        //        arguments
        //    );
        //    if (!isAction)
        //        dyn = Expression.Convert(dyn, returnType);
        //    return Expression.Lambda(delegateType,
        //        dyn,
        //        parameters
        //    ).Compile();
        //}

        //public static T GetDynamicInstanceMemberInvokeDelegate<T>(this Type type, string name) where T : Delegate
        //{
        //    if (type is null)
        //        throw new ArgumentNullException(nameof(type));
        //    if (name is null)
        //        throw new ArgumentNullException(nameof(name));

        //    return (T)type.GetDynamicInstanceMemberInvokeDelegate(name, typeof(T));
        //}


        #endregion GetDynamicInstanceMemberInvokeDelegate


    }
}
