using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGUI.Graphics;
using MonoGUI.Controls;
using MonoGUI.Engine;
using MonoGUI.GameComponents;
/*
 * File:		GuiPoint
 * Purpose:		Represents an x- and y-coordinate pair in two-dimensional space.
 * 
 * Author(s):	RW: Robert Warnestam
 * 
 * History:		2018-05-25  RW  Created
 * 
 */
namespace MonoGUI.Engine
{

    /// <summary>
    /// Represents an x- and y-coordinate pair in two-dimensional space.
    /// </summary>
    public struct GuiPoint
    {
        /// <summary>
        /// Gets or sets the X-coordinate value of this Point structure. 
        /// </summary>
        public int X;

        /// <summary>
        /// Gets or sets the Y-coordinate value of this Point. 
        /// </summary>
        public int Y;

        public GuiPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"{X},{Y}";
        }
    }


}



