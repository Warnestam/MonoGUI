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
 * File:		IGuiDraggable
 * Purpose:		Interface for controls that are dragable
 * 
 * Author(s):	RW: Robert Warnestam
 * 
 * History:		2017-11-18  RW  Created
 * 
 */
namespace MonoGUI.Engine
{

    public interface IGuiDraggable
    {
        Point GetPosition();
        void SetPosition(Point point);
        void StartDrag();
        void EndDrag();
        bool IsDragEnabled();
    }


}



