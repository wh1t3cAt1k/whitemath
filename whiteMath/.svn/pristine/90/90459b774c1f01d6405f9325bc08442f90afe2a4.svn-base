using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace whiteMath
{
    /// <summary>
    /// A generic interface for providing generic type <typeparamref name="T"/>
    /// with an integer metrics.
    /// 
    /// The method maps the <typeparamref name="T"/> space to integer numbers space.
    /// Two <typeparamref name="T"/> values for which the same integer metric is produced
    /// belong, in a way, to the same 'equivalence class'.
    /// </summary>
    /// <typeparam name="T">The type to be provided with an integer metric.</typeparam>
    public interface IMetricProvider<T>
    {
        int GetMetric(T obj);
    }
}
