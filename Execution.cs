using System;
using System.Collections.Generic;
using System.Text;

namespace mk.helpers
{
    public static class Execution
    {
        public static TimeSpan Time(Action action)
            => Time(action, null);
        public static TimeSpan Time( Action action, Action<TimeSpan> onComplete)
        {
            var start = DateTime.Now;
            action.Invoke();
            var ts = DateTime.Now - start;
            onComplete?.Invoke(ts);
            return ts;
        }
    }

}
