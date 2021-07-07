using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Numerics;

namespace Mimp.SeeSharper.Reflection
{
    public static partial class TypeExtensions
    {


        #region Void


        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="void"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsVoid(this Type type) =>
            type == typeof(void);

        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="void"/> or <see cref="null"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullOrVoid(this Type? type) =>
            type is null || type.IsVoid();


        #endregion


        #region Bool

        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="bool"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsBool(this Type type) =>
            type == typeof(bool);


        #endregion Bool


        #region Number

        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="byte"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsByte(this Type type) =>
            type == typeof(byte);

        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="sbyte"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSByte(this Type type) =>
            type == typeof(sbyte);

        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="short"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsShort(this Type type) =>
            type == typeof(short);

        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="ushort"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsUShort(this Type type) =>
            type == typeof(ushort);

        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="int"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsInt(this Type type) =>
            type == typeof(int);

        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="uint"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsUInt(this Type type) =>
            type == typeof(uint);

        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="long"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsLong(this Type type) =>
            type == typeof(long);

        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="ulong"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsULong(this Type type) =>
            type == typeof(ulong);

        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="BigInteger"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsBigInteger(this Type type) =>
            type == typeof(BigInteger);

        /// <summary>
        /// Check if <paramref name="type"/> is a integer.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <seealso cref="IsByte(Type)"/>
        /// <seealso cref="IsSByte(Type)"/>
        /// <seealso cref="IsShort(Type)"/>
        /// <seealso cref="IsUShort(Type)"/>
        /// <seealso cref="IsInt(Type)"/>
        /// <seealso cref="IsUInt(Type)"/>
        /// <seealso cref="IsLong(Type)"/>
        /// <seealso cref="IsULong(Type)"/>
        /// <seealso cref="IsBigInteger(Type)(Type)"/>
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


        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="float"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsFloat(this Type type) =>
            type == typeof(float);

        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="double"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsDouble(this Type type) =>
            type == typeof(double);

        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="decimal"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsDecimal(this Type type) =>
            type == typeof(decimal);

        /// <summary>
        /// Check if <paramref name="type"/> is a floating number.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <seealso cref="IsFloat(Type)"/>
        /// <seealso cref="IsDouble(Type)"/>
        /// <seealso cref="IsDecimal(Type)"/>
        public static bool IsFloatingNumber(this Type type) =>
               type.IsFloat()
            || type.IsDouble()
            || type.IsDecimal();


        /// <summary>
        /// Check if <paramref name="type"/> is a number.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNumber(this Type type) =>
            type.IsInteger() || type.IsFloatingNumber();


        #endregion Number


        #region Char


        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="char"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsChar(this Type type) =>
            type == typeof(char);


        #endregion Char


        #region String


        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="string"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsString(this Type type) =>
            type == typeof(string);


        #endregion


        #region Time


        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsTimeSpan(this Type type) =>
            type == typeof(TimeSpan);


        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="DateTime"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsDateTime(this Type type) =>
            type == typeof(DateTime);


        #endregion


        #region Nullable


        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="Nullable{T}"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsNullable(this Type type) =>
            type.Inherit(typeof(Nullable<>));

        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="Nullable{T}"/> with generic <paramref name="valueType"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsNullable(this Type type, Type valueType) =>
            type.InheritGenericDefinition(typeof(Nullable<>), new[] { valueType ?? throw new ArgumentNullException(nameof(valueType)) });

        /// <summary>
        /// Return the generic type of <see cref="Nullable{T}"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If type isn't <see cref="Nullable{T}"/></exception>
        public static Type? GetNullableValueType(this Type type)
        {
            var types = type.GetGenericArguments(typeof(Nullable<>));
            return types.Any() ? types.First() : null;
        }


        #endregion


        #region IEnumerable


        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="IEnumerable"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsIEnumerable(this Type type) =>
            type.Inherit(typeof(IEnumerable));

        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="IEnumerable{T}"/> with generic <paramref name="valueType"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsIEnumerable(this Type type, Type valueType) =>
            type.InheritGenericDefinition(typeof(IEnumerable<>), new[] { valueType });

        /// <summary>
        /// Return the inherit <see cref="IEnumerable"/> type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If type isnt' <see cref="IEnumerable"/>.</exception>
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

        /// <summary>
        /// Return the value type of <see cref="IEnumerable"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If type isn't <see cref="IEnumerable"/>.</exception>
        public static Type? GetIEnumerableValueType(this Type type)
        {
            try
            {
                var types = type.GetGenericArguments(typeof(IEnumerable<>));
                return types.Any() ? types.First() : null;
            }
            catch
            {
                if (type.IsIEnumerable())
                    return typeof(object);
                throw;
            }
        }


        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="IEnumerator"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsIEnumerator(this Type type) =>
            type.Inherit(typeof(IEnumerator));

        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="IEnumerator{T}"/> with generic <paramref name="valueType"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsIEnumerator(this Type type, Type valueType) =>
            type.InheritGenericDefinition(typeof(IEnumerator<>), new[] { valueType });

        /// <summary>
        /// Return the inherit <see cref="IEnumerator"/> type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If type isnt' <see cref="IEnumerator"/>.</exception>
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

        /// <summary>
        /// Return the value type of <see cref="IEnumerator"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If type isn't <see cref="IEnumerator"/>.</exception>
        public static Type? GetIEnumeratorValueType(this Type type)
        {
            try
            {
                var types = type.GetGenericArguments(typeof(IEnumerator<>));
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


        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsIAsyncEnumerable(this Type type) =>
            type.Inherit(typeof(IAsyncEnumerable<>));

        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsIAsyncEnumerable(this Type type, Type valueType) =>
            type.InheritGenericDefinition(typeof(IAsyncEnumerable<>), new[] { valueType });

        /// <summary>
        /// Return the inherit <see cref="IAsyncEnumerable{T}"/> type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If type isnt' <see cref="IAsyncEnumerable{T}"/>.</exception>
        public static Type GetIAsyncEnumerableType(this Type type)
        {
            return type.GetGenericType(typeof(IAsyncEnumerable<>));
        }

        /// <summary>
        /// Return the value type of <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If type isn't <see cref="IAsyncEnumerable{T}"/>.</exception>
        public static Type? GetIAsyncEnumerableValueType(this Type type)
        {
            var types = type.GetGenericArguments(typeof(IAsyncEnumerable<>));
            return types.Any() ? types.First() : null;
        }


        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="IAsyncEnumerator{T}"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsIAsyncEnumerator(this Type type) =>
            type.Inherit(typeof(IAsyncEnumerator<>));

        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="IAsyncEnumerator{T}"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsIAsyncEnumerator(this Type type, Type valueType) =>
            type.InheritGenericDefinition(typeof(IAsyncEnumerator<>), new[] { valueType });

        /// <summary>
        /// Return the inherit <see cref="IAsyncEnumerator{T}"/> type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If type isnt' <see cref="IAsyncEnumerator{T}"/>.</exception>
        public static Type? GetIAsyncEnumeratorType(this Type type)
        {
            return type.GetGenericType(typeof(IAsyncEnumerator<>));
        }

        /// <summary>
        /// Return the value type of <see cref="IAsyncEnumerator{T}"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If type isn't <see cref="IAsyncEnumerator{T}"/>.</exception>
        public static Type? GetIAsyncEnumeratorValueType(this Type type)
        {
            var types = type.GetGenericArguments(typeof(IAsyncEnumerator<>));
            return types.Any() ? types.First() : null;
        }


#endif
        #endregion Async


        #endregion IEnumerable


        #region ICollection


        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="ICollection"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsICollection(this Type type) =>
            type.Inherit(typeof(ICollection));

        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="ICollection{T}"/> with generic <paramref name="valueType"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsICollection(this Type type, Type valueType) =>
            type.InheritGenericDefinition(typeof(ICollection<>), new[] { valueType });

        /// <summary>
        /// Return the inherit <see cref="ICollection"/> type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If type isnt' <see cref="ICollection"/>.</exception>
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

        /// <summary>
        /// Return the value type of <see cref="ICollection"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If type isn't <see cref="ICollection"/>.</exception>
        public static Type? GetICollectionValueType(this Type type)
        {
            try
            {
                var types = type.GetGenericArguments(typeof(ICollection<>));
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


        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="ISet{T}"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsISet(this Type type) =>
            type.Inherit(typeof(ISet<>));

        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="ISet{T}"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsISet(this Type type, Type valueType) =>
            type.InheritGenericDefinition(typeof(ISet<>), new[] { valueType });

        /// <summary>
        /// Return the inherit <see cref="ISet{T}"/> type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If type isnt' <see cref="ISet{T}"/>.</exception>
        public static Type GetISetType(this Type type)
        {
            return type.GetGenericType(typeof(ISet<>));
        }

        /// <summary>
        /// Return the value type of <see cref="ISet{T}"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If type isn't <see cref="ISet{T}"/>.</exception>
        public static Type? GetISetValueType(this Type type)
        {
            var types = type.GetGenericArguments(typeof(ISet<>));
            return types.Any() ? types.First() : null;
        }


        #endregion ISet


        #region IList


        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="IList"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsIList(this Type type) =>
            type.Inherit(typeof(IList));

        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="IList{T}"/> with generic <paramref name="valueType"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsIList(this Type type, Type valueType) =>
            type.InheritGenericDefinition(typeof(IList<>), new[] { valueType });

        /// <summary>
        /// Return the inherit <see cref="IList"/> type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If type isnt' <see cref="IList"/>.</exception>
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

        /// <summary>
        /// Return the value type of <see cref="IList"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If type isn't <see cref="IList"/>.</exception>
        public static Type? GetIListValueType(this Type type)
        {
            try
            {
                var types = type.GetGenericArguments(typeof(IList<>));
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


        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="IDictionary"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsIDictionary(this Type type) =>
            type.Inherit(typeof(IDictionary));

        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="IDictionary{T,V}"/> with generic <paramref name="keyType"/> and <paramref name="valueType"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsIDictionary(this Type type, Type keyType, Type valueType) =>
            type.InheritGenericDefinition(typeof(IDictionary<,>), new[] { keyType, valueType });

        /// <summary>
        /// Return the inherit <see cref="IDictionary"/> type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If type isnt' <see cref="IDictionary"/>.</exception>
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

        /// <summary>
        /// Return the value types of <see cref="IDictionary"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If type isn't <see cref="IDictionary"/>.</exception>
        public static IEnumerable<Type>? GetIDictionaryKeyValueType(this Type type)
        {
            try
            {
                var types = type.GetGenericArguments(typeof(IDictionary<,>));
                return types.Any() ? types : null;
            }
            catch
            {
                if (type.IsIDictionary())
                    return new[] { typeof(object), typeof(object) };
                throw;
            }
        }

        /// <summary>
        /// Return the value types of <see cref="IDictionary"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If type isn't <see cref="IDictionary"/>.</exception>
        public static KeyValuePair<Type, Type>? GetIDictionaryPairType(this Type type)
        {
            var types = type.GetIDictionaryKeyValueType()?.ToArray();
            return types is null ? null : new KeyValuePair<Type, Type>(types[0], types[1]);
        }


        #endregion IDictionary


        #region KeyValuePair


        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="KeyValuePair{K,V}"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsKeyValuePair(this Type type) =>
            type.Inherit(typeof(KeyValuePair<,>));

        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="KeyValuePair{K,V}"/> with generic <paramref name="keyType"/> and <paramref name="valueType"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsKeyValuePair(this Type type, Type keyType, Type valueType) =>
            type.InheritGenericDefinition(typeof(KeyValuePair<,>), new[] { keyType, valueType });

        /// <summary>
        /// Return the inherit <see cref="KeyValuePair{K,V}"/> type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If type isnt' <see cref="KeyValuePair{K,V}"/>.</exception>
        public static Type GetKeyValuePairType(this Type type)
        {
            return type.GetGenericType(typeof(KeyValuePair<,>));
        }

        /// <summary>
        /// Return the value types of <see cref="KeyValuePair{K,V}"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If type isn't <see cref="KeyValuePair{K,V}"/>.</exception>
        public static IEnumerable<Type>? GetKeyValuePairKeyValueType(this Type type)
        {
            var types = type.GetGenericArguments(typeof(KeyValuePair<,>));
            return types.Any() ? types : null;
        }

        /// <summary>
        /// Return the value types of <see cref="KeyValuePair{K,V}"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If type isn't <see cref="IDictionary"/>.</exception>
        public static KeyValuePair<Type, Type>? GetKeyValuePairPairType(this Type type)
        {
            var types = type.GetKeyValuePairKeyValueType()?.ToArray();
            return types is null ? null : new KeyValuePair<Type, Type>(types[0], types[1]);
        }


        #endregion KeyValuePair


        #region Dynamic


        /// <summary>
        /// Check if <paramref name="type"/> is <see cref="IDynamicMetaObjectProvider"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsDynamic(this Type type) =>
            type.IsAssignableTo(typeof(IDynamicMetaObjectProvider));


        #endregion

    }
}
