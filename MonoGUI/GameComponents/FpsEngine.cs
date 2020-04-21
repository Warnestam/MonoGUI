using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

/*
 * File:		FpsEngine
 * Purpose:		Component for calculating FPS
 * 
 * Author(s):	RW: Robert Warnestam
 * 
 * History:		2009-04-16  RW  Created
 * 
 * 
 */
namespace MonoGUI.GameComponents
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class FpsEngine : Microsoft.Xna.Framework.GameComponent
    {

        #region Private members

        private int fLastFPS;
        private int fCurrentFPS;
        private DateTime fLastTime;

        #endregion

        #region Constructor

        public FpsEngine(Game game)
            : base(game)
        {
        }


        #endregion

        #region GameComponent

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            DateTime currentTime = DateTime.Now;
            if (currentTime.Second == fLastTime.Second &&
                currentTime.Minute == fLastTime.Minute)
            {
                fCurrentFPS++;
            }
            else
            {
                fLastTime = currentTime;
                fLastFPS = fCurrentFPS;
                fCurrentFPS = 1;
            }
            base.Update(gameTime);
        }

        #endregion

        #region Public properties and methods

        /// <summary>
        /// Reset counters
        /// </summary>
        public void Reset()
        {
            fLastFPS = 0;
            fCurrentFPS = 0;
        }

        /// <summary>
        /// Gets the current FPS
        /// </summary>
        public int FrameRate
        {
            get
            {
                return fLastFPS;
            }
        }

        #endregion

    }
}