using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoExperience.Engines.PolygonEngine
{
    public class MyRectangle
    {

        public double OrigoX { get; set; }
        public double OrigoY { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Angle { get; set; }

        public MyRectangle(double x, double y, double w, double h, double v)
        {
            OrigoX = x;
            OrigoY = y;
            Width = w;
            Height = h;
            Angle = v;
        }

        public List<MyPoint> CalculateCorners()
        {
            double sinA = Math.Sin(Angle);
            double cosA = Math.Cos(Angle);
            double halfWidth = Width / 2;
            double halfHeight = Height / 2;
            double h2y = halfHeight * cosA;
            double h2x = halfHeight * sinA;
            double w2y = halfWidth * sinA;
            double w2x = -halfWidth * cosA;

            MyPoint p1 = new MyPoint(OrigoX + h2x + w2x, OrigoY + h2y + w2y);
            MyPoint p2 = new MyPoint(OrigoX + h2x - w2x, OrigoY + h2y - w2y);
            MyPoint p3 = new MyPoint(OrigoX - h2x - w2x, OrigoY - h2y - w2y);
            MyPoint p4 = new MyPoint(OrigoX - h2x + w2x, OrigoY - h2y + w2y);
            List<MyPoint> result = new List<MyPoint>();
            result.Add(p1);
            result.Add(p2);
            result.Add(p3);
            result.Add(p4);
            return result;
        }


    }
}
