using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
#if NullableAttributes
using System.Diagnostics.CodeAnalysis;
#endif

namespace Mimp.SeeSharper.Reflection.Dynamic
{
    public sealed class DictionaryDynamicObject : IDictionary<string, object?>, IDynamicMetaObjectProvider, INotifyPropertyChanging, INotifyPropertyChanged
    {

        private event PropertyChangingEventHandler? PropertyChanging;
        event PropertyChangingEventHandler? INotifyPropertyChanging.PropertyChanging
        {
            add => PropertyChanging += value;
            remove => PropertyChanging -= value;
        }

        private event PropertyChangedEventHandler? PropertyChanged;
        event PropertyChangedEventHandler? INotifyPropertyChanged.PropertyChanged
        {
            add => PropertyChanged += value;
            remove => PropertyChanged -= value;
        }


        private readonly InternalDynamicObject _data;


        public DictionaryDynamicObject()
        {
            _data = new InternalDynamicObject();
            _data.PropertyChanging += (s, e) => PropertyChanging?.Invoke(this, e);
            _data.PropertyChanged += (s, e) => PropertyChanged?.Invoke(this, e);
        }


        #region IDynamicMetaObjectProvider


        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter) =>
            new DelegateMemberDynamicMetaObject(value => ((DictionaryDynamicObject)value)._data, this, parameter, BindingRestrictions.GetTypeRestriction(parameter, GetType()));


        #endregion


        #region IDictionary<string, object>


        ICollection<string> IDictionary<string, object?>.Keys => ((IDictionary<string, object?>)_data).Keys;

        ICollection<object?> IDictionary<string, object?>.Values => ((IDictionary<string, object?>)_data).Values;

        int ICollection<KeyValuePair<string, object?>>.Count => ((ICollection<KeyValuePair<string, object?>>)_data).Count;

        bool ICollection<KeyValuePair<string, object?>>.IsReadOnly => ((ICollection<KeyValuePair<string, object?>>)_data).IsReadOnly;

        object? IDictionary<string, object?>.this[string key]
        {
            get => ((IDictionary<string, object?>)_data)[key];
            set => ((IDictionary<string, object?>)_data)[key] = value;
        }

        void IDictionary<string, object?>.Add(string key, object? value) =>
            ((IDictionary<string, object?>)_data).Add(key, value);

        bool IDictionary<string, object?>.ContainsKey(string key) =>
            ((IDictionary<string, object?>)_data).ContainsKey(key);

        bool IDictionary<string, object?>.Remove(string key) =>
            ((IDictionary<string, object?>)_data).Remove(key);

        bool IDictionary<string, object?>.TryGetValue(
            string key,
#if NullableAttributes
                [MaybeNullWhen(false)]    
#endif
            out object? value
        ) => ((IDictionary<string, object?>)_data).TryGetValue(key, out value);

        void ICollection<KeyValuePair<string, object?>>.Add(KeyValuePair<string, object?> item) =>
            ((ICollection<KeyValuePair<string, object?>>)_data).Add(item);

        void ICollection<KeyValuePair<string, object?>>.Clear() =>
            ((ICollection<KeyValuePair<string, object?>>)_data).Clear();

        bool ICollection<KeyValuePair<string, object?>>.Contains(KeyValuePair<string, object?> item) =>
            ((ICollection<KeyValuePair<string, object?>>)_data).Contains(item);

        void ICollection<KeyValuePair<string, object?>>.CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex) =>
            ((ICollection<KeyValuePair<string, object?>>)_data).CopyTo(array, arrayIndex);

        bool ICollection<KeyValuePair<string, object?>>.Remove(KeyValuePair<string, object?> item) =>
            ((ICollection<KeyValuePair<string, object?>>)_data).Remove(item);

        IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator() =>
            ((IEnumerable<KeyValuePair<string, object?>>)_data).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            ((IEnumerable)_data).GetEnumerator();

        #endregion IDictionary<string, object>


        private class InternalDynamicObject : DynamicObject, IDictionary<string, object?>, IDynamicMetaObjectProvider, INotifyPropertyChanging, INotifyPropertyChanged
        {

            public event PropertyChangingEventHandler? PropertyChanging;

            public event PropertyChangedEventHandler? PropertyChanged;


            protected IDictionary<string, object?> Data { get; }


            public InternalDynamicObject()
            {
                Data = new Dictionary<string, object?>();
            }


            public bool TryGetValue(
                string name,
#if NullableAttributes
                [MaybeNullWhen(false)]    
#endif
                out object value
            )
            {
                return Data.TryGetValue(name, out value!);
            }

            public void SetValue(string name, object? value)
            {
                TryGetValue(name, out var oldValue);
                if (ReferenceEquals(oldValue, value))
                    return;
                PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(name));
                Data[name] = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }

            public bool Remove(string name)
            {
                if (!TryGetValue(name, out var oldValue))
                    return false;
                PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(name));
                Data.Remove(name);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
                return true;
            }

            public void Clear()
            {
                var keys = Keys.ToList();
                foreach (var k in keys)
                    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(k));
                Data.Clear();
                foreach (var k in keys)
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(k));
            }


            #region DynamicObject


            public override IEnumerable<string> GetDynamicMemberNames() =>
                Keys;

            public override bool TrySetMember(SetMemberBinder binder, object? value)
            {
                SetValue(binder.Name, value);
                return true;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object? result) =>
                TryGetValue(binder.Name, out result);


            #endregion


            #region IDictionary<string, object>


            public ICollection<string> Keys => Data.Keys;

            public ICollection<object?> Values => Data.Values;

            public int Count => Data.Count;

            public bool IsReadOnly => Data.IsReadOnly;

            public object? this[string key]
            {
                get => Data[key];
                set => SetValue(key, value);
            }

            public void Add(string key, object? value) =>
                SetValue(key, value);

            public bool ContainsKey(string key) =>
                Data.ContainsKey(key);

            public void Add(KeyValuePair<string, object?> item) =>
                SetValue(item.Key, item.Value);

            public bool Contains(KeyValuePair<string, object?> item) =>
                Data.Contains(item);

            public void CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex) =>
                Data.CopyTo(array, arrayIndex);

            public bool Remove(KeyValuePair<string, object?> item)
            {
                if (Contains(item))
                    return Remove(item.Key);
                return false;
            }

            public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() =>
                Data.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() =>
                ((IEnumerable)Data).GetEnumerator();


            #endregion IDictionary<string, object>

        }

    }
}
