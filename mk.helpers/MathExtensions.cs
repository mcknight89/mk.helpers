using System;

namespace mk.helpers
{
    /// <summary>
    /// Small maths helpers for games.
    /// </summary>
    public static class MathExtensions
    {
        /// <summary>
        /// Moves towards the target by at most step, without passing it.
        /// </summary>
        public static float Approach(this float value, float target, float step)
        {
            if (value < target)
                return Math.Min(value + step, target);

            return Math.Max(value - step, target);
        }

        /// <summary>
        /// Wraps an index into 0 to count - 1, negatives included (-1 becomes count - 1).
        /// </summary>
        public static int Wrap(this int value, int count)
        {
            if (count <= 0)
                return 0;

            return (value % count + count) % count;
        }
    }
}
