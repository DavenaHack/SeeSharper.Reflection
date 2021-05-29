using System;
using System.ComponentModel;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Mimp.SeeSharper.Reflection.Dynamic
{
    public class MemberDynamicObject : IDynamicMetaObjectProvider, INotifyPropertyChanging, INotifyPropertyChanged
    {

        #region INotifyPropertyChanging


        private event PropertyChangingEventHandler? PropertyChanging;
        event PropertyChangingEventHandler? INotifyPropertyChanging.PropertyChanging
        {
            add => PropertyChanging += value;
            remove => PropertyChanging -= value;
        }
        protected void OnPropertyChanging(PropertyChangingEventArgs args) =>
            PropertyChanging?.Invoke(this, args);
        protected void OnPropertyChanging([CallerMemberName] string? propertyName = null) =>
            OnPropertyChanging(new PropertyChangingEventArgs(propertyName));


        #endregion INotifyPropertyChanging


        #region INotifyPropertyChanged


        private event PropertyChangedEventHandler? PropertyChanged;
        event PropertyChangedEventHandler? INotifyPropertyChanged.PropertyChanged
        {
            add => PropertyChanged += value;
            remove => PropertyChanged -= value;
        }
        protected void OnPropertyChanged(PropertyChangedEventArgs args) =>
            PropertyChanged?.Invoke(this, args);
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));


        #endregion INotifyPropertyChanged


        private readonly DictionaryDynamicObject _data;


        public MemberDynamicObject()
        {
            _data = new DictionaryDynamicObject();
            ((INotifyPropertyChanging)_data).PropertyChanging += (s, e) => OnPropertyChanging(e);
            ((INotifyPropertyChanged)_data).PropertyChanged += (s, e) => OnPropertyChanged(e);
        }


        #region IDynamicMetaObjectProvider


        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
        {
            if (parameter is null)
                throw new ArgumentNullException(nameof(parameter));
            
            return new DelegateMemberDynamicMetaObject(value => ((MemberDynamicObject)value)._data, this, parameter, BindingRestrictions.GetTypeRestriction(parameter, GetType()));
        }


        #endregion

    }
}
