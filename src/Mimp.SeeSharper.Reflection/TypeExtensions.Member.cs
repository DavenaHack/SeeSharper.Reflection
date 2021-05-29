using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Mimp.SeeSharper.Reflection
{
    public static partial class TypeExtensions
    {


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
                throw new InvalidOperationException($@"Type ""{type}"" has no property ""{name}""");
            if (members.Count != 1)
                throw new InvalidOperationException($@"Property ""{name}"" is ambiguous for ""{type}""");
            return members[0];
        }

        public static MemberInfo GetInstancePropertyOrField(this Type type, string name)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetInstancePropertyOrField(name, true);
        }

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
                throw new InvalidOperationException($@"Type ""{type}"" has no property ""{name}""");
            if (members.Count != 1)
                throw new InvalidOperationException($@"Property ""{name}"" is ambiguous for ""{type}""");
            return members[0];
        }

        public static MemberInfo GetStaticPropertyOrField(this Type type, string name)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetStaticPropertyOrField(name, true);
        }


        #region GetInstanceMemberAccessDelegate


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

        public static T GetInstanceMemberAccessDelegate<T>(this Type type, string name) where T : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (T)type.GetInstanceMemberAccessDelegate(name, typeof(T));
        }

        public static Func<object, object?> GetInstanceMemberAccessFunc(this Type type, string name)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetInstanceMemberAccessDelegate<Func<object, object?>>(name);
        }


        #endregion GetInstanceMemberAccessDelegate


        #region GetInstanceMemberAssignDelegate


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

        public static T GetInstanceMemberAssignDelegate<T>(this Type type, string name) where T : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (T)type.GetInstanceMemberAssignDelegate(name, typeof(T));
        }

        public static Action<object, object?> GetInstanceMemberAssignAction(this Type type, string name)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetInstanceMemberAssignDelegate<Action<object, object?>>(name);
        }


        #endregion GetInstanceMemberAssignDelegate


        #region GetInstanceMemberInvokeDelegate


        public static Delegate GetInstanceMemberInvokeDelegate(this Type type, string name, bool ignoreCase, IEnumerable<Type>? genericParameters, IEnumerable<Type>? types, Type delegateType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            return type.GetInstanceMethod(name, ignoreCase, genericParameters, types).GetInstanceDelegate(delegateType);
        }

        public static Delegate GetInstanceMemberInvokeDelegate(this Type type, string name, IEnumerable<Type>? genericParameters, IEnumerable<Type>? types, Type delegateType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            return type.GetInstanceMethod(name, genericParameters, types).GetInstanceDelegate(delegateType);
        }

        public static Delegate GetInstanceMemberInvokeDelegate(this Type type, string name, bool ignoreCase, int genericParameters, IEnumerable<Type>? types, Type delegateType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            return type.GetInstanceMethod(name, ignoreCase, genericParameters, types).GetInstanceDelegate(delegateType);
        }

        public static Delegate GetInstanceMemberInvokeDelegate(this Type type, string name, int genericParameters, IEnumerable<Type>? types, Type delegateType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            return type.GetInstanceMethod(name, genericParameters, types).GetInstanceDelegate(delegateType);
        }

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

        public static Delegate GetInstanceMemberInvokeDelegate(this Type type, string name, bool ignoreCase, IEnumerable<Type>? types, Type delegateType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            return type.GetInstanceMethod(name, ignoreCase, types).GetInstanceDelegate(delegateType);
        }

        public static Delegate GetInstanceMemberInvokeDelegate(this Type type, string name, IEnumerable<Type>? types, Type delegateType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            return type.GetInstanceMethod(name, types).GetInstanceDelegate(delegateType);
        }

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

        public static T GetInstanceMemberInvokeDelegate<T>(this Type type, string name, bool ignoreCase, IEnumerable<Type>? genericParameters, IEnumerable<Type>? types) where T : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (T)type.GetInstanceMemberInvokeDelegate(name, ignoreCase, genericParameters, types, typeof(T));
        }

        public static T GetInstanceMemberInvokeDelegate<T>(this Type type, string name, IEnumerable<Type>? genericParameters, IEnumerable<Type>? types) where T : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (T)type.GetInstanceMemberInvokeDelegate(name, genericParameters, types, typeof(T));
        }

        public static T GetInstanceMemberInvokeDelegate<T>(this Type type, string name, bool ignoreCase, int genericParameters, IEnumerable<Type>? types) where T : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (T)type.GetInstanceMemberInvokeDelegate(name, ignoreCase, genericParameters, types, typeof(T));
        }

        public static T GetInstanceMemberInvokeDelegate<T>(this Type type, string name, int genericParameters, IEnumerable<Type>? types) where T : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (T)type.GetInstanceMemberInvokeDelegate(name, genericParameters, types, typeof(T));
        }

        public static T GetInstanceMemberInvokeDelegate<T>(this Type type, string name, bool ignoreCase, int genericParameters, int parameterTypes) where T : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (T)type.GetInstanceMemberInvokeDelegate(name, ignoreCase, genericParameters, parameterTypes, typeof(T));
        }

        public static T GetInstanceMemberInvokeDelegate<T>(this Type type, string name, int genericParameters, int parameterTypes) where T : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (T)type.GetInstanceMemberInvokeDelegate(name, genericParameters, parameterTypes, typeof(T));
        }

        public static T GetInstanceMemberInvokeDelegate<T>(this Type type, string name, bool ignoreCase, IEnumerable<Type>? types) where T : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (T)type.GetInstanceMemberInvokeDelegate(name, ignoreCase, types, typeof(T));
        }

        public static T GetInstanceMemberInvokeDelegate<T>(this Type type, string name, IEnumerable<Type>? types) where T : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (T)type.GetInstanceMemberInvokeDelegate(name, types, typeof(T));
        }

        public static T GetInstanceMemberInvokeDelegate<T>(this Type type, string name, bool ignoreCase, int parameterTypes) where T : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (T)type.GetInstanceMemberInvokeDelegate(name, ignoreCase, parameterTypes, typeof(T));
        }

        public static T GetInstanceMemberInvokeDelegate<T>(this Type type, string name, int parameterTypes) where T : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (T)type.GetInstanceMemberInvokeDelegate(name, parameterTypes, typeof(T));
        }

        public static T GetInstanceMemberInvokeDelegate<T>(this Type type, string name, bool ignoreCase) where T : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (T)type.GetInstanceMemberInvokeDelegate(name, ignoreCase, typeof(T));
        }

        public static T GetInstanceMemberInvokeDelegate<T>(this Type type, string name) where T : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (T)type.GetInstanceMemberInvokeDelegate(name, typeof(T));
        }


        #endregion GetInstanceMemberInvokeDelegate


        #region GetDynamicInstanceMemberAccessDelegate


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

        public static T GetDynamicInstanceMemberAccessDelegate<T>(this Type type, string name) where T : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (T)type.GetDynamicInstanceMemberAccessDelegate(name, typeof(T));
        }

        public static Func<object, object?> GetDynamicInstanceMemberAccessFunc(this Type type, string name)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetDynamicInstanceMemberAccessDelegate<Func<object, object?>>(name);
        }


        #endregion GetDynamicInstanceMemberAccessDelegate


        #region GetDynamicInstanceMemberAssignDelegate


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

        public static T GetDynamicInstanceMemberAssignDelegate<T>(this Type type, string name) where T : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (T)type.GetDynamicInstanceMemberAssignDelegate(name, typeof(T));
        }

        public static Action<object, object?> GetDynamicInstanceMemberAssignAction(this Type type, string name)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetDynamicInstanceMemberAssignDelegate<Action<object, object?>>(name);
        }


        #endregion GetDynamicInstanceMemberAssignDelegate


        #region GetDynamicInstanceMemberInvokeDelegate


        public static Delegate GetDynamicInstanceMemberInvokeDelegate(this Type type, string name, Type delegateType)    // TODO add generics
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            var parameterTypes = delegateType.GetDelegateParameterTypes().ToArray();
            if (parameterTypes.Length < 1)
                throw new ArgumentException($@"Delegate ""{delegateType}"" has an incorrect number of parameters", nameof(delegateType));

            var instance = Expression.Parameter(parameterTypes[0], "instance");

            var bindArguments = new Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[parameterTypes.Length];
            var parameters = new ParameterExpression[parameterTypes.Length];
            var arguments = new Expression[parameterTypes.Length];
            bindArguments[0] = Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(0, null);
            parameters[0] = instance;
            arguments[0] = Expression.Convert(instance, type);
            for (var i = 1; i < parameterTypes.Length; i++)
            {
                var argName = $"arg{i - 1}";
                bindArguments[i] = Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(0, argName);
                parameters[i] = Expression.Parameter(parameterTypes[i], argName);
                arguments[i] = parameters[i];
            }

            var returnType = delegateType.GetDelegateReturnType();
            var isAction = returnType == typeof(void);
            Expression dyn = Expression.Dynamic(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(
                    isAction ? Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags.ResultDiscarded : 0, name, Type.EmptyTypes /* TODO add generics */, type, bindArguments),
                isAction ? typeof(void) : typeof(object),
                arguments
            );
            if (!isAction)
                dyn = Expression.Convert(dyn, returnType);
            return Expression.Lambda(delegateType,
                dyn,
                parameters
            ).Compile();
        }

        public static T GetDynamicInstanceMemberInvokeDelegate<T>(this Type type, string name) where T : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return (T)type.GetDynamicInstanceMemberInvokeDelegate(name, typeof(T));
        }


        #endregion GetDynamicInstanceMemberInvokeDelegate


    }
}
