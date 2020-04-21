using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoExperience.Engines.PolygonEngine
{

    public class MyPoint: IComparable
    {
        public double X { get; private set; }
        public double Y { get; private set; }

        public MyPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public int CompareTo(object obj)
        {
            MyPoint other = (MyPoint)obj;
            if (this.X < other.X)
            {
                return -1;
            }
            else if (this.X > other.X)
            {
                return 1;
            }
            else
            {
                if (this.Y < other.Y)
                {
                    return -1;
                }
                else if (this.Y > other.Y)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        public override string ToString()
        {
            return $"({X:F0},{Y:F0})";
        }

        public static MyPoint Add(MyPoint p1, MyPoint p2)
        {
            return new MyPoint(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static MyPoint Sub(MyPoint p1, MyPoint p2)
        {
            return new MyPoint(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static double Cross(MyPoint p1, MyPoint p2)
        {
            return p1.X * p2.Y - p1.Y * p2.X;
        }


    }

    
}
