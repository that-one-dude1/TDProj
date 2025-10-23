using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TDProj
{
    public static class PathGenerator
    {
        public static List<Point> GenerateFullPath(List<Point> waypoints)
        {
            List<Point> fullPath = new();

            for (int i = 0; i < waypoints.Count - 1; i++)
            {
                Point start = waypoints[i];
                Point end = waypoints[i + 1];

                if (start.Y == end.Y)
                {
                    int step = start.X < end.X ? 1 : -1;
                    for (int x = start.X; x != end.X + step; x += step)
                        fullPath.Add(new Point(x, start.Y));
                }
                else if (start.X == end.X)
                {
                    int step = start.Y < end.Y ? 1 : -1;
                    for (int y = start.Y; y != end.Y + step; y += step)
                        fullPath.Add(new Point(start.X, y));
                }
                else
                {
                    throw new Exception("Path segments must be straight lines (no diagonals).");
                }
            }

            return fullPath;
        }
    }
}
