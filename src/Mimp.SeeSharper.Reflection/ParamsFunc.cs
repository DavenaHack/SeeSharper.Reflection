namespace Mimp.SeeSharper.Reflection
{
    
    public delegate object ParamsFunc(params object?[] parameters);

    public delegate T ParamsFunc<T>(params object?[] parameters);

}
