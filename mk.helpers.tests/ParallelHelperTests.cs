using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mk.helpers.tests
{
    [TestClass]
    public class ParallelHelperTests
    {
        [TestMethod]
        public void While_LoopExecutesCorrectly()
        {
            int counter = 0;
            bool Condition() => counter < 10;
            void Body() => counter++;


            ParallelHelper.While(Condition, new ParallelOptions(), Body);

            Assert.IsTrue(counter > 5, "Counter should be incremented 5 times or more");
        }

        [TestMethod]
        public void While_LoopWithParallelOptionsExecutesCorrectly()
        {
            int counter = 0;
            bool Condition() => counter < 10;
            void Body() => counter++;

            ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
            ParallelHelper.While(Condition, options, Body);

            Assert.IsTrue(counter > 5, "Counter should be incremented 10 times or more");
        }

        
    }
}
