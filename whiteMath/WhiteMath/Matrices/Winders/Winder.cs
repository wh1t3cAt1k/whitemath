using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhiteMath.Matrices
{
    /// <summary>
    /// Minimal interface functionality for winders.
    /// </summary>
    public interface IWinder
    {
        IndexPair GetNextIndexPair();

        /// <summary>
        /// Resets the winder so the indexing starts from the beginning
        /// (often from the from the [0,0], but this may vary depending on the implementation)
        /// </summary>
        void reset();
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

            formTrace();
        }

        public void reset()
        {
            currentIndex = 0;
        }

        protected abstract void formTrace();  // forms the trace
        
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
