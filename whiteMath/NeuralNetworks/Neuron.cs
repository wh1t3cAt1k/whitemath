using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace whiteMath.NeuralNetworks
{
    /// <summary>
    /// The interface designed to provide values of
    /// specified type T.
    /// </summary>
    /// <typeparam name="T">The type of the value provided.</typeparam>
    /// <typeparam name="C">The calculator for the <typeparamref name="T"/> type.</typeparam>
    public interface IValueProvider<T, C> where C: ICalc<T>, new()
    {
        bool            IsNeuron { get; }
        Neuron<T, C>    AsNeuron { get; } 
        
        Numeric<T,C>    ProvideValue();
    }

    /// <summary>
    /// The IValueProvider implementation that shall provide
    /// a constant value.
    /// </summary>
    /// <typeparam name="T">The type of the value provided.</typeparam>
    /// <typeparam name="C">The calculator for the <typeparamref name="T"/> type.</typeparam>
    public class ConstValueProvider<T, C> where C : ICalc<T>, new()
    {
        private Numeric<T, C> value;

        public ConstValueProvider(Numeric<T, C> value)
        {
            this.value = value;
        }

        bool            IsNeuron { get { return false; } }
        Neuron<T, C>    AsNeuron { get { throw new NotSupportedException(); } }

        public Numeric<T, C> ProvideValue()
        {
            return value;
        }
    }

    /// <summary>
    /// This class represents a single neuron of a neural network.
    /// </summary>
    /// <typeparam name="T">The numeric type of neuron's entry and output signal.</typeparam>
    /// <typeparam name="C">The calculator for the <typeparamref name="T"/> type.</typeparam>
    public class Neuron<T, C>: IValueProvider<T, C> where C: ICalc<T>, new()
    {
        private static readonly Summator<Numeric<T, C>> summator 
            = new Summator<Numeric<T, C>>(Numeric<T, C>.Zero, delegate(Numeric<T, C> a, Numeric<T, C> b) { return a + b; });

        /// <summary>
        /// The function of the neuron.
        /// </summary>
        public Func<T, T> Function { get; private set; }

        /// <summary>
        /// The neurons that shall provide entry vector values.
        /// </summary>
        public List<IValueProvider<T,C>> EntrySignals  { get; private set; }
        
        /// <summary>
        /// The weights for the entry vector values.
        /// </summary>
        public List<T>                   Weights       { get; private set; }

        // -------- interface implementations --------

        public bool         IsNeuron { get { return true; } }
        public Neuron<T, C> AsNeuron { get { return this; } }

        public Numeric<T, C> ProvideValue()
        {
            return summator.Sum_SmallerToBigger(
                delegate(int i)
                {
                    return this.EntrySignals[i].ProvideValue() * this.Weights[i];
                }, 
                0, 
                this.EntrySignals.Count - 1, 
                Numeric<T, C>.NumericComparer); 
        }
    }
}
