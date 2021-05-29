using System;
using System.Linq.Expressions;

namespace Mimp.SeeSharper.Reflection.Expression
{
    public static class FuncExtensions
    {

        public static MemberExpression ResolveMemberAccessor<T, M>(this Func<T, M> memberAccessor)
        {
            if (memberAccessor is null)
                throw new ArgumentNullException(nameof(memberAccessor));

            Expression<Func<T, M>> e = t => memberAccessor(t);
            if (e.Body is InvocationExpression i)
                if (i.Expression is MemberExpression m)
                    return m;

            throw new InvalidOperationException($"Can't resolve function as {nameof(MemberExpression)}");
        }

    }
}
