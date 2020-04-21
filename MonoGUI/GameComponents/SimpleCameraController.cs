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
 * File:		SimpleCameraController
 * Purpose:		Class for handling movement of a camera
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2010-03-04  RW	
 * 
 * History:		2010-03-04  RW  Created
 * 
 */
namespace MonoGUI.GameComponents
{

    /// <summary>
    /// Class for handling movement of a camera
    /// </summary>
    public class SimpleCameraController : Microsoft.Xna.Framework.GameComponent
    {

        #region Private membets

        private InputManager fManager;
        private SimpleCamera fCamera;

        // Camera & movement
        private float fAngleSpeedX = 0.0f;
        private float fAngleSpeedY = 0.0f;
        private float fAngleSpeedZ = 0.0f;
        private float fVelocitySpeed = 0.0f;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the camera
        /// </summary>
        public SimpleCamera Camera
        {
            get
            {
                return fCamera;
            }
        }

        public bool SteerEnabled { get; private set; }

        public bool UseInverseMovement { get; set; } = true;

        public Vector3 ForwardDirection { get; set; } = Vector3.Forward;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new input manager object
        /// </summary>
        public SimpleCameraController(Game game, InputManager manager, Viewport viewport)
            : base(game)
        {
            fManager = manager;
            fCamera = new SimpleCamera(viewport);
            SteerEnabled = true;
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
            if (SteerEnabled)
            {
                // Update movement
                float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (delta > 0.1f)
                    delta = 0.1f;
                float timeFactor = 100.0f * delta;

                float angleSpeedAdd = 0.0002f * timeFactor;
                float angleSpeedMult = 1.0f - (0.1f * timeFactor);
                float maxAngleChange = 0.05f;
                float velocitySpeedMult = 1.0f - (0.1f * timeFactor);
                float velocitySpeedAdd = 0.5f * timeFactor;

                bool updateCamera = false;
                bool angleChanged = false;

                // Camera - speed
                if (fManager.IsKeyDown(Keys.A))
                {
                    fVelocitySpeed += velocitySpeedAdd;
                }
                else if (fManager.IsKeyDown(Keys.Z))
                {
                    fVelocitySpeed -= velocitySpeedAdd;
                }
                else
                {
                    fVelocitySpeed *= velocitySpeedMult;
                }
                if (fVelocitySpeed < -0.001f || fVelocitySpeed > 0.001f)
                {
                    Quaternion quat = fCamera.Rotation;
                    if (UseInverseMovement)
                    {
                        quat = Quaternion.Inverse(quat);
                    }
                    Vector3 forwardDirection = Vector3.Transform(ForwardDirection, quat);
                    fCamera.Position += timeFactor * fVelocitySpeed * forwardDirection;
                    updateCamera = true;
                }

                // Camera - angle
                if (fManager.IsKeyDown(Keys.Left))
                {
                    fAngleSpeedY -= angleSpeedAdd;
                    if (fAngleSpeedY < -maxAngleChange)
                        fAngleSpeedY = -maxAngleChange;
                    angleChanged = true;
                }
                else if (fManager.IsKeyDown(Keys.Right))
                {
                    fAngleSpeedY += angleSpeedAdd;
                    if (fAngleSpeedY > maxAngleChange)
                        fAngleSpeedY = maxAngleChange;
                    angleChanged = true;
                }
                if (fManager.IsKeyDown(Keys.Down))
                {
                    fAngleSpeedX -= angleSpeedAdd;
                    if (fAngleSpeedX < -maxAngleChange)
                        fAngleSpeedX = -maxAngleChange;
                    angleChanged = true;
                }
                else if (fManager.IsKeyDown(Keys.Up))
                {
                    fAngleSpeedX += angleSpeedAdd;
                    if (fAngleSpeedX > maxAngleChange)
                        fAngleSpeedX = maxAngleChange;
                    angleChanged = true;
                }
                if (fManager.IsKeyDown(Keys.NumPad1))
                {
                    fAngleSpeedZ -= angleSpeedAdd;
                    if (fAngleSpeedZ < -maxAngleChange)
                        fAngleSpeedZ = -maxAngleChange;
                    angleChanged = true;
                }
                else if (fManager.IsKeyDown(Keys.NumPad3))
                {
                    fAngleSpeedZ += angleSpeedAdd;
                    if (fAngleSpeedZ > maxAngleChange)
                        fAngleSpeedZ = maxAngleChange;
                    angleChanged = true;
                }
                // Camera - angle - Mouse
                if (fManager.CaptureMouse && fManager.CheckMouseMovement())
                {
                    Point move = fManager.GetMouseMovement();
                    // tiden borde väl inte påverka detta?
                    fAngleSpeedY += move.X * 0.0001f;
                    fAngleSpeedX += move.Y * 0.0001f;
                    angleChanged = true;
                }

                if (!angleChanged)
                {
                    fAngleSpeedX *= angleSpeedMult;
                    fAngleSpeedY *= angleSpeedMult;
                    fAngleSpeedZ *= angleSpeedMult;
                }

                if (fAngleSpeedX < -0.0001f || fAngleSpeedX > 0.0001f ||
                    fAngleSpeedY < -0.0001f || fAngleSpeedY > 0.0001f ||
                    fAngleSpeedZ < -0.0001f || fAngleSpeedZ > 0.0001f)
                {
                    Quaternion rotationChange;
                    if (UseInverseMovement)
                    {
                        rotationChange =
                            Quaternion.CreateFromAxisAngle(Vector3.Right, timeFactor * fAngleSpeedX) * //pitch
                            Quaternion.CreateFromAxisAngle(Vector3.Up, timeFactor * fAngleSpeedY) * // yaw
                            Quaternion.CreateFromAxisAngle(Vector3.Backward, timeFactor * fAngleSpeedZ); // roll
                    }
                    else
                    {
                        rotationChange =
                            Quaternion.CreateFromAxisAngle(Vector3.Left, timeFactor * fAngleSpeedX) * //pitch
                            Quaternion.CreateFromAxisAngle(Vector3.Down, timeFactor * fAngleSpeedY) * // yaw
                            Quaternion.CreateFromAxisAngle(Vector3.Forward, timeFactor * fAngleSpeedZ); // roll
                    }
                    fCamera.Rotation = rotationChange * fCamera.Rotation;

                    updateCamera = true;
                }

                if (updateCamera)
                {
                    fCamera.Update();
                }
            }

            base.Update(gameTime);
        }

        #endregion

        #region Public methods

        public void Enable()
        {
            SteerEnabled = true;
        }

        public void Disable()
        {
            SteerEnabled = false;
            fAngleSpeedX = 0;
            fAngleSpeedY = 0;
            fAngleSpeedZ = 0;
            fVelocitySpeed = 0;
        }

        /// <summary>
        /// Gets a help text of how to control the camera. Used by Base Engine
        /// </summary>
        /// <returns></returns>
        public string GetHelp()
        {
            return @"Left/Right - Pitch
Up/Down - Yaw
Numpad 1/3 - Roll
A/Z - Speed
Mouse - Pitch/Yaw";
        }

        /// <summary>
        /// Gets info about the camera position. Used by Base Engine
        /// </summary>
        /// <returns></returns>
        public string GetInfo()
        {
            Vector3 pos = fCamera.Position;
            Quaternion rot = fCamera.Rotation;

            return String.Format("Position: (X={0},Y={1},Z={2})\nOritentation: (X={3},Y={4},Z={5},W={6})",
               (int)pos.X, (int)pos.Y, (int)pos.Z,
               (int)(100.0f * rot.X),
               (int)(100.0f * rot.Y),
               (int)(100.0f * rot.Z),
               (int)(100.0f * rot.W));
        }

        #endregion

    }


}
