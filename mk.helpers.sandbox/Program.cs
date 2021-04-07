using System;

namespace mk.helpers.sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            StaticData.Add("SomeInt", "1");

            int? i = StaticData.GetInt("SomeInt");
        }
    }
}
