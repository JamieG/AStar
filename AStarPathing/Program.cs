using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace AStarPathing
{
    class Program
    {
        static void Main(string[] args)
        {
            int i = 1;
            short width = 128;
            short height = 128;

            if (!Directory.Exists("Paths"))
                Directory.CreateDirectory("Paths");

            var timer = new Stopwatch();

            var times = new List<long>();
            var stepTimes = new List<double>();

            var grid = new Grid(width, height);

            for (int x = 0; x < width; x++)
            {
                timer.Start();

                Node start = grid[x, 0];

                if (grid.Collided(start.Location))
                {
                    //x--;
                    continue;
                }

                Node goal = grid[(width-1)-x, height-1];

                if (grid.Collided(goal.Location))
                {
                    //x--;
                    continue;
                }

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
                Console.WriteLine("Step: {0:0000} - {1:000.00000000}ms - {2:p}", path.Count, timer.ElapsedMilliseconds, (double)(i++) / width);
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

                foreach (Rectangle rectangle in Grid.Obstacles)
                    graphics.FillRectangle(new SolidBrush(Color.White), rectangle.X*scalar, rectangle.Y*scalar, rectangle.Width*scalar, rectangle.Height*scalar);

                foreach (Node closedNode in search.Closed.Values)
                    CircleAtPoint(graphics, new PointF(closedNode.Location.X*scalar, closedNode.Location.Y*scalar), 2, Color.DimGray);

                graphics.DrawLines(new Pen(new SolidBrush(Color.Cornsilk)), path.Select(n => new PointF(n.Location.X*scalar, n.Location.Y*scalar)).ToArray());

                CircleAtPoint(graphics, new PointF(start.Location.X*scalar, start.Location.Y*scalar), 2, Color.Red);
                CircleAtPoint(graphics, new PointF(goal.Location.X*scalar, goal.Location.Y*scalar), 2, Color.Green);

                

                image.Save(string.Format("Paths\\{0}_{1}-{2}_{3}.png", start.Location.X, start.Location.Y, goal.Location.X, goal.Location.Y), ImageFormat.Png);
            }
        }

        private static void CircleAtPoint(Graphics graphics, PointF center, float radius, Color color)
        {
            RectangleF shifted = new RectangleF(center.X - radius, center.Y - radius, radius*2, radius*2);
            graphics.DrawEllipse(new Pen(new SolidBrush(color)), shifted);
        }
    }
}
