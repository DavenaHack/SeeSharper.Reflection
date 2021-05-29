using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mimp.SeeSharper.Reflection
{
    public static partial class TypeExtensions
    {


        #region GetMethods


        public static MethodInfo[] GetMethods(this Type type, string name, bool hasPublic, bool isStatic, bool ignoreCase, IEnumerable<Type>? genericParameters, IEnumerable<Type>? types)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            var genParams = genericParameters?.ToArray();
            var ts = types?.ToArray();

            var methods = new List<MethodInfo>();
            foreach (var m in type.GetRuntimeMethods())
            {
                var meth = m;

                if (isStatic != meth.IsStatic)
                    continue;

                if (!string.Equals(meth.Name, name, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture))
                {
                    var i = meth.Name.LastIndexOf('.');
                    if (i < 0)
                        continue;
                    var n = meth.Name.Substring(i + 1);
                    if (!string.Equals(n, name, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture))
                        continue;
                }

                if (hasPublic && !meth.IsPublic)
                {
                    var i = meth.Name.LastIndexOf('.');
                    if (i < 0)
                        continue;
                    if (!meth.Attributes.HasFlag(MethodAttributes.NewSlot)) // is interface public
                        continue;
                }

                if (genParams is null || genParams.Length == 0)
                {
                    if (meth.IsGenericMethod || meth.IsGenericMethodDefinition)
                        continue;
                }
                else
                {
                    if (meth.IsGenericMethod)
                    {
                        var gts = meth.GetGenericArguments();
                        if (gts.Length != genParams.Length)
                            continue;
                        var hasGens = true;
                        for (var i = 0; i < gts.Length; i++)
                        {
                            var t = genParams[i];
                            if (!t.IsNullOrVoid() && gts[i] != t)
                            {
                                hasGens = false;
                                break;
                            }
                        }
                        if (!hasGens)
                            continue;
                    }
                    else if (meth.IsGenericMethodDefinition)
                    {
                        if (meth.GetGenericArguments().Length != genParams.Length)
                            continue;
                        if (genParams.Any(t => !t.IsNullOrVoid())) // not generic definition
                            try
                            {
                                meth = meth.MakeGenericMethod(genParams);
                            }
                            catch
                            {
                                continue;
                            }
                    }
                    else
                    {
                        continue;
                    }
                }

                if (ts is not null)
                {
                    var mts = meth.GetMethodTypes().ToArray();
                    if (mts.Length != ts.Length)
                        continue;
                    var hasTypes = true;
                    for (var i = 0; i < ts.Length; i++)
                    {
                        var t = ts[i];
                        if (!t.IsNullOrVoid() && mts[i] != t)
                        {
                            hasTypes = false;
                            break;
                        }
                    }
                    if (!hasTypes)
                        continue;
                }

                methods.Add(m);
            }
            return methods.ToArray();
        }

        public static MethodInfo[] GetMethods(this Type type, string name, bool hasPublic, bool isStatic, bool ignoreCase, int genericParameters, IEnumerable<Type>? types)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetMethods(name, hasPublic, isStatic, ignoreCase, genericParameters < 0 ? null : new Type[genericParameters], types);
        }

        public static MethodInfo[] GetMethods(this Type type, string name, bool hasPublic, bool isStatic, bool ignoreCase, int genericParameters, int parameterTypes)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetMethods(name, hasPublic, isStatic, ignoreCase, genericParameters, parameterTypes < 0 ? null : new Type[parameterTypes + 1]);
        }

        public static MethodInfo[] GetMethods(this Type type, string name, bool hasPublic, bool isStatic, bool ignoreCase, IEnumerable<Type>? types)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetMethods(name, hasPublic, isStatic, ignoreCase, null, types);
        }

        public static MethodInfo[] GetMethods(this Type type, string name, bool hasPublic, bool isStatic, bool ignoreCase, int parameterTypes)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetMethods(name, hasPublic, isStatic, ignoreCase, null, parameterTypes < 0 ? null : new Type[parameterTypes + 1]);
        }


        #endregion GetMethods


        #region GetMethod


        public static MethodInfo GetMethod(this Type type, string name, bool hasPublic, bool isStatic, bool ignoreCase, IEnumerable<Type>? genericParameters, IEnumerable<Type>? types)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            var methods = type.GetMethods(name, hasPublic, isStatic, ignoreCase, genericParameters, types);
            if (methods is null || methods.Length == 0)
                throw new InvalidOperationException($@"Type ""{type}"" has no method ""{name}""");
            if (methods.Length != 1)
                throw new InvalidOperationException($@"Method ""{name}"" is ambiguous for ""{type}""");
            return methods[0];
        }

        public static MethodInfo GetMethod(this Type type, string name, bool hasPublic, bool isStatic, bool ignoreCase, int genericParameters, IEnumerable<Type>? types)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetMethod(name, hasPublic, isStatic, ignoreCase, genericParameters < 0 ? null : new Type[genericParameters], types);
        }

        public static MethodInfo GetMethod(this Type type, string name, bool hasPublic, bool isStatic, bool ignoreCase, int genericParameters, int parameterTypes)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetMethod(name, hasPublic, isStatic, ignoreCase, genericParameters, parameterTypes < 0 ? null : new Type[parameterTypes + 1]);
        }

        public static MethodInfo GetMethod(this Type type, string name, bool hasPublic, bool isStatic, bool ignoreCase, IEnumerable<Type>? types)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetMethod(name, hasPublic, isStatic, ignoreCase, null, types);
        }

        public static MethodInfo GetMethod(this Type type, string name, bool hasPublic, bool isStatic, bool ignoreCase, int parameterTypes)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetMethod(name, hasPublic, isStatic, ignoreCase, null, parameterTypes < 0 ? null : new Type[parameterTypes + 1]);
        }


        #region GetInstanceMethod


        public static MethodInfo GetInstanceMethod(this Type type, string name, bool ignoreCase, IEnumerable<Type>? genericParameters, IEnumerable<Type>? types)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetMethod(name, true, false, ignoreCase, genericParameters, types);
        }

        public static MethodInfo GetInstanceMethod(this Type type, string name, IEnumerable<Type>? genericParameters, IEnumerable<Type>? types)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetMethod(name, true, false, true, genericParameters, types);
        }

        public static MethodInfo GetInstanceMethod(this Type type, string name, bool ignoreCase, int genericParameters, IEnumerable<Type>? types)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetMethod(name, true, false, ignoreCase, genericParameters, types);
        }

        public static MethodInfo GetInstanceMethod(this Type type, string name, int genericParameters, IEnumerable<Type>? types)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetMethod(name, true, false, true, genericParameters, types);
        }

        public static MethodInfo GetInstanceMethod(this Type type, string name, bool ignoreCase, int genericParameters, int parameterTypes)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetMethod(name, true, false, ignoreCase, genericParameters, parameterTypes);
        }

        public static MethodInfo GetInstanceMethod(this Type type, string name, int genericParameters, int parameterTypes)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetMethod(name, true, false, true, genericParameters, parameterTypes);
        }

        public static MethodInfo GetInstanceMethod(this Type type, string name, bool ignoreCase, IEnumerable<Type>? types)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetMethod(name, true, false, ignoreCase, null, types);
        }

        public static MethodInfo GetInstanceMethod(this Type type, string name, IEnumerable<Type>? types)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetMethod(name, true, false, true, null, types);
        }

        public static MethodInfo GetInstanceMethod(this Type type, string name, bool ignoreCase, int parameterTypes)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetMethod(name, true, false, ignoreCase, parameterTypes);
        }

        public static MethodInfo GetInstanceMethod(this Type type, string name, int parameterTypes)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetMethod(name, true, false, true, parameterTypes);
        }

        public static MethodInfo GetInstanceMethod(this Type type, string name, bool ignoreCase)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetMethod(name, true, false, ignoreCase, null, null);
        }

        public static MethodInfo GetInstanceMethod(this Type type, string name)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetMethod(name, true, false, true, null, null);
        }


        #endregion GetInstanceMethod


        #region GetStaticMethod


        public static MethodInfo GetStaticMethod(this Type type, string name, bool ignoreCase, IEnumerable<Type>? genericParameters, IEnumerable<Type>? types)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetMethod(name, true, true, ignoreCase, genericParameters, types);
        }

        public static MethodInfo GetStaticMethod(this Type type, string name, IEnumerable<Type>? genericParameters, IEnumerable<Type>? types)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetMethod(name, true, true, true, genericParameters, types);
        }

        public static MethodInfo GetStaticMethod(this Type type, string name, bool ignoreCase, int genericParameters, IEnumerable<Type>? types)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetMethod(name, true, true, ignoreCase, genericParameters, types);
        }

        public static MethodInfo GetStaticMethod(this Type type, string name, int genericParameters, IEnumerable<Type>? types)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetMethod(name, true, true, true, genericParameters, types);
        }

        public static MethodInfo GetStaticMethod(this Type type, string name, bool ignoreCase, int genericParameters, int parameterTypes)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetMethod(name, true, true, ignoreCase, genericParameters, parameterTypes);
        }

        public static MethodInfo GetStaticMethod(this Type type, string name, int genericParameters, int parameterTypes)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetMethod(name, true, true, true, genericParameters, parameterTypes);
        }

        public static MethodInfo GetStaticMethod(this Type type, string name, bool ignoreCase, IEnumerable<Type>? types)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetMethod(name, true, true, ignoreCase, null, types);
        }

        public static MethodInfo GetStaticMethod(this Type type, string name, IEnumerable<Type>? types)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetMethod(name, true, true, true, null, types);
        }

        public static MethodInfo GetStaticMethod(this Type type, string name, bool ignoreCase, int parameterTypes)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetMethod(name, true, true, ignoreCase, parameterTypes);
        }

        public static MethodInfo GetStaticMethod(this Type type, string name, int parameterTypes)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetMethod(name, true, true, true, parameterTypes);
        }

        public static MethodInfo GetStaticMethod(this Type type, string name, bool ignoreCase)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetMethod(name, true, true, ignoreCase, null, null);
        }

        public static MethodInfo GetStaticMethod(this Type type, string name)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return type.GetMethod(name, true, true, true, null, null);
        }


        #endregion GetStaticMethod


        #endregion GetMethod


    }
}
