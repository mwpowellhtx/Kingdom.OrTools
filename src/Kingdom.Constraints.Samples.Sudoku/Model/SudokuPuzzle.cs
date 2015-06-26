using System;
using System.Collections;
using System.Collections.Generic;

namespace Kingdom.Constraints.Samples.Sudoku
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SudokuPuzzle : IDictionary<Address, int>
    {
        /// <summary>
        /// Grid backing field.
        /// </summary>
        private readonly IDictionary<Address, int> _grid
            = new Dictionary<Address, int>();

        /// <summary>
        /// Returns a default grid.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<KeyValuePair<Address, int>> GetDefaultGrid()
        {
            for (var row = 0; row < 9; row++)
                for (var column = 0; column < 9; column++)
                    yield return new KeyValuePair<Address, int>(new Address(row, column), 0);
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SudokuPuzzle()
        {
            foreach (var item in GetDefaultGrid())
                _grid.Add(new Address(item.Key), item.Value);
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="other"></param>
        public SudokuPuzzle(SudokuPuzzle other)
        {
            _grid = new Dictionary<Address, int>();
            foreach (var item in other._grid)
                _grid.Add(new Address(item.Key), item.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(Address key, int value)
        {
            throw new InvalidOperationException(@"Adding is not supported");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(Address key)
        {
            return _grid.ContainsKey(key);
        }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<Address> Keys
        {
            get { return _grid.Keys; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(Address key)
        {
            throw new InvalidOperationException(@"Removing is not supported");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(Address key, out int value)
        {
            value = default(int);
            if (!ContainsKey(key)) return false;
            value = this[key];
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<int> Values
        {
            get { return _grid.Values; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int this[Address key]
        {
            get { return _grid[key]; }
            set
            {
                value.VerifyValue();
                _grid[key] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<Address, int> item)
        {
            throw new InvalidOperationException(@"Adding is not supported");
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            throw new InvalidOperationException(@"Clearing is not supported");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<Address, int> item)
        {
            return _grid.Contains(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<Address, int>[] array, int arrayIndex)
        {
            _grid.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return _grid.Count; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly
        {
            get { return _grid.IsReadOnly; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<Address, int> item)
        {
            throw new InvalidOperationException(@"Removing is not supported");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<Address, int>> GetEnumerator()
        {
            return _grid.GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
