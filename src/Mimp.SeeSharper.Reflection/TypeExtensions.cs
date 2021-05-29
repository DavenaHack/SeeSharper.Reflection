using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Mimp.SeeSharper.Reflection
{
    public static partial class TypeExtensions
    {


        #region Void


        public static bool IsVoid(this Type type) =>
            type == typeof(void);

        public static bool IsNullOrVoid(this Type? type) =>
            type is null || type.IsVoid();


        #endregion


        #region Bool


        public static bool IsBool(this Type type) =>
            type == typeof(bool);


        #endregion Bool


        #region Number


        public static bool IsByte(this Type type) =>
            type == typeof(byte);
        public static bool IsSByte(this Type type) =>
            type == typeof(sbyte);
        public static bool IsShort(this Type type) =>
            type == typeof(short);
        public static bool IsUShort(this Type type) =>
            type == typeof(ushort);
        public static bool IsInt(this Type type) =>
            type == typeof(int);
        public static bool IsUInt(this Type type) =>
            type == typeof(uint);
        public static bool IsLong(this Type type) =>
            type == typeof(long);
        public static bool IsULong(this Type type) =>
            type == typeof(ulong);
        public static bool IsBigInteger(this Type type) =>
            type == typeof(BigInteger);


        public static bool IsInteger(this Type type) =>
               type.IsByte()
            || type.IsSByte()
            || type.IsShort()
            || type.IsUShort()
            || type.IsInt()
            || type.IsUInt()
            || type.IsLong()
            || type.IsULong()
            || type.IsBigInteger();


        public static bool IsFloat(this Type type) =>
            type == typeof(float);
        public static bool IsDouble(this Type type) =>
            type == typeof(double);
        public static bool IsDecimal(this Type type) =>
            type == typeof(decimal);

        public static bool IsFloatingNumber(this Type type) =>
               type.IsFloat()
            || type.IsDouble()
            || type.IsDecimal();


        public static bool IsNumber(this Type type) =>
            type.IsInteger() || type.IsFloatingNumber();


        #endregion Number


        #region Char


        public static bool IsChar(this Type type) =>
            type == typeof(char);


        #endregion Char


        #region String


        public static bool IsString(this Type type) =>
            type == typeof(string);


        #endregion


        #region Time


        public static bool IsTimeSpan(this Type type) =>
            type == typeof(TimeSpan);


        public static bool IsDateTime(this Type type) =>
            type == typeof(DateTime);


        #endregion


        #region Nullable


        public static bool IsNullable(this Type type) =>
            type.Inherit(typeof(Nullable<>));

        public static bool IsNullable(this Type type, Type valueType) =>
            type.InheritGenericDefinition(typeof(Nullable<>), new[] { valueType ?? throw new ArgumentNullException(nameof(valueType)) });

        public static Type? GetNullableValueType(this Type type)
        {
            var types = type.GetGenericArgumentsRequired(typeof(Nullable<>));
            return types.Any() ? types.First() : null;
        }


        #endregion


        #region IEnumerable


        public static bool IsIEnumerable(this Type type) =>
            type.Inherit(typeof(IEnumerable));

        public static bool IsIEnumerable(this Type type, Type valueType) =>
            type.InheritGenericDefinition(typeof(IEnumerable<>), new[] { valueType });

        public static Type GetIEnumerableType(this Type type)
        {
            try
            {
                return type.GetGenericType(typeof(IEnumerable<>));
            }
            catch
            {
                if (type.IsIEnumerable())
                    return typeof(IEnumerable);
                throw;
            }
        }

        public static Type? GetIEnumerableValueType(this Type type)
        {
            try
            {
                var types = type.GetGenericArgumentsRequired(typeof(IEnumerable<>));
                return types.Any() ? types.First() : null;
            }
            catch
            {
                if (type.IsIEnumerable())
                    return typeof(object);
                throw;
            }
        }


        public static bool IsIEnumerator(this Type type) =>
            type.Inherit(typeof(IEnumerator));

        public static bool IsIEnumerator(this Type type, Type valueType) =>
            type.InheritGenericDefinition(typeof(IEnumerator<>), new[] { valueType });

        public static Type GetIEnumeratorType(this Type type)
        {
            try
            {
                return type.GetGenericType(typeof(IEnumerator<>));
            }
            catch
            {
                if (type.IsIEnumerator())
                    return typeof(IEnumerator);
                throw;
            }
        }

        public static Type? GetIEnumeratorValueType(this Type type)
        {
            try
            {
                var types = type.GetGenericArgumentsRequired(typeof(IEnumerator<>));
                return types.Any() ? types.First() : null;
            }
            catch
            {
                if (type.Inherit(typeof(IEnumerator)))
                    return typeof(object);
                throw;
            }
        }


        #region Async
#if NET || NETSTANDARD2_1 || NETCOREAPP3_1


        public static bool IsIAsyncEnumerable(this Type type) =>
            type.Inherit(typeof(IAsyncEnumerable<>));

        public static bool IsIAsyncEnumerable(this Type type, Type valueType) =>
            type.InheritGenericDefinition(typeof(IAsyncEnumerable<>), new[] { valueType });

        public static Type GetIAsyncEnumerableType(this Type type)
        {
            return type.GetGenericType(typeof(IAsyncEnumerable<>));
        }

        public static Type? GetIAsyncEnumerableValueType(this Type type)
        {
            var types = type.GetGenericArgumentsRequired(typeof(IAsyncEnumerable<>));
            return types.Any() ? types.First() : null;
        }


        public static bool IsIAsyncEnumerator(this Type type) =>
            type.Inherit(typeof(IAsyncEnumerator<>));

        public static bool IsIAsyncEnumerator(this Type type, Type valueType) =>
            type.InheritGenericDefinition(typeof(IAsyncEnumerator<>), new[] { valueType });

        public static Type? GetIAsyncEnumeratorType(this Type type)
        {
            return type.GetGenericType(typeof(IAsyncEnumerator<>));
        }

        public static Type? GetIAsyncEnumeratorValueType(this Type type)
        {
            var types = type.GetGenericArgumentsRequired(typeof(IAsyncEnumerator<>));
            return types.Any() ? types.First() : null;
        }


#endif
        #endregion Async


        #endregion IEnumerable


        #region ICollection


        public static bool IsICollection(this Type type) =>
            type.Inherit(typeof(ICollection));

        public static bool IsICollection(this Type type, Type valueType) =>
            type.InheritGenericDefinition(typeof(ICollection<>), new[] { valueType });

        public static Type GetICollectionType(this Type type)
        {
            try
            {
                return type.GetGenericType(typeof(ICollection<>));
            }
            catch
            {
                if (type.IsICollection())
                    return typeof(ICollection);
                throw;
            }
        }

        public static Type? GetICollectionValueType(this Type type)
        {
            try
            {
                var types = type.GetGenericArgumentsRequired(typeof(ICollection<>));
                return types.Any() ? types.First() : null;
            }
            catch
            {
                if (type.IsICollection())
                    return typeof(object);
                throw;
            }
        }


        #endregion ICollection


        #region ISet


        public static bool IsISet(this Type type) =>
            type.Inherit(typeof(ISet<>));

        public static bool IsISet(this Type type, Type valueType) =>
            type.InheritGenericDefinition(typeof(ISet<>), new[] { valueType });

        public static Type GetISetType(this Type type)
        {
            return type.GetGenericType(typeof(ISet<>));
        }

        public static Type? GetISetValueType(this Type type)
        {
            var types = type.GetGenericArgumentsRequired(typeof(ISet<>));
            return types.Any() ? types.First() : null;
        }


        #endregion ISet


        #region IList


        public static bool IsIList(this Type type) =>
            type.Inherit(typeof(IList));

        public static bool IsIList(this Type type, Type valueType) =>
            type.InheritGenericDefinition(typeof(IList<>), new[] { valueType });

        public static Type GetIListType(this Type type)
        {
            try
            {
                return type.GetGenericType(typeof(IList<>));
            }
            catch
            {
                if (type.IsIList())
                    return typeof(IList);
                throw;
            }
        }

        public static Type? GetIListValueType(this Type type)
        {
            try
            {
                var types = type.GetGenericArgumentsRequired(typeof(IList<>));
                return types.Any() ? types.First() : null;
            }
            catch
            {
                if (type.IsIList())
                    return typeof(object);
                throw;
            }
        }


        #endregion IList


        #region IDictionary


        public static bool IsIDictionary(this Type type) =>
            type.Inherit(typeof(IDictionary));

        public static bool IsIDictionary(this Type type, Type keyType, Type valueType) =>
            type.InheritGenericDefinition(typeof(IDictionary<,>), new[] { keyType, valueType });

        public static Type GetIDictionaryType(this Type type)
        {
            try
            {
                return type.GetGenericType(typeof(IDictionary<,>));
            }
            catch
            {
                if (type.IsIDictionary())
                    return typeof(IDictionary);
                throw;
            }
        }

        public static IEnumerable<Type>? GetIDictionaryKeyValueType(this Type type)
        {
            try
            {
                var types = type.GetGenericArgumentsRequired(typeof(IDictionary<,>));
                return types.Any() ? types : null ;
            }
            catch
            {
                if (type.IsIDictionary())
                    return new[] { typeof(object), typeof(object) };
                throw;
            }
        }

        public static KeyValuePair<Type, Type>? GetIDictionaryPairType(this Type type)
        {
            var types = type.GetIDictionaryKeyValueType()?.ToArray();
            return types is null ? null : new KeyValuePair<Type, Type>(types[0], types[1]);
        }


        #endregion IDictionary


        #region KeyValuePair


        public static bool IsKeyValuePair(this Type type) =>
            type.Inherit(typeof(KeyValuePair<,>));

        public static bool IsKeyValuePair(this Type type, Type keyType, Type valueType) =>
            type.InheritGenericDefinition(typeof(KeyValuePair<,>), new[] { keyType, valueType });

        public static Type GetKeyValuePairType(this Type type)
        {
            return type.GetGenericType(typeof(KeyValuePair<,>));
        }

        public static IEnumerable<Type>? GetKeyValuePairKeyValueType(this Type type)
        {
            var types = type.GetGenericArgumentsRequired(typeof(KeyValuePair<,>));
            return types.Any() ? types : null ;
        }

        public static KeyValuePair<Type, Type>? GetKeyValuePairPairType(this Type type)
        {
            var types = type.GetKeyValuePairKeyValueType()?.ToArray();
            return types is null ? null : new KeyValuePair<Type, Type>(types[0], types[1]);
        }


        #endregion KeyValuePair


    }
}
