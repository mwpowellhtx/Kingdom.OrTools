using System.Collections.Generic;

namespace Kingdom.OrTools.Sat.Parameters
{
    /// <inheritdoc />
    /// <summary>
    /// Repeated Parameters are simply ones which may be rolled up as a Collection, a List, etc.
    /// For simplicity we will assume this means <see cref="ICollection{T}"/>.
    /// </summary>
    public interface IRepeatedParameter<T> : IParameter<ICollection<T>>
    {
    }
}
