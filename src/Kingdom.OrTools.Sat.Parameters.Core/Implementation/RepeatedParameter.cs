using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.Sat.Parameters
{
    using static Characters;
    using static String;

    // TODO: TBD: arguably, I could see "Repeated" Parameter actually *BEING*, i.e. implementing, ICollection, not simply *HAVING* an ICollection Value property.
    // TODO: TBD: or at minimum, *IN ADDITION TO* being a Parameter of type ICollection<T>, it is also a Collection of type T.
    /// <inheritdoc cref="Parameter{T}" />
    /// <typeparam name="T"></typeparam>
    /// <see cref="ICollection{T}"/>
    public abstract class RepeatedParameter<T> : Parameter<ICollection<T>>, IRepeatedParameter<T>
    {
        /// <summary>
        /// Gets or Sets the Value.
        /// In this case we want to preclude the possibility of there being a Null value.
        /// </summary>
        /// <inheritdoc />
        /// <see cref="List{T}"/>
        /// <see cref="Enumerable.ToList{TSource}"/>
        public override ICollection<T> Value
        {
            get => base.Value;
            set => base.Value = value ?? new T[] { }.ToList();
        }

        /// <inheritdoc />
        /// <param name="ordinal"></param>
        protected RepeatedParameter(long ordinal)
            : this(new T[] { }, ordinal)
        {
        }

        /// <inheritdoc />
        /// <param name="values"></param>
        /// <param name="ordinal"></param>
        /// <see cref="IEnumerable{T}"/>
        /// <see cref="List{T}"/>
        /// <see cref="Enumerable.ToList{TSource}"/>
        protected RepeatedParameter(IEnumerable<T> values, long ordinal)
            : base(values.ToList(), ordinal)
        {
        }

        /// <inheritdoc />
        protected override ConvertParameterToStringCallback<ICollection<T>> Convert
            => x => Join($"{Comma}", x.Select(y => ConvertElement(y)));

        /// <summary>
        /// Similar to <see cref="Parameter{T}.Convert"/>, except applied across each of the
        /// <see cref="ICollection{T}"/> Elements. Also similar, default performs a direct
        /// String Interpolation of the <typeparamref name="T"/> type value.
        /// </summary>
        /// <see cref="Parameter{T}.Convert"/>
        protected virtual ConvertParameterToStringCallback<T> ConvertElement => x => $"{x}";
    }
}
