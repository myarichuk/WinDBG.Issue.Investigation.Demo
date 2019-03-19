using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Livelock
{
    //livelock demo
  class Program
    {
        public class Fork
        {
            public Philosopher Owner { get; set; }

            public event Action OnEating;

            public void SignalEating()
            {
                OnEating?.Invoke();
            }
        }

        public class Philosopher
        {
            private readonly ManualResetEventSlim _cancelEvent;
            public string Name { get; set; }
            public bool IsHungry { get; private set; }

            public Philosopher(string name, ManualResetEventSlim cancelEvent)
            {
                _cancelEvent = cancelEvent;
                Name = name ?? throw new ArgumentNullException(nameof(name));
                IsHungry = true;
            }

            public void EatWith(Fork fork, params Philosopher[] philosophers)
            {
                while (IsHungry && !_cancelEvent.IsSet)
                {
                    //don't have the spoon - patiently wait for spouse
                    if (fork.Owner != this)
                    {
                        Thread.Sleep(5);
                        continue;
                    }

                    //if spouse is hungry, insist on passing the spoon
                    if (philosophers.Any(p =>p.IsHungry))
                    {
                        fork.Owner = philosophers.FirstOrDefault(p =>p.IsHungry) ?? throw new InvalidDataException("philosophers collection to eat with should not be empty..");
                        continue;
                    }

                    //spouse wasn't hungry, finally I can eat, then give the spoon to spouse...
                    fork.SignalEating();
                    fork.Owner =  philosophers.FirstOrDefault(p =>p.IsHungry);
                }
            }
        }
        
        static void Main(string[] args)
        {
            Console.WriteLine($"Process ID: {Process.GetCurrentProcess().Id}");
            var mre = new ManualResetEventSlim();
            Console.Write("Working, press any key to stop...");

            const int philosophersCount = 3; //its enough to have 2!
            var philosophers = new List<Philosopher>();
            for(int i = 0; i < philosophersCount; i++)
                philosophers.Add(new Philosopher($"Philosopher{i + 1}", mre));

            var fork = new Fork { Owner = philosophers[0] };
            
            fork.OnEating += () => Console.WriteLine($"{fork.Owner.Name} is eating...");

            var threads = new List<Thread>();
            for (int i = 0; i < philosophersCount; i++)
            {
                var philosopher = philosophers[i];
                threads.Add(new Thread(() => philosopher.EatWith(fork, philosophers.Where(p => p != philosopher).ToArray())));
                threads[i].Start(); 
            }

            Console.ReadKey();
            mre.Set();

            for (int i = 0; i < philosophersCount; i++)
            {
                threads[i].Join(2000);
            }

            Console.WriteLine("done.");
        }
    }
}
