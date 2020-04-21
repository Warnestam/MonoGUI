using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


/*
 * File:		InputManager
 * Purpose:		Class for handling mouse and keyboard
 * 
 * Author(s):	RW: Robert Warnestam
 * 
 * History:		2010-02-28  RW  Created
 * 
 */
namespace MonoGUI.GameComponents
{

    /// <summary>
    /// Class for handling mouse and keyboard
    /// </summary>
    public class InputManager : Microsoft.Xna.Framework.GameComponent
    {

        #region Private membets

        private KeyboardState fOldKeyboardState;
        private KeyboardState fNewKeyboardState;
        private MouseState fCenterMouse;
        private MouseState fMouseState;
        private MouseState fPreviousMouseState;
        private bool fCaptureMouse = true;

        #endregion

        #region Properties

        /// <summary>
        /// Get/set if we should capture the mouse
        /// </summary>
        public bool CaptureMouse
        {
            get
            {
                return fCaptureMouse;
            }
            set
            {
                fCaptureMouse = value;
                this.Game.IsMouseVisible = !value;
                Mouse.SetPosition(fCenterMouse.X, fCenterMouse.Y);
                fMouseState = fCenterMouse;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new input manager object
        /// </summary>
        public InputManager(Game game)
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
            //if (gameTime.ElapsedGameTime.Milliseconds > 1000)
            {
                fOldKeyboardState = fNewKeyboardState;
                fNewKeyboardState = Keyboard.GetState();
                fPreviousMouseState = fMouseState;
                fMouseState = Mouse.GetState();
                if (fCaptureMouse)
                    Mouse.SetPosition(fCenterMouse.X, fCenterMouse.Y);
            }

            base.Update(gameTime);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// See if a key is pressed
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool KeyPressed(Keys key)
        {
            bool result = false;
            if (fNewKeyboardState.IsKeyDown(key) && fOldKeyboardState.IsKeyUp(key))
                result = true;
            return result;
        }

        /// <summary>
        /// See if a key is down
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsKeyDown(Keys key)
        {
            return fNewKeyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// See if a key was down
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool WasKeyDown(Keys key)
        {
            return fOldKeyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Inits the mouse
        /// </summary>
        public void InitMouse()
        {
            Mouse.SetPosition(
                Game.GraphicsDevice.Viewport.Width / 2,
                Game.GraphicsDevice.Viewport.Height / 2);
            fCenterMouse = Mouse.GetState();
        }

        /// <summary>
        /// See if the user has moved the mouse
        /// </summary>
        public bool CheckMouseMovement()
        {
            return fCenterMouse != fMouseState;
        }

        /// <summary>
        /// Get the mouse state
        /// </summary>
        /// <returns></returns>
        public MouseState GetMouseState()
        {
            return fMouseState;
        }

        /// <summary>
        /// Get the previous mouse state
        /// </summary>
        /// <returns></returns>
        public MouseState GetPreviousMouseState()
        {
            return fPreviousMouseState;
        }

        /// <summary>
        /// Gets the last mouse move 
        /// </summary>
        /// <returns></returns>
        public Point GetMouseMovement()
        {
            return new Point(fMouseState.X - fCenterMouse.X, fMouseState.Y - fCenterMouse.Y);
        }




        #endregion

    }


}
