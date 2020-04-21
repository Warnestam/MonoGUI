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


/*
 * File:		SimpleCamera
 * Purpose:		Class representering a quaternion camera
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2010-02-28  RW	
 * 
 * History:		2010-02-28  RW  Created
 * 
 */
namespace MonoGUI.GameComponents
{

    /// <summary>
    /// Class representering a quaternion camera
    /// </summary>
    public class SimpleCamera
    {

        #region Private members

        private Vector3 fPosition;
        private Quaternion fRotation = Quaternion.Identity;

        private Matrix fViewMatrix;
        private Matrix fProjectionMatrix;
        private Matrix fWorldMatrix = Matrix.Identity;
        private float fAspectRatio;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a camera
        /// </summary>
        /// <param name="viewport"></param>
        public SimpleCamera(Viewport viewport)
        {
            fAspectRatio = ((float)viewport.Width) / ((float)viewport.Height);
            fProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                                        MathHelper.ToRadians(40.0f),
                                        fAspectRatio,
                                        1.0f,
                                        30000.0f);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get/set the position 
        /// </summary>
        public Vector3 Position
        {
            get { return fPosition; }
            set { fPosition = value; }
        }

        /// <summary>
        ///  Get/set the rotation of the camera
        /// </summary>
        public Quaternion Rotation
        {
            get { return fRotation; }
            set { fRotation = value; }
        }

        /// <summary>
        ///  Get/set the view matrix
        /// </summary>
        public Matrix ViewMatrix
        {
            get { return fViewMatrix; }
        }

        /// <summary>
        ///  Get/set the world matrix
        /// </summary>
        public Matrix WorldMatrix
        {
            get { return fWorldMatrix; }
        }

        /// <summary>
        ///  Get/set the projection matrix
        /// </summary>
        public Matrix ProjectionMatrix
        {
            get { return fProjectionMatrix; }
        }

        #endregion

        #region Public methods

        public void SetProjectionMatrix(float fieldOfView, float nearPlaneDistance, float farPlaneDistance)
        {
            fProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                fieldOfView, fAspectRatio, nearPlaneDistance, farPlaneDistance);
        }


        /// <summary>
        /// Update the matrixes 
        /// </summary>
        public void Update()
        {
            //Not in SRT order (scale * rotation * translation)...

            Matrix rot = Matrix.CreateFromQuaternion(fRotation);
            Matrix pos = Matrix.CreateTranslation(-fPosition);
            fViewMatrix = pos * rot;
        }

        #endregion


    }

}
