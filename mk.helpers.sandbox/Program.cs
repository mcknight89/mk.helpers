using System;
using System.Threading;

namespace mk.helpers.sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var tl = new ThreadLord<string>((d) =>
            {
                Thread.Sleep(10);
            });

            for (int i = 0; i < 10000; i++)
                tl.Enqueue(Guid.NewGuid().ToString());

            tl.LimitQueue(1000).Start();


            for (int i = 0; i < 10000; i++)
            {
                
                if (i%100==1)
                    Console.WriteLine($"E: {tl.Enqueued} P: {tl.Processed}, LS: {tl.ProcessedLastSecond}");

                tl.Enqueue(Guid.NewGuid().ToString());
                tl.Enqueue(Guid.NewGuid().ToString());
                tl.Enqueue(Guid.NewGuid().ToString());
                tl.Enqueue(Guid.NewGuid().ToString());
                tl.Enqueue(Guid.NewGuid().ToString());
                tl.Enqueue(Guid.NewGuid().ToString());
                tl.Enqueue(Guid.NewGuid().ToString());
                tl.Enqueue(Guid.NewGuid().ToString());
                tl.Enqueue(Guid.NewGuid().ToString());
                tl.Enqueue(Guid.NewGuid().ToString());
            }

            tl.WaitAllAndStop(() =>
            {
                Console.WriteLine($"E: {tl.Enqueued} P: {tl.Processed}, LS: {tl.ProcessedLastSecond}");
            });

            var ts = Execution.Time(() =>
            {
                Thread.Sleep(1000);
            }, (ts) =>
            {
                Console.WriteLine($"#1 time {ts.TotalMilliseconds}");
            });
            Console.WriteLine($"#2 time {ts.TotalMilliseconds}");
        }
    }
}
