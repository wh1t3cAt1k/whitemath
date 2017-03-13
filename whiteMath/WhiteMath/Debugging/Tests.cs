#if DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhiteMath
{
    public class Tests
    {
        private class RandomMetricProvider : IMetricProvider<int>
        {
            System.Random gen = new System.Random();

            public int GetMetric(int obj)
            {
                return gen.Next(-100, 100);
            }
        }

        public static void SummatorMinCurMetric_Test()
        {
            Summator<int> summator = new Summator<int>(0, delegate(int one, int two) { return one + two; });
            IMetricProvider<int> mprovider = new RandomMetricProvider();

            Console.WriteLine("SUM: " + summator.SumByMinimumCurrentMetricSum(delegate(int i) { return 2 * i; }, 1, 100, mprovider));
            Console.WriteLine("SUM SEQ: " + summator.SumSequentially(delegate(int i) { return 2 * i; }, 1, 100));
        }

        public static int _DBM_LinkedLists_SummatorMinCurMetricTime()
        {
            return 0;
        }
    }
}

#endif