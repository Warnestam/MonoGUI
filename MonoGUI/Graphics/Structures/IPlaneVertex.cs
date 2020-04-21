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
 * File:		IPlaneVertex
 * Purpose:		Interface for a vertex containing: Position+Normal+TextureCoordinate
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
    /// Interface for a vertex containing: Position+Normal
    /// </summary>
    public interface IPlaneVertex : IVertexType
    {

        Vector3 VertexPosition { get; set; }
        Vector3 VertexNormal { get; set; }
        Vector2 TextureCoordinate { get; set; }

    }



}
