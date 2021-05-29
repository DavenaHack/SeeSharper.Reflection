using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Mimp.SeeSharper.Reflection
{
    public static class MethodInfoExtensions
    {

        public static IEnumerable<Type> GetParameterTypes(this MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            foreach (var p in method.GetParameters())
                yield return p.ParameterType;
        }

        public static IEnumerable<Type> GetMethodTypes(this MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            foreach (var t in method.GetParameterTypes())
                yield return t;
            yield return method.ReturnType;
        }

        public static IEnumerable<Type> GetInstanceMethodTypes(this MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            yield return method.ReflectedType!;
            foreach (var t in method.GetMethodTypes())
                yield return t;
        }


        public static bool IsAction(this MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            return method.ReturnType.IsVoid();
        }

        public static bool IsFunc(this MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            return !method.ReturnType.IsVoid();
        }


        #region Delegate


        public static Delegate GetInstanceParameterDelegate(this MethodInfo method, Type delegateType)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            var delegateParameterTypes = delegateType.GetDelegateParameterTypes().ToArray();
            if (delegateParameterTypes.Length != 2)
                throw new ArgumentException($@"Delegate ""{delegateType}"" has to have 2 parameters - instance and parameters", nameof(delegateType));
            if (!delegateParameterTypes[1].IsArray)
                throw new ArgumentException($@"The type for the parameters has to be an array", nameof(delegateType));

            var instance = Expression.Parameter(delegateParameterTypes[0], "instance");

            var parameters = Expression.Parameter(delegateParameterTypes[1], "args");
            var paras = method.GetParameters();
            var arguments = new Expression[paras.Length];
            for (var i = 0; i < arguments.Length; i++)
                arguments[i] = Expression.Convert(
                        Expression.ArrayIndex(parameters, Expression.Constant(i)),
                        paras[i].ParameterType
                    );

            return Expression.Lambda(delegateType, Expression.Block(
                Expression.IfThen(
                    Expression.NotEqual(Expression.ArrayLength(parameters), Expression.Constant(arguments.Length)),
                    Expression.Throw(
                        Expression.New(typeof(TargetParameterCountException).GetConstructor(new[] { typeof(string) })!,
                        Expression.Constant($"An incorrect number of parameters was passed.")))
                ),
                Expression.Convert(Expression.Call(Expression.Convert(instance, method.ReflectedType!), method, arguments), delegateType.GetDelegateReturnType())
            ), instance, parameters).Compile();
        }


        public static Delegate GetInstanceParameterDelegate(this MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            return method.GetInstanceParameterDelegate(method.IsAction() ? typeof(Action<object, object?[]>) : typeof(Func<object, object?[], object?>));
        }

        public static Delegate GetInstanceParamsDelegate(this MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            return method.GetInstanceParameterDelegate(method.IsAction() ? typeof(InstanceAction) : typeof(InstanceFunc));
        }

        public static T GetInstanceParameterDelegate<T>(this MethodInfo method) where T : Delegate
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            return (T)method.GetInstanceParameterDelegate(typeof(T));
        }

        public static Func<object, object?[], object?> GetInstanceParameterFunc(this MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            return method.GetInstanceParameterDelegate<Func<object, object?[], object?>>();
        }

        public static InstanceFunc GetInstanceParamsFunc(this MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            return method.GetInstanceParameterDelegate<InstanceFunc>();
        }

        public static Action<object, object?[]> GetInstanceParameterAction(this MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            return method.GetInstanceParameterDelegate<Action<object, object?[]>>();
        }

        public static InstanceAction GetInstanceParamsAction(this MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            return method.GetInstanceParameterDelegate<InstanceAction>();
        }

        public static Delegate GetInstanceDelegate(this MethodInfo method, Type delegateType)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            var delegateParameterTypes = delegateType.GetDelegateParameterTypes().ToArray();
            if (delegateParameterTypes.Length < 1)
                throw new ArgumentException($@"Delegate ""{delegateType}"" has no parameter for the instance", nameof(delegateType));

            var paras = method.GetParameters().ToArray();
            if (delegateParameterTypes.Length - 1 != paras.Length)
                throw new ArgumentException($@"Delegate ""{delegateType}"" has an incorrect number of parameters", nameof(delegateType));

            var instance = Expression.Parameter(delegateParameterTypes[0], "instance");

            var parameters = new ParameterExpression[delegateParameterTypes.Length];
            parameters[0] = instance;
            for (var i = 1; i < parameters.Length; i++)
                parameters[i] = Expression.Parameter(delegateParameterTypes[i], $"arg{i - 1}");

            var arguments = new Expression[paras.Length];
            for (var i = 0; i < arguments.Length; i++)
                arguments[i] = Expression.Convert(
                        parameters[i + 1],
                        paras[i].ParameterType
                    );
            return Expression.Lambda(delegateType, Expression.Call(Expression.Convert(instance, method.ReflectedType!), method, arguments), parameters).Compile();
        }

        public static T GetInstanceDelegate<T>(this MethodInfo method) where T : Delegate
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            return (T)method.GetInstanceDelegate(typeof(T));
        }

        public static Delegate GetInstanceDelegate(this MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            return method.GetInstanceDelegate(Expression.GetDelegateType(method.GetInstanceMethodTypes().ToArray()));
        }


        #endregion


    }
}
