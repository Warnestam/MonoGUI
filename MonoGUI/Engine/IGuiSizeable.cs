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
 * File:		IGuiSizeable
 * Purpose:		Interface for controls that has a sizeable border
 * 
 * Author(s):	RW: Robert Warnestam
 * 
 * History:		2019-04-16  RW  Created
 * 
 */
namespace MonoGUI.Engine
{
    [Flags]
    public enum GuiSizeElements { None = 0x00, Left = 0x01, Top = 0x02, Width = 0x04, Height = 0x08 };

    public interface IGuiSizeable
    {
        GuiRect GetSizeRectangle();
        void SetSizeRectangle(GuiRect rect);
        void StartResize();
        void EndResize();
        bool IsSizingEnabled();
        GuiSizeElements GetElements(Point point);
    }


}



