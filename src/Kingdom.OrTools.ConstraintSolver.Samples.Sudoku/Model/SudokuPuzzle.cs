using System;
using System.Collections;
using System.Collections.Generic;

namespace Kingdom.OrTools.ConstraintSolver.Samples.Sudoku
{
    using static Domain;

    /// <inheritdoc cref="ISudokuPuzzle" />
    public partial class SudokuPuzzle : IDictionary<Address, int>
    {
        /// <summary>
        /// Grid backing field.
        /// </summary>
        private readonly IDictionary<Address, int> _grid = new Dictionary<Address, int>();

        private void DictionaryAction(Action<IDictionary<Address, int>> action) => action.Invoke(_grid);

        private TResult DictionaryFunc<TResult>(Func<IDictionary<Address, int>, TResult> func) => func.Invoke(_grid);

        /// <summary>
        /// Returns a default grid.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<KeyValuePair<Address, int>> GetDefaultGrid()
        {
            for (var row = MinimumValue; row < MaximumValue; row++)
            {
                for (var column = MinimumValue; column < MaximumValue; column++)
                {
                    yield return new KeyValuePair<Address, int>(new Address(row, column), 0);
                }
            }
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SudokuPuzzle()
        {
            foreach (var item in GetDefaultGrid())
            {
                DictionaryAction(x => x.Add(new Address(item.Key), item.Value));
            }
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="other"></param>
        public SudokuPuzzle(SudokuPuzzle other)
        {
            _grid = new Dictionary<Address, int>();
            foreach (var item in other._grid)
            {
                DictionaryAction(x => x.Add(new Address(item.Key), item.Value));
            }
        }

        private static InvalidOperationException ThrowNotSupported(string callerName)
            => new InvalidOperationException($"{callerName} is not supported");

        /// <inheritdoc />
        public void Add(Address key, int value) => throw ThrowNotSupported(nameof(Add));

        /// <inheritdoc />
        public bool ContainsKey(Address key) => DictionaryFunc(x => x.ContainsKey(key));

        /// <inheritdoc />
        public ICollection<Address> Keys => DictionaryFunc(x => x.Keys);

        /// <inheritdoc />
        public bool Remove(Address key) => throw ThrowNotSupported(nameof(Remove));

        /// <inheritdoc />
        public bool TryGetValue(Address key, out int value)
        {
            var y = value = default(int);
            if (!DictionaryFunc(x => x.TryGetValue(key, out y)))
            {
                return false;
            }

            value = y;
            return true;
        }

        /// <inheritdoc />
        public ICollection<int> Values => DictionaryFunc(x => x.Values);

        /// <inheritdoc />
        public int this[Address key]
        {
            get => DictionaryFunc(x => x[key]);
            set
            {
                value.VerifyValue();
                DictionaryAction(x => x[key] = value);
            }
        }

        /// <inheritdoc />
        public void Add(KeyValuePair<Address, int> item) => throw ThrowNotSupported(nameof(Add));

        /// <inheritdoc />
        public void Clear() => throw ThrowNotSupported(nameof(Clear));

        /// <inheritdoc />
        public bool Contains(KeyValuePair<Address, int> item) => DictionaryFunc(x => x.Contains(item));

        /// <inheritdoc />
        public void CopyTo(KeyValuePair<Address, int>[] array, int arrayIndex)
            => DictionaryAction(x => x.CopyTo(array, arrayIndex));

        /// <inheritdoc />
        public int Count => DictionaryFunc(x => x.Count);

        /// <inheritdoc />
        public bool IsReadOnly => DictionaryFunc(x => x.IsReadOnly);

        /// <inheritdoc />
        public bool Remove(KeyValuePair<Address, int> item) => throw ThrowNotSupported(nameof(Remove));

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<Address, int>> GetEnumerator() => DictionaryFunc(x => x.GetEnumerator());

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
