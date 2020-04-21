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
 * File:		GuiEnum
 * Purpose:		Some enumerations for GuiEngine
 * 
 * Author(s):	RW: Robert Warnestam
 * 
 * History:		2017-11-09  RW  Created
 * 
 */
namespace MonoGUI.Engine
{

    /// <summary>
    /// Display mode used by GuiEngine
    /// </summary>
    public enum GuiDisplayMode { Window, WindowKeepAspectRatio, Fullscreen };

    /// <summary>
    /// Current mouse state in GuiEngine
    /// </summary>
    public enum GuiMouseState { Moving, Pressed, Dragging, Sizing };

    /// <summary>
    /// Vertical alignments of controls
    /// </summary>
    public enum GuiVerticalAlignment {
        /// <summary>The child element is aligned to the top of the parent's layout slot.</summary>
        Top,
        /// <summary>An element aligned to the center of the layout slot for the parent element.</summary>
        Center,
        /// <summary>The child element is aligned to the bottom of the parent's layout slot.</summary>
        Bottom,
        /// <summary>The child element stretches to fill the parent's layout slot.</summary>
        Stretch
    }

    /// <summary>
    /// Horizontal alignments of controls
    /// </summary>
    public enum GuiHorizontalAlignment
    {
        /// <summary>An element aligned to the left of the layout slot for the parent element.</summary>
        Left,
        /// <summary>An element aligned to the center of the layout slot for the parent element.</summary>
        Center,
        /// <summary>An element aligned to the right of the layout slot for the parent element.</summary>
        Right,
        /// <summary>An element stretched to fill the entire layout slot of the parent element.</summary>
        Stretch
    }
  
    /// <summary>
    ///Visibility of an element
    /// </summary>
    public enum GuiVisibility
    {
        /// <summary>
        /// Normally visible.
        /// </summary>
        Visible = 0,

        /// <summary>
        /// Occupies space in the layout, but is not visible (completely transparent).
        /// </summary>
        Hidden,

        /// <summary>
        /// Not visible and does not occupy any space in layout, as if it doesn't exist.
        /// </summary>
        Collapsed
    }

    /// <summary>
    /// Dock - Enum which describes how to position and stretch the child of a DockPanel.
    /// </summary>
    public enum GuiDock
    {
        /// <summary>
        /// Position this child at the left of the remaining space.
        /// </summary>
        Left,

        /// <summary>
        /// Position this child at the top of the remaining space.
        /// </summary>
        Top,

        /// <summary>
        /// Position this child at the right of the remaining space.
        /// </summary>
        Right,

        /// <summary>
        /// Position this child at the bottom of the remaining space.
        /// </summary>
        Bottom,
    }

    public enum GuiStackPanelOrientation { Horizontal, Vertical };

    public enum GuiWindowState { Normal, Maximized };

    public enum GuiExpandablePanelState { Expanded, Collapsed };

}



