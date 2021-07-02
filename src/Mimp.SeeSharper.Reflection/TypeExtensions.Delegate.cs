using System;
using System.Collections.Generic;
using System.Reflection;

namespace Mimp.SeeSharper.Reflection
{
    public static partial class TypeExtensions
    {


        /// <summary>
        /// Check if <paramref name="type"/> is a <see cref="Delegate"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsDelegate(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.IsAssignableTo(typeof(Delegate));
        }

        /// <summary>
        /// Check if <paramref name="type"/> is a <see cref="Delegate"/> with no return type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsAction(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.GetDelegateInvoke().IsAction();
        }

        /// <summary>
        /// Check if <paramref name="type"/> is a <see cref="Delegate"/> with a return type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsFunc(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.GetDelegateInvoke().IsFunc();
        }


        /// <summary>
        /// Return the invoke <see cref="MethodInfo"/> from the delegate.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If type isn't a delegate.</exception>
        public static MethodInfo GetDelegateInvoke(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (!type.IsDelegate())
                throw new ArgumentException($@"Type ""{type}"" have to be a {nameof(Delegate)}", nameof(type));

            return type.GetMethod("Invoke")!;
        }

        /// <summary>
        /// Return the parameter types of the delegate.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If type isn't a delegate.</exception>
        public static IEnumerable<Type> GetDelegateParameterTypes(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.GetDelegateInvoke().GetParameterTypes();
        }

        /// <summary>
        /// Return the return type of the delegate.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If type isn't a delegate.</exception>
        public static Type GetDelegateReturnType(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.GetDelegateInvoke().ReturnType;
        }

        /// <summary>
        /// Return the types of the delegate.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">If type isn't a delegate.</exception>
        public static IEnumerable<Type> GetDelegateTypes(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.GetDelegateInvoke().GetMethodTypes();
        }


    }
}
