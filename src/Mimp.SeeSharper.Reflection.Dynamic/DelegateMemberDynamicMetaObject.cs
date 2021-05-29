using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;

namespace Mimp.SeeSharper.Reflection.Dynamic
{
    public class DelegateMemberDynamicMetaObject : DynamicMetaObject
    {


        protected IDynamicMetaObjectProvider? DelegateProvider { get; }

        protected Expression? DelegateExpression { get; }

        protected DynamicMetaObject? DelegateMetaObject { get; }

        protected IEnumerable<string>? Members { get; }


        public DelegateMemberDynamicMetaObject(Expression<Func<object, IDynamicMetaObjectProvider>>? delegateProviderExpression, object value, Expression expression, BindingRestrictions restrictions)
            : base(expression, restrictions, value)
        {
            if (delegateProviderExpression is null && value is null)
                throw new ArgumentNullException(nameof(delegateProviderExpression));
            if (delegateProviderExpression is not null)
            {
                DelegateProvider = delegateProviderExpression?.Compile()(value) ?? throw new ArgumentNullException(nameof(delegateProviderExpression));
                DelegateExpression = Expression.Invoke(delegateProviderExpression, expression);
                DelegateMetaObject = DelegateProvider.GetMetaObject(DelegateExpression);
            }
            if (value is not null)
            {
                var type = value.GetType();
                Members = type.GetProperties().Select(p => p.Name)
                    .Concat(type.GetFields().Select(f => f.Name))
                    .Concat(type.GetMethods().Where(m => !m.IsSpecialName).Select(m => m.Name))
                    .ToArray();
            }
        }

        public override DynamicMetaObject BindDeleteMember(DeleteMemberBinder binder)
        {
            if (Value is not null)
                if (DelegateMetaObject is null || Members!.Contains(binder.Name))
                    return base.BindDeleteMember(binder);
            return DelegateMetaObject!.BindDeleteMember(binder);
        }

        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            if (Value is not null)
                if (DelegateMetaObject is null || Members!.Contains(binder.Name))
                    return base.BindGetMember(binder);
            return DelegateMetaObject!.BindGetMember(binder);
        }

        public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        {
            if (Value is not null)
                if (DelegateMetaObject is null || Members!.Contains(binder.Name))
                    return base.BindSetMember(binder, value);
            return DelegateMetaObject!.BindSetMember(binder, value);
        }

        public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
        {
            if (Value is not null)
                if (DelegateMetaObject is null || Members!.Contains(binder.Name))
                    return base.BindInvokeMember(binder, args);
            return DelegateMetaObject!.BindInvokeMember(binder, args);
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            if (Value is not null)
                return Members!.Concat(DelegateMetaObject!.GetDynamicMemberNames()).Distinct().ToArray();
            return DelegateMetaObject!.GetDynamicMemberNames();
        }

    }
}
