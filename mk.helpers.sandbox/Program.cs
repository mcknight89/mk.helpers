using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace mk.helpers.sandbox
{
    class Program
    {
        static void Main(string[] args)
        {

            using (var stream = File.OpenRead(@"C:\Users\mk\Pictures\2a398c06-lg.webp"))
            {
                var type = stream.ReadImageFormatFromHeader();

            }
                //var obj1 = new
                //{
                //    Name = "Mike",
                //    DOB = "22/11/89"
                //};
                //var obj2 = new
                //{
                //    Name = "Chris, adf ,qadfdaf ,ad,f, da \t\t",
                //    DOB = "10/01/20"
                //};




                //var res = "some shit".ToBase64();

                //var csv1 = obj1.ToCsv(true);

                //var csv = new[] { obj1, obj2 }.ToCsv(true);

                //Tasks.



                return;



            //var arr = new[]
            //{
            //    "a",
            //    "b",
            //    "c"
            //};

            //var dict = arr.Select((d, i) => new { d, i }).ToConcurrentDictionary(d => d.d, d => d.i);

            //var get = dict.TryGet("c");

            //var tl = new ThreadLord<string>((d) =>
            //{
            //    Thread.Sleep(10);
            //});

            //for (int i = 0; i < 10000; i++)
            //    tl.Enqueue(Guid.NewGuid().ToString());

            //tl.LimitQueue(1000).Start();


            //for (int i = 0; i < 10000; i++)
            //{
                
            //    if (i%100==1)
            //        Console.WriteLine($"E: {tl.Enqueued} P: {tl.Processed}, LS: {tl.ProcessedLastSecond}");

            //    tl.Enqueue(Guid.NewGuid().ToString());
            //    tl.Enqueue(Guid.NewGuid().ToString());
            //    tl.Enqueue(Guid.NewGuid().ToString());
            //    tl.Enqueue(Guid.NewGuid().ToString());
            //    tl.Enqueue(Guid.NewGuid().ToString());
            //    tl.Enqueue(Guid.NewGuid().ToString());
            //    tl.Enqueue(Guid.NewGuid().ToString());
            //    tl.Enqueue(Guid.NewGuid().ToString());
            //    tl.Enqueue(Guid.NewGuid().ToString());
            //    tl.Enqueue(Guid.NewGuid().ToString());
            //}

            //tl.WaitAllAndStop(() =>
            //{
            //    Console.WriteLine($"E: {tl.Enqueued} P: {tl.Processed}, LS: {tl.ProcessedLastSecond}");
            //});

            //var ts = Execution.Time(() =>
            //{
            //    Thread.Sleep(1000);
            //}, (ts) =>
            //{
            //    Console.WriteLine($"#1 time {ts.TotalMilliseconds}");
            //});
            //Console.WriteLine($"#2 time {ts.TotalMilliseconds}");
        }
    }
}
