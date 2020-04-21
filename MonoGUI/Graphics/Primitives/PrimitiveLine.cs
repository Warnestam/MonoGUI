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
 * Purpose:		Class for rendering lines (with 16 bits indexing)
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2010-02-28  RW	
 * 
 * History:		2010-02-28  RW  Created
 * 
 */
namespace MonoGUI.Graphics
{

    /// <summary>
    /// A class to make primitive objects out of lines. (with 16 bits indexing)
    /// </summary>
    public class PrimitiveLine
    {

        #region Private membets

        private const int DEFAULT_BUFFER_SIZE = 1024;
        private const int MAX_LINES = 16383;

        private int fLines;
        private int fMaxLines;
        private short fIndex;
        private bool fUseVertexBuffer;

        // For DrawUserIndexedPrimitives
        private VertexPositionColor[] fPointVertices;
        private short[] fPointIndices;
        // For DrawIndexedPrimitives
        private IndexBuffer fIndexBuffer;
        private VertexBuffer fVertexBuffer;
        private bool fVertexBufferLoaded;
        private GraphicsDevice fDevice;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current number of lines
        /// </summary>
        public int Lines
        {
            get
            {
                return fLines;
            }
        }

        /// <summary>
        /// Gets the current free number of lines
        /// </summary>
        public int LinesFree
        {
            get
            {
                return MAX_LINES - fLines;
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
            }
        }

        #endregion

        #region Constructor / destructor

        /// <summary>
        /// Creates a new primitive line object.
        /// </summary>
        /// <param name="graphicsDevice"></param>
        public PrimitiveLine(GraphicsDevice graphicsDevice)
            : this(graphicsDevice, DEFAULT_BUFFER_SIZE)
        {
            fDevice = graphicsDevice;
        }

        /// <summary>
        /// Creates a new primitive line object.
        /// </summary>
        /// <param name="graphicsDevice">The Graphics Device object to use.</param>
        public PrimitiveLine(GraphicsDevice graphicsDevice, int bufferSize)
        {
            fLines = 0;
            fIndex = 0;
            fMaxLines = bufferSize;
            fPointVertices = new VertexPositionColor[bufferSize * 2];
            fPointIndices = new short[bufferSize * 2];
        }

        /// <summary>
        /// Called when the primive line object is destroyed.
        /// </summary>
        ~PrimitiveLine()
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
            if (fLines >= fMaxLines)
                Expand();
            fPointVertices[fIndex] = point1;
            fPointIndices[fIndex] = fIndex++;
            fPointVertices[fIndex] = point2;
            fPointIndices[fIndex] = fIndex++;
            fLines++;
            fVertexBufferLoaded = false;
        }

        /// <summary>
        /// Clears all vectors from the primitive line object.
        /// </summary>
        public void Clear()
        {
            fIndex = 0;
            fLines = 0;
            fVertexBufferLoaded = false;
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
        /// Creates a circle 
        /// </summary>
        /// <param name="radius">The radius (half the width) of the circle.</param>
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

        #endregion

        #region Draw method

        /// <summary>
        /// Renders the primtive line object.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use to render the primitive line object.</param>
        public void Render()
        {
            if (fLines > 0)
            {
                if (fUseVertexBuffer)
                {
                    if (!fVertexBufferLoaded)
                        InitVertexBuffer();

                    fDevice.SetVertexBuffer(fVertexBuffer);
                    fDevice.Indices = fIndexBuffer;
                    fDevice.DrawIndexedPrimitives(
                        PrimitiveType.LineList,
                        0, // baseVertex
                        0, // startIndex
                        fLines); // primitiveCount                    
                }
                else
                {
                    fDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                       PrimitiveType.LineList,
                      fPointVertices,
                       0,  // vertex buffer offset to add to each element of the index buffer
                       fIndex,  // number of vertices in pointList
                       fPointIndices,  // the index buffer
                       0,  // first index element to read
                       fLines  // number of primitives to draw
                   );
                }
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Expand the internal list of lines
        /// </summary>
        private void Expand()
        {
            VertexPositionColor[] pointVertices = new VertexPositionColor[fMaxLines * 4];
            short[] pointIndices = new short[fMaxLines * 4];
            for (int i = 0; i < fMaxLines * 2; i++)
            {
                pointIndices[i] = fPointIndices[i];
                pointVertices[i] = fPointVertices[i];
            }
            fPointIndices = pointIndices;
            fPointVertices = pointVertices;
            fMaxLines = fMaxLines * 2;
        }

        /// <summary>
        /// Init the vertex buffer
        /// </summary>
        /// <param name="device"></param>
        private void InitVertexBuffer()
        {
            fVertexBufferLoaded = true;

            fIndexBuffer = new IndexBuffer(fDevice, IndexElementSize.SixteenBits, fIndex * 2, BufferUsage.None);
            fVertexBuffer = new VertexBuffer(fDevice, typeof(VertexPositionColor), fLines * 2, BufferUsage.None);

            fIndexBuffer.SetData<short>(fPointIndices, 0, fIndex);
            fVertexBuffer.SetData<VertexPositionColor>(fPointVertices, 0, fLines * 2);
        }

        #endregion

    }


}
