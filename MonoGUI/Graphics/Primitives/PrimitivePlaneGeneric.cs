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
 * File:		PrimitivePlaneGeneric
 * Purpose:		Class for rendering a plane (with 16 bits indexing)
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2010-06-23  RW	
 * 
 * History:		2010-06-23  RW  Created
 * 
 */
namespace MonoGUI.Graphics
{      

    /// <summary>
    /// Class for rendering a plane (with 16 bits indexing)
    /// </summary>
    public class PrimitivePlaneGeneric<T> where T : struct, IPlaneVertex
    {

        #region Private members

        private int fNumberOfSegments;
        private int fNumberOfPointsOnLine;
        private int fNumberOfLines;
        private int fNumberOfTriangles;
        private int fNumberOfVertices;

        private T[] fVertices;
        private short[] fIndicesForLines;
        private short[] fIndicesForTriangles;

        #endregion

        #region Constructor & Destructor

        /// <summary>
        /// Create a plane
        /// </summary>
        /// <param name="graphicsDevice">The Graphics Device object to use.</param>
        public PrimitivePlaneGeneric(GraphicsDevice graphicsDevice,
            int segments,
            float minX, float maxX, float minZ, float maxZ, float defaultY)
            : this(graphicsDevice, segments, minX, maxX, minZ, maxZ, defaultY, 1.0f)
        {
        }

        /// <summary>
        /// Create a plane
        /// </summary>
        /// <param name="graphicsDevice">The Graphics Device object to use.</param>
        public PrimitivePlaneGeneric(GraphicsDevice graphicsDevice,
        int segments,
            float minX, float maxX, float minZ, float maxZ, float defaultY,
            float textureScale)
        {
            InitPlane(segments, minX, maxX, minZ, maxZ, defaultY, textureScale);
        }

        /// <summary>
        /// Called when the primive line object is destroyed.
        /// </summary>
        ~PrimitivePlaneGeneric()
        {
        }
        
        #endregion

        #region Private methods
        
        /// <summary>
        /// Init the plane
        /// </summary>
        /// <param name="segments"></param>
        /// <param name="minX"></param>
        /// <param name="maxX"></param>
        /// <param name="minZ"></param>
        /// <param name="maxZ"></param>
        /// <param name="defaultY"></param>
        /// <param name="color"></param>
        private void InitPlane(
            int segments,
            float minX, float maxX, float minZ, float maxZ, float defaultY,
            float textureScale)
        {
            if (segments > 180)
                throw new Exception("To many segments");
            
            fNumberOfSegments = segments;
            fNumberOfPointsOnLine = segments + 1;
            fNumberOfVertices = fNumberOfPointsOnLine * fNumberOfPointsOnLine;
            fNumberOfLines = 2 * fNumberOfSegments * fNumberOfPointsOnLine;
            fNumberOfTriangles = 2 * fNumberOfSegments * fNumberOfSegments;
            int index;

            // Init points
            fVertices = new T[fNumberOfVertices];
            
            Vector3 position = new Vector3();
            position.Y = defaultY;
            float deltaZ = maxZ - minZ;
            float deltaX = maxX - minX;
            for (int z = 0; z <= fNumberOfSegments; z++)
            {
                float factorZ = (float)z / fNumberOfSegments;
                position.Z = minZ + factorZ * deltaZ;
                for (int x = 0; x <= fNumberOfSegments; x++)
                {
                    float factorX = (float)x / fNumberOfSegments;
                    position.X = minX + factorX * deltaX;
                    index = GetIndex(x, z);
                    T vertex = new T();
                    vertex.VertexPosition = position;
                    vertex.VertexNormal = Vector3.Zero;
                    vertex.TextureCoordinate = new Vector2(factorX * textureScale, factorZ * textureScale);
                    fVertices[index] = vertex;                    
                }
            }

            // Init indices for lines
            fIndicesForLines = new short[2*fNumberOfLines];
            index = 0;
            for (int z = 0; z < fNumberOfPointsOnLine; z++)
            {
                for (int x = 0; x < fNumberOfSegments; x++)
                {
                    fIndicesForLines[index++] = GetIndex(x, z);
                    fIndicesForLines[index++] = GetIndex(x + 1, z);
                }
            }
            for (int z = 0; z < fNumberOfSegments; z++)
            {
                for (int x = 0; x < fNumberOfPointsOnLine; x++)
                {
                    fIndicesForLines[index++] = GetIndex(x, z);
                    fIndicesForLines[index++] = GetIndex(x, z + 1);
                }
            }

            // Init indices for triangles
            fIndicesForTriangles = new short[3 * fNumberOfTriangles];
            index = 0;
            for (int z = 0; z < fNumberOfSegments; z++)
            {
                for (int x = 0; x < fNumberOfSegments; x++)
                {
                    short i00 = GetIndex(x, z);
                    short i10 = GetIndex(x + 1, z);
                    short i01 = GetIndex(x, z + 1);
                    short i11 = GetIndex(x+1, z+1);
                    fIndicesForTriangles[index++] = i00;
                    fIndicesForTriangles[index++] = i10;
                    fIndicesForTriangles[index++] = i01;
                    fIndicesForTriangles[index++] = i10;
                    fIndicesForTriangles[index++] = i11;
                    fIndicesForTriangles[index++] = i01;
                }
            }
        }

        /// <summary>
        /// Get index to a coordinate
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private short GetIndex(int x, int y)
        {
            return (short)(y * fNumberOfPointsOnLine + x);
        }

        #endregion

        #region Render methods

        /// <summary>
        /// Render the plane as squares / lines
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use to render the object.</param>
        public void RenderLines(SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.DrawUserIndexedPrimitives<T>(
                PrimitiveType.LineList,
               fVertices,
                0,  // vertex buffer offset to add to each element of the index buffer
                fNumberOfVertices,  // number of vertices in pointList
                fIndicesForLines,  // the index buffer
                0,  // first index element to read
                fNumberOfLines  // number of primitives to draw
                );
        }

        /// <summary>
        /// Render the plane as triangles
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use to render the object.</param>
        public void RenderTriangles(SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.DrawUserIndexedPrimitives<T>(
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
        /// Get a point in the plane
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public T GetPoint(int x, int z)
        {
            return fVertices[GetIndex(x, z)];
        }

        /// <summary>
        /// Set a point on the plane
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="point"></param>
        public void SetPoint(int x, int z, T point)
        {
            fVertices[GetIndex(x, z)] = point;
        }

        /// <summary>
        /// Set the height of a point on the plabe
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="y"></param>
        public void SetHeight(int x, int z, float y)
        {
            int index = GetIndex(x, z);
            Vector3 position = fVertices[index].VertexPosition;
            position.Y = y;
            fVertices[index].VertexPosition = position;
        }

        /// <summary>
        /// Generate normals of the plane
        /// </summary>
        public void GenerateNormals()
        {
            for (int i = 0; i < fNumberOfVertices; i++)
                fVertices[i].VertexNormal = new Vector3(0, 0, 0);

            int index = 0;
            for (int i = 0; i < fNumberOfTriangles; i++)
            {
                Vector3 vector1 = fVertices[fIndicesForTriangles[index + 1]].VertexPosition -
                    fVertices[fIndicesForTriangles[index]].VertexPosition;
                Vector3 vector2 = fVertices[fIndicesForTriangles[index]].VertexPosition -
                    fVertices[fIndicesForTriangles[index + 2]].VertexPosition;
                Vector3 normal = Vector3.Cross(vector1, vector2);
                normal.Normalize();
                fVertices[fIndicesForTriangles[index++]].VertexNormal += normal;
                fVertices[fIndicesForTriangles[index++]].VertexNormal += normal;
                fVertices[fIndicesForTriangles[index++]].VertexNormal += normal;
            }

            for (int i = 0; i < fNumberOfVertices; i++)
                fVertices[i].VertexNormal.Normalize();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the number of segments of the plane
        /// </summary>
        public int Segments
        {
            get
            {
                return fNumberOfSegments;
            }
        }

        #endregion
  
    }


}
