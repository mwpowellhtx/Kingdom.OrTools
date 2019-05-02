namespace Kingdom.OrTools.Sat.Parameters
{
    using Kingdom.OrTools.Sat.Parameters;

    public class IntParameter : Parameter<int>
    {
        public IntParameter(long ordinal) : base(ordinal) { }

        public IntParameter(int value, long ordinal) : base(value, ordinal) { }
    }

    /// <typeparam name="T"></typeparam>
    /// <inheritdoc cref="Parameter"/>
    public abstract class Parameter<T> : Parameter, IParameter<T>
    {
        /// <inheritdoc />
        public virtual T Value { get; set; }

        protected Parameter(long ordinal) : base(ordinal) => Initialize();

        /// <inheritdoc />
        /// <param name="value"></param>
        /// <param name="ordinal"></param>
        protected Parameter(T value, long ordinal) : base(ordinal) => Initialize(value);

        private void Initialize(T value = default(T))
        {
            Value = value;
        }

        /// <summary>
        /// Override in order to specify a different method of Conversion
        /// to <see cref="string"/>. Default behavior simply assumes a direct
        /// String Interpolation of the <see cref="Value"/>.
        /// </summary>
        /// <returns></returns>
        protected virtual ConvertParameterToStringCallback<T> Convert => x => $"{x}";

        /// <inheritdoc />
        public override string ToString() => Convert(Value);
    }
}
