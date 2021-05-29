using System.Collections.Generic;

namespace Mimp.SeeSharper.Reflection.Test.Mock
{
    public interface ITripleDictionary<K, V, T> : IDictionary<K, V>, IComparer<T> where T : V
    {
    }
}
