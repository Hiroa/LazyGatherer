using System;
using System.Collections.Generic;

namespace LazyGatherer.Solver.Comparator
{
    internal class DoubleEpsilonComparer : IComparer<double>
    {
        private readonly double epsilon;

        internal DoubleEpsilonComparer(double epsilon)
        {
            this.epsilon = epsilon;
        }

        public int Compare(double x, double y)
        {
            var diff = Math.Abs(x - y);
            return diff < epsilon ? 0 : x.CompareTo(y);
        }
    }
}
