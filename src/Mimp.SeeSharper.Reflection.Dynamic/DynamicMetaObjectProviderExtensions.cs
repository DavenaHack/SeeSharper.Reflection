using System;
using System.Dynamic;
using System.Linq.Expressions;

namespace Mimp.SeeSharper.Reflection.Dynamic
{
    public static class DynamicMetaObjectProviderExtensions
    {


        /// <summary>
        /// Return the <see cref="DynamicMetaObject"/> of <paramref name="provider"/>.
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static DynamicMetaObject GetMetaObject(this IDynamicMetaObjectProvider provider)
        {
            if (provider is null)
                throw new ArgumentNullException(nameof(provider));

            return provider.GetMetaObject(Expression.Constant(provider));
        }


    }
}
