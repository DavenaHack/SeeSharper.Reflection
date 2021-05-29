using System;
using System.Dynamic;
using System.Linq.Expressions;

namespace Mimp.SeeSharper.Reflection.Dynamic
{
    public static class DynamicMetaObjectProviderExtensions
    {


        public static DynamicMetaObject GetMetaObject(this IDynamicMetaObjectProvider provider)
        {
            if (provider is null)
                throw new ArgumentNullException(nameof(provider));

            return provider.GetMetaObject(Expression.Constant(provider));
        }


    }
}
