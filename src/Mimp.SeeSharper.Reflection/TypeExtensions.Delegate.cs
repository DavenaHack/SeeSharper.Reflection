using System;
using System.Collections.Generic;
using System.Reflection;

namespace Mimp.SeeSharper.Reflection
{
    public static partial class TypeExtensions
    {


        public static bool IsDelegate(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.Inherit(typeof(Delegate));
        }

        public static bool IsAction(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.GetDelegateInvoke().IsAction();
        }

        public static bool IsFunc(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.GetDelegateInvoke().IsFunc();
        }


        public static MethodInfo GetDelegateInvoke(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (!type.IsDelegate())
                throw new ArgumentException($@"Type ""{type}"" have to be a {nameof(Delegate)}", nameof(type));

            return type.GetMethod("Invoke")!;
        }

        public static IEnumerable<Type> GetDelegateParameterTypes(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.GetDelegateInvoke().GetParameterTypes();
        }

        public static Type GetDelegateReturnType(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.GetDelegateInvoke().ReturnType;
        }

        public static IEnumerable<Type> GetDelegateTypes(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.GetDelegateInvoke().GetMethodTypes();
        }


    }
}
