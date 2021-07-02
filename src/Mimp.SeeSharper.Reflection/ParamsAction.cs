namespace Mimp.SeeSharper.Reflection
{

    /// <summary>
    /// <see cref="ParamsAction"/> represent a delegate to invoke a method and allow to pass the parameters as params.
    /// </summary>
    /// <param name="parameters"></param>
    public delegate void ParamsAction(params object?[] parameters);

}
