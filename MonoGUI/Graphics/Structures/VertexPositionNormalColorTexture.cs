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
 * File:		VertexPositionNormalColorTexture
 * Purpose:		Structure for a vertex containing: Position+Normal+Color+Texture
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
    public struct VertexPositionNormalColorTexture : IVertexType
    {

        public Vector3 Position;
        public Vector3 Normal;
        public Color Color;
        public Vector2 TextureCoordinate;

        public VertexPositionNormalColorTexture(Vector3 position, Color color, Vector2 textureCoordinate)
        {
            Position = position;
            Normal = Vector3.Zero;
            Color = color;
            TextureCoordinate = textureCoordinate;
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(24, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(28, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        );

        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }

    }


}
