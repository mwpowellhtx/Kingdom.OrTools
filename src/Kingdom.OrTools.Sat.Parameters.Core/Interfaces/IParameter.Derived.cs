namespace Kingdom.OrTools.Sat.Parameters
{
    /// <summary>
    /// Represents a Strongly Typed <typeparamref name="T"/> <see cref="IParameter"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <inheritdoc />
    public interface IParameter<T> : IParameter
    {
        /// <summary>
        /// Gets or Sets the Value.
        /// </summary>
        T Value { get; set; }
    }
}
