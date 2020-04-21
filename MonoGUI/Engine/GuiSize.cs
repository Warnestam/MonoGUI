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
 * File:		GuiSize
 * Purpose:		Representation of a 2-dimensional size
 * 
 * Author(s):	RW: Robert Warnestam
 * 
 * History:		2018-02-25  RW  Created
 * 
 */
namespace MonoGUI.Engine
{

    /// <summary>
    /// Representation of a 2-dimensional size
    /// </summary>
    public struct GuiSize
    {

        private readonly static GuiSize EMPTY_SIZE = CreateEmptySize();
        
        public int Width;

        public int Height;

        public GuiSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public static GuiSize Empty
        {
            get
            {
                return EMPTY_SIZE;
            }
        }

        static private GuiSize CreateEmptySize()
        {
            GuiSize size = new GuiSize();
            size.Width = -1;
            size.Height = -1;
            return size;
        }

        public bool IsEmpty
        {
            get
            {
                return Width < 0;
            }
        }

        public override string ToString()
        {
            return $"{Width},{Height}";
        }
    }


}



