using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;

namespace Mystery_App_5
{
   
    public class AMysteryClass
    {
        private readonly int _delay;

        public AMysteryClass(int delay) => _delay = delay;
        ~AMysteryClass() => Thread.Sleep(_delay);
    }
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine($"Process ID: {Process.GetCurrentProcess().Id}");
            Console.Write("Press any key to start the process. Attach WinDBG before continuing..");
            Console.ReadKey();
            
            Console.WriteLine("Running...");
            var mre = new ManualResetEventSlim();
            var task = Task.Run(() =>
            {
                int x = 0;
                var random = new Random();
                while (!mre.IsSet)
                {
                    Task.Run(() =>
                    {
                        var _ = new AMysteryClass(random.Next(100, 5000));
                    });
                }
            });
            
            //166x86
            Console.ReadKey();
            mre.Set();

            Task.WaitAll(task);
            Console.WriteLine("OK, bye!");
        }
    }
}
