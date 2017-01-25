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
        protected int currentIndex = 0;

        protected int rows;
        protected int columns;

        protected int elements;
        
        protected Winder(int rowCount, int columnCount)
        {
            this.rows = rowCount;
            this.columns = columnCount;

            this.elements = rows * columns;
            this.trace = new IndexPair[elements];

            MakeTrace();
        }

        public void Reset()
        {
            currentIndex = 0;
        }

        protected abstract void MakeTrace();  // forms the trace
        
        /// <summary>
        /// Returns the next index pair for winding/unwinding the matrix with/into a flat array.
        /// </summary>
        /// <returns>The IndexPair object.</returns>
        public IndexPair GetNextIndexPair()
        {
            if(currentIndex==trace.Length) currentIndex=0;
            return this.trace[currentIndex++];
        }
    }
}
