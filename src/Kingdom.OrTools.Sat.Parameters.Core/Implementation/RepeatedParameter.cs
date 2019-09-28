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
    /// <see cref="IRepeatedParameter{T}"/>
    /// <see cref="ICollection{T}"/>
    public abstract class RepeatedParameter<T> : Parameter<ICollection<T>>, IRepeatedParameter<T>
    {
        /// <inheritdoc />
        public Type ItemType { get; } = typeof(T);

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
            set => base.Value = (value ?? new T[] { }).ToList();
        }

        /// <inheritdoc />
        /// <param name="ordinal"></param>
        protected RepeatedParameter(long ordinal) : this(new T[] { }, ordinal)
        {
        }

        /// <inheritdoc />
        /// <param name="values"></param>
        /// <param name="ordinal"></param>
        /// <see cref="IEnumerable{T}"/>
        /// <see cref="List{T}"/>
        /// <see cref="Enumerable.ToList{TSource}"/>
        protected RepeatedParameter(IEnumerable<T> values, long ordinal) : base(values.ToList(), ordinal)
        {
        }

        /// <inheritdoc />
        /// <see cref="Value"/>
        /// <see cref="ItemType"/>
        /// <see cref="IParameterValueRenderingOptions"/>
        /// <see cref="Parameter.ParameterName"/>
        /// <see cref="Colon"/>
        /// <see cref="Comma"/>
        /// <see cref="LeftSquareBracket"/>
        /// <see cref="RightSquareBracket"/>
        /// <see cref="SquareBrackets"/>
        /// <see cref="!:https://groups.google.com/forum/#!searchin/or-tools-discuss/sat$20parameter$20string%7Csort:date"/>
        /// <see cref="!:https://groups.google.com/forum/#!searchin/or-tools-discuss/sat$20parameter$20string%7Csort:date/or-tools-discuss/X4Y_ZpKIUp8/kz-xiKSYEAAJ"/>
        public override string ToString(IParameterValueRenderingOptions options)
        {
            // Here we need to leverage ItemType instead of ValueType.
            string RenderRepeatedItems()
            {
                if (Value.Any())
                {
                    return Join(
                        Join($"{Comma} ", Value.Select(x => options[ItemType].Invoke(x)))
                        , LeftSquareBracket, RightSquareBracket
                    );
                }

                return SquareBrackets;
            }

            return $"{ParameterName}{Colon} {RenderRepeatedItems()}";
        }
    }
}
