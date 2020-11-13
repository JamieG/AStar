using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using AStar;

namespace AStarPathing
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var runs = 100;

            var image = (Bitmap) Image.FromFile("cavern.gif");

            var width = image.Width;
            var height = image.Height;

            var grid = new Grid(width, height);

            if (!Directory.Exists("Paths"))
                Directory.CreateDirectory("Paths");

            var timer = new Stopwatch();

            var times = new List<long>();
            var stepTimes = new List<double>();

            var search = new AStarSearch(grid);

            for (var runIndex = 0; runIndex < runs; runIndex++)
            {
                var start = new Vector2Int(10, 10);
                var goal = new Vector2Int (width - 10, height - 10);

                for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++) {
                    grid[x, y].Blocked = (image.GetPixel(x, y).R + image.GetPixel(x, y).G + image.GetPixel(x, y).B) / 3 < 128;
                }

                timer.Start();

                var path = search.Find(start, goal);
                timer.Stop();

                if (!path.Any() || !path.Last().Location.Equals(goal) || !path.First().Location.Equals(start))
                    throw new Exception("Failure");

                RenderPath(width, height, start, goal, path, search, grid, runIndex, timer.Elapsed);

                times.Add(timer.ElapsedMilliseconds);
                stepTimes.Add((double) timer.ElapsedMilliseconds / path.Length);

                Console.SetCursorPosition(0, 0);
                Console.WriteLine("Start: {0} Goal: {1}", start, goal);
                Console.SetCursorPosition(0, 1);
                Console.WriteLine("Step: {0:0000} - {1:000.00000000}ms - {2:p}", path.Length, timer.ElapsedMilliseconds,
                    (double) (runIndex + 1) / runs);

                timer.Reset();
            }

            Console.WriteLine("Elapsed: Avg: {0:000}ms Min: {1:000}ms Max: {2:000}ms StepAvg:{3:0.000000}ms",
                times.Average(), times.Min(), times.Max(), stepTimes.Average());

            Console.ReadKey();
        }

        private static void RenderPath(int width, int height, Vector2Int start, Vector2Int goal, IList<Cell> path,
            AStarSearch search, IGridProvider grid, int runIndex, TimeSpan elapsed)
        {
            var scalar = 10;

            var verdana = new FontFamily("Verdana");
            var statsFont = new Font(verdana, 36, FontStyle.Bold, GraphicsUnit.Pixel);

            using (var image = new Bitmap(width * scalar, height * scalar, PixelFormat.Format32bppArgb)) {
                using (var graphics = Graphics.FromImage(image)) {
                    graphics.Clear(Color.White);

                    int closedCount = 0;

                    for (var x = 0; x < width; x++) {
                        for (var y = 0; y < height; y++) {
                            if (grid[new Vector2Int(x, y)].Blocked) {
                                graphics.FillRectangle(new SolidBrush(Color.DarkGray), x * scalar - scalar / 2,
                                    y * scalar - scalar / 2, scalar, scalar);
                            } else if (grid[new Vector2Int(x, y)].Closed) {
                                closedCount++;
                                CircleAtPoint(graphics,
                                    new PointF(x * scalar, y * scalar), 4,
                                    Color.IndianRed);
                            }
                        }
                    }

                    graphics.DrawLines(new Pen(new SolidBrush(Color.LimeGreen), 8),
                        path.Select(n => new PointF(n.Location.X * scalar, n.Location.Y * scalar)).ToArray());

                    CircleAtPoint(graphics, new PointF(start.X * scalar, start.Y * scalar), 5, Color.Red);
                    CircleAtPoint(graphics, new PointF(goal.X * scalar, goal.Y * scalar), 5, Color.Green);

                    graphics.DrawString(
                        $"Elapsed: {elapsed.TotalMilliseconds:000}ms Closed: {closedCount:00000}",
                        statsFont, new SolidBrush(Color.Black), 2, height * scalar - (statsFont.GetHeight(graphics) + 2));

                    image.Save(
                        $"Paths\\{start.X}_{start.Y}-{goal.X}_{goal.Y}_{runIndex}.png",
                        ImageFormat.Png);
                }
            }
        }

        private static void CircleAtPoint(Graphics graphics, PointF center, float radius, Color color)
        {
            var shifted = new RectangleF(center.X - radius, center.Y - radius, radius * 2, radius * 2);
            graphics.FillEllipse(new SolidBrush(color), shifted);
        }
    }
}