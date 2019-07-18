using System;
using System.Collections.Generic;

namespace Kingdom.OrTools.Sat.Parameters
{
    /// <summary>
    /// Represents the core set of Repeated <see cref="IParameter"/> concerns. Really,
    /// for Repeated Parameters, the <see cref="IParameter.WeaklyTypedValue"/> is of
    /// minimal consequence apart from setting or getting, populating, etc. We almost
    /// always want to work with the Value, the <see cref="ICollection{T}"/>, whose
    /// individual Items are Rendered as part of working with the Repeated Parameter.
    /// </summary>
    /// <inheritdoc />
    public interface IRepeatedParameter : IParameter
    {
        /// <summary>
        /// Gets the <see cref="ICollection{T}"/> ItemType. Not to be confused with
        /// <see cref="IParameter.ValueType"/>, which in this case really refers to the
        /// <see cref="ICollection{T}"/> itself.
        /// </summary>
        Type ItemType { get; }
    }

    /// <summary>
    /// Repeated Parameters are simply ones which may be rolled up as a Collection, a List,
    /// etc. For simplicity we will assume this means <see cref="ICollection{T}"/>.
    /// </summary>
    /// <inheritdoc cref="IParameter{T}"/>
    /// <see cref="IRepeatedParameter"/>
    public interface IRepeatedParameter<T> : IParameter<ICollection<T>>, IRepeatedParameter
    {
    }
}
