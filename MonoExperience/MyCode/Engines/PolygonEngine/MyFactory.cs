using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// RW 20200322

namespace MonoExperience.Engines.PolygonEngine
{
    public static class MyFactory
    {

        public static List<MyRectangle> GetRandomRectangles()
        {
            Random rnd = new Random();

            List<MyRectangle> result = new List<MyRectangle>();
            int numRectangles = rnd.Next(30) + 1;
            int minX = -200;
            int maxX = 200;
            int minY = -200;
            int maxY = 200;
            int minSize = 10;
            int maxSize = 100;

            
            for (int i = 0; i < numRectangles; i++)
            {
                double origoX = minX + (maxX - minX) * rnd.NextDouble();
                double origoY = minY + (maxY - minY) * rnd.NextDouble();
                double width = minSize + (maxSize - minSize) * rnd.NextDouble();
                double height = minSize + (maxSize - minSize) * rnd.NextDouble();
                double angle = 2 * Math.PI * rnd.NextDouble();
                result.Add(new MyRectangle(origoX, origoY, width, height, angle));
            }
            
            //double f = 30;// 30.0f;
            //double ox =  -400;
            //double oy = -200;
            //result.Add(new MyRectangle(f * 4.0 + ox, f * 7.5 + oy, f * 6.0, f * 3.0, 0.0 * Math.PI / 180));
            //result.Add(new MyRectangle(f * 8.0 + ox, f * 11.5 + oy, f * 6.0, f * 3.0, 0.0 * Math.PI / 180));
            //result.Add(new MyRectangle(f * 9.5 + ox, f * 6.0 + oy, f * 6.0, f * 3.0, 90.0 * Math.PI / 180));
            //result.Add(new MyRectangle(f * 4.5 + ox, f * 3.0 + oy, f * 4.4721, f * 2.2361, 26.565 * Math.PI / 180));




            return result;
        }

    }
}
