using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Mimp.SeeSharper.Reflection
{
    public static partial class TypeExtensions
    {


        public static ConstructorInfo GetConstructorRequired(this Type type, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, IEnumerable<Type> types, IEnumerable<ParameterModifier> modifiers)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (binder is null)
                throw new ArgumentNullException(nameof(binder));
            if (types is null)
                throw new ArgumentNullException(nameof(types));
            if (types.Any(t => t is null))
                throw new ArgumentNullException(nameof(types), "At least one type is null.");
            if (modifiers is null)
                throw new ArgumentNullException(nameof(modifiers));

            return type.GetConstructor(bindingAttr, binder, callConvention, types.ToArray(), modifiers.ToArray()) ?? throw new InvalidOperationException("No matching constructor found");
        }

        public static ConstructorInfo GetConstructorRequired(this Type type, BindingFlags bindingAttr, Binder binder, IEnumerable<Type> types, IEnumerable<ParameterModifier> modifiers)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (binder is null)
                throw new ArgumentNullException(nameof(binder));
            if (types is null)
                throw new ArgumentNullException(nameof(types));
            if (types.Any(t => t is null))
                throw new ArgumentNullException(nameof(types), "At least one type is null.");
            if (modifiers is null)
                throw new ArgumentNullException(nameof(modifiers));

            return type.GetConstructor(bindingAttr, binder, types.ToArray(), modifiers.ToArray()) ?? throw new InvalidOperationException("No matching constructor found");
        }

        public static ConstructorInfo GetConstructorRequired(this Type type, IEnumerable<Type> types)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (types is null)
                throw new ArgumentNullException(nameof(types));
            if (types.Any(t => t is null))
                throw new ArgumentNullException(nameof(types), "At least one type is null.");

            return type.GetConstructor(types.ToArray()) ?? throw new InvalidOperationException("No matching constructor found");
        }

        public static ConstructorInfo GetConstructorRequired(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.GetConstructorRequired(Type.EmptyTypes);
        }




        public static Delegate GetNewDelegate(this Type type, Type delegateType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            return type.GetConstructorRequired(delegateType.GetDelegateParameterTypes()).GetDelegate(delegateType);
        }

        public static T GetNewDelegate<T>(this Type type) where T : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return (T)type.GetNewDelegate(typeof(T));
        }

        public static Func<object> GetNewFunc(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.GetNewDelegate<Func<object>>();
        }

        public static Delegate GetNewParameterDelegate(this Type type, IEnumerable<Type> types, Type delegateType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (types is null)
                throw new ArgumentNullException(nameof(types));
            if (types.Any(t => t is null))
                throw new ArgumentNullException(nameof(types), "At least on type is null");
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));

            return type.GetConstructorRequired(types).GetParameterDelegate(delegateType);
        }

        public static Delegate GetNewParameterDelegate(this Type type, IEnumerable<Type> types)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (types is null)
                throw new ArgumentNullException(nameof(types));
            if (types.Any(t => t is null))
                throw new ArgumentNullException(nameof(types), "At least on type is null");

            return type.GetNewParameterDelegate(types, Expression.GetFuncType(types.Concat(new[] { type }).ToArray()));
        }

        public static T GetNewParameterDelegate<T>(this Type type, IEnumerable<Type> types) where T : Delegate
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (types is null)
                throw new ArgumentNullException(nameof(types));
            if (types.Any(t => t is null))
                throw new ArgumentNullException(nameof(types), "At least on type is null");

            return (T)type.GetNewParameterDelegate(types, typeof(T));
        }

        public static Func<object?[], object> GetNewParameterFunc(this Type type, IEnumerable<Type> types)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (types is null)
                throw new ArgumentNullException(nameof(types));
            if (types.Any(t => t is null))
                throw new ArgumentNullException(nameof(types), "At least on type is null");

            return type.GetNewParameterDelegate<Func<object?[], object>>(types);
        }

        public static ParamsFunc GetNewParamsFunc(this Type type, IEnumerable<Type> types)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (types is null)
                throw new ArgumentNullException(nameof(types));
            if (types.Any(t => t is null))
                throw new ArgumentNullException(nameof(types), "At least on type is null");

            return type.GetNewParameterDelegate<ParamsFunc>(types);
        }

        public static object New(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.GetNewFunc()();
        }

        public static object New(this Type type, IEnumerable<Type> types, object?[] values)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (types is null)
                throw new ArgumentNullException(nameof(types));
            if (types.Any(t => t is null))
                throw new ArgumentNullException(nameof(types), "At least on type is null");

            return type.GetNewParameterFunc(types)(values ?? Array.Empty<object?>());
        }


    }
}
