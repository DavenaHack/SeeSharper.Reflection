namespace Mimp.SeeSharper.Reflection.Test.Mock
{
    public interface IRecursiveGeneric<T> where T : IRecursiveGeneric<T>
    {
    }
}
