using System;

namespace Kingdom.OrTools.Samples.Sudoku
{
    /// <summary>
    /// Sets up a basic addressable convention for purposes of addressing the grid.
    /// </summary>
    /// <inheritdoc />
    public class Address : IEquatable<Address>
    {
        /// <summary>
        /// Gets or sets the Row.
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// Gets or sets the Column.
        /// </summary>
        public int Column { get; set; }

        // ReSharper disable once UnusedMember.Global
        /// <summary>
        /// Default Constructor.
        /// </summary>
        /// <inheritdoc />
        public Address()
            : this(default(int), default(int))
        {
        }

        /// <summary>
        /// Copy Constructor.
        /// </summary>
        /// <param name="other"></param>
        /// <inheritdoc />
        public Address(Address other)
            : this(other.Row, other.Column)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        public Address(int row, int column)
        {
            Row = row;
            Column = column;
        }

        /// <summary>
        /// Returns whether <paramref name="a"/> Equals <paramref name="b"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Equals(Address a, Address b)
            => ReferenceEquals(a, b)
               || (!(a == null || b == null)
                   && a.Row == b.Row && a.Column == b.Column);

        /// <summary>
        /// Returns whether this object Equals an <paramref name="other"/> one.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <inheritdoc />
        public virtual bool Equals(Address other) => Equals(this, other);

        // ReSharper disable NonReadonlyMemberInGetHashCode
        /// <summary>
        /// Returns the Hash Code.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => 3 * Row ^ 5 * Column;

        /// <summary>
        /// Implicitly converts the <paramref name="address"/> to a <see cref="Row"/> major
        /// <see cref="Tuple{T1,T2}"/>. Literally, <see cref="Tuple{T1,T2}.Item1"/> will be
        /// <see cref="Row"/>, <see cref="Tuple{T1,T2}.Item2"/> will be <see cref="Column"/>.
        /// </summary>
        /// <param name="address"></param>
        /// <remarks>Sadly, we cannot set the Tuple Item properties, or it would make better
        /// sense to simple derive directly from Tuple itself.</remarks>
        public static implicit operator Tuple<int, int>(Address address) => Tuple.Create(address.Row, address.Column);
    }
}
