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
using MonoGUI.GameComponents;
using MonoGUI.Graphics;

/*
 * File:		MyShader
 * Purpose:		Engine for playing with shaders
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2010-07-10  RW	
 * 
 * History:		2010-07-10  RW  Created
 *              2020-04-08  RW  Moved to MonoExperience
 * 
 * TODO: Move SimpleCameraController to Components
 * 
 * Models & Shaders from RB Whittaker's Wiki
 *      http://rbwhitaker.wikidot.com
 *      
 * 
 */
namespace MonoExperience
{

    /// <summary>
    /// Engine for playing with shaders
    /// </summary>
    public class MyShaderEngine : BaseEngine
    {
        const string ORGINAL_SHADER_TEXT = "*OriginalText";
        const string ORGINAL_SHADER = "*Original";
        string[] MODELS = new string[] { "Helicopter", "LargeAsteroid", "Ship", "Ship2", "MyFirstModel/dude", "earth", "daisy" };
        // Not working: cottage_fbx, box, minion
        string[] SHADERS = new string[] { ORGINAL_SHADER, ORGINAL_SHADER_TEXT, "Ambient", "DiffuseLightning", "SpecularLightning", "TextureShader", "NormalMapShader" };// , "ToonShader" };
        string[] SKYBOXES = new string[] { "EmptySpace", "Islands", "SkyBox", "SunInSpace", "Sunset" };

        #region Private members

        private SpriteBatch fSpriteBatch;
        private Random fRandom = new Random();
        private SimpleCameraController fCamera;
        private bool fHalted = false;
        private float fWorldAngle;

        private List<Model> fModels;
        private List<Effect> fShaders;
        // Special for Helicopter
        private Texture2D fNormalMap;
        // SkyBox
        private SkyBox fSkyBox;
        private List<TextureCube> fSkyBoxTextures;
        private int fCurrentSkybox = 0;

        private int fCurrentModel = 0;
        private int fCurrentShader = 0;

        #endregion

        #region Constructor

        /// <summary>
        /// Create the engine
        /// </summary>
        /// <param name="game"></param>
        public MyShaderEngine(EngineContainer cnt) : base(cnt)
        {
            // TODO: Construct any child components here
        }

        #endregion

        #region Properties

        #endregion

        #region DrawableGameComponent

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();


            InitializeTransform();
            InitializeEffect();
        }

        protected override void Dispose(bool disposing)
        {
            // TODO
            base.Dispose(disposing);
        }

        /// <summary>
        /// Loads any component specific content
        /// </summary>
        protected override void LoadContent()
        {
            fSpriteBatch = new SpriteBatch(Game.GraphicsDevice);

            fModels = new List<Model>();
            foreach (string model in MODELS)
            {
                if (model.Contains("/"))
                    fModels.Add(this.Game.Content.Load<Model>(model));
                else
                    fModels.Add(this.Game.Content.Load<Model>(String.Format("MyShader/{0}", model)));
            }
            fShaders = new List<Effect>();
            foreach (string shader in SHADERS)
                if (shader.Contains("*"))
                    fShaders.Add(null);
                else
                    fShaders.Add(this.Game.Content.Load<Effect>(String.Format("MyShader/{0}", shader)));

            // Special
            fNormalMap = this.Game.Content.Load<Texture2D>("MyShader/HelicopterNormalMap");

            // SkyBox
            fSkyBoxTextures = new List<TextureCube>();
            foreach (string skybox in SKYBOXES)
                fSkyBoxTextures.Add(this.Game.Content.Load<TextureCube>(String.Format("SkyBoxes/{0}", skybox)));
            fSkyBox = new SkyBox(GraphicsDevice, fSkyBoxTextures[0], this.Game.Content, 5000f);

            InitializeTransform();
            InitializeEffect();


            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            HandleInput(gameTime);
            fCamera.Update(gameTime);

            double seconds = gameTime.TotalGameTime.TotalSeconds;


            base.Update(gameTime);
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Matrix world = fCamera.Camera.WorldMatrix;
            Matrix view = fCamera.Camera.ViewMatrix;
            Matrix projection = fCamera.Camera.ProjectionMatrix;
            if (!fHalted)
            {
                fWorldAngle += Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds * 1.5f);
            }
            world = Matrix.CreateRotationY(fWorldAngle);

            GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            fSkyBox.SetTexture(fSkyBoxTextures[fCurrentSkybox]);
            fSkyBox.Render(fCamera.Camera.ViewMatrix, fCamera.Camera.ProjectionMatrix, fCamera.Camera.Position, world);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            //GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

            Model model = fModels[fCurrentModel];

            Matrix wMatrix = Matrix.CreateScale(30) * world;
            Matrix[] modelTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(modelTransforms);

            string shaderName = SHADERS[fCurrentShader];


            foreach (ModelMesh mesh in model.Meshes)
            {
                try
                {
                    if (shaderName.Equals(ORGINAL_SHADER))
                    {
                        //With standard effect...
                        foreach (BasicEffect x in mesh.Effects)
                        {
                            Matrix worldMatrix = modelTransforms[mesh.ParentBone.Index] * wMatrix;
                            x.World = worldMatrix * mesh.ParentBone.Transform;
                            x.EnableDefaultLighting();
                            x.PreferPerPixelLighting = true;
                            x.View = view;
                            x.Projection = projection;
                        }
                        mesh.Draw();
                    }
                    else if (shaderName.Equals(ORGINAL_SHADER_TEXT))
                    {
                        //With standard effect...
                        foreach (BasicEffect x in mesh.Effects)
                        {
                            Matrix worldMatrix = modelTransforms[mesh.ParentBone.Index] * wMatrix;
                            x.World = worldMatrix * mesh.ParentBone.Transform;
                            x.EnableDefaultLighting();
                            x.PreferPerPixelLighting = true;
                            x.View = view;
                            x.Projection = projection;
                            x.TextureEnabled = true;
                            //x.AmbientLightColor = new Vector3(1.0f);
                            //x.World = modelTransforms[mesh.ParentBone.Index];
                        }
                        mesh.Draw();
                    }
                    else
                    {
                        Effect effect = fShaders[fCurrentShader];
                        Effect[] originalEffects = new Effect[mesh.MeshParts.Count];
                        for (int i = 0; i < mesh.MeshParts.Count; i++)
                        {
                            originalEffects[i] = mesh.MeshParts[i].Effect;
                        }
                        try
                        {
                            for (int i = 0; i < mesh.MeshParts.Count; i++)
                            {
                                mesh.MeshParts[i].Effect = effect;
                                Matrix worldMatrix = modelTransforms[mesh.ParentBone.Index] * wMatrix;
                                Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * world));
                                Vector3 viewVector = new Vector3(fCamera.Camera.ViewMatrix.M13, fCamera.Camera.ViewMatrix.M23, fCamera.Camera.ViewMatrix.M33);
                                effect.Parameters["World"].SetValue(worldMatrix * mesh.ParentBone.Transform);
                                effect.Parameters["View"].SetValue(view);
                                effect.Parameters["Projection"].SetValue(projection);
                                switch (shaderName)
                                {
                                    case "Ambient":
                                        effect.Parameters["AmbientColor"].SetValue(Color.Green.ToVector4());
                                        effect.Parameters["AmbientIntensity"].SetValue(0.5f);
                                        break;
                                    case "DiffuseLightning":
                                        effect.Parameters["AmbientColor"].SetValue(Color.White.ToVector4());
                                        effect.Parameters["AmbientIntensity"].SetValue(0.2f);
                                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                                        break;
                                    case "SpecularLightning":
                                        effect.Parameters["AmbientColor"].SetValue(Color.Red.ToVector4());
                                        effect.Parameters["AmbientIntensity"].SetValue(0.5f);
                                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                                        effect.Parameters["ViewVector"].SetValue(viewVector);
                                        break;
                                    case "TextureShader":
                                        effect.Parameters["AmbientColor"].SetValue(1.0f);
                                        effect.Parameters["AmbientIntensity"].SetValue(0.1f);
                                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                                        effect.Parameters["ViewVector"].SetValue(viewVector);
                                        // TODO: now only the first texture in the model     
                                        effect.Parameters["ModelTexture"].SetValue((originalEffects[i] as BasicEffect)?.Texture);
                                        break;
                                    case "NormalMapShader":
                                        //effect.Parameters["DiffuseLightDirection"].SetValue(new Vector3(0, 0.5f, 1));
                                        //effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                                        //effect.Parameters["ModelTexture"].SetValue((originalEffects[i] as BasicEffect)?.Texture);
                                        // NORMAL MAP ONLY FOR HELICOPTER
                                        //effect.Parameters["NormalMap"].SetValue(fNormalMap);
                                        break;
                                    case "ToonShader":
                                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                                        effect.Parameters["Texture"].SetValue((originalEffects[i] as BasicEffect)?.Texture);
                                        break;
                                }
                            }
                            mesh.Draw();
                        }
                        finally
                        {
                            for (int i = 0; i < mesh.MeshParts.Count; i++)
                                mesh.MeshParts[i].Effect = originalEffects[i];
                        }
                    }
                }
                catch(Exception e)
                {
                    
                }
            }



            base.Draw(gameTime);
        }

        #endregion

        #region BaseEngine

        public override string GetName()
        {
            return "My Shader";
        }

        public override string GetHelp()
        {
            string text1 = "H - Halt rotation\nM - Change model\nS - Change shader\nT - Change skybox texture";
            string text2 = fCamera.GetHelp();
            return String.Format("{0}\n{1}", text1, text2);
        }

        public override string GetInfo()
        {
            string text1 = String.Format("Model: {0}\nShader: {1}\nSkybox: {2}",
                MODELS[fCurrentModel], SHADERS[fCurrentShader], SKYBOXES[fCurrentSkybox]);
            string text2 = fCamera.GetInfo();
            return String.Format("{0}\n{1}", text1, text2);
        }

        public override string GetAbout()
        {
            return "Playing with shaders.\n\n Original shader author: \nRB Whittaker's Wiki\nhttp://rbwhitaker.wikidot.com";
        }



        /// <summary>
        /// Handle the input
        /// </summary>
        /// <param name="gameTime"></param>
        private void HandleInput(GameTime gameTime)
        {
            if (this.Manager.KeyPressed(Keys.H))
            {
                fHalted = !fHalted;
            }
            else if (this.Manager.KeyPressed(Keys.M))
            {
                fCurrentModel++;
                if (fCurrentModel >= MODELS.Length)
                    fCurrentModel = 0;
            }
            else if (this.Manager.KeyPressed(Keys.S))
            {
                fCurrentShader++;
                if (fCurrentShader >= SHADERS.Length)
                    fCurrentShader = 0;
            }
            else if (this.Manager.KeyPressed(Keys.T))
            {
                fCurrentSkybox++;
                if (fCurrentSkybox >= SKYBOXES.Length)
                    fCurrentSkybox = 0;
            }
        }

        public override void DisplayChanged()
        {

        }

        #endregion

        #region Private methods

        /// <summary>
        /// Initializes the transforms used by the game.
        /// </summary>
        private void InitializeTransform()
        {
            fCamera = new SimpleCameraController(this.Game, this.Manager, GraphicsDevice.Viewport);

            fCamera.Camera.Rotation = Quaternion.CreateFromYawPitchRoll(0, 0.0f, 0);
            fCamera.Camera.Position = new Vector3(0.0f, 0.0f, 500.0f);
            fCamera.Camera.Update();
        }

        /// <summary>
        /// Initializes the effect (loading, parameter setting, and technique selection)
        /// used by the game.
        /// </summary>
        private void InitializeEffect()
        {
        }

        #endregion

    }
}
