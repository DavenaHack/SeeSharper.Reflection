using System;
using System.Linq.Expressions;

namespace Mimp.SeeSharper.Reflection.Expression
{
    public static class FuncExtensions
    {


        /// <summary>
        /// Try to resolve <paramref name="memberAccessor"/> to a <see cref="MemberExpression"/>.
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <typeparam name="TMember"></typeparam>
        /// <param name="memberAccessor"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If can't resolve it.</exception>
        public static MemberExpression ResolveMemberAccessor<TInstance, TMember>(this Func<TInstance, TMember> memberAccessor)
        {
            if (memberAccessor is null)
                throw new ArgumentNullException(nameof(memberAccessor));

            Expression<Func<TInstance, TMember>> e = t => memberAccessor(t);
            if (e.Body is InvocationExpression i)
                if (i.Expression is MemberExpression m)
                    return m;

            throw new InvalidOperationException($"Can't resolve function as {nameof(MemberExpression)}");
        }


    }
}
