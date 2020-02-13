using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Mystery_App_4
{
    class Program
    {
        private static void ThrowCorruptedStateException()
        {
            try
            {
                var ptr = new IntPtr(123);
                Marshal.StructureToPtr(123, ptr, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine($"Process ID: {Process.GetCurrentProcess().Id}");
            Console.Write("Press any key to start the process. Attach WinDBG before continuing..");
            Console.ReadKey();

            ThrowCorruptedStateException();

            Console.WriteLine("This should not be seen on screen, because the process will crash with AccessViolationException...");
        }
    }
}
