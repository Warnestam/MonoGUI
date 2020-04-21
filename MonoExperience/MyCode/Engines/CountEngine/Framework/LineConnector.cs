using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// RW 2019-03-10


namespace MonoExperience.Engines.CountEngine
{


    class LineConnector
    {

        public List<Line> Lines = new List<Line>();


        public LineConnector()
        {
        }

  
        public List<Point> DistributePoints(int points, double startFactor)
        {
            if (startFactor < 0 || startFactor >= 1.0f)
                throw new Exception("startFactor out of range");

            List<Point> result = new List<Point>();

            double totalLength = CalculateLength();
            double delta = totalLength / points;
            double goalPosition = delta * startFactor;

            int currentLineIndex = 0;
            int currentPointIndex = 0;
            Point p0 = new Point();
            Point p1 = new Point();

            double currentPosition = 0;
            double currentLength = 0;
            double nextPosition = 0;

            bool pointsFound = false;
            for (int i = 0; i < points; i++)
            {
                while (!pointsFound)
                {
                    // Find two points
                    bool getNext = true;
                    while (getNext)
                    {
                        if (currentPointIndex == 0)
                        {
                            if (Lines[currentLineIndex].Points.Count > 1)
                            {
                                p0 = Lines[currentLineIndex].Points[currentPointIndex++];
                                p1 = Lines[currentLineIndex].Points[currentPointIndex++];
                                currentLength = CalculateLength(p0, p1);
                                getNext = false;
                            }
                            else
                                currentLineIndex++;
                        }
                        else
                        {
                            if (currentPointIndex < Lines[currentLineIndex].Points.Count)
                            {
                                p0 = p1;
                                p1 = Lines[currentLineIndex].Points[currentPointIndex++];
                                currentLength = CalculateLength(p0, p1);
                                getNext = false;
                            }
                            else
                            {
                                currentPointIndex = 0;
                                currentLineIndex++;
                            }
                        }
                    }
                    // We have two points
                    nextPosition = currentPosition + currentLength;
                    if (nextPosition > goalPosition)
                    {
                        pointsFound = true;
                    }
                    else
                    {
                        currentPosition = nextPosition;
                    }
                }
                // We have a position between p0 and p1 where we will hava a point
                // p0 is on currentPosition and p1 is on nextPosition, we are searching for the goal position
                double innerFactor = (goalPosition - currentPosition) / (nextPosition - currentPosition);
                Point innerPoint = CalculatePointBetween(p0, p1, innerFactor);
                result.Add(innerPoint);
                goalPosition += delta;
                if (goalPosition >= nextPosition)
                {
                    currentPosition = nextPosition;
                    pointsFound = false;
                }
                else
                {
                }
            }
            return result;
        }

        private double CalculateLength()
        {
            double total = 0;
            foreach (var line in Lines)
            {
                total += CalculateLength(line);
            }
            return total;
        }

        private double CalculateLength(Line line)
        {
            double total = 0;
            if (line.Points.Count > 1)
            {
                for (int i = 1; i < line.Points.Count; i++)
                {
                    var p0 = line.Points[i - 1];
                    var p1 = line.Points[i];
                    double length = Math.Sqrt((p1.X - p0.X) * (p1.X - p0.X) + (p1.Y - p0.Y) * (p1.Y - p0.Y));
                    total += CalculateLength(p0, p1);
                }
            }
            return total;
        }

        private double CalculateLength(Point p0, Point p1)
        {
            double result = 0;
            if (p0 != p1)
            {
                result = Math.Sqrt((p1.X - p0.X) * (p1.X - p0.X) + (p1.Y - p0.Y) * (p1.Y - p0.Y));
            }
            return result;
        }

        private Point CalculatePointBetween(Point p0, Point p1, double factor)
        {
            int dX = p1.X - p0.X;
            int dY = p1.Y - p0.Y;
            int x = Convert.ToInt32(p0.X + dX * factor);
            int y = Convert.ToInt32(p0.Y + dY * factor);
            return new Point(x, y);
        }
    }
}
