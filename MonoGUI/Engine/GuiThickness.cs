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
 * File:		GuiThickness
 * Purpose:		Margin for a GuiElement
 * 
 * Author(s):	RW: Robert Warnestam
 * 
 * History:		2018-03-03  RW  Created
 * 
 */
namespace MonoGUI.Engine
{

    /// <summary>
    /// Margin for a GuiElement
    /// </summary>
    public struct GuiThickness
    {

        /// <summary>
        /// Gets or sets the width, in pixels, of the lower side of the bounding rectangle.
        /// </summary>
        public int Bottom;

        /// <summary>
        /// Gets or sets the width, in pixels, of the left side of the bounding rectangle. 
        /// </summary>
        public int Left;

        /// <summary>
        /// Gets or sets the width, in pixels, of the right side of the bounding rectangle. 
        /// </summary>
        public int Right;

        /// <summary>
        /// Gets or sets the width, in pixels, of the upper side of the bounding rectangle.
        /// </summary>
        public int Top;

        public GuiThickness(int thickness)
        {
            Bottom = thickness;
            Left = thickness;
            Right = thickness;
            Top = thickness;
        }

        public GuiThickness(int top, int right, int bottom, int left)
        {
            Bottom = bottom;
            Left = left;
            Right = right;
            Top = top;
        }

        public int Width => Left + Right;

        public int Height => Top + Bottom;

        public override string ToString()
        {
            return $"{Left},{Top},{Right},{Bottom}";
        }
    }


}



