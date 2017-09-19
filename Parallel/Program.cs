using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelExperiments
{
    class Program
    {
        static Stopwatch timer = new Stopwatch();
        static void Main(string[] args)
        {
            int max = 10000000;
            var rando = new Random();
            var randoms = new List<int>();
            var doubles = new List<int>();
            var million = Enumerable.Range(0, max);
            TimeIt(() =>
            {
                for (int i = 0; i < max; i++)
                {
                    randoms.Add(rando.Next());
                }
            });
            TimeIt(() =>
            {
                million.Select(x => x * 2);
            });

            var parallelRandoms = new ConcurrentBag<int>();
            TimeIt(() => Parallel.For(0, max, x => parallelRandoms.Add(x)));
            parallelRandoms = new ConcurrentBag<int>();
            TimeIt(() => Parallel.ForEach(million, x => parallelRandoms.Add(x)));
            TimeIt(() => million.AsParallel().Select(x => x * 2));

            Console.WriteLine($"Writing a line from suplook + master");
            HashSet<int> toCheck = new HashSet<int>(randoms.Union(parallelRandoms));
            ConcurrentBag<int> toCheckParallel = new ConcurrentBag<int>(toCheck);
            var allNums = million.Union(doubles).ToList();
            Console.WriteLine("Lookups");
            TimeIt(() => Parallel.ForEach(allNums, x => toCheck.Contains(x)));
            TimeIt(() => Parallel.ForEach(allNums, x => toCheckParallel.Contains(x)));
            Console.WriteLine("Another write line from master to be tested with VS");

            Console.ReadKey();
        }

        static void TimeIt(Action action)
        {
            timer.Reset();
            timer.Start();
            action.Invoke();
            timer.Stop();
            Console.WriteLine($"Time Elapsed : {timer.ElapsedMilliseconds}");
        }

    }
}
