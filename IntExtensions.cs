using System;
using System.Collections.Generic;
using System.Text;

namespace mk.helpers
{
    public static class IntExtensions
    {
        public static int? ZeroToNull(this int value)
        {
            return value == 0 ? null : (int?)value;
        }
        public static int? ZeroToNull(this int? value)
        {
            return value == 0 ? null : value;
        }
    }
}
