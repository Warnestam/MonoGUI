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
 * File:		PointSprite1aEngine 
 * Purpose:		A simple engine that point sprites
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2010-03-05  RW	
 * 
 * History:		2010-03-05  RW  Created
 *              2020-04-08  RW  Moved to MonoExperience
 * 
 * 
 */
namespace MonoExperience
{

    /// <summary>
    /// A simple engine that draws point sprites
    /// </summary>
    public class PointSprite1aEngine : BaseEngine
    {

        #region internal struct

        private struct MyPoint
        {
            public Vector3 Position;
            public Vector3 Direction;
            public float Speed;
            public float Size;
        }

        #endregion

        #region Private members

        private const float POINTS_PER_SECOND = 1500.0f;
        private const float MAX_SPEED = 1000.0f;
        private const float MIN_SPEED = 30.0f;
        private const float MAX_DISTANCE = 1000.0f;
        private const float POINT_SIZE = 100.0f;

        private SpriteBatch fSpriteBatch;
        private Random fRandom = new Random();
        private SimpleCameraController fCamera;
        private bool fHalted;

        private float fPointsToCreate;

        private Effect fShader;
        private DynamicVertexBuffer fVertexBuffer;
        private DynamicIndexBuffer fIndexBuffer;
        private int fNumberOfParticles;
        private int fNumberOfVertices;
        private int fNumberOfIndices;
        private List<MyPoint> fPoints = new List<MyPoint>();
        private Texture2D fTexture;

        #endregion

        #region Constructor

        /// <summary>
        /// Create the engine
        /// </summary>
        /// <param name="game"></param>
        public PointSprite1aEngine(EngineContainer cnt) : base(cnt)
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

            fShader = Game.Content.Load<Effect>("PointSprite1/BillboardShader");
            fTexture = Game.Content.Load<Texture2D>("PointSprite1/point");

            InitializeTransform();
            InitializeEffect();

            base.LoadContent();
        }

        int speedy = 0;

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            HandleInput(gameTime);
            fCamera.Update(gameTime);

            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!fHalted)
            {
                UpdatePoints(seconds);
                fPointsToCreate += (seconds * POINTS_PER_SECOND);
                if (fPointsToCreate > 1.0f)
                {
                    int newPoints = (int)fPointsToCreate;
                    fPointsToCreate -= newPoints;
                    AddNewPoints(newPoints);
                }
            }
            if (fIndexBuffer == null)
            {
                CalculateBuffers();
            }
            else if (!fHalted)
            {
                // I was previously using point sprites for this sample. With XNA 4.0 that function disappeared
                // and the framerate of this sample dropped as more data was needed from the CPU to the GPU
                // Simple trick to create a higher frame rate...
                //speedy=(speedy+1)%5;
                //if (speedy==0)
                CalculateBuffers();
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
            return "Point Sprite 1a";
        }

        public override string GetHelp()
        {
            string text1 = "H - Toggle halt";
            string text2 = fCamera.GetHelp();
            return String.Format("{0}\n{1}", text1, text2);
        }

        public override string GetInfo()
        {
            string text1 = String.Format("Points: {0}", fPoints.Count);
            string text2 = fCamera.GetInfo();
            return String.Format("{0}\n{1}", text1, text2);
        }

        public override string GetAbout()
        {
            return @"Render billboards without using the Billboard class. In XNA 4.0 there are no longer support for point sprites";
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
            fCamera.Camera.Position = new Vector3(0.0f, 0.0f, -40.0f);
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
        private void AddNewPoints(int points)
        {

            for (int i = 0; i < points; i++)
            {
                MyPoint point = new MyPoint();
                point.Position = Vector3.Zero;
                float sx = (float)(fRandom.NextDouble() - 0.5f);
                float sy = (float)(fRandom.NextDouble() - 0.5f);
                float sz = (float)(fRandom.NextDouble() - 0.5f);
                point.Direction = new Vector3(sx, sy, sz);
                point.Direction.Normalize();
                float heavy = (float)fRandom.NextDouble();
                heavy = heavy * heavy * heavy * heavy;
                point.Speed = (1.0f - heavy) * MAX_SPEED + MIN_SPEED;
                point.Size = heavy * POINT_SIZE;
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
                point.Position += point.Direction * point.Speed * seconds;
                float distance = Vector3.Distance(point.Position, Vector3.Zero);
                if (distance > MAX_DISTANCE)
                {
                    fPoints.RemoveAt(i);
                    i--;
                }
                else
                {
                    fPoints[i] = point;
                }
            }
        }

        /// <summary>
        /// Render the points
        /// </summary>
        private void DrawPoints()
        {
            if (fPoints.Count > 0 && fIndexBuffer != null)
            {
                BlendState test = new BlendState();
                test.AlphaBlendFunction = BlendFunction.Add;
                test.AlphaDestinationBlend = test.ColorDestinationBlend = Blend.One;
                test.AlphaSourceBlend = test.ColorSourceBlend = Blend.SourceAlpha;
                GraphicsDevice.BlendState = test;
                GraphicsDevice.DepthStencilState = DepthStencilState.None;

                GraphicsDevice.SetVertexBuffer(fVertexBuffer);
                GraphicsDevice.Indices = fIndexBuffer;

                Matrix vp = fCamera.Camera.ViewMatrix * fCamera.Camera.ProjectionMatrix;
                fShader.Parameters["world"].SetValue(fCamera.Camera.WorldMatrix);
                fShader.Parameters["vp"].SetValue(vp);
                fShader.Parameters["particleTexture"].SetValue(fTexture);
                for (int ps = 0; ps < fShader.CurrentTechnique.Passes.Count; ps++)
                {
                    fShader.CurrentTechnique.Passes[ps].Apply();
                    fSpriteBatch.GraphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList,
                            0, // baseVertex
                            0,  // minVertexIndex
                           fNumberOfVertices, // numVertices
                            0, // startIndex
                            2 * fNumberOfParticles); // primitiveCount
                }
            }
        }

        private void CalculateBuffers()
        {
            if (fPoints.Count > 0)
            {
                fNumberOfParticles = fPoints.Count;
                fNumberOfVertices = fPoints.Count * 4;
                fNumberOfIndices = fPoints.Count * 6;
                VertexBillboardParticle[] vertices = new VertexBillboardParticle[fNumberOfVertices];
                short[] indices = new short[fNumberOfIndices];
                int verticeNumber = 0;
                int indiceNumber = 0;
                for (int i = 0; i < fNumberOfParticles; i++)
                {
                    MyPoint point = fPoints[i];
                    int verticeNumberBase = verticeNumber;
                    vertices[verticeNumber++] = new VertexBillboardParticle(point.Position, Vector2.Zero, point.Size, Color.White);
                    vertices[verticeNumber++] = new VertexBillboardParticle(point.Position, new Vector2(1, 0), point.Size, Color.White);
                    vertices[verticeNumber++] = new VertexBillboardParticle(point.Position, new Vector2(0, 1), point.Size, Color.White);
                    vertices[verticeNumber++] = new VertexBillboardParticle(point.Position, Vector2.One, point.Size, Color.White);
                    indices[indiceNumber++] = (short)(0 + verticeNumberBase);
                    indices[indiceNumber++] = (short)(1 + verticeNumberBase);
                    indices[indiceNumber++] = (short)(2 + verticeNumberBase);
                    indices[indiceNumber++] = (short)(1 + verticeNumberBase);
                    indices[indiceNumber++] = (short)(3 + verticeNumberBase);
                    indices[indiceNumber++] = (short)(2 + verticeNumberBase);
                }

                // 2020-04-01 Out of memory, which hasent happened before
                //fIndexBuffer = new DynamicIndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, fNumberOfIndices, BufferUsage.WriteOnly);
                //fIndexBuffer.SetData(indices);
                //fVertexBuffer = new DynamicVertexBuffer(GraphicsDevice, VertexBillboardParticle.VertexDeclaration, fNumberOfVertices, BufferUsage.WriteOnly);
                //fVertexBuffer.SetData(vertices);

                if (fIndexBuffer != null)
                {
                    fIndexBuffer.Dispose();
                }
                if (fVertexBuffer != null)
                {
                    fVertexBuffer.Dispose();
                }

                fIndexBuffer = new DynamicIndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, fNumberOfIndices, BufferUsage.WriteOnly);
                fVertexBuffer = new DynamicVertexBuffer(GraphicsDevice, VertexBillboardParticle.VertexDeclaration, fNumberOfVertices, BufferUsage.WriteOnly);

                fIndexBuffer.SetData(indices);
                fVertexBuffer.SetData(vertices);
            }
        }

        #endregion

    }
}
