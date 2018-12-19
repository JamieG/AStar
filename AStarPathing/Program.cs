﻿using System;
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

            var image = (Bitmap) Image.FromFile("grid.png");

            var width = image.Width;
            var height = image.Height;

            var grid = new Grid(width, height);

            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
                grid[x, y].Blocked = image.GetPixel(x, y).R > 128;

            if (!Directory.Exists("Paths"))
                Directory.CreateDirectory("Paths");

            var timer = new Stopwatch();

            var times = new List<long>();
            var stepTimes = new List<double>();

            var start = grid[width / 2, 0];
            var goal = grid[width / 2, height - 1];

            for (var runIndex = 0; runIndex < runs; runIndex++)
            {
                timer.Start();

                var search = new AStarSearch(grid);


                var path = search.Find(start, goal);
                timer.Stop();

                if (!path.Any() || !path.Last().Equals(goal) || !path.First().Equals(start))
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

        private static void RenderPath(int width, int height, Cell start, Cell goal, IList<Cell> path,
            AStarSearch search, IGrid grid, int runIndex, TimeSpan elapsed)
        {
            var scalar = 10;

            var verdana = new FontFamily("Verdana");
            var statsFont = new Font(verdana, 36, FontStyle.Bold, GraphicsUnit.Pixel);

            using (var image = new Bitmap(width * scalar, height * scalar, PixelFormat.Format32bppArgb))
            using (var graphics = Graphics.FromImage(image))
            {
                graphics.Clear(Color.White);

                for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                    if (grid[x, y].Blocked)
                        graphics.DrawRectangle(new Pen(new SolidBrush(Color.Black)), x * scalar - scalar / 2,
                            y * scalar - scalar / 2, scalar, scalar);
                    else if (search.Data[x * width + y].Closed)
                        CircleAtPoint(graphics,
                            new PointF(x * scalar, y * scalar), 2,
                            Color.Gainsboro);

                graphics.DrawLines(new Pen(new SolidBrush(Color.LawnGreen), 2),
                    path.Select(n => new PointF(n.Location.X * scalar, n.Location.Y * scalar)).ToArray());

                CircleAtPoint(graphics, new PointF(start.Location.X * scalar, start.Location.Y * scalar), 2, Color.Red);
                CircleAtPoint(graphics, new PointF(goal.Location.X * scalar, goal.Location.Y * scalar), 2, Color.Green);

                graphics.DrawString(
                    $"Elapsed: {elapsed.TotalMilliseconds:000}ms Closed: {search.Data.Count(x => x.Closed):00000}",
                    statsFont, new SolidBrush(Color.Black), 2, height * scalar - (statsFont.GetHeight(graphics) + 2));

                image.Save(
                    $"Paths\\{start.Location.X}_{start.Location.Y}-{goal.Location.X}_{goal.Location.Y}_{runIndex}.png",
                    ImageFormat.Png);
            }
        }

        private static void CircleAtPoint(Graphics graphics, PointF center, float radius, Color color)
        {
            var shifted = new RectangleF(center.X - radius, center.Y - radius, radius * 2, radius * 2);
            graphics.DrawEllipse(new Pen(new SolidBrush(color)), shifted);
        }
    }
}