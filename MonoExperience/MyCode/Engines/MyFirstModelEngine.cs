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
 * File:		MyFirstModelEngine
 * Purpose:		A simple engine that draws a model
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2010-03-04  RW	
 * 
 * History:		2010-03-04  RW  Created
 *              2020-04-08  RW  Moved to MonoExperience
 * 
 * 
 */
namespace MonoExperience
{

    /// <summary>
    /// A simple engine that draws a model
    /// </summary>
    public class MyFirstModelEngine : BaseEngine
    {

        #region Private members

        private SpriteBatch fSpriteBatch;
        private Random fRandom = new Random();
        private BasicEffect fBasicEffect;
        private Model fModel1;
        private Model fModel2;
        private SimpleCameraController fCamera;
        private PrimitiveLine fLines;
        private bool fHalted = false;
        private BlendState fStateBlend;
        private DepthStencilState fStateDepth;
        private DepthStencilState fStateNoDepth;
        private RasterizerState fStateRasterizer;
        private SamplerState fStateSampler;

        #endregion

        #region Constructor

        /// <summary>
        /// Create the engine
        /// </summary>
        /// <param name="game"></param>
        public MyFirstModelEngine(EngineContainer cnt) : base(cnt)
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

            InitializeTransform();
            InitializeEffect();

            fModel1 = this.Game.Content.Load<Model>("MyFirstModel/grid");
            fModel2 = this.Game.Content.Load<Model>("MyFirstModel/dude");

            fLines = new PrimitiveLine(Game.GraphicsDevice);
            fLines.AddLine(
                            new VertexPositionColor(new Vector3(0, 0, 0), Color.Red),
                            new VertexPositionColor(new Vector3(1000, 0, 0), Color.Yellow));
            fLines.AddLine(
                            new VertexPositionColor(new Vector3(0, 0, 0), Color.Green),
                            new VertexPositionColor(new Vector3(0, 1000, 0), Color.LightGreen));
            fLines.AddLine(
                            new VertexPositionColor(new Vector3(0, 0, 0), Color.Blue),
                            new VertexPositionColor(new Vector3(0, 0, 1000), Color.Cyan));
            fLines.AddLine(
                             new VertexPositionColor(new Vector3(0, 0, 0), Color.DarkRed),
                             new VertexPositionColor(new Vector3(-1000, 0, 0), Color.DarkRed));
            fLines.AddLine(
                            new VertexPositionColor(new Vector3(0, 0, 0), Color.DarkGreen),
                            new VertexPositionColor(new Vector3(0, -1000, 0), Color.DarkGreen));
            fLines.AddLine(
                            new VertexPositionColor(new Vector3(0, 0, 0), Color.DarkBlue),
                            new VertexPositionColor(new Vector3(0, 0, -1000), Color.DarkBlue));
            for (int r = 0; r < 360; r += 10)
            {
                float angle = MathHelper.ToRadians(r);
                fLines.AddCircle(0, 35, 0, 40, 50, Color.Red, 0, 0);
                fLines.AddCircle(0, 35, 0, 40, 50, Color.Tomato, angle, 0);
                fLines.AddCircle(0, 35, 0, 40, 50, Color.Orange, 0, angle);
            }
            fStateBlend = BlendState.Opaque;
            fStateDepth = DepthStencilState.Default;
            fStateNoDepth = DepthStencilState.None;
            fStateRasterizer = new RasterizerState() { FillMode = FillMode.Solid, CullMode = CullMode.None };
            fStateSampler = new SamplerState() { AddressU = TextureAddressMode.Wrap, AddressV = TextureAddressMode.Wrap };

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
            GraphicsDevice.DepthStencilState = fStateDepth;
            GraphicsDevice.BlendState = fStateBlend;
            GraphicsDevice.RasterizerState = fStateRasterizer;
            GraphicsDevice.SamplerStates[0] = fStateSampler;

            Matrix world = fCamera.Camera.WorldMatrix;
            Matrix view = fCamera.Camera.ViewMatrix;
            Matrix projection = fCamera.Camera.ProjectionMatrix;
            if (!fHalted)
            {
                double seconds = gameTime.TotalGameTime.TotalSeconds;
                float angle = (float)seconds / 3;
                world = Matrix.CreateRotationY(angle);
            }

            foreach (EffectPass pass in fBasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                fLines.Render();
            } 
            foreach (ModelMesh mesh in fModel1.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = view;
                    effect.Projection = projection;
                    effect.World = world;
                }
                mesh.Draw();
            }
            foreach (ModelMesh mesh in fModel2.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = view;
                    effect.Projection = projection;
                    effect.World = world;
                }
                mesh.Draw();
            }
            fBasicEffect.View = view;
            fBasicEffect.Projection = projection;
            fBasicEffect.World = world;
            foreach (EffectPass pass in fBasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                fLines.Render();
            }

            base.Draw(gameTime);
        }

        #endregion

        #region BaseEngine

        public override string GetName()
        {
            return "My First Model";
        }

        public override string GetHelp()
        {
            string text1 = "H - Halt rotation";
            string text2 = fCamera.GetHelp();
            return String.Format("{0}\n{1}", text1, text2);
        }

        public override string GetInfo()
        {
            string text1 = "";
            string text2 = fCamera.GetInfo();
            return String.Format("{0}\n{1}", text1, text2);
        }

        public override string GetAbout()
        {
            return "Just a simple model sample\nColor of axises: X-Red, Y-Green, Z, Blue (dark is on the negative side)\nRemember that XNA is a right handed system!";
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
            fCamera.Camera.Position = new Vector3(0.0f, 67.0f, 500.0f);
            fCamera.Camera.Update();
        }

        /// <summary>
        /// Initializes the effect (loading, parameter setting, and technique selection)
        /// used by the game.
        /// </summary>
        private void InitializeEffect()
        {
            fBasicEffect = new BasicEffect(GraphicsDevice);
            fBasicEffect.LightingEnabled = false;
            fBasicEffect.VertexColorEnabled = true; 
        }

        #endregion

    }
}
