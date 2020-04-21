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
using MonoGUI.Controls;
using MonoGUI.Engine;
using MonoGUI.GameComponents;
using MonoGUI.Graphics;



/*
 * File:		BasicLines1Engine
 * Purpose:		A simple engine that draws some lines 
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2010-02-28  RW	
 * 
 * History:		2010-02-28  RW  Created
 * 
 */
namespace MonoExperience
{

    /// <summary>
    /// A simple engine that draws some lines
    /// </summary>
    public class BasicLines1Engine : BaseEngine
    {

        public enum LineMode { Circles, Tree };

        #region Private members

        private SpriteBatch fSpriteBatch;
        private Random fRandom = new Random();
        private Matrix fWorldMatrix;
        private Matrix fViewMatrix;
        private Matrix fProjectionMatrix;
        private BasicEffect fBasicEffect;
        private PrimitiveLine fLines;
        private bool fUseVertexBuffer;
        private float PI2 = MathHelper.Pi + MathHelper.Pi;

        private float fAngle;
        private float fAngleSpeed;
        private bool fAngleChanged;
        private LineMode fMode = LineMode.Circles;

        #endregion

        #region Constructor

        /// <summary>
        /// Create the engine
        /// </summary>
        /// <param name="game"></param>
        public BasicLines1Engine(EngineContainer cnt) : base(cnt)
        {
            // TODO: Construct any child components here
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get/set if should be using the graphic card to store the primitives
        /// </summary>
        public bool UseeVertexBuffer
        {
            get
            {
                return fUseVertexBuffer;
            }
            set
            {
                fUseVertexBuffer = value;
            }
        }

        #endregion

        public void Reset()
        {
            SetupLines();
        }

        #region DrawableGameComponent

        private void DoUpdateScale()
        {
            InitializeTransform();
            InitializeEffect();
        }

        private void SetupLines()
        {
            switch (fMode)
            {
                case LineMode.Circles:
                    SetupCircles();
                    break;
                case LineMode.Tree:
                    SetupTree();
                    break;
            }
        }

        private void SetupCircles()
        {
            int maxLines = 16000;
            fLines = new PrimitiveLine(fSpriteBatch.GraphicsDevice);
            bool first = true;
            while (fLines.Lines < maxLines)
            {
                //int w = Game.GraphicsDevice.Viewport.Width;
                //int h = Game.GraphicsDevice.Viewport.Height;
                float x = (float)(1500.0f * (fRandom.NextDouble() - 0.5f));
                float y = (float)(1500.0f * (fRandom.NextDouble() - 0.5f));
                int radius = fRandom.Next(400) + 10;
                int segments = 50;
                byte r = (byte)fRandom.Next(256);
                byte g = (byte)fRandom.Next(256);
                byte b = (byte)fRandom.Next(256);
                byte a = 255;
                if (first)
                {
                    x = 0;
                    y = 0;
                    radius = 500;
                    r = 255;
                    g = 255;
                    b = 255;
                    first = false;
                }
                while (fLines.Lines < maxLines && radius > 20)
                {
                    Color color = new Color(r, g, b, a);
                    fLines.AddCircle(x, y, 0, radius, segments, color);
                    radius -= 20;
                    x++;
                    if (r > 10)
                    {
                        r -= 10;
                    }
                    if (g > 10)
                    {
                        g -= 10;
                    }
                    if (b > 10)
                    {
                        b -= 10;
                    }
                }
            }
        }

        private void SetupTree()
        {
            int maxLines = 16000;
            fLines = new PrimitiveLine(fSpriteBatch.GraphicsDevice);
            double maxRadius = Game.GraphicsDevice.Viewport.Width / 4.0f;

            while (fLines.Lines < maxLines)
            {
                byte r = (byte)fRandom.Next(256);
                byte g = (byte)fRandom.Next(256);
                byte b = (byte)fRandom.Next(256);
                byte a = 0;
                Color c = new Color(r, g, b, a);
                VertexPositionColor p1 = new VertexPositionColor(new Vector3(0, 0, 0), c);
                AddTreeBranch(p1, 8, maxRadius, maxLines);
            }
        }

        private void AddTreeBranch(VertexPositionColor p1, int branches, double maxRadius, int maxLines)
        {
            double radius = fRandom.NextDouble() * maxRadius;
            if (radius > 4.0f && fLines.Lines < maxLines)
            {
                double maxInnerRadius = radius / 1.8f;
                for (int i = 0; i < branches; i++)
                {
                    VertexPositionColor p2 = GetRandomPoint(p1, radius);
                    p2.Color.A += 32;
                    fLines.AddLine(p1, p2);
                    AddTreeBranch(p2, branches, maxInnerRadius, maxLines);
                }
            }
        }

        private VertexPositionColor GetRandomPoint(VertexPositionColor p, double radius)
        {
            double angle = fRandom.NextDouble() * PI2;
            float x = p.Position.X + (float)(radius * Math.Cos(angle));
            float y = p.Position.Y + (float)(radius * Math.Sin(angle));
            VertexPositionColor result = new VertexPositionColor(new Vector3(x, y, 0), p.Color);
            return result;
        }

        /// <summary>
        /// Initializes the transforms used by the game.
        /// </summary>
        private void InitializeTransform()
        {

            fViewMatrix = Matrix.CreateLookAt(
               new Vector3(0.0f, 0.0f, 1.0f),
               Vector3.Zero,
               Vector3.Up
               );

            fProjectionMatrix = Matrix.CreateOrthographicOffCenter(
                0,
                (float)GraphicsDevice.Viewport.Width,
                (float)GraphicsDevice.Viewport.Height,
                0,
                1.0f, 1000.0f);
        }

        /// <summary>
        /// Initializes the effect (loading, parameter setting, and technique selection)
        /// used by the game.
        /// </summary>
        private void InitializeEffect()
        {
            fBasicEffect = new BasicEffect(GraphicsDevice);
            fBasicEffect.VertexColorEnabled = true;

            fAngleChanged = true;
            fBasicEffect.View = fViewMatrix;
            fBasicEffect.Projection = fProjectionMatrix;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        protected override void Initialize()
        {
            InitializeTransform();
            InitializeEffect();
            base.Initialize();
        }

        /// <summary>
        /// Loads any component specific content
        /// </summary>
        protected override void LoadContent()
        {
            fSpriteBatch = new SpriteBatch(Game.GraphicsDevice);

            DoUpdateScale();
            SetupLines();
            base.LoadContent();
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float timeFactor = 100.0f * delta;

            float angleSpeedAdd = 0.001f * timeFactor;
            float angleSpeedMult = 1.0f - (0.1f * timeFactor);
            float maxSpeedChange = 0.05f;
            if (this.Manager.KeyPressed(Keys.V))
            {
                fUseVertexBuffer = !fUseVertexBuffer;
            }
            if (this.Manager.KeyPressed(Keys.M))
            {
                ToggleMode();
                SetupLines();
            }
            if (this.Manager.IsKeyDown(Keys.Z))
            {
                fAngleSpeed -= angleSpeedAdd;
                if (fAngleSpeed < -maxSpeedChange)
                    fAngleSpeed = -maxSpeedChange;
            }
            else if (this.Manager.IsKeyDown(Keys.X))
            {
                fAngleSpeed += angleSpeedAdd;
                if (fAngleSpeed >= maxSpeedChange)
                    fAngleSpeed = maxSpeedChange;
            }
            else
            {
                fAngleSpeed *= angleSpeedMult;
            }
            if (fAngleSpeed < -0.001f || fAngleSpeed > 0.001f)
            {
                fAngle += timeFactor * fAngleSpeed;
                if (fAngle >= PI2)
                    fAngle -= PI2;
                else if (fAngle < 0.0f)
                    fAngle += PI2;
                fAngleChanged = true;
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (fAngleChanged)
            {
                fAngleChanged = false;

                fWorldMatrix = Matrix.CreateRotationZ(fAngle) *
                    Matrix.CreateTranslation(
                    GraphicsDevice.Viewport.Width / 2f,
                    GraphicsDevice.Viewport.Height / 2f, 0);

            }
            fBasicEffect.World = fWorldMatrix;

            fLines.UseVertexBuffer = fUseVertexBuffer;

            foreach (EffectPass pass in fBasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                fLines.Render();
            }
            base.Draw(gameTime);
        }

        internal override GuiWindow DoCreateControlWindow(GuiEngine guiEngine)
        {
            return new BasicLines1EngineControlWindow(guiEngine, this);
        }

        #endregion

        #region BaseEngine

        public override string GetName()
        {
            return "Basic Lines 1.0";
        }

        public override string GetHelp()
        {
            return "V Toggle UseVertexBuffer\nZ/X Rotate\nM Toggle mode";
        }

        public override string GetInfo()
        {
            return String.Format("Number of lines: {0}\nAngle: {1:D}\nUse vertex buffer: {2}",
                fLines.Lines,
                (int)(180.0f * fAngle / MathHelper.Pi),
                fUseVertexBuffer
                );
        }

        public override string GetAbout()
        {
            return @"Draw lines using the PrimitiveLine class";
        }

        public override void DisplayChanged()
        {
            DoUpdateScale();
        }


        #endregion

        public void ToggleMode()
        {
            switch (fMode)
            {
                case LineMode.Circles:
                    fMode = LineMode.Tree;
                    break;
                case LineMode.Tree:
                    fMode = LineMode.Circles;
                    break;
            }
        }


    }
}
