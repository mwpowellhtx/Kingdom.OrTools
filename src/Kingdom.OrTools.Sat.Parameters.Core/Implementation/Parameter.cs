using System;
using System.Reflection;

namespace Kingdom.OrTools.Sat.Parameters
{
    using static Characters;

    /// <inheritdoc />
    public abstract class Parameter : IParameter
    {
        /// <inheritdoc />
        public long Ordinal { get; }

        /// <inheritdoc />
        public string ParameterName
        {
            get
            {
                string GetMemberName<TMemberInfo>(TMemberInfo mi) where TMemberInfo : MemberInfo
                    => mi.GetCustomAttribute<ParameterNameAttribute>(true)?.Name ?? mi.Name;
                return GetMemberName(GetType());
            }
        }

        /// <inheritdoc />
        public abstract object WeaklyTypedValue { get; }

        /// <inheritdoc />
        public Type ValueType { get; }

        // TODO: TBD: parameters have a canonical name? based on the parsed bits...
        // TODO: TBD: along same lines as ctor-given ordinal, pass canonical name via ctor...
        // TODO: TBD: might also call it lexical, or lexicon, based on parsed option names, etc

        /// <summary>
        /// Protected Constructor.
        /// </summary>
        /// <param name="ordinal"></param>
        /// <param name="valueType"></param>
        protected Parameter(long ordinal, Type valueType)
        {
            Ordinal = ordinal;
            ValueType = valueType;
        }

        /// <inheritdoc />
        /// <see cref="ToString(IParameterValueRenderingOptions)"/>
        /// <see cref="ParameterValueRenderingOptions.DefaultOptions"/>
        public override string ToString() => ToString(ParameterValueRenderingOptions.DefaultOptions);

        /// <inheritdoc />
        /// <see cref="ParameterName"/>
        /// <see cref="ValueType"/>
        /// <see cref="WeaklyTypedValue"/>
        /// <see cref="IParameterValueRenderingOptions"/>
        /// <see cref="RenderParameterValueCallback"/>
        /// <see cref="Colon"/>
        /// <see cref="!:https://groups.google.com/forum/#!searchin/or-tools-discuss/sat$20parameter$20string%7Csort:date"/>
        /// <see cref="!:https://groups.google.com/forum/#!searchin/or-tools-discuss/sat$20parameter$20string%7Csort:date/or-tools-discuss/X4Y_ZpKIUp8/kz-xiKSYEAAJ"/>
        public virtual string ToString(IParameterValueRenderingOptions options)
            => $"{ParameterName}{Colon} {options[ValueType].Invoke(WeaklyTypedValue)}";
    }
}
