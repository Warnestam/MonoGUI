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
 * File:		PlaneMultiTexture
 * Purpose:		Structure for a vertex containing: Position+Normal+Texture
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
    /// Structure for a vertex containing: Position+Normal+Color+Texture
    /// </summary>
    public struct PlaneMultiTexture : IPlaneVertex
    {

        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TextureCoordinate;
        public Vector4 TextureWeights;

        public PlaneMultiTexture(Vector3 position, Vector2 textureCoordinate, Vector4 textureWeights)
        {
            Position = position;
            Normal = Vector3.Zero;
            TextureCoordinate = textureCoordinate;
            TextureWeights = textureWeights;
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0*4, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(3*4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(6*4, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(8*4, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1)
        );

        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }


        #region IPlaneVertex Members

        Vector3 IPlaneVertex.VertexPosition
        {
            get
            {
                return Position;
            }
            set
            {
                Position = value;
            }
        }

        Vector3 IPlaneVertex.VertexNormal
        {
            get
            {
                return Normal;
            }
            set
            {
                Normal = value;
            }
        }

        Vector2 IPlaneVertex.TextureCoordinate
        {
            get
            {
                return TextureCoordinate;
            }
            set
            {
                TextureCoordinate = value;
            }
        }

        #endregion

    }


}
