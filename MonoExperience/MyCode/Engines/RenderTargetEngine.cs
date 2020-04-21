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
 * File:		RenderTargetEngine
 * Purpose:		A engine that renders to a texture
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2010-06-21  RW	
 * 
 * History:		2010-06-21  RW  Created
 *              2020-04-08  RW  Moved to MonoExperience
 * 
 * 
 */
namespace MonoExperience
{

    /// <summary>
    /// A engine that renders to a texture
    /// </summary>
    public class RenderTargetEngine : BaseEngine
    {

        #region Private members

        private SpriteBatch fSpriteBatch;
        private Random fRandom = new Random();
        private RenderTarget2D fTarget;
        private AlphaTestEffect fTargetEffect;
        private DynamicBillboard fBillboard;
        private BasicEffect fLineEffect;
        private Model fModelGrid;
        private SpriteFont fFont;
        private DynamicPrimitiveLine fLines; 
        private SimpleCameraController fCamera;
        private Texture2D fTestTexture;

        private bool fHalted = false;
        private float fWorldAngle;
        private float fPlaneAngle;


        private const int TARGET_WIDTH = 512;
        private const int TARGET_HEIGHT = 256;

        #endregion

        #region Constructor

        /// <summary>
        /// Create the engine
        /// </summary>
        /// <param name="game"></param>
        public RenderTargetEngine(EngineContainer cnt) : base(cnt)
        {
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

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        /// <summary>
        /// Loads any component specific content
        /// </summary>
        protected override void LoadContent()
        {
            fSpriteBatch = new SpriteBatch(Game.GraphicsDevice);

            fModelGrid = this.Game.Content.Load<Model>("RenderTarget/grid");
            fFont = this.Game.Content.Load<SpriteFont>("RenderTarget/MyFont");
            fTestTexture = this.Game.Content.Load<Texture2D>("RenderTarget/checker");
            fTarget = new RenderTarget2D(GraphicsDevice, TARGET_WIDTH, TARGET_HEIGHT, false, SurfaceFormat.Color, DepthFormat.Depth24);
          
            InitializeTransform();
            InitializeEffect();
            InitializeLines();
            InitializeBillboard();
       
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
                fWorldAngle += Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds * 0.5f);
            }
            world = Matrix.CreateRotationY(fWorldAngle);
            fPlaneAngle += Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds * 2.5f);

            CreateRenderTarget(gameTime);

            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            
            GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            fTargetEffect.View = view;
            fTargetEffect.Projection = projection;
            fTargetEffect.World = world;
            fTargetEffect.Texture = fTarget;
            fTargetEffect.CurrentTechnique.Passes[0].Apply();
            fBillboard.Render();

            base.Draw(gameTime);
        }
      
        /// <summary>
        /// Render to a render target
        /// </summary>
        /// <param name="gameTime"></param>
        private void CreateRenderTarget(GameTime gameTime)
        {
            RenderTargetBinding[] previousRenderTargets = GraphicsDevice.GetRenderTargets();

            GraphicsDevice.SetRenderTarget(fTarget);
            GraphicsDevice.Clear(Color.Transparent);
            
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            float aspectRatio = ((float)GraphicsDevice.Viewport.Width) / ((float)GraphicsDevice.Viewport.Height);
            Matrix modelWorld = Matrix.CreateRotationY(fPlaneAngle) *
                Matrix.CreateRotationX(MathHelper.ToRadians(90));
            Matrix modelProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(40.0f), aspectRatio, 1.0f, 30000.0f);
            Matrix modelView = Matrix.CreateLookAt(new Vector3(0, 0, 4000), Vector3.Zero, Vector3.Up);
            foreach (ModelMesh mesh in fModelGrid.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = modelView;
                    effect.Projection = fCamera.Camera.ProjectionMatrix;
                    effect.World = modelWorld;
                }
                mesh.Draw();
            }

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;            

            Matrix view = Matrix.CreateLookAt(
              new Vector3(0.0f, 0.0f, 1.0f),
              Vector3.Zero,Vector3.Up
              );

            Matrix projection = Matrix.CreateOrthographicOffCenter(
                0,TARGET_WIDTH,TARGET_HEIGHT,0,1.0f, 1000.0f);         

            
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            fLineEffect.World = Matrix.Identity;
            fLineEffect.Projection = projection;
            fLineEffect.View = view;
            fLineEffect.VertexColorEnabled = true;
            fLineEffect.CurrentTechnique.Passes[0].Apply();
            
            fLines.Render();
            fSpriteBatch.Begin();
            string text = DateTime.Now.ToString("HH:mm:ss");
            Vector2 size = fFont.MeasureString(text);
            Vector2 position = new Vector2(TARGET_WIDTH/2-size.X/2,TARGET_HEIGHT/2-size.Y/2);
            fSpriteBatch.DrawString(fFont,text, position, Color.Cornsilk);
            

            fSpriteBatch.End();
            

            GraphicsDevice.SetRenderTargets(previousRenderTargets);
        }

        #endregion

        #region BaseEngine

        public override string GetName()
        {
            return "Render Target Test";
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
            return @"Render primitives to a render target";
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
            //fCamera = new SimpleCamera(GraphicsDevice.Viewport);
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
          
            fTargetEffect = new AlphaTestEffect(Game.GraphicsDevice);
            fTargetEffect.Alpha = 1.0f;
            fTargetEffect.DiffuseColor = Color.White.ToVector3();
            fTargetEffect.VertexColorEnabled = false;
            fTargetEffect.AlphaFunction = CompareFunction.GreaterEqual;
            fTargetEffect.FogEnabled = false;
            fTargetEffect.ReferenceAlpha = 200;

            fLineEffect = new BasicEffect(GraphicsDevice);
            fLineEffect.VertexColorEnabled = true;
        }

        /// <summary>
        /// Initializes the lines
        /// </summary>
        private void InitializeLines()
        {
            fLines = new DynamicPrimitiveLine(GraphicsDevice);
            for (int i = 0; i < 256; i++)
            {
                fLines.AddCircle(TARGET_WIDTH/2, TARGET_HEIGHT/2, 0, i/5.0f, 50, new Color(i, 255-i, 255-i, 255));
            }
            for (int i = 0; i < 5; i++)
            {
                Color color = new Color(255-i*25, 0, 0, 255);
                fLines.AddLine(
                    new VertexPositionColor(new Vector3(i, i, 0), color), 
                    new VertexPositionColor(new Vector3(TARGET_WIDTH - i - 1, i, 0), color));
                fLines.AddLine(
                    new VertexPositionColor(new Vector3(TARGET_WIDTH - i - 1, i, 0), color), 
                    new VertexPositionColor(new Vector3(TARGET_WIDTH - i - 1, TARGET_HEIGHT - i - 1, 0), color));
                fLines.AddLine(
                    new VertexPositionColor(new Vector3(TARGET_WIDTH - i - 1, TARGET_HEIGHT - i - 1, 0), color), 
                    new VertexPositionColor(new Vector3(i, TARGET_HEIGHT - i - 1, 0), color));
                fLines.AddLine(
                    new VertexPositionColor(new Vector3(i, TARGET_HEIGHT - i - 1, 0), color), 
                    new VertexPositionColor(new Vector3(i, i, 0), color));
            }
        }
        /// <summary>
        /// Initializes the billboard
        /// </summary>
        private void InitializeBillboard()
        {
            fBillboard = new DynamicBillboard(GraphicsDevice);
            fBillboard.Mode = BillboardMode.Quad;
            fBillboard.UseVertexBuffer = true;
            const float DISTANCE = 1000f;

            for (int x = -4; x <= 4; x++)
            {
                for (int y = -4; y <= 4; y++)
                {
                    for (int z = -4; z <= 4; z++)
                    {
                        fBillboard.AddObject(
                            new Vector3(DISTANCE * x, DISTANCE * y, DISTANCE * z), Color.White, 400f);
                    }
                }
            }
        }


        #endregion

    }
}
