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
 * File:		PointSprite2aEngine
 * Purpose:		A simple engine that point sprites
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2010-03-19  RW	
 * 
 * History:		2010-03-19  RW  Created
 *              2020-04-08  RW  Moved to MonoExperience
 * 
 */
namespace MonoExperience
{

    /// <summary>
    /// A simple engine that draws point sprites
    /// </summary>
    public class PointSprite2aEngine : BaseEngine
    {

        #region internal struct

        private struct MyPoint
        {
            public Vector3 Position;
            public float Radius;
            public float Angle;
            public float AngleSpeed;
            public float PositionY;
            public float Size;
            public int TextureIndex;
        }

        #endregion

        #region Private members

        private const int NUMBER_OF_POINTS = 5000;
        private const float POINT_SIZE = 100.0f;
        private const float RADIUS = 1300.0f;
        private const float RADIUS_VARITAION = 1000.0f;

        private SpriteBatch fSpriteBatch;
        private Random fRandom = new Random();
        private SimpleCameraController fCamera;

        private string[] TEXTURE_NAMES = { "p1", "p2", "p3", "p4", "p5", "p6", "p7", "p8", "p9", "p10", "p11", "p12", "p13" };

        private bool fHalted;

        private float fPointsToCreate;

        private List<MyPoint> fPoints = new List<MyPoint>();
        private Effect fShader;
        private List<Billboard> fBillboards;
        private bool fUseVertexBuffer;
        private bool fBillboardChanged;
        private List<Texture2D> fTextures;
        private BlendState fBlendState;

        #endregion

        #region Constructor

        /// <summary>
        /// Create the engine
        /// </summary>
        /// <param name="game"></param>
        public PointSprite2aEngine(EngineContainer cnt) : base(cnt)
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


            InitializeTransform();
            InitializeEffect();
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

            fTextures = new List<Texture2D>();
            foreach(string name in TEXTURE_NAMES)
                fTextures.Add(Game.Content.Load<Texture2D>(String.Format("PointSprite2/{0}",name)));
            fShader = Game.Content.Load<Effect>("PointSprite2/BillboardShader");

            fBlendState = new BlendState();
            fBlendState.AlphaBlendFunction = BlendFunction.Add;
            fBlendState.AlphaDestinationBlend = fBlendState.ColorDestinationBlend = Blend.One;
            fBlendState.AlphaSourceBlend = fBlendState.ColorSourceBlend = Blend.SourceAlpha;

            InitializeTransform();
            InitializeEffect();
            CreatePoints();

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

            if (!fHalted)
            {
                float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
                UpdatePoints(seconds);
            }
            if (fBillboards == null)
            {
                InitBillboard();
            }
            else if (!fHalted && fBillboardChanged)
            {
                // I was previously using point sprites for this sample. With XNA 4.0 that function disappeared
                // and the framerate of this sample dropped as more data was needed from the CPU to the GPU
                // Simple trick to create a higher frame rate...
                //speedy=(speedy+1)%5;
                //if (speedy==0)
                InitBillboard();
            }
          
            base.Update(gameTime);
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {            
            DrawPoints();
            base.Draw(gameTime);
        }

        #endregion

        #region BaseEngine

        public override string GetName()
        {
            return "Point Sprite 2.0A";
        }

        public override string GetHelp()
        {
            string text1 = @"H - Toggle halt
V - Toggle use vertex buffer";
            string text2 = fCamera.GetHelp();
            return String.Format("{0}\n{1}", text1, text2);
        }

        public override string GetInfo()
        {
            string text1 = String.Format("Points: {0}\nVertexBuffer: {1}", fPoints.Count, fUseVertexBuffer);
            string text2 = fCamera.GetInfo();
            return String.Format("{0}\n{1}", text1, text2);
        }

        public override string GetAbout()
        {
            return @"Render billboards using the Billboard class. In XNA 4.0 there are no longer support for point sprites";
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
            else if (this.Manager.KeyPressed(Keys.V))
            {
                fUseVertexBuffer = !fUseVertexBuffer;
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

            fCamera.Camera.Rotation = Quaternion.CreateFromYawPitchRoll(0, 0.3f, 0);
            fCamera.Camera.Position = new Vector3(0.0f, 3000.0f, 10000.0f);
            fCamera.Camera.Update();
        }

        /// <summary>
        /// Initializes the effect (loading, parameter setting, and technique selection)
        /// used by the game.
        /// </summary>
        private void InitializeEffect()
        {            
        }

        /// <summary>
        /// Create new points with random velocity
        /// </summary>
        /// <param name="points"></param>
        private void CreatePoints()
        {
            for (int i = 0; i < NUMBER_OF_POINTS; i++)
            {
                MyPoint point = new MyPoint();
                point.Angle = (float)fRandom.NextDouble() * MathHelper.TwoPi;
                point.AngleSpeed = (float)fRandom.NextDouble() + 0.2f;
                //point.Position
                point.PositionY = (float)((fRandom.NextDouble() - 0.5f) * 2.0f * RADIUS_VARITAION);
                point.Radius = RADIUS + (float)((fRandom.NextDouble() - 0.5f) * 2.0f * RADIUS_VARITAION); 
                point.TextureIndex = fRandom.Next(fTextures.Count);
                point.Size = POINT_SIZE;
                fPoints.Add(point);
            }
        }

        /// <summary>
        /// Update point positions
        /// </summary>
        /// <param name="points"></param>
        private void UpdatePoints(float seconds)
        {
            for (int i = 0; i < fPoints.Count; i++)
            {
                MyPoint point = fPoints[i];
                point.Angle += point.AngleSpeed * seconds;
                if (point.Angle > MathHelper.TwoPi)
                    point.Angle -= MathHelper.TwoPi;
                Vector3 position = new Vector3(
                    (float)(point.Radius * Math.Sin(point.Angle)),
                    point.PositionY,
                    (float)(point.Radius * Math.Cos(point.Angle)));
                point.Position = position;
                fPoints[i] = point;
            }
            fBillboardChanged = true;
        }

        /// <summary>
        /// Render the points
        /// </summary>
        private void DrawPoints()
        {
            if (fPoints.Count > 0)
            {
                if (fPoints.Count > 0 && fBillboards != null)
                {
                    GraphicsDevice.BlendState = fBlendState;
                    GraphicsDevice.DepthStencilState = DepthStencilState.None;
                    Matrix vp = fCamera.Camera.ViewMatrix * fCamera.Camera.ProjectionMatrix;
                    fShader.Parameters["world"].SetValue(fCamera.Camera.WorldMatrix);
                    fShader.Parameters["vp"].SetValue(vp);
                    int i=0;
                    foreach(Billboard billboard in fBillboards)
                    {
                        fShader.Parameters["particleTexture"].SetValue(fTextures[i]);                   
                        for (int ps = 0; ps < fShader.CurrentTechnique.Passes.Count; ps++)
                        {
                            fShader.CurrentTechnique.Passes[ps].Apply();
                            billboard.Render();
                        }
                        i++;
                    }
                }
            }
        }

        private void InitBillboard()
        {
            fBillboardChanged = false;
            if (fBillboards == null)
            {
                fBillboards = new List<Billboard>();
                for (int i = 0; i < fTextures.Count; i++)
                    fBillboards.Add( new Billboard(GraphicsDevice) );
            }
            else
            {
                for (int i = 0; i < fTextures.Count; i++)
                    fBillboards[i].Clear();
            }
            foreach (MyPoint point in fPoints)
            {
                Color color = Color.White;
                fBillboards[point.TextureIndex].AddObject(point.Position, color, point.Size);
            }
        }

        #endregion

    }
}
