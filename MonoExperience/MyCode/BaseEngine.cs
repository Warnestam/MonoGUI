using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGUI.Controls;
using MonoGUI.Engine;
using MonoGUI.GameComponents;
using MonoGUI.Graphics;



/*
 * File:		BaseEngine
 * Purpose:		Define base class for all engines in the "XNA Experience"
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2010-02-28  RW	
 * 
 * History:		2010-02-28  RW  Created
 *              2020-03-31  RW  Control Window
 * 
 */
namespace MonoExperience
{

    /// <summary>
    /// Base class for all engines in the "XNA Experience"
    /// </summary>
    public abstract class BaseEngine
    {

        #region Private members

        private EngineContainer fContainer;
        private Color fBackColor;
        private bool fInitialized;
        private bool fHasContent;
        private bool fHasUpdated;
        private GuiWindow fControlWindow;

        #endregion

        #region Properties

        public EngineContainer EngineContainer { get => fContainer; }
        public Game Game { get => fContainer.Game; }
        public GraphicsDevice GraphicsDevice { get => fContainer.Game.GraphicsDevice; }
        public InputManager Manager { get => fContainer.Manager; }

        public Color BackColor
        {
            get
            {
                return fBackColor;
            }
            set
            {
                fBackColor = value;
            }
        }

        #endregion

        #region Abstract methods

        /// <summary>
        /// Get name of engine
        /// </summary>
        /// <returns></returns>
        public abstract string GetName();

        /// <summary>
        /// Get help for the engine (with line feeds)
        /// </summary>
        /// <returns></returns>
        public abstract string GetHelp();

        /// <summary>
        /// Get running information from the engine (with line feeds)
        /// </summary>
        /// <returns></returns>
        public abstract string GetInfo();

        /// <summary>
        /// Get description of engine (with line feeds)
        /// </summary>
        /// <returns></returns>
        public abstract string GetAbout();

        /// <summary>
        /// Resoluation or window size has changed
        /// </summary>
        public abstract void DisplayChanged();

        //internal virtual void DoInitialize() { }
        //internal virtual void DoDispose(bool disposing) { }
        //internal virtual void DoLoadContent() { }
        //internal virtual void DoUnloadContent() { }
        //internal virtual void DoUpdate(GameTime gameTime) { }
        //internal virtual void DoDraw(GameTime gameTime) { }
        
        internal virtual GuiWindow DoCreateControlWindow(GuiEngine guiEngine) => null;

        protected virtual void Initialize() { }
        protected virtual void Dispose(bool disposing) { }
        protected virtual void LoadContent() { }
        protected virtual void UnloadContent() { }
        protected virtual void Draw(GameTime gameTime) { }
        protected virtual void Update(GameTime gameTime) { }


        #endregion

        #region Constructor

        /// <summary>
        /// Create the engine
        /// </summary>
        /// <param name="game"></param>
        public BaseEngine(EngineContainer cnt)
        {
            fContainer = cnt;
            fBackColor = Color.DarkBlue;
        }

        #endregion

        public void DoInitialize()
        {
            //if (!fInitialized)
            {
                Initialize();
                fInitialized = true;
            }
        }

        public void DoDispose(bool disposing)
        {
            if (fInitialized)
            {
                Dispose(disposing);
                fInitialized = false;
            }
        }

        public void DoLoadContent()
        {
            //if (!fHasContent)
            {
                LoadContent();
                fHasContent = true;
            }
        }

        public void DoUnloadContent()
        {
            //if (fHasContent)
            {
                UnloadContent();
                fHasContent = true;
            }
        }

        public void DoUpdate(GameTime gameTime)
        {
            if (fInitialized && fHasContent)
            {
                fHasUpdated = true;
                Update(gameTime);
            }
        }

        public void DoDraw(GameTime gameTime)
        {
            if (fInitialized && fHasContent && fHasUpdated)
            {
                Draw(gameTime);
            }
        }

        public GuiWindow GetControlWindow(GuiEngine guiEngine)
        {
            if (fControlWindow==null)
            {
                fControlWindow = DoCreateControlWindow(guiEngine);
            }
            return fControlWindow;
        }

    }

}
