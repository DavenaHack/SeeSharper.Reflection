namespace Mimp.SeeSharper.Reflection
{

    /// <summary>
    /// <see cref="InstanceFunc"/> represent a delegate to invoke instance method and allow to pass the parameters as params.
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="parameters"></param>
    public delegate object InstanceFunc(object instance, params object?[] parameters);
}
