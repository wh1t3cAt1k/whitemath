using System;
using System.Runtime.InteropServices;
using whiteMath.General;

namespace whiteMath.Time
{
    /// <summary>
    /// The class designed to accurately measure time intervals.
    /// </summary>
    public class NanoStopWatch
    {
        private bool processFinished = true;
        private bool paused = false;

        /// <summary>
        /// Returns the boolean value determining whether the timer has finished working.
        /// </summary>
        public bool Finished { get { return processFinished; } }

        /// <summary>
        /// Returns the boolean value determining whether the timer is currently paused, but not stopped.
        /// </summary>
        public bool Paused { get { return paused; } }

        private long frequency = 0;

        private long firstCount = 0;
        private long sum = 0;
        private long secondCount = 0;

        public decimal getIntervalInMillis()
        {
            checkStopped();
            return (secondCount - firstCount + sum) * 1000 / (decimal)frequency;
        }

        public long getIntervalInTicks()
        {
            checkStopped();
            return (sum + secondCount - firstCount);
        }

        public long getMeasuredTPS()
        {
            checkStopped();
            return frequency;
        }

        // checks if the timer is stopped - else throws an exception.
        private void checkStopped()
        {
            if (!processFinished) throw new ApplicationException("Timer is not stopped.");
            return;
        }

        // ----------------------- FUNCTIONALITY ------------------

        /// <summary>
        /// Starts the timer.
        /// </summary>
        public void start()
        {
            if (!processFinished && !paused)
                throw new ApplicationException("Cannot start the timer - it's already running.");
                
            paused = processFinished = false;

            // do the calculations, if not supported, throw an exception.
            if (NativeMethods.QueryPerformanceCounter(ref firstCount) <= 0 || NativeMethods.QueryPerformanceFrequency(ref frequency) <= 0)
                throw new NotSupportedException("The computer hardware does not support high-performance timers.");
        }

        /// <summary>
        /// Temporarily pauses the timer.
        /// If used frequently, precision can be lost significantly.
        /// </summary>
        public void pause()
        {
            if (processFinished)
                throw new ApplicationException("Cannot pause the timer - it's not started.");

            paused = true;

            long temp = 0;

            NativeMethods.QueryPerformanceCounter(ref temp);
            NativeMethods.QueryPerformanceFrequency(ref frequency);
                
            sum += temp - firstCount;
            firstCount = temp;
        }

        /// <summary>
        /// Stops the timer so the result can be evaluated.
        /// </summary>
        public void stop()
        {
            if (processFinished)
                throw new ApplicationException("Cannot stop the timer - it's not started.");

            NativeMethods.QueryPerformanceCounter(ref secondCount);
            NativeMethods.QueryPerformanceFrequency(ref frequency);

            this.processFinished = true;
        }

        /// <summary>
        /// Resets the timer in any condition to the all-zero state.
        /// </summary>
        public void reset()
        {
            firstCount = sum = secondCount = frequency = 0;
            this.processFinished = true;
        }
    }
}
