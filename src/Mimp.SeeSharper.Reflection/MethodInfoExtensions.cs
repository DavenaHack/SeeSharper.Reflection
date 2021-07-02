using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Mimp.SeeSharper.Reflection
{
    public static class MethodInfoExtensions
    {


        /// <summary>
        /// Return all parameter types from method.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetParameterTypes(this MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            foreach (var p in method.GetParameters())
                yield return p.ParameterType;
        }

        /// <summary>
        /// Return all parameter types and the return type from method.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetMethodTypes(this MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            foreach (var t in method.GetParameterTypes())
                yield return t;
            yield return method.ReturnType;
        }

        /// <summary>
        /// Return the reflected type and all types from method.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetInstanceMethodTypes(this MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            yield return method.ReflectedType!;
            foreach (var t in method.GetMethodTypes())
                yield return t;
        }

        /// <summary>
        /// Check if method has no return type.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static bool IsAction(this MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            return method.ReturnType.IsVoid();
        }

        /// <summary>
        /// Check if method has a return type
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static bool IsFunc(this MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            return !method.ReturnType.IsVoid();
        }


        #region Delegate


        /// <summary>
        /// Return a delegate to invoke the method with the instance and a array of parameters.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="delegateType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate has no parameter for instance and the parameters.</exception>
        /// <exception cref="InvalidOperationException">If the return type or instance type isn't castable to the delegate types.</exception>
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

        /// <summary>
        /// Return a delegate to invoke the method with the instance and a array of parameters.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate has no parameter for instance and the parameters.</exception>
        /// <exception cref="InvalidOperationException">If the return type or instance type isn't castable to the delegate types.</exception>
        public static TDelegate GetInstanceParameterDelegate<TDelegate>(this MethodInfo method) where TDelegate : Delegate
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            return (TDelegate)method.GetInstanceParameterDelegate(typeof(TDelegate));
        }

        /// <summary>
        /// Return a delegate to invoke the method with the instance and a array of parameters.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Delegate GetInstanceParameterDelegate(this MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            return method.GetInstanceParameterDelegate(method.IsAction() ? typeof(Action<object, object?[]>) : typeof(Func<object, object?[], object?>));
        }

        /// <summary>
        /// Returns <see cref="InstanceAction"/> if the <paramref name="method"/> is a action 
        /// otherwise it will return a <see cref="InstanceFunc"/>.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Delegate GetInstanceParamsDelegate(this MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            return method.GetInstanceParameterDelegate(method.IsAction() ? typeof(InstanceAction) : typeof(InstanceFunc));
        }

        /// <summary>
        /// Return a function to invoke the method with the instance and a array of parameters.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If the method isn't function.</exception>
        public static Func<object, object?[], object?> GetInstanceParameterFunc(this MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            return method.GetInstanceParameterDelegate<Func<object, object?[], object?>>();
        }

        /// <summary>
        /// Return a delegate to invoke the method with the instance and a array of parameters.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If the method isn't function.</exception>
        public static InstanceFunc GetInstanceParamsFunc(this MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            return method.GetInstanceParameterDelegate<InstanceFunc>();
        }

        /// <summary>
        /// Return a action to invoke the method with the instance and a array of parameters.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If the method isn't action.</exception>
        public static Action<object, object?[]> GetInstanceParameterAction(this MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            return method.GetInstanceParameterDelegate<Action<object, object?[]>>();
        }

        /// <summary>
        /// Return a action to invoke the method with the instance and a array of parameters.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If the method isn't action.</exception>
        public static InstanceAction GetInstanceParamsAction(this MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            return method.GetInstanceParameterDelegate<InstanceAction>();
        }

        /// <summary>
        /// Return a delegate to invoke the method.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="delegateType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate has no parameter for instance or the delegate hasn't equal parameter as the method.</exception>
        /// <exception cref="InvalidOperationException">If the delegate types or instance type isn't castable to the method types.</exception>
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

        /// <summary>
        /// Return a delegate to invoke the method.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate has no parameter for instance or the delegate hasn't equal parameter as the method.</exception>
        /// <exception cref="InvalidOperationException">If the delegate types or instance type isn't castable to the method types.</exception>
        public static TDelegate GetInstanceDelegate<TDelegate>(this MethodInfo method) where TDelegate : Delegate
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            return (TDelegate)method.GetInstanceDelegate(typeof(TDelegate));
        }

        /// <summary>
        /// Return a delegate to invoke the method.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Delegate GetInstanceDelegate(this MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            return method.GetInstanceDelegate(Expression.GetDelegateType(method.GetInstanceMethodTypes().ToArray()));
        }


        #endregion


    }
}
