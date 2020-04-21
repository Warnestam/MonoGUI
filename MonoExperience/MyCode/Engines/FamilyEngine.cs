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
 * File:		FamilyEngine 
 * Purpose:		A simple engine with billboards
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2010-06-16  RW	
 * 
 * History:		2010-06-16  RW  Created
 *              2020-04-01  RW  Moved to MonoExperience
 * 
 * 
 */
namespace MonoExperience
{

    /// <summary>
    /// A simple engine that draws point sprites
    /// </summary>
    public class FamilyEngine : BaseEngine
    {

        #region internal struct

        private struct MyPoint
        {
            public Vector3 Position;
            public float Size;
            public int TextureIndex;
        }

        #endregion

        #region Private members

        private enum TextureMode { Faces, Test };

        private const float POINT_SIZE = 200.0f;

        private SpriteBatch fSpriteBatch;
        private Random fRandom = new Random();
        private SimpleCameraController fCamera;

        private List<MyPoint> fPoints = new List<MyPoint>();
        private Effect fShader;
        private AlphaTestEffect fAlpha1;
        private AlphaTestEffect fAlpha2;
        private BasicEffect fBasicEffect;
        private Billboard fBillboard1;
        private Billboard fBillboard2;
        private Billboard fBillboard3;
        private bool fUseVertexBuffer;
        private Texture2D fTexture1a;
        private Texture2D fTexture2a;
        private Texture2D fTexture3a;
        private Texture2D fTexture1b;
        private Texture2D fTexture2b;
        private Texture2D fTexture3b;

        private BlendState fBlendState;

        private bool fHalted = false;
        private float fWorldAngle;
        private BillboardMode fDrawMode = BillboardMode.PointList;
        private TextureMode fTextureMode = TextureMode.Faces;

        #endregion

        #region Constructor

        /// <summary>
        /// Create the engine
        /// </summary>
        /// <param name="game"></param>
        public FamilyEngine(EngineContainer cnt) : base(cnt)
        {
            // TODO: Construct any child components here
        }

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

            fShader = Game.Content.Load<Effect>("Family/BillboardShader");
            fTexture1a = Game.Content.Load<Texture2D>("Family/karin");
            fTexture2a = Game.Content.Load<Texture2D>("Family/oskar");
            fTexture3a = Game.Content.Load<Texture2D>("Family/tage");

            fTexture1b = Game.Content.Load<Texture2D>("Family/test1");
            fTexture2b = Game.Content.Load<Texture2D>("Family/test2");
            fTexture3b = Game.Content.Load<Texture2D>("Family/test3");

            fAlpha1 = new AlphaTestEffect(Game.GraphicsDevice);
            fAlpha1.Alpha = 1.0f;
            fAlpha1.DiffuseColor = Color.White.ToVector3();
            fAlpha1.VertexColorEnabled = false;
            fAlpha1.AlphaFunction = CompareFunction.GreaterEqual;
            fAlpha1.FogEnabled = false;
            fAlpha1.ReferenceAlpha = 200;

            fAlpha2 = new AlphaTestEffect(Game.GraphicsDevice);
            fAlpha2.Alpha = 1.0f;
            fAlpha2.DiffuseColor = Color.White.ToVector3();
            fAlpha2.VertexColorEnabled = false;
            fAlpha2.AlphaFunction = CompareFunction.Less;
            fAlpha2.FogEnabled = false;
            fAlpha2.ReferenceAlpha = 200;

            fBasicEffect = new BasicEffect(GraphicsDevice);
            fBasicEffect.LightingEnabled = false;
            fBasicEffect.VertexColorEnabled = true;
            
            fBlendState = new BlendState();
            fBlendState.AlphaBlendFunction = BlendFunction.Add;
            fBlendState.AlphaDestinationBlend = fBlendState.ColorDestinationBlend = Blend.One;
            fBlendState.AlphaSourceBlend = fBlendState.ColorSourceBlend = Blend.SourceAlpha;
            fBlendState = BlendState.NonPremultiplied;

            InitializeTransform();

            InitPoints();

            
          
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            fCamera.Update(gameTime);

            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (fBillboard1 == null)
            {
                InitBillboard();
            }
            if (this.Manager.KeyPressed(Keys.V))
            {
                fUseVertexBuffer = !fUseVertexBuffer;
                fBillboard1.UseVertexBuffer = fBillboard2.UseVertexBuffer = fBillboard3.UseVertexBuffer = fUseVertexBuffer;
            }
            else if (this.Manager.KeyPressed(Keys.H))
            {
                fHalted = !fHalted;
            }
            else if (this.Manager.KeyPressed(Keys.M))
            {
                if (fDrawMode == BillboardMode.PointList)
                    fDrawMode = BillboardMode.Quad;
                else
                    fDrawMode = BillboardMode.PointList;
                fBillboard1.Mode = fBillboard2.Mode = fBillboard3.Mode = fDrawMode;
                InitBillboard();
            }
            else if (this.Manager.KeyPressed(Keys.T))
            {
                if (fTextureMode == TextureMode.Faces)
                    fTextureMode = TextureMode.Test;
                else
                    fTextureMode = TextureMode.Faces;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            DrawPoints(gameTime);

            base.Draw(gameTime);
        }

        #endregion

        #region BaseEngine

        public override string GetName()
        {
            return "Family Engine";
        }

        public override string GetHelp()
        {
            string text1 = @"V - Toggle use vertex buffer
H - Halt rotation
M - Toggle draw mode
T - Toggle textures";
            string text2 = fCamera.GetHelp();
            return String.Format("{0}\n{1}", text1, text2);
        }

        public override string GetInfo()
        {
            string text1 = String.Format("Points: {0}\nVertexBuffer: {1}\nDraw Mode: {2}\nTexture Mode: {3}",
                fPoints.Count, fUseVertexBuffer, fDrawMode, fTextureMode);
            string text2 = fCamera.GetInfo();
            return String.Format("{0}\n{1}", text1, text2);
        }

        public override string GetAbout()
        {
            return "Playing with billboards and shaders\nTrying to get them to display correctly regardsless of position of objects and camera";
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
            fCamera.Camera.SetProjectionMatrix(MathHelper.ToRadians(35.0f), 1, 100000);
            fCamera.Camera.Rotation = Quaternion.CreateFromYawPitchRoll(0, 0.4f, 0);
            fCamera.Camera.Position = new Vector3(0.0f, 2000.0f, 5000.0f);
            fCamera.Camera.Update();
        }


        /// <summary>
        /// Create new points with random velocity
        /// </summary>
        /// <param name="points"></param>
        private void InitPoints()
        {
            for (int x = -1000; x <= 1000; x += 200)
            {
                for (int z = -1000; z <= 1000; z += 200)
                {
                    MyPoint point = new MyPoint();
                    point.Position = new Vector3(x, 0, z);
                    point.Size = POINT_SIZE;
                    point.TextureIndex = 0;
                    fPoints.Add(point);

                    point = new MyPoint();
                    point.Position = new Vector3(x, POINT_SIZE, z);
                    point.Size = POINT_SIZE;
                    point.TextureIndex = 1;
                    fPoints.Add(point);

                    point = new MyPoint();
                    point.Position = new Vector3(x, 2 * POINT_SIZE, z);
                    point.Size = POINT_SIZE;
                    point.TextureIndex = 2;
                    fPoints.Add(point);
                }
            }
        }

        /// <summary>
        /// Update point positions
        /// </summary>
        /// <param name="points"></param>
        private void UpdatePoints(float seconds)
        {
            // NADA
        }

        /// <summary>
        /// Render the points
        /// </summary>
        private void DrawPoints(GameTime gameTime)
        {
            if (fPoints.Count > 0 && fBillboard1 != null)
            {
                Matrix world = fCamera.Camera.WorldMatrix;
                Matrix view = fCamera.Camera.ViewMatrix;
                Matrix projection = fCamera.Camera.ProjectionMatrix;
                if (!fHalted)
                {
                    fWorldAngle += Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds * 0.5f);
                }
                world = Matrix.CreateRotationY(fWorldAngle);

                fBasicEffect.World = world;
                fBasicEffect.View = view;
                fBasicEffect.Projection = projection;
                fBasicEffect.CurrentTechnique.Passes[0].Apply();
                
                Texture2D[] textures;
                if (fTextureMode == TextureMode.Faces)
                    textures = new Texture2D[] { fTexture1a, fTexture2a, fTexture3a };
                else
                    textures = new Texture2D[] { fTexture1b, fTexture2b, fTexture3b };

                if (fDrawMode == BillboardMode.PointList)
                {
                    GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                    GraphicsDevice.RasterizerState = RasterizerState.CullNone;
                    GraphicsDevice.BlendState = BlendState.Opaque;              
                    Matrix vp = view * projection;
                    fShader.Parameters["world"].SetValue(world);
                    fShader.Parameters["vp"].SetValue(vp);
                    fShader.Parameters["particleTexture"].SetValue(textures[0]);
                    fShader.CurrentTechnique.Passes[0].Apply();
                    fBillboard1.Render();
                    fShader.Parameters["particleTexture"].SetValue(textures[1]);
                    fShader.CurrentTechnique.Passes[0].Apply();
                    fBillboard2.Render();
                    fShader.Parameters["particleTexture"].SetValue(textures[2]);
                    fShader.CurrentTechnique.Passes[0].Apply();
                    fBillboard3.Render();
                }
                else
                {
                    // Step 1
                    GraphicsDevice.BlendState = BlendState.NonPremultiplied;
                    GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                    GraphicsDevice.RasterizerState = RasterizerState.CullNone;

                    fAlpha1.View = view;
                    fAlpha1.Projection = projection;
                    fAlpha1.World = world;
                    fAlpha1.Texture = null;
                    fAlpha1.CurrentTechnique.Passes[0].Apply();
                    
                    fAlpha1.Texture = textures[0];
                    fAlpha1.CurrentTechnique.Passes[0].Apply();
                    fBillboard1.Render();

                    fAlpha1.Texture = textures[1];
                    fAlpha1.CurrentTechnique.Passes[0].Apply();
                    fBillboard2.Render();

                    fAlpha1.Texture = textures[2];
                    fAlpha1.CurrentTechnique.Passes[0].Apply();
                    fBillboard3.Render();

                    // A second rending process can be neccessary for other textures

                    // Step 2
                    //GraphicsDevice.BlendState = BlendState.NonPremultiplied;
                    //GraphicsDevice.DepthStencilState = DepthStencilState.None;
                    //GraphicsDevice.RasterizerState = RasterizerState.CullNone;

                    //fAlpha2.View = view;
                    //fAlpha2.Projection = projection;
                    //fAlpha2.World = world;
                    //fAlpha2.Texture = null;
                    //fAlpha2.CurrentTechnique.Passes[0].Apply();

                    //fAlpha2.Texture = textures[0];
                    //fAlpha2.CurrentTechnique.Passes[0].Apply();
                    //fBillboard1.Render();

                    //fAlpha2.Texture = textures[1];
                    //fAlpha2.CurrentTechnique.Passes[0].Apply();
                    //fBillboard2.Render();

                    //fAlpha2.Texture = textures[2];
                    //fAlpha2.CurrentTechnique.Passes[0].Apply();
                    //fBillboard3.Render();

                    //GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                }
            }
        }

        private void InitBillboard()
        {
            if (fBillboard1 == null)
            {
                fBillboard1 = new Billboard(GraphicsDevice);
                fBillboard2 = new Billboard(GraphicsDevice);
                fBillboard3 = new Billboard(GraphicsDevice);
            }
            else
            {
                fBillboard1.Clear();
                fBillboard2.Clear();
                fBillboard3.Clear();
            }
            fBillboard1.Mode = fBillboard2.Mode = fBillboard3.Mode = fDrawMode;
   
            foreach (MyPoint point in fPoints)
            {
                Color color = Color.White;
                switch (point.TextureIndex)
                {
                    case 0:
                        fBillboard1.AddObject(point.Position, color, point.Size);
                        break;
                    case 1:
                        fBillboard2.AddObject(point.Position, color, point.Size);
                        break;
                    case 2:
                        fBillboard3.AddObject(point.Position, color, point.Size);
                        break;
                }
            }
        }

        #endregion

    }
}
