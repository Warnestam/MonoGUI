using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


/*
 * File:		PrimitiveLine32
 * Purpose:		Class for rendering unlimited lines (with 16 bits indexing)
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2010-06-21  RW	
 * 
 * History:		2010-06-21  RW  Created
 * 
 */
namespace MonoGUI.Graphics
{

    /// <summary>
    /// A class to make primitive objects out of lines. (with 16 bits indexing)
    /// </summary>
    public class DynamicPrimitiveLine
    {

        #region Private membets

        private List<PrimitiveLine> fLines = new List<PrimitiveLine>();

        private GraphicsDevice fDevice;
        private bool fUseVertexBuffer;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current number of lines
        /// </summary>
        public int Lines
        {
            get
            {
                int count = 0;
                foreach (PrimitiveLine lines in fLines)
                    count += lines.Lines; ;
                return count;
            }
        }

        /// <summary>
        /// Get/set if should be using the graphic card to store the primitives
        /// </summary>
        public bool UseVertexBuffer
        {
            get
            {
                return fUseVertexBuffer;
            }
            set
            {
                fUseVertexBuffer = value;
                foreach (PrimitiveLine lines in fLines)
                    lines.UseVertexBuffer = value;
            }
        }

        #endregion

        #region Constructor / destructor

        /// <summary>
        /// Creates a new dynamic primitive line object.
        /// </summary>
        /// <param name="graphicsDevice"></param>
        public DynamicPrimitiveLine(GraphicsDevice graphicsDevice)
        {
            fDevice = graphicsDevice;
            Clear();
        }

        /// <summary>
        /// Called when the dynamic primitive line object is destroyed.
        /// </summary>
        ~DynamicPrimitiveLine()
        {
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Add a new line
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        public void AddLine(VertexPositionColor point1, VertexPositionColor point2)
        {
            PrimitiveLine lines;
            if (fLines.Count == 0)
            {
                lines = new PrimitiveLine(fDevice);
                lines.UseVertexBuffer = fUseVertexBuffer;
                fLines.Add(lines);
            }
            else
            {
                lines = fLines[fLines.Count - 1];
                if (lines.LinesFree < 1)
                {
                    lines = new PrimitiveLine(fDevice);
                    lines.UseVertexBuffer = fUseVertexBuffer;
                    fLines.Add(lines);
                }
            }
            lines.AddLine(point1, point2);
        }

        /// <summary>
        /// Creates a circle starting from 0, 0.
        /// </summary>
        /// <param name="radius">The radius (half the width) of the circle.</param>
        /// <param name="sides">The number of sides on the circle (the more the detailed).</param>
        public void AddCircle(float x, float y, float z, float radius, int sides, Color color)
        {
            float max = 2 * (float)Math.PI;
            float step = max / (float)sides;

            VertexPositionColor p0 = new VertexPositionColor();
            VertexPositionColor p1 = new VertexPositionColor();
            VertexPositionColor p2 = new VertexPositionColor();
            bool isFirst = true;
            for (float theta = 0; theta < max; theta += step)
            {
                VertexPositionColor p = new VertexPositionColor(new Vector3(
                    x + radius * (float)Math.Cos((double)theta),
                    y + radius * (float)Math.Sin((double)theta),
                    z
                    ), color);

                if (isFirst)
                {
                    isFirst = false;
                    p0 = p;
                    p1 = p;
                }
                else
                {
                    p2 = p1;
                    p1 = p;
                    this.AddLine(p2, p1);
                }
            }
            // then add the first vector again so it's a complete loop
            this.AddLine(p1, p0);
        }


        /// <summary>
        /// Creates a circle starting from 0, 0.
        /// </summary>
        /// <param name="radius">The radius (half the width) of the circle.</param>
        /// <param name="sides">The number of sides on the circle (the more the detailed).</param>
        public void AddCircle(float x, float y, float z, float radiusStart, float radiusStop,
            int sides, Color colorStart, Color colorStop)
        {
            float max = 2.0f * (float)Math.PI;
            float step = max / (float)sides;

            VertexPositionColor p0 = new VertexPositionColor();
            VertexPositionColor p1 = new VertexPositionColor();
            VertexPositionColor p2 = new VertexPositionColor();
            bool isFirst = true;
            for (int i = 0; i <= sides; i++)
            {
                //for (float theta = 0; theta < max; theta += step)
                float f = (float)i / (float)sides;
                float theta = 2.0f * (float)Math.PI * f;

                float rf = radiusStart + (radiusStop - radiusStart) * f;

                float f1 = f * 2.0f;
                if (f1 > 1.0f)
                    f1 = 2.0f - f1;
                float f2 = 1.0f - f1;

                int r = Convert.ToInt16(colorStart.R * f2 + colorStop.R * f1);
                int g = Convert.ToInt16(colorStart.G * f2 + colorStop.G * f1);
                int b = Convert.ToInt16(colorStart.B * f2 + colorStop.B * f1);

                Color color = new Color(r, g, b, 255);

                VertexPositionColor p = new VertexPositionColor(new Vector3(
                    x + rf * (float)Math.Cos((double)theta),
                    y + rf * (float)Math.Sin((double)theta),
                    z
                    ), color);

                if (isFirst)
                {
                    isFirst = false;
                    //p0 = p;
                    p1 = p;
                }
                else
                {
                    p2 = p1;
                    p1 = p;
                    this.AddLine(p2, p1);
                }
            }
            //this.AddLine(p1, p0);
        }

        /// <summary>
        /// Creates a circle 
        /// </summary>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="sides">The number of sides on the circle (the more the detailed).</param>
        public void AddCircle(float x, float y, float z, float radius, int sides, Color color,
            float angle1, float angle2)
        {
            float max = 2 * (float)Math.PI;
            float step = max / (float)sides;

            VertexPositionColor p0 = new VertexPositionColor();
            VertexPositionColor p1 = new VertexPositionColor();
            VertexPositionColor p2 = new VertexPositionColor();
            bool isFirst = true;
            for (float theta = 0; theta < max; theta += step)
            {
                Vector3 position = new Vector3(
                    radius * (float)Math.Cos((double)theta),
                    radius * (float)Math.Sin((double)theta),
                    0
                    );

                Matrix rotationMatrix = Matrix.CreateRotationX(angle1);
                rotationMatrix *= Matrix.CreateRotationY(angle2);
                position = Vector3.Transform(position, rotationMatrix) + new Vector3(x, y, z);


                VertexPositionColor p = new VertexPositionColor(position, color);
                if (isFirst)
                {
                    isFirst = false;
                    p0 = p;
                    p1 = p;
                }
                else
                {
                    p2 = p1;
                    p1 = p;
                    this.AddLine(p2, p1);
                }
            }
            // then add the first vector again so it's a complete loop
            this.AddLine(p1, p0);
        }

        /// <summary>
        /// Clears all vectors from the dynamic lines object.
        /// </summary>
        public void Clear()
        {
            fLines.Clear();
        }

        #endregion

        #region Draw method

        /// <summary>
        /// Renders the dynamic primitive lines object.
        /// </summary>
        public void Render()
        {
            foreach (PrimitiveLine lines in fLines)
                lines.Render();
        }

        #endregion

    }


}
