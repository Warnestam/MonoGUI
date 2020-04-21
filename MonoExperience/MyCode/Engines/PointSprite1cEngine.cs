﻿using System;
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
 * File:		PointSprite1cEngine 
 * Purpose:		A simple engine that point sprites
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2010-06-14  RW	
 * 
 * History:		2010-06-14  RW  Created
 *              2020-04-08  RW  Moved to MonoExperience
 * 
 * 
 */
namespace MonoExperience
{

    /// <summary>
    /// A simple engine that draws point sprites
    /// </summary>
    public class PointSprite1cEngine  : BaseEngine
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

        private const float POINTS_PER_SECOND = 2000.0f;
        private const int INITIAL_POINTS = 500000;        
        private const float MAX_SPEED = 1000.0f;
        private const float MIN_SPEED = 30.0f;
        private const float MAX_DISTANCE = 30000.0f;
        private const float POINT_SIZE = 100.0f;

        private SpriteBatch fSpriteBatch;
        private Random fRandom = new Random();
        private SimpleCameraController fCamera;

        private float fPointsToCreate;

        private List<MyPoint> fPoints = new List<MyPoint>();
        private Effect fShader;
        private DynamicBillboard fBillboard;
        private bool fUseVertexBuffer;
        private bool fBillboardChanged;
        private Texture2D fTexture;
        private BlendState fBlendState;

        #endregion

        #region Constructor

        /// <summary>
        /// Create the engine
        /// </summary>
        /// <param name="game"></param>
        public PointSprite1cEngine(EngineContainer cnt) : base(cnt)
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

            fShader = Game.Content.Load<Effect>("PointSprite1/BillboardShader");
            fTexture = Game.Content.Load<Texture2D>("PointSprite1/pointWhite");

            fBlendState = new BlendState();
            fBlendState.AlphaBlendFunction = BlendFunction.Add;
            fBlendState.AlphaDestinationBlend = fBlendState.ColorDestinationBlend = Blend.One;
            fBlendState.AlphaSourceBlend = fBlendState.ColorSourceBlend = Blend.SourceAlpha;

            InitializeTransform();
            InitializeEffect();

            AddNewPoints(INITIAL_POINTS);

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

            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (fBillboard == null)
            {
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
            return "Point Sprite 1c";
        }

        public override string GetHelp()
        {
            string text1 = @"V - Toggle use vertex buffer";
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
            return @"Render unlimited billboards using DynamicBillboard. In XNA 4.0 there are no longer support for point sprites";
        }


        /// <summary>
        /// Handle the input
        /// </summary>
        /// <param name="gameTime"></param>
        private void HandleInput(GameTime gameTime)
        {
            if (this.Manager.KeyPressed(Keys.V))
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
            fCamera.Camera.SetProjectionMatrix(MathHelper.ToRadians(35.0f), 1, 100000);
            fCamera.Camera.Rotation = Quaternion.CreateFromYawPitchRoll(0, 0.0f, 0);
            fCamera.Camera.Position = new Vector3(0.0f, 0.0f, 5000.0f);
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
                point.Position = point.Direction * Convert.ToSingle((fRandom.NextDouble() * MAX_DISTANCE));
                float heavy = (float)fRandom.NextDouble();
                heavy = heavy * heavy * heavy * heavy;
                point.Speed = (1.0f - heavy) * MAX_SPEED + MIN_SPEED;
                point.Size = heavy * POINT_SIZE;
                fPoints.Add(point);
            }
            fBillboardChanged = true;
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
            fBillboardChanged = true;
        }

        /// <summary>
        /// Render the points
        /// </summary>
        private void DrawPoints()
        {
            if (fPoints.Count > 0 && fBillboard != null)
            {               
                GraphicsDevice.BlendState = fBlendState;
                GraphicsDevice.DepthStencilState = DepthStencilState.None;
                Matrix vp = fCamera.Camera.ViewMatrix * fCamera.Camera.ProjectionMatrix;
                fShader.Parameters["world"].SetValue(fCamera.Camera.WorldMatrix);
                fShader.Parameters["vp"].SetValue(vp);
                fShader.Parameters["particleTexture"].SetValue(fTexture);
                for (int ps = 0; ps < fShader.CurrentTechnique.Passes.Count; ps++)
                {
                    fShader.CurrentTechnique.Passes[ps].Apply();
                    fBillboard.Render();
                }
            }
        }

        private void InitBillboard()
        {
            fBillboardChanged = false;
            if (fBillboard == null)
                fBillboard = new DynamicBillboard(GraphicsDevice);
            else
                fBillboard.Clear();
            foreach (MyPoint point in fPoints)
            {
                Color color = Color.White;
                Vector3 direction = point.Direction;
                int r = (byte)(128 + 127 * direction.X);
                int g = (byte)(128 + 127 * direction.Y);
                int b = (byte)(128 + 127 * direction.Z);
                color = new Color(r, g, b, 255);

                fBillboard.AddObject(point.Position, color, point.Size);
            }
        }

        #endregion

    }
}
