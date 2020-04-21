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
 * File:		PrimitiveTriangles
 * Purpose:		Class for rendering triangles (with 16 bits indexing)
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2019-05-15  RW	
 * 
 * History:		2019-05-15  RW  Created
 * 
 */
namespace MonoGUI.Graphics
{

    /// <summary>
    /// Class for rendering triangles (with 16 bits indexing)
    /// </summary>
    public class PrimitiveTriangles
    {

        #region Private members

        private short fNumberOfTriangles;
        private short fNumberOfVertices;

        private VertexPositionNormalColor[] fVertices;
        private short[] fIndicesForTriangles;

        #endregion

        #region Constructor & Destructor

        /// <summary>
        /// Create a plane
        /// </summary>
        public PrimitiveTriangles()
        {
            fVertices = new VertexPositionNormalColor[32767];
            fIndicesForTriangles = new short[32767];
        }

        #endregion

        #region Public methods

        public int GetAvailableVertices()
        {
            return fVertices.Length - fNumberOfVertices;
        }

        public int GetAvailableTriangleIndices()
        {
            return fIndicesForTriangles.Length - fNumberOfTriangles * 3;
        }

        public short AddVertice(VertexPositionNormalColor vertex)
        {
            short index = fNumberOfVertices;
            fVertices[fNumberOfVertices++] = vertex;
            return index;
        }

        public short AddTriangle(short index1, short index2, short index3)
        {
            short index = (short)(fNumberOfTriangles * 3);
            fIndicesForTriangles[index] = index1;
            fIndicesForTriangles[index + 1] = index2;
            fIndicesForTriangles[index + 2] = index3;
            fNumberOfTriangles++;
            return index;
        }

        #endregion

        #region Render methods

        /// <summary>
        /// Render the plane as triangles
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use to render the object.</param>
        public void RenderTriangles(SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalColor>(
               PrimitiveType.TriangleList,
               fVertices,
               0,  // vertex buffer offset to add to each element of the index buffer
               fNumberOfVertices,  // number of vertices in pointList
               fIndicesForTriangles,  // the index buffer
               0,  // first index element to read
               fNumberOfTriangles  // number of primitives to draw
               );
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Generate normals of the plane
        /// </summary>
        public void GenerateNormals()
        {
            int index = 0;
            for (int i = 0; i < fNumberOfTriangles; i++)
            {
                Vector3 vector1 = fVertices[fIndicesForTriangles[index + 1]].Position - fVertices[fIndicesForTriangles[index]].Position;
                Vector3 vector2 = fVertices[fIndicesForTriangles[index]].Position -
                    fVertices[fIndicesForTriangles[index + 2]].Position;
                Vector3 normal = Vector3.Cross(vector1, vector2);
                normal.Normalize();
                fVertices[fIndicesForTriangles[index++]].Normal = normal;
                fVertices[fIndicesForTriangles[index++]].Normal = normal;
                fVertices[fIndicesForTriangles[index++]].Normal = normal;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the number of triangles
        /// </summary>
        public short NumberOfTriangles
        {
            get
            {
                return fNumberOfTriangles;
            }
        }

        /// <summary>
        /// Get the number of vertices
        /// </summary>
        public short NumberOfVertices
        {
            get
            {
                return fNumberOfVertices;
            }
        }

        #endregion

    }


}
