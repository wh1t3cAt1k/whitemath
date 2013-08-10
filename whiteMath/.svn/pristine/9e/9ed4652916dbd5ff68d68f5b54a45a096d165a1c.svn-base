using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using whiteMath.General;
using whiteMath.Statistics;
using whiteMath.Randoms;

namespace whiteMath.Statistics
{
    public enum DeviationType { Upwards, Downwards, EitherSide }

    public static class SequenceFilteringExtensions
    {
        /// <summary>
        /// Filters the surges in a sequence whose absolute values deviate from the specified positive value by a 
        /// nonnegative value specified.
        /// </summary>
        /// <typeparam name="T">The type of elements in the incoming sequence.</typeparam>
        /// <typeparam name="C">The calculator for the sequence elements type.</typeparam>
        /// <param name="sequence">A calling sequence object.</param>
        /// <param name="centerValue">Some value. Sequence elements that deviate from this value by more than <paramref name="allowedEpsilon"/> will be filtered out.</param>
        /// <param name="allowedEpsilon">The allowed nonnegative value of deviation from <paramref name="centerValue"/>.</param>
        /// <param name="deviationType">
        /// If deviation type is Downwards, values which are smaller than <paramref name="centerValue"/> 
        /// by <paramref name="allowedEpsilon"/> will be filtered.
        /// If deviation type is Upwards, values which are bigger than <paramref name="centerValue"/> 
        /// by <paramref name="allowedEpsilon"/> will be filtered.
        /// If deviation type is EitherSide, all values that differ from the <paramref name="centerValue"/>
        /// by <paramref name="allowedEpsilon"/> will be filtered.
        /// </param>
        /// <param name="filteredValues">A reference to a collection to store the filtered values. May be null.</param>
        /// <returns>The list containing all incoming values except for the filtered ones.</returns>
        public static List<T> filterSurgesByEpsilonDeviationFromValue<T, C>(this IEnumerable<T> sequence, T centerValue, T allowedEpsilon, DeviationType deviationType = DeviationType.EitherSide, ICollection<T> filteredValues = null) where C : ICalc<T>, new()
        {
            ICalc<T> calc = Numeric<T, C>.Calculator;

            (allowedEpsilon >= Numeric<T,C>.Zero).Assert(new ArgumentException("Allowed deviation epsilon should be a non-negative value."));

            List<T> result = new List<T>();

            foreach(Numeric<T,C> current in sequence)
                if (deviationType == DeviationType.EitherSide   && WhiteMath<T,C>.Abs(centerValue - current) < (Numeric<T,C>)allowedEpsilon ||
                    deviationType == DeviationType.Downwards    && centerValue - current < allowedEpsilon ||
                    deviationType == DeviationType.Upwards      && current - centerValue < allowedEpsilon)
                        result.Add(current);
                else if (filteredValues != null)
                    filteredValues.Add(current);

            return result;
        }

        /// <summary>
        /// Filters the surges in a sequence whose absolute values deviate from the specified positive value by a 
        /// positive factor specified.
        /// </summary>
        /// <typeparam name="T">The type of elements in the incoming sequence.</typeparam>
        /// <typeparam name="C">The calculator for the sequence elements type.</typeparam>
        /// <param name="sequence">A calling sequence object.</param>
        /// <param name="centerValue">A nonnegative value. Sequence elements that deviate from this value by more than <paramref name="allowedDeviationFactor"/> will be filtered out.</param>
        /// <param name="allowedDeviationFactor">The allowed factor of absolute deviation.</param>
        /// <param name="deviationType">
        /// If deviation type is Downwards, values which are smaller than <paramref name="centerValue"/> 
        /// by a factor of <paramref name="allowedDeviationFactor"/> will be filtered.
        /// If deviation type is Upwards, values which are bigger than <paramref name="centerValue"/> 
        /// by a factor of <paramref name="allowedDeviationFactor"/> will be filtered.
        /// If deviation type is EitherSide, all values that differ from the <paramref name="centerValue"/>
        /// by a factor of <paramref name="allowedDeviationFactor"/> will be filtered.
        /// </param>
        /// <param name="filteredValues">A reference to a collection to store the filtered values. May be null.</param>
        /// <returns>The list containing all incoming values except for the filtered ones.</returns>
        public static List<T> filterSurgesByAbsoluteFactorDeviationFromValue<T, C>(this IEnumerable<T> sequence, T centerValue, double allowedDeviationFactor, DeviationType deviationType = DeviationType.EitherSide, ICollection<T> filteredValues = null) where C: ICalc<T>, new()
        {
            ICalc<T> calc = Numeric<T, C>.Calculator;

            (centerValue > Numeric<T, C>.Zero).Assert(new ArgumentException("The center value specified should be positive."));
            (allowedDeviationFactor > 0).Assert(new ArgumentException("The allowed deviation factor should be positive."));

            List<T> result = new List<T>();

            foreach (Numeric<T, C> current in sequence)
            {
                Numeric<T, C> absCurrent = WhiteMath<T, C>.Abs(current);

                Numeric<T, C> multipliedValue = (Numeric<T,C>)allowedDeviationFactor * centerValue;
                Numeric<T, C> multipliedCurrent = (Numeric<T,C>)allowedDeviationFactor * absCurrent;

                if (deviationType == DeviationType.EitherSide && (absCurrent > multipliedValue || multipliedCurrent < centerValue) ||
                    deviationType == DeviationType.Upwards && absCurrent > multipliedValue ||
                    deviationType == DeviationType.Downwards && multipliedCurrent < centerValue)
                {
                    // Filter the surge.

                    if (filteredValues != null)
                        filteredValues.Add(current);
                }
                else
                    result.Add(current);
            }

            return result;
        }
    }

    public static class SequenceSurgesAddingExtensions
    {
        /// <summary>
        /// Adds absolute surges from a symmetric distribution random generator to each value of a numeric sequence and returns the
        /// noisy sequence in the form of a list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the numeric sequence.</typeparam>
        /// <typeparam name="C">The calculator for the <typeparamref name="T"/> type.</typeparam>
        /// <param name="sequence">The calling numeric sequence object.</param>
        /// <param name="symmetricDistributionGenerator">A random generator providing values of some symmetric distribution.</param>
        /// <param name="deviationType">Which deviations (positive, negative or both) should be added to the sequence.</param>
        /// <returns>The list containing values from the incoming sequence by preserving the order, noised by absolute values from symmetric distribution generator.</returns>
        public static List<T> addAbsoluteSurgesFromSymmetricDistribution<T, C>(this IEnumerable<T> sequence, IRandomUnbounded<T> symmetricDistributionGenerator, DeviationType deviationType) where C: ICalc<T>, new()
        {
            List<T>     result      = new List<T>(sequence.Count());

            foreach (Numeric<T, C> value in sequence)
                if (deviationType == DeviationType.EitherSide)
                    result.Add(value + symmetricDistributionGenerator.Next());
                else if (deviationType == DeviationType.Downwards)
                    result.Add(value - WhiteMath<T, C>.Abs(symmetricDistributionGenerator.Next()));
                else
                    result.Add(value + WhiteMath<T, C>.Abs(symmetricDistributionGenerator.Next()));

            return result;
        }

        /// <summary>
        /// Adds relative (factor) surges from a symmetric distribution random generator to each value of a numeric sequence
        /// and returns the nosiy sequence in the form of a list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the numeric sequence.</typeparam>
        /// <typeparam name="C">The calculator for the <typeparamref name="T"/> type.</typeparam>
        /// <param name="sequence">The calling numeric sequence object.</param>
        /// <param name="symmetricDistributionGenerator">A random generator providing values of some symmetric distribution.</param>
        /// <param name="deviationType">Which deviations (positive, negative or both) should be added to the sequence.</param>
        /// <returns></returns>
        public static List<T> addRelativeSurgesFromSymmetricDistribution<T, C>(this IEnumerable<T> sequence, IRandomUnbounded<T> symmetricDistributionGenerator, DeviationType deviationType) where C : ICalc<T>, new()
        {
            List<T>     result      = new List<T>(sequence.Count());
            
            foreach (Numeric<T, C> value in sequence)
                if (deviationType == DeviationType.EitherSide)
                    result.Add(value + value * symmetricDistributionGenerator.Next());
                else if (deviationType == DeviationType.Downwards)
                    result.Add(value - WhiteMath<T,C>.Abs(value * symmetricDistributionGenerator.Next()));
                else
                    result.Add(value + WhiteMath<T, C>.Abs(value * symmetricDistributionGenerator.Next()));

            return result;
        }
    }
}
