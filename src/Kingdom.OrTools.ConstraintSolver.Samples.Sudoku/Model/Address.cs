using System;

namespace Kingdom.OrTools.ConstraintSolver.Samples.Sudoku
{
    /// <summary>
    /// Sets up a basic addressable convention for purposes of addressing the grid.
    /// </summary>
    /// <inheritdoc cref="IEquatable{T}"/>
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

        /// <inheritdoc />
        public Address()
            : this(default(int), default(int))
        {
        }

        /// <inheritdoc />
        public Address(Address other)
            : this(other.Row, other.Column)
        {
        }

        /// <summary>
        /// Constructor
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

        /// <inheritdoc />
        public virtual bool Equals(Address other)
        {
            return Equals(this, other);
        }

        // ReSharper disable once NonReadonlyMemberInGetHashCode
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => 3 * Row
                                             // ReSharper disable once NonReadonlyMemberInGetHashCode
                                             ^ 5 * Column;
    }
}
