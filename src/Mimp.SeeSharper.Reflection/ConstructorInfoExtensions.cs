using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Mimp.SeeSharper.Reflection
{
    public static class ConstructorInfoExtensions
    {


        /// <summary>
        /// Return all parameter types from consturctor.
        /// </summary>
        /// <param name="constructor"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<Type> GetParameterTypes(this ConstructorInfo constructor)
        {
            if (constructor is null)
                throw new ArgumentNullException(nameof(constructor));

            foreach (var p in constructor.GetParameters())
                yield return p.ParameterType;
        }

        /// <summary>
        /// Return all parameter types and reflected type from constructor.
        /// </summary>
        /// <param name="constructor"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<Type> GetConstructorTypes(this ConstructorInfo constructor)
        {
            if (constructor is null)
                throw new ArgumentNullException(nameof(constructor));

            foreach (var t in constructor.GetParameterTypes())
                yield return t;
            yield return constructor.ReflectedType!;
        }


        #region Delegate


        /// <summary>
        /// Return a compiled delegate to invoke the constructor with a array of the parameters.
        /// </summary>
        /// <param name="constructor"></param>
        /// <param name="delegateType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate hasn't one parameter with a array as type.</exception>
        /// <exception cref="InvalidOperationException">If the return type of the delegate isn't castable to the constructor reflected type.</exception>
        public static Delegate GetParameterDelegate(this ConstructorInfo constructor, Type delegateType)
        {
            if (constructor is null)
                throw new ArgumentNullException(nameof(constructor));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            var delegateParameterTypes = delegateType.GetDelegateParameterTypes().ToArray();
            if (delegateParameterTypes.Length != 1)
                throw new ArgumentException($@"Delegate ""{delegateType}"" has to have exact one parameter for the parameters", nameof(delegateType));
            var parametersType = delegateParameterTypes[0];
            if (!parametersType.IsArray)
                throw new ArgumentException($@"The type for the parameters has to be an array", nameof(delegateType));

            var parameters = Expression.Parameter(parametersType, "args");
            var paras = constructor.GetParameters();
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
                Expression.Convert(Expression.New(constructor, arguments), delegateType.GetDelegateReturnType())
            ), parameters).Compile();
        }

        /// <summary>
        /// Return a compiled delegate to invoke the constructor with a array of the parameters.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="constructor"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate hasn't one parameter with a array as type.</exception>
        /// <exception cref="InvalidOperationException">If the return type of the delegate isn't castable to the constructor reflected type.</exception>
        public static TDelegate GetParameterDelegate<TDelegate>(this ConstructorInfo constructor) where TDelegate : Delegate
        {
            if (constructor is null)
                throw new ArgumentNullException(nameof(constructor));

            return (TDelegate)constructor.GetParameterDelegate(typeof(TDelegate));
        }

        /// <summary>
        /// Return a compiled function to invoke the constructor with a array of the parameters.
        /// </summary>
        /// <param name="constructor"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Func<object?[], object> GetParameterFunc(this ConstructorInfo constructor)
        {
            if (constructor is null)
                throw new ArgumentNullException(nameof(constructor));

            return constructor.GetParameterDelegate<Func<object?[], object>>();
        }

        /// <summary>
        /// Return a compiled function to invoke the constructor with a array of the parameters.
        /// </summary>
        /// <param name="constructor"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static ParamsFunc GetParamsFunc(this ConstructorInfo constructor)
        {
            if (constructor is null)
                throw new ArgumentNullException(nameof(constructor));

            return constructor.GetParameterDelegate<ParamsFunc>();
        }


        /// <summary>
        /// Return a compiled delegate to invoke the constructor.
        /// </summary>
        /// <param name="constructor"></param>
        /// <param name="delegateType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate hasn't equal parameter as the constructor.</exception>
        /// <exception cref="InvalidOperationException">If one of the parameter types or the return type of the delegate isn't castable to the constructor types.</exception>
        public static Delegate GetDelegate(this ConstructorInfo constructor, Type delegateType)
        {
            if (constructor is null)
                throw new ArgumentNullException(nameof(constructor));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            var delegateParameterTypes = delegateType.GetDelegateParameterTypes().ToArray();

            var paras = constructor.GetParameters().ToArray();
            if (delegateParameterTypes.Length != paras.Length)
                throw new ArgumentException($@"Delegate ""{delegateType}"" has an incorrect number of parameters", nameof(delegateType));

            var parameters = new ParameterExpression[delegateParameterTypes.Length];
            for (var i = 0; i < parameters.Length; i++)
                parameters[i] = Expression.Parameter(delegateParameterTypes[i], $"arg{i}");

            var arguments = new Expression[paras.Length];
            for (var i = 0; i < arguments.Length; i++)
                arguments[i] = Expression.Convert(
                        parameters[i],
                        paras[i].ParameterType
                    );

            return Expression.Lambda(delegateType, Expression.New(constructor, arguments), parameters).Compile();
        }

        /// <summary>
        /// Return a compiled delegate to invoke the constructor.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="constructor"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If the delegate hasn't equal parameter as the constructor.</exception>
        /// <exception cref="InvalidOperationException">If one of the parameter types or the return type of the delegate isn't castable to the constructor types.</exception>
        public static TDelegate GetDelegate<TDelegate>(this ConstructorInfo constructor) where TDelegate : Delegate
        {
            if (constructor is null)
                throw new ArgumentNullException(nameof(constructor));

            return (TDelegate)constructor.GetDelegate(typeof(TDelegate));
        }


        /// <summary>
        /// Return a compiled delegate to invoke the constructor with the structor like the constructor.
        /// </summary>
        /// <param name="constructor"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Delegate GetDelegate(this ConstructorInfo constructor)
        {
            if (constructor is null)
                throw new ArgumentNullException(nameof(constructor));

            return constructor.GetDelegate(Expression.GetDelegateType(constructor.GetConstructorTypes().ToArray()));
        }


        #endregion


    }

}
