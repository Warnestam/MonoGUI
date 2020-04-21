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
 * File:		DynamicPrimitiveTriangles
 * Purpose:		Class for rendering a lot of triangles
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
    /// Class for rendering a lot of triangles
    /// </summary>
    public class DynamicPrimitiveTriangles
    {

        #region Private membets

        private List<PrimitiveTriangles> fTriangles = new List<PrimitiveTriangles>();

        #endregion

        #region Properties

        #endregion

        #region Constructor / destructor

        /// <summary>
        /// Creates a new dynamic primitive line object.
        /// </summary>
        public DynamicPrimitiveTriangles()
        {
            Clear();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Make sure we have room for some new data
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="points"></param>
        public bool Allocate(int vertices, int points)
        {
            bool moreSpaceAllocated = false;

            if (fTriangles.Count == 0)
            {
                fTriangles.Add(new PrimitiveTriangles());
                moreSpaceAllocated = true;
            }
            else
            {
                PrimitiveTriangles primitive = fTriangles[fTriangles.Count - 1];
                if (primitive.GetAvailableTriangleIndices() < points || primitive.GetAvailableVertices() < vertices)
                {
                    fTriangles.Add(new PrimitiveTriangles());
                    moreSpaceAllocated = true;
                }
            }
            return moreSpaceAllocated;
        }

        public short AddVertice(VertexPositionNormalColor vertex)
        {
            PrimitiveTriangles primitive = fTriangles[fTriangles.Count - 1];
            return primitive.AddVertice(vertex);
        }

        public short AddTriangle(short index1, short index2, short index3)
        {
            PrimitiveTriangles primitive = fTriangles[fTriangles.Count - 1];
            return primitive.AddTriangle(index1, index2, index3);
        }

        public short AddTriangle(int index1, int index2, int index3)
        {
            PrimitiveTriangles primitive = fTriangles[fTriangles.Count - 1];
            return primitive.AddTriangle((short)index1, (short)index2, (short)index3);
        }

        /// <summary>
        /// Clears all vectors from the dynamic object.
        /// </summary>
        public void Clear()
        {
            fTriangles.Clear();
        }

        /// <summary>
        /// Generate normals of the plane
        /// </summary>
        public void GenerateNormals()
        {
            foreach (var primitive in fTriangles)
            {
                primitive.GenerateNormals();
            }
        }

        #endregion

        private short GetIndexXZ(short[,] matrix, int x, int z,
            int segments,
            float minX, float minZ, float y, float deltaX, float deltaZ, Color color)
        {
            short index = matrix[x, z];
            if (index < 0)
            {
                float factorX = (float)x / segments;
                float factorZ = (float)z / segments;
                VertexPositionNormalColor vertex = new VertexPositionNormalColor();
                vertex.Position = new Vector3(minX + factorX * deltaX, y, minZ + factorZ * deltaZ);
                vertex.Normal = Vector3.Zero;
                vertex.Color = color;
                index = AddVertice(vertex);
                matrix[x, z] = index;
            }
            return index;
        }

        private short GetIndexXY(short[,] matrix, int x, int y,
            int segments,
            float minX, float minY, float z, float deltaX, float deltaY, Color color)
        {
            short index = matrix[x, y];
            if (index < 0)
            {
                float factorX = (float)x / segments;
                float factorY = (float)y / segments;
                VertexPositionNormalColor vertex = new VertexPositionNormalColor();
                vertex.Position = new Vector3(minX + factorX * deltaX, minY + factorY * deltaY, z);
                vertex.Normal = Vector3.Zero;
                vertex.Color = color;
                index = AddVertice(vertex);
                matrix[x, y] = index;
            }
            return index;
        }

        private short GetIndexYZ(short[,] matrix, int y, int z,
            int segments,
            float minY, float minZ, float x, float deltaY, float deltaZ, Color color)
        {
            short index = matrix[y, z];
            if (index < 0)
            {
                float factorY = (float)y / segments;
                float factorZ = (float)z / segments;
                VertexPositionNormalColor vertex = new VertexPositionNormalColor();
                vertex.Position = new Vector3(x, minY + factorY * deltaY, minZ + factorZ * deltaZ);
                vertex.Normal = Vector3.Zero;
                vertex.Color = color;
                index = AddVertice(vertex);
                matrix[y, z] = index;
            }
            return index;
        }

        private short[,] InitMatrix(int segments)
        {
            short[,] temp = new short[segments + 1, segments + 1];
            for (int ix = 0; ix <= segments; ix++)
            {
                for (int iz = 0; iz <= segments; iz++)
                {
                    temp[ix, iz] = -1;
                }
            }
            return temp;
        }

        #region Public methods: Add geometry

        public void AddPlaneXZ(int segments,
            float minX, float maxX, float minZ, float maxZ, float defaultY,
            Color color, bool reverse)
        {
            float deltaX = maxX - minX;
            float deltaZ = maxZ - minZ;
            short[,] temp = null;

            for (int z = 0; z < segments; z++)
            {
                float factorZ = (float)z / segments;
                for (int x = 0; x < segments; x++)
                {
                    if (Allocate(2, 4) || temp == null)
                    {
                        // space expanded and we have lost our old points
                        temp = InitMatrix(segments);
                    }
                    short i00 = GetIndexXZ(temp, x, z, segments, minX, minZ, defaultY, deltaX, deltaZ, color);
                    short i10 = GetIndexXZ(temp, x + 1, z, segments, minX, minZ, defaultY, deltaX, deltaZ, color);
                    short i01 = GetIndexXZ(temp, x, z + 1, segments, minX, minZ, defaultY, deltaX, deltaZ, color);
                    short i11 = GetIndexXZ(temp, x + 1, z + 1, segments, minX, minZ, defaultY, deltaX, deltaZ, color);
                    if (reverse)
                    {
                        AddTriangle(i00, i01, i10);
                        AddTriangle(i10, i01, i11);
                    }
                    else
                    {
                        AddTriangle(i00, i10, i01);
                        AddTriangle(i10, i11, i01);
                    }
                }
            }
        }

        public void AddPlaneXY(int segments,
            float minX, float maxX, float minY, float maxY, float defaultZ,
            Color color, bool reverse)
        {
            float deltaX = maxX - minX;
            float deltaY = maxY - minY;
            short[,] temp = null;

            for (int y = 0; y < segments; y++)
            {
                float factorY = (float)y / segments;
                for (int x = 0; x < segments; x++)
                {
                    if (Allocate(2, 4) || temp == null)
                    {
                        temp = InitMatrix(segments);
                    }
                    short i00 = GetIndexXY(temp, x, y, segments, minX, minY, defaultZ, deltaX, deltaY, color);
                    short i10 = GetIndexXY(temp, x, y + 1, segments, minX, minY, defaultZ, deltaX, deltaY, color);
                    short i01 = GetIndexXY(temp, x + 1, y, segments, minX, minY, defaultZ, deltaX, deltaY, color);
                    short i11 = GetIndexXY(temp, x + 1, y + 1, segments, minX, minY, defaultZ, deltaX, deltaY, color);
                    if (reverse)
                    {
                        AddTriangle(i00, i01, i10);
                        AddTriangle(i10, i01, i11);
                    }
                    else
                    {
                        AddTriangle(i00, i10, i01);
                        AddTriangle(i10, i11, i01);
                    }
                }
            }
        }

        public void AddPlaneYZ(int segments,
         float minY, float maxY, float minZ, float maxZ, float defaultX,
         Color color, bool reverse)
        {
            float deltaY = maxY - minY;
            float deltaZ = maxZ - minZ;
            short[,] temp = null;

            for (int z = 0; z < segments; z++)
            {
                float factorZ = (float)z / segments;
                for (int y = 0; y < segments; y++)
                {
                    if (Allocate(2, 4) || temp == null)
                    {
                        // space expanded and we have lost our old points
                        temp = InitMatrix(segments);
                    }
                    short i00 = GetIndexYZ(temp, y, z, segments, minY, minZ, defaultX, deltaY, deltaZ, color);
                    short i10 = GetIndexYZ(temp, y, z + 1, segments, minY, minZ, defaultX, deltaY, deltaZ, color);
                    short i01 = GetIndexYZ(temp, y + 1, z, segments, minY, minZ, defaultX, deltaY, deltaZ, color);
                    short i11 = GetIndexYZ(temp, y + 1, z + 1, segments, minY, minZ, defaultX, deltaY, deltaZ, color);
                    if (reverse)
                    {
                        AddTriangle(i00, i01, i10);
                        AddTriangle(i10, i01, i11);
                    }
                    else
                    {
                        AddTriangle(i00, i10, i01);
                        AddTriangle(i10, i11, i01);
                    }
                }
            }
        }

        public void AddBox(int segments, Vector3 position, float size, Color color)
        {
            float halfSize = size / 2;
            float minX = position.X - halfSize;
            float maxX = position.X + halfSize;
            float minY = position.Y - halfSize;
            float maxY = position.Y + halfSize;
            float minZ = position.Z - halfSize;
            float maxZ = position.Z + halfSize;
            AddBox(segments, minX, maxX, minY, maxY, minZ, maxZ, color);
        }
        public void AddBox(int segments,
         float minX, float maxX, float minY, float maxY, float minZ, float maxZ,
         Color color)
        {
            this.AddPlaneXZ(segments, minX, maxX, minZ, maxZ, maxY, color, true);
            this.AddPlaneXY(segments, minX, maxX, minY, maxY, maxZ, color, true);
            this.AddPlaneYZ(segments, minY, maxY, minZ, maxZ, maxX, color, true);
            this.AddPlaneXZ(segments, minX, maxX, minZ, maxZ, minY, color, false);
            this.AddPlaneXY(segments, minX, maxX, minY, maxY, minZ, color, false);
            this.AddPlaneYZ(segments, minY, maxY, minZ, maxZ, minX, color, false);
        }

        private int AddSphereVertex(Vector3 position, Vector3 origo, Color color)
        {
            VertexPositionNormalColor vertex = new VertexPositionNormalColor();
            vertex.Position = position + origo;
            vertex.Normal = Vector3.Zero;
            vertex.Color = color;
            int index = AddVertice(vertex);
            return index;
        }

        public void AddSphere(int tessellation, Vector3 position, float diameter, Color color)
        {
            int verticalSegments = tessellation;
            int horizontalSegments = tessellation * 2;
            float radius = diameter / 2;
            int vertices = 2 + (verticalSegments - 1) * horizontalSegments;
            int indices = 2 * 3 * horizontalSegments + 6 * (verticalSegments - 2) * horizontalSegments;
            Allocate(vertices, indices);

            // Start with a single vertex at the bottom of the sphere.
            int startIndex = AddSphereVertex(Vector3.Down * radius, position, color);
            int lastIndex = -1;
            // Create rings of vertices at progressively higher latitudes.
            for (int i = 0; i < verticalSegments - 1; i++)
            {
                float latitude = ((i + 1) * MathHelper.Pi / verticalSegments) - MathHelper.PiOver2;
                float dy = (float)Math.Sin(latitude);
                float dxz = (float)Math.Cos(latitude);
                // Create a single ring of vertices at this latitude.
                for (int j = 0; j < horizontalSegments; j++)
                {
                    float longitude = j * MathHelper.TwoPi / horizontalSegments;
                    float dx = (float)Math.Cos(longitude) * dxz;
                    float dz = (float)Math.Sin(longitude) * dxz;
                    Vector3 normal = new Vector3(dx, dy, dz);
                    AddSphereVertex(normal * radius, position, color);
                }
            }
            // Finish with a single vertex at the top of the sphere.
            lastIndex = AddSphereVertex(Vector3.Up * radius, position, color);
            // Create a fan connecting the bottom vertex to the bottom latitude ring.
            for (int i = 0; i < horizontalSegments; i++)
            {
                AddTriangle(startIndex + 0, startIndex + 1 + (i + 1) % horizontalSegments, startIndex + 1 + i);
            }
            // Fill the sphere body with triangles joining each pair of latitude rings.
            for (int i = 0; i < verticalSegments - 2; i++)
            {
                for (int j = 0; j < horizontalSegments; j++)
                {
                    int nextI = i + 1;
                    int nextJ = (j + 1) % horizontalSegments;
                    AddTriangle(startIndex + 1 + i * horizontalSegments + j, startIndex + 1 + i * horizontalSegments + nextJ, startIndex + 1 + nextI * horizontalSegments + j);
                    AddTriangle(startIndex + 1 + i * horizontalSegments + nextJ, startIndex + 1 + nextI * horizontalSegments + nextJ, startIndex + 1 + nextI * horizontalSegments + j);
                }
            }
            // Create a fan connecting the top vertex to the top latitude ring.
            for (int i = 0; i < horizontalSegments; i++)
            {
                int currentVertex = lastIndex + 1;
                AddTriangle(currentVertex-1, currentVertex - 2 - (i + 1) % horizontalSegments, currentVertex - 2 - i);
            }
        }

        #endregion

        #region Draw method

        /// <summary>
        /// Renders the dynamic primitive lines object.
        /// </summary>
        public void RenderTriangles(SpriteBatch spriteBatch)
        {
            foreach (var current in fTriangles)
                current.RenderTriangles(spriteBatch);
        }

        #endregion

        public int NumberOfTriangles
        {
            get
            {
                int count = 0;
                foreach (var primitive in fTriangles)
                {
                    count += primitive.NumberOfTriangles;
                }
                return count;
            }
        }

    }


}
