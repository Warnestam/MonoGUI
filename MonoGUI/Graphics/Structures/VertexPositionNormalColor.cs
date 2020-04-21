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
 * File:		VertexPositionNormalColor
 * Purpose:		Structure for a vertex containing: Position+Normal+Color
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
    /// Structure for a vertex containing: Position+Normal+Color
    /// </summary>
    public struct VertexPositionNormalColor : IVertexType
    {

        public Vector3 Position;
        public Vector3 Normal;
        public Color Color;

        public VertexPositionNormalColor(Vector3 position, Color color)
        {
            Position = position;
            Normal = Vector3.Zero;
            Color = color;
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(24, VertexElementFormat.Color, VertexElementUsage.Color, 0)
        );

        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }

    }


}
