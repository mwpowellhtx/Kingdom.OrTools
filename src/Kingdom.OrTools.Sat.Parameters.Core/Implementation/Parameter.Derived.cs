namespace Kingdom.OrTools.Sat.Parameters
{
    /// <typeparam name="T"></typeparam>
    /// <inheritdoc cref="Parameter"/>
    public abstract class Parameter<T> : Parameter, IParameter<T>
    {
        /// <inheritdoc />
        public virtual T Value { get; set; }

        /// <inheritdoc />
        public override object WeaklyTypedValue => Value;

        /// <inheritdoc />
        protected Parameter(long ordinal) : base(ordinal, typeof(T)) => Initialize();

        /// <inheritdoc />
        /// <param name="value"></param>
        /// <param name="ordinal"></param>
        protected Parameter(T value, long ordinal) : base(ordinal, typeof(T)) => Initialize(value);

        private void Initialize(T value = default)
        {
            Value = value;
        }
    }
}
