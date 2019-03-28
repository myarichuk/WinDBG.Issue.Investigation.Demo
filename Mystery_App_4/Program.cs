using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Mystery_App_4
{
    class Program
    {
        private static void TryToAccessInvalidMemory()
        {
            var ptr = Marshal.AllocHGlobal(1024);
            Marshal.WriteByte(ptr, 65536,123); //try to access unallocated memory            
        }

        static void Main(string[] args)
        {
            Console.WriteLine($"Process ID: {Process.GetCurrentProcess().Id}");
            Console.Write("Press any key to start the process. Attach WinDBG before continuing..");
            Console.ReadKey();

            TryToAccessInvalidMemory();

            Console.WriteLine("This should not be seen on screen, because the process will crash with AccessViolationException...");
        }
    }
}
