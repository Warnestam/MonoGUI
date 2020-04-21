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
 * File:		SkyBox
 * Purpose:		Class for rendering a skybox
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2010-07-11  RW	
 * 
 * History:		2010-07-11  RW  Code from RB Whittaker's Wiki: http://rbwhitaker.wikidot.com
 * 
 * 
 */
namespace MonoGUI.Graphics
{

    /// <summary>
    /// Class for rendering a skybox
    /// </summary>
    public class SkyBox
    {
       
        #region Private membets

        private const int DEFAULT_BUFFER_SIZE = 1024;

        private Model fModel;
        private TextureCube fTexture;
        private Effect fEffect;

        private float fSize = 50f;

        private GraphicsDevice fDevice;

        #endregion

        #region Properties

      
        #endregion

        #region Constructor / destructor

        /// <summary>
        /// Creates a new skybox object.
        /// </summary>
        /// <param name="graphicsDevice">The Graphics Device object to use.</param>
        public SkyBox(GraphicsDevice graphicsDevice, TextureCube skyboxTexture, ContentManager content, float size)
        {
            fDevice = graphicsDevice;
            fModel = content.Load<Model>("Skyboxes/SkyBoxModel");
            fEffect = content.Load<Effect>("Skyboxes/SkyBoxShader");
            fTexture = skyboxTexture;
            fSize = size;
        }

        /// <summary>
        /// Called when the billboard object is destroyed.
        /// </summary>
        ~SkyBox()
        {
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Set skybox texture
        /// </summary>
        /// <param name="texture"></param>
        public void SetTexture(TextureCube texture)
        {
            fTexture = texture;
        }

        #endregion

        #region Draw method

        /// <summary>
        /// Renders the billboard object.
        /// </summary>
        public void Render(Matrix view, Matrix projection, Vector3 cameraPosition, Matrix world)
        {
            //Matrix wMatrix = Matrix.CreateScale(fSize) * world;
            //Matrix[] modelTransforms = new Matrix[fModel.Bones.Count];
            //fModel.CopyAbsoluteBoneTransformsTo(modelTransforms); 
            //foreach (ModelMesh mesh in fModel.Meshes)
            //{
            //   //  With standard effect...
            //    foreach (BasicEffect effect in mesh.Effects)
            //    {
            //        Matrix worldMatrix = modelTransforms[mesh.ParentBone.Index] * wMatrix;
            //        effect.World = worldMatrix * mesh.ParentBone.Transform;
            //        effect.EnableDefaultLighting();
            //        effect.PreferPerPixelLighting = true;
            //        effect.View = view;
            //        effect.Projection = projection;
            //    }
            //    mesh.Draw();
            //}
            //return;

            // Go through each pass in the effect, but we know there is only one...
            foreach (EffectPass pass in fEffect.CurrentTechnique.Passes)
            {
                // Draw all of the components of the mesh, but we know the cube really
                // only has one mesh
                foreach (ModelMesh mesh in fModel.Meshes)
                {
                    // Assign the appropriate values to each of the parameters
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        part.Effect = fEffect;
                        part.Effect.Parameters["World"].SetValue(Matrix.CreateScale(fSize) * Matrix.CreateTranslation(cameraPosition));
                        part.Effect.Parameters["View"].SetValue(view);
                        part.Effect.Parameters["Projection"].SetValue(projection);
                        part.Effect.Parameters["SkyBoxTexture"].SetValue(fTexture);
                        part.Effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                    }
                    // Draw the mesh with the skybox effect
                    mesh.Draw();
                }
            }

        }

        #endregion
            
    }


}
