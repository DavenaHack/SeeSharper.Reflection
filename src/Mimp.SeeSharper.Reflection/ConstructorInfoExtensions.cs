using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Mimp.SeeSharper.Reflection
{
    public static class ConstructorInfoExtensions
    {


        public static IEnumerable<Type> GetParameterTypes(this ConstructorInfo constructor)
        {
            if (constructor is null)
                throw new ArgumentNullException(nameof(constructor));

            foreach (var p in constructor.GetParameters())
                yield return p.ParameterType;
        }

        public static IEnumerable<Type> GetConstructorTypes(this ConstructorInfo constructor)
        {
            if (constructor is null)
                throw new ArgumentNullException(nameof(constructor));

            foreach (var t in constructor.GetParameterTypes())
                yield return t;
            yield return constructor.ReflectedType!;
        }


        #region Delegate


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


        public static T GetParameterDelegate<T>(this ConstructorInfo constructor) where T : Delegate
        {
            if (constructor is null)
                throw new ArgumentNullException(nameof(constructor));

            return (T)constructor.GetParameterDelegate(typeof(T));
        }

        public static Func<object?[], object> GetParameterFunc(this ConstructorInfo constructor)
        {
            if (constructor is null)
                throw new ArgumentNullException(nameof(constructor));

            return constructor.GetParameterDelegate<Func<object?[], object>>();
        }

        public static ParamsFunc GetParamsFunc(this ConstructorInfo constructor)
        {
            if (constructor is null)
                throw new ArgumentNullException(nameof(constructor));

            return constructor.GetParameterDelegate<ParamsFunc>();
        }

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

        public static T GetDelegate<T>(this ConstructorInfo constructor) where T : Delegate
        {
            if (constructor is null)
                throw new ArgumentNullException(nameof(constructor));

            return (T)constructor.GetDelegate(typeof(T));
        }

        public static Delegate GetDelegate(this ConstructorInfo constructor)
        {
            if (constructor is null)
                throw new ArgumentNullException(nameof(constructor));

            return constructor.GetDelegate(Expression.GetDelegateType(constructor.GetConstructorTypes().ToArray()));
        }


        #endregion


    }

}
