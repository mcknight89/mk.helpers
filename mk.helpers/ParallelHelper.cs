using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace mk.helpers
{
    /// <summary>
    /// Provides utility methods for parallel execution using the Parallel.ForEach pattern.
    /// </summary>
    public static class ParallelHelper
    {
        /// <summary>
        /// Executes the specified <paramref name="body"/> action in parallel as long as the specified <paramref name="condition"/> returns true.
        /// </summary>
        /// <param name="condition">A function that returns a boolean value indicating whether the loop should continue.</param>
        /// <param name="options">The <see cref="ParallelOptions"/> that configure the parallel execution.</param>
        /// <param name="body">The action to be executed in parallel.</param>
        public static void While(Func<bool> condition, ParallelOptions options, Action body)
        {
            Parallel.ForEach(IterateUntilFalse(condition), options, ignored => body());
        }

        /// <summary>
        /// Generates an enumerable sequence of boolean values that represent iterations as long as the specified <paramref name="condition"/> returns true.
        /// </summary>
        /// <param name="condition">A function that returns a boolean value indicating whether the loop should continue.</param>
        /// <returns>An enumerable sequence of boolean values.</returns>
        internal static IEnumerable<bool> IterateUntilFalse(Func<bool> condition)
        {
            while (condition()) yield return true;
        }
    }
}
