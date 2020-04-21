using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;

using MonoGUI.Graphics;
using MonoGUI.Engine;

/*
 * File:		GuiContainerControl
 * Purpose:		A control to hold a list of controls
 * 
 * Author(s):	RW: Robert Warnestam
 * 
 * History:		2018-11-14  RW  Created
 * 
 */
namespace MonoGUI.Controls
{

    #region Interface for Child objects

    public interface IGuiChild
    {
        GuiElement Control { get; set; }
    }

    #endregion

    /// <summary>
    /// A control to hold a list of controls
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GuiContainerControl<T> : GuiElement where T : IGuiChild
    {

        #region Private members

        private List<T> fChilds { get; set; } = new List<T>();

        #endregion

        #region Properties

        public List<T> Childs
        {
            get => fChilds;
            set
            {
                if (fChilds != value)
                {
                    fChilds = value;
                    if (fChilds != null)
                    {
                        foreach (var child in fChilds)
                            child.Control.Parent = this;
                    }
                    InvalidateMeasure();
                }
            }
        }

        #endregion

        #region Overrides

        protected override void DoInvalidateMeasure()
        {
            if (Childs != null)
            {
                foreach (var child in fChilds)
                    child.Control.InvalidateMeasure();
            }
        }

        public override void Initialize(GraphicsDevice device)
        {
            base.Initialize(device);
            
            if (Childs != null)
            {
                foreach (var child in Childs)
                {
                    child.Control.Initialize(device);
                }
            }
        }

        public override void LoadContent(GuiEngine engine)
        {
            base.LoadContent(engine);
            if (Childs != null)
            {
                foreach (var child in Childs)
                {
                    child.Control.LoadContent(engine);
                }
            }
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            if (Childs != null)
            {
                foreach (var child in Childs)
                {
                    child.Control.UnloadContent();
                }
            }
        }

        #endregion

    }


}
