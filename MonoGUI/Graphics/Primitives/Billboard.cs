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
 * File:		Billboard
 * Purpose:		Class for rendering billboards
 * 
 *              Billboard of the type BillboardMode.PointList requires a special shader since it 
 *              has the same coordinate for all corners of the quad
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2010-06-13  RW	
 * 
 * History:		2010-06-13  RW  Created
 * 
 */
namespace MonoGUI.Graphics
{

    public enum BillboardMode { PointList, Quad };

    /// <summary>
    /// Class for rendering billboards
    /// </summary>
    public class Billboard
    {
       
        #region Private membets

        private const int DEFAULT_BUFFER_SIZE = 1024;

        private const int MAX_OBJECTS = 5460;

        private int fObjects;
        private int fMaxObjects;
        private bool fUseVertexBuffer;
        private BillboardMode fMode = BillboardMode.PointList;

        private short fIndexVertice;
        private short fIndexIndice;
        private VertexBillboardParticle[] fVertices;
        private short[] fIndices;
        // For DrawIndexedPrimitives
        private IndexBuffer fIndexBuffer;
        private VertexBuffer fVertexBuffer;
        private bool fVertexBufferLoaded;
         
        private GraphicsDevice fDevice;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the maximum number of objects
        /// </summary>
        public static int MaxObjects
        {
            get
            {
                return MAX_OBJECTS;
            }
        }

        /// <summary>
        /// Gets the current number of objects
        /// </summary>
        public int NumberOfObjects
        {
            get
            {
                return fObjects;
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
                fVertexBufferLoaded = false;
            }
        }

        /// <summary>
        /// Get/set how billboard vertices should be created
        /// </summary>
        public BillboardMode Mode
        {
            get
            {
                return fMode;
            }
            set
            {
                fMode = value;
                Clear();
            }
        }

        #endregion

        #region Constructor / destructor

        /// <summary>
        /// Creates a new billboard object.
        /// </summary>
        /// <param name="graphicsDevice"></param>
        public Billboard(GraphicsDevice graphicsDevice)
            : this(graphicsDevice, DEFAULT_BUFFER_SIZE)
        { }

        /// <summary>
        /// Creates a new billboard object.
        /// </summary>
        /// <param name="graphicsDevice">The Graphics Device object to use.</param>
        public Billboard(GraphicsDevice graphicsDevice, int bufferSize)
        {
            fDevice = graphicsDevice;
            fObjects = 0;
            fIndexVertice = 0;
            fIndexIndice = 0;
            fMaxObjects = bufferSize;
            fVertices = new VertexBillboardParticle[bufferSize * 4];
            fIndices = new short[bufferSize * 6];
        }

        /// <summary>
        /// Called when the billboard object is destroyed.
        /// </summary>
        ~Billboard()
        {
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Add a new object
        /// </summary>
        public bool AddObject(Vector3 position, Color color, float size)
        {
            if (fObjects > MAX_OBJECTS)
                return false;
            if (fObjects >= fMaxObjects)
                Expand();
            int verticeNumberBase = fIndexVertice;

            if (fMode == BillboardMode.PointList)
            {
                fVertices[fIndexVertice++] = new VertexBillboardParticle(position, Vector2.Zero, size, color);
                fVertices[fIndexVertice++] = new VertexBillboardParticle(position, new Vector2(1, 0), size, color);
                fVertices[fIndexVertice++] = new VertexBillboardParticle(position, Vector2.One, size, color);
                fVertices[fIndexVertice++] = new VertexBillboardParticle(position, new Vector2(0, 1), size, color);
            }
            else
            {
                float size2 = size * 0.5f;
                fVertices[fIndexVertice++] = new VertexBillboardParticle(new Vector3(position.X - size2, position.Y - size2, position.Z), new Vector2(0, 1), size, color);
                fVertices[fIndexVertice++] = new VertexBillboardParticle(new Vector3(position.X + size2, position.Y - size2, position.Z), new Vector2(1, 1), size, color);
                fVertices[fIndexVertice++] = new VertexBillboardParticle(new Vector3(position.X + size2, position.Y + size2, position.Z), new Vector2(1, 0), size, color);
                fVertices[fIndexVertice++] = new VertexBillboardParticle(new Vector3(position.X - size2, position.Y + size2, position.Z), new Vector2(0, 0), size, color);
            }
            fIndices[fIndexIndice++] = (short)(0 + verticeNumberBase);
            fIndices[fIndexIndice++] = (short)(1 + verticeNumberBase);
            fIndices[fIndexIndice++] = (short)(2 + verticeNumberBase);
            fIndices[fIndexIndice++] = (short)(0 + verticeNumberBase);
            fIndices[fIndexIndice++] = (short)(2 + verticeNumberBase);
            fIndices[fIndexIndice++] = (short)(3 + verticeNumberBase);
            fObjects++;
            fVertexBufferLoaded = false;
            return true;
        }

        /// <summary>
        /// Clears all vectors from the billboard object.
        /// </summary>
        public void Clear()
        {
            fIndexVertice = 0;
            fIndexIndice = 0;
            fObjects = 0;
            fVertexBufferLoaded = false;
        }

        #endregion

        #region Draw method

        /// <summary>
        /// Renders the billboard object.
        /// </summary>
        public void Render()
        {
            if (fObjects > 0)
            {
                if (fUseVertexBuffer)
                {
                    if (!fVertexBufferLoaded)
                        InitVertexBuffer();
                    fDevice.SetVertexBuffer(fVertexBuffer);
                    fDevice.Indices = fIndexBuffer;
                    fDevice.DrawIndexedPrimitives( PrimitiveType.TriangleList, 0, 0, fIndexVertice, 0, 2 * fObjects);
                }
                else
                {
                    fDevice.DrawUserIndexedPrimitives<VertexBillboardParticle>(PrimitiveType.TriangleList, fVertices, 0, fIndexVertice, fIndices, 0, 2 * fObjects
                   );
                }
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Expand the internal list of billboards
        /// </summary>
        private void Expand()
        {
            fMaxObjects = fMaxObjects * 2;

            VertexBillboardParticle[] vertices = new VertexBillboardParticle[fMaxObjects * 4];
            short[] indices = new short[fMaxObjects * 6];
            for (int i = 0; i < fVertices.Length; i++)
            {
                vertices[i] = fVertices[i];
            }
            for (int i = 0; i < fIndices.Length; i++)
            {
                indices[i] = fIndices[i];
            }
            fIndices = indices;
            fVertices = vertices;

        }

        /// <summary>
        /// Init the vertex buffer
        /// </summary>
        /// <param name="device"></param>
        private void InitVertexBuffer()
        {
            fVertexBufferLoaded = true;
            fIndexBuffer = new IndexBuffer(fDevice, IndexElementSize.SixteenBits, fIndexIndice, BufferUsage.None);
            fVertexBuffer = new VertexBuffer(fDevice, typeof(VertexBillboardParticle), fIndexVertice, BufferUsage.None);
            fIndexBuffer.SetData<short>(fIndices, 0, fIndexIndice);
            fVertexBuffer.SetData<VertexBillboardParticle>(fVertices, 0, fIndexVertice);
            // In this case its slower to use a dynamic buffer
            //fIndexBuffer = new DynamicIndexBuffer(fDevice, IndexElementSize.SixteenBits, fIndexIndice, BufferUsage.WriteOnly);
            //fIndexBuffer.SetData(fIndices);
            //fVertexBuffer = new DynamicVertexBuffer(fDevice, VertexBillboardParticle.VertexDeclaration, fIndexVertice, BufferUsage.WriteOnly);
            //fVertexBuffer.SetData(fVertices);
        }

        #endregion

    }


}
