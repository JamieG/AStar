using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CapitalStaging
{
    using System.Drawing;
    using System.Drawing.Imaging;

    class Program
    {
        static void Main(string[] args)
        {
            var timer = new Stopwatch();

            var times = new List<long>();
            var stepTimes = new List<double>();

            int i = 1;
            short width = 100;
            short height = 100;
            int runs = 50;

            var grid = new Grid(width, height);

            var rnd = new Random();

            for (int x = 0; x < runs; x++)
            {
                timer.Start();

                Node start = new Node((short)(rnd.Next(1, width)), (short)(rnd.Next(1, height)));
                Node goal = new Node((short)(rnd.Next(1, width)), (short)(rnd.Next(1, height)));

                //Node start = new Node(10, 10);
                //Node goal = new Node(90, 90);

                var search = new AStarSearch(grid);

                IList<Node> path = search.Find(start, goal);

                if (!path.Any() || !path.Last().Equals(goal) || !path.First().Equals(start))
                    throw new Exception("Failure");

                RenderPath(width, height, start, goal, path, search);

                timer.Stop();

                times.Add(timer.ElapsedMilliseconds);
                stepTimes.Add((double)timer.ElapsedMilliseconds / path.Count);

                Console.SetCursorPosition(0, 0);
                Console.WriteLine("Start: {0} Goal: {1}", start, goal);
                Console.SetCursorPosition(0, 1);
                Console.WriteLine("Step: {0:0000} - {1:000.00000000}ms - {2:p}", path.Count, timer.ElapsedMilliseconds, (double)(i++) / runs);
                Console.SetCursorPosition(0, 2);
                Console.WriteLine("Elapsed: Avg: {0:000}ms Min: {1:000}ms Max: {2:000}ms StepAvg:{3:0.000000}ms", times.Average(), times.Min(), times.Max(), stepTimes.Average());
                timer.Reset();
            }

            Console.ReadKey();
        }

        private static void RenderPath(int width, int height, Node start, Node goal, IList<Node> path, AStarSearch search)
        {
            int scalar = 10;

            using (var image = new Bitmap(width*scalar, height*scalar, PixelFormat.Format32bppArgb))
            using (var graphics = Graphics.FromImage(image))
            {
                graphics.Clear(Color.Black);

                foreach (Node closedNode in search.Closed.Values)
                    CircleAtPoint(graphics, new PointF(closedNode.X*scalar, closedNode.Y*scalar), 2, Color.DimGray);

                graphics.DrawLines(new Pen(new SolidBrush(Color.Cornsilk)), path.Select(n => new PointF(n.X*scalar, n.Y*scalar)).ToArray());

                CircleAtPoint(graphics, new PointF(start.X*scalar, start.Y*scalar), 2, Color.Red);
                CircleAtPoint(graphics, new PointF(goal.X*scalar, goal.Y*scalar), 2, Color.Green);
               
                image.Save(string.Format("Paths\\{0}_{1}-{2}_{3}.png", start.X, start.Y, goal.X, goal.Y), ImageFormat.Png);
            }
        }

        private static void CircleAtPoint(Graphics graphics, PointF center, float radius, Color color)
        {
            RectangleF shifted = new RectangleF(center.X - radius, center.Y - radius, radius*2, radius*2);
            graphics.DrawEllipse(new Pen(new SolidBrush(color)), shifted);
        }
    }
}
