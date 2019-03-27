using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Mystery_App_3
{
  class Program
    {
        public class SomeObjectThatUsesLotsOfMemory
        {
            public string Foo { get; }  

            public SomeObjectThatUsesLotsOfMemory()
            {
                Foo = new string('a', 128);
            }
        }

        public class ApplicationObjectA
        {
            public int Number { get; set; }
        }
        
        public class ApplicationObjectB
        {
            public float Number { get; set; }
        }

        public class ApplicationObjectC
        {
            public float Number { get; set; }
        }

        static void Main(string[] args)
        {
            Console.WriteLine($"Process ID: {Process.GetCurrentProcess().Id}");
            Console.Write("Working, press any key to stop... (note: the full dump will be written immediately after stopping)");

            var refs = new List<object>();
            var mre = new ManualResetEventSlim();
            var i = 0;
            var task = Task.Run(() =>
            {
                while (!mre.IsSet)
                {
                    if (i % 5 == 0)
                        new ApplicationObjectB();
                    if (i % 8 == 0)
                        new ApplicationObjectA();

                    if (i % 10 == 0)
                        new ApplicationObjectC();

                    if(i % 50 == 0)
                        refs.Add(new SomeObjectThatUsesLotsOfMemory());

                    if (i % 10000 == 0)
                    {
                        GC.Collect(2, GCCollectionMode.Forced);
                        GC.WaitForPendingFinalizers();
                    }

                    i++;
                }

            });

            Console.ReadKey();
            mre.Set();

            Task.WaitAll(task);            
        }
    }
}
