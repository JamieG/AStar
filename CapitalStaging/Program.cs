using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CapitalStaging
{
    class Program
    {
        static void Main(string[] args)
        {
            var timer = new Stopwatch();

            var times = new List<long>();
            var stepTimes = new List<double>();

            int i = 1;
            short width = 1024;
            short height = 1024;

            var grid = new Grid(width, height);

            for (int x = 0; x < width / 2; x++)
            {
                timer.Start();

                Node start = new Node((short)x, (short)x);
                Node goal = new Node((short)-x, (short)-x);

                var search = new AStarSearch(grid, start, goal);

                IList<Node> path = search.Find();

                if (!path.Any() || !path.Last().Equals(goal) || !path.First().Equals(start))
                    throw new Exception("Failure");

                timer.Stop();

                times.Add(timer.ElapsedMilliseconds);
                stepTimes.Add((double)timer.ElapsedMilliseconds/path.Count);

                Console.SetCursorPosition(0, 0);
                Console.WriteLine("Start: {0} Goal: {1}", start, goal);
                Console.SetCursorPosition(0, 1);
                Console.WriteLine("Step: {0:0000} - {1:000.00000000}ms - {2:p}", path.Count, timer.ElapsedMilliseconds, (double)(i++) / (width / 2f));
                Console.SetCursorPosition(0, 2);
                Console.WriteLine("Elapsed: Avg: {0:000}ms Min: {1:000}ms Max: {2:000}ms StepAvg:{3:0.000000}ms", times.Average(), times.Min(), times.Max(), stepTimes.Average());
                timer.Reset();
            }

            Console.ReadKey();
        }
    }
}
