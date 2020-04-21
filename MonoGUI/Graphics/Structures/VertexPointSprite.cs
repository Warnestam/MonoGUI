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
    public struct VertexPointSprite : IVertexType
    {

        
        public Vector3 Position;
        public float PointSize;

        public VertexPointSprite(Vector3 position, float pointSize)
        {
            Position = position;
            PointSize = pointSize;
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Single, VertexElementUsage.PointSize, 0)
        );

        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }


    }


}
