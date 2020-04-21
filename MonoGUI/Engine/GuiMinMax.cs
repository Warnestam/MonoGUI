using System;
using System.Collections.Generic;
using System.Text;
using MonoGUI.Controls;
/*
 * File:		GuiMinMax
 * Purpose:		Helper class from Microsoft
 * 
 * Author(s):	RW: Robert Warnestam
 * 
 * History:		2018-11-14  RW  Created
 * 
 */
namespace MonoGUI.Engine
{

    /// <summary>
    /// Get min/max data from an GuiElement
    /// </summary>
    internal struct GuiMinMax
    {

        internal GuiMinMax(GuiElement e)
        {
            maxHeight = e.MaxHeight;
            minHeight = e.MinHeight;
            maxWidth = e.MaxWidth;
            minWidth = e.MinWidth;

            maxHeight = GetMax(e.Height, e.MinHeight, e.MaxHeight);
            minHeight = GetMin(e.Height, e.MinHeight, e.MaxHeight);

            maxWidth = GetMax(e.Width, e.MinWidth, e.MaxWidth);
            minWidth = GetMin(e.Width, e.MinWidth, e.MaxWidth);
        }

        private int GetMax(int? desiredValue, int minValue, int maxValue)
        {
            int value = desiredValue.GetValueOrDefault(int.MaxValue);
            value = Math.Max(Math.Min(value, maxValue), minValue);
            return value;
        }

        private int GetMin(int? desiredValue, int minValue, int maxValue)
        {
            int value = desiredValue.GetValueOrDefault(0);
            value = Math.Max(Math.Min(value, maxValue), minValue);
            return value;
        }

        internal int minWidth;
        internal int maxWidth;
        internal int minHeight;
        internal int maxHeight;
    }

}
