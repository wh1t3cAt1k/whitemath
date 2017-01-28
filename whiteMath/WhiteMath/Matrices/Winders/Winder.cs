using System.Collections.ObjectModel;

namespace WhiteMath.Matrices.Winders
{
    /// <summary>
    /// Minimal interface functionality for winders.
    /// </summary>
    public interface IWinder
    {
        IndexPair GetNextIndexPair();

        /// <summary>
        /// Resets the winder so the indexing starts from the beginning.
        /// </summary>
        void Reset();
    }

    /// <summary>
    /// Provides standard support for winding and unwinding between Matrix objects
    /// and single-dimensional arrays of Matrix elements.
    /// </summary>
    public abstract class Winder: IWinder
    {
        protected internal IndexPair[] trace;
		protected int _currentIndex = 0;

        protected int _rowCount;
        protected int _columnCount;
        protected int _elementCount;
        
        protected Winder(int rowCount, int columnCount)
        {
            this._rowCount = rowCount;
            this._columnCount = columnCount;
            this._elementCount = this._rowCount * this._columnCount;

            this.trace = new IndexPair[_elementCount];

            MakeTrace();
        }

        public void Reset()
        {
            _currentIndex = 0;
        }

        protected abstract void MakeTrace();  // forms the trace
        
        /// <summary>
        /// Returns the next index pair for winding/unwinding the matrix with/into a flat array.
        /// </summary>
        /// <returns>The IndexPair object.</returns>
        public IndexPair GetNextIndexPair()
        {
			if (_currentIndex == trace.Length)
			{
				_currentIndex = 0;
			}

            return this.trace[_currentIndex++];
        }
    }
}
