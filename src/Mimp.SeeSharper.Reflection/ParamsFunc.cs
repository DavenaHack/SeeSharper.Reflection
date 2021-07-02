namespace Mimp.SeeSharper.Reflection
{

    /// <summary>
    /// <see cref="ParamsFunc"/> represent a delegate to invoke a method and allow to pass the parameters as params.
    /// </summary>
    /// <param name="parameters"></param>
    public delegate object ParamsFunc(params object?[] parameters);

}
