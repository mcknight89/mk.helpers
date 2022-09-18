using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace mk.helpers
{
    public static class ParallelHelper
    {

        public static void While(Func<bool> condition, ParallelOptions options, Action body)
        {
            Parallel.ForEach(IterateUntilFalse(condition), options, ignored => body());
        }

        private static IEnumerable<bool> IterateUntilFalse(Func<bool> condition)
        {
            while (condition()) yield return true;
        }
    }

    public static class TasksOptions
    {


    }

    public static class Tasks
    {

        public static void While(Func<bool> condition, ParallelOptions options, Action body)
        {
            Parallel.ForEach(IterateUntilFalse(condition), options, ignored => body());
        }

        private static IEnumerable<bool> IterateUntilFalse(Func<bool> condition)
        {
            while (condition()) yield return true;
        }
    }


    
}
