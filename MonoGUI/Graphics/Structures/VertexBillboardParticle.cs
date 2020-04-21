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
 * File:		VertexPointSprite
 * Purpose:		Structure for a vertex containing: Position+Size
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2010-03-05  RW	
 * 
 * History:		2010-03-05  RW  Created
 * 
 */
namespace MonoGUI.Graphics
{

    /// <summary>
    /// Structure for a vertex containing: Position+Normal+Color
    /// </summary>
    public struct VertexBillboardParticle : IVertexType
    {

        
        public Vector3 Position;
        public Vector2 TextureCoordinate;
        public Color Color;
        public float Size;       

        public VertexBillboardParticle(Vector3 position, Vector2 textureCoordinate, float spriteSize, Color color)
        {
            Position = position;
            TextureCoordinate = textureCoordinate;
            Size = spriteSize;
            Color = color;
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(4 * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(4 * 5, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(4 * 6, VertexElementFormat.Single, VertexElementUsage.Position, 1)
        );

        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }


    }


}
