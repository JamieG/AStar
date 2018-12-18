using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace AStarPathing
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var i = 1;
            int width = 100;
            int height = 100;

            Bitmap image = (Bitmap)Image.FromFile("grid.png");

            width = image.Width;
            height = image.Height;

            var grid = new Grid(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grid[x,y].Blocked = image.GetPixel(x, y).R > 128;
                }
            }

            if (!Directory.Exists("Paths"))
                Directory.CreateDirectory("Paths");

            var timer = new Stopwatch();

            var times = new List<long>();
            var stepTimes = new List<double>();

            

            for (var x = 0; x < width; x++)
            {
                timer.Start();

                var start = grid[x, 0];

                if (grid.Collided(start.Location)) continue;

                var goal = grid[width - 1 - x, height - 1];

                if (grid.Collided(goal.Location)) continue;

                var search = new AStarSearch(grid);

                var path = search.Find(start, goal);

                if (!path.Any() || !path.Last().Equals(goal) || !path.First().Equals(start))
                    throw new Exception("Failure");

                RenderPath(width, height, start, goal, path, search, grid);

                timer.Stop();

                times.Add(timer.ElapsedMilliseconds);
                stepTimes.Add((double) timer.ElapsedMilliseconds / path.Length);

                Console.SetCursorPosition(0, 0);
                Console.WriteLine("Start: {0} Goal: {1}", start, goal);
                Console.SetCursorPosition(0, 1);
                Console.WriteLine("Step: {0:0000} - {1:000.00000000}ms - {2:p}", path.Length, timer.ElapsedMilliseconds,
                    (double) i++ / width);
                Console.SetCursorPosition(0, 2);
                Console.WriteLine("Elapsed: Avg: {0:000}ms Min: {1:000}ms Max: {2:000}ms StepAvg:{3:0.000000}ms",
                    times.Average(), times.Min(), times.Max(), stepTimes.Average());
                timer.Reset();
            }

            Console.ReadKey();
        }

        private static void RenderPath(int width, int height, Node start, Node goal, IList<Node> path,
            AStarSearch search, IGrid grid)
        {
            var scalar = 10;

            using (var image = new Bitmap(width * scalar, height * scalar, PixelFormat.Format32bppArgb))
            using (var graphics = Graphics.FromImage(image))
            {
                graphics.Clear(Color.DarkGray);

                for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    if (grid[x, y].Blocked)
                    {
                        graphics.FillRectangle(new SolidBrush(Color.Black), (x * scalar) - scalar / 2,
                            (y * scalar) - scalar / 2,
                            scalar, scalar);
                    }
                }

                foreach (var closedNode in search.Closed.Values)
                    CircleAtPoint(graphics, new PointF(closedNode.Location.X * scalar, closedNode.Location.Y * scalar),
                        2, Color.DimGray);

                graphics.DrawLines(new Pen(new SolidBrush(Color.LawnGreen), 2),
                    path.Select(n => new PointF(n.Location.X * scalar, n.Location.Y * scalar)).ToArray());

                CircleAtPoint(graphics, new PointF(start.Location.X * scalar, start.Location.Y * scalar), 2, Color.Red);
                CircleAtPoint(graphics, new PointF(goal.Location.X * scalar, goal.Location.Y * scalar), 2, Color.Green);
                
                image.Save(
                    string.Format("Paths\\{0}_{1}-{2}_{3}.png", start.Location.X, start.Location.Y, goal.Location.X,
                        goal.Location.Y), ImageFormat.Png);
            }
        }

        private static void CircleAtPoint(Graphics graphics, PointF center, float radius, Color color)
        {
            var shifted = new RectangleF(center.X - radius, center.Y - radius, radius * 2, radius * 2);
            graphics.DrawEllipse(new Pen(new SolidBrush(color)), shifted);
        }
    }
}