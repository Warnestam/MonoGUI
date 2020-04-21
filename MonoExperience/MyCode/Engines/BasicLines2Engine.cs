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
using MonoGUI.Graphics;

/*
 * File:		BasicLines2Engine
 * Purpose:		A simple engine that draws a lot of lines 
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2010-02-28  RW	
 * 
 * History:		2010-02-28  RW  Created
 *              2010-07-xx  RW  using DynamicPrimitiveLines instead of PrimitiveLineExtra
 *              2020-04-06  RW  Moved to MonoExperience
 */
namespace MonoExperience
{

    /// <summary>
    /// A simple engine that draws some lines
    /// </summary>
    public class BasicLines2Engine : BaseEngine
    {

        #region Private members

        private SpriteBatch fSpriteBatch;
        private Random fRandom = new Random();
        private Matrix fWorldMatrix;
        private Matrix fViewMatrix;
        private Matrix fProjectionMatrix;
        private BasicEffect fBasicEffect;
        private DynamicPrimitiveLine fLines;
        private bool fUseVertexBuffer;
        private float PI2 = MathHelper.Pi + MathHelper.Pi;

        private float fAngle;
        private float fAngleSpeed;
        private bool fAngleChanged;
        private bool fManyLines = false;
        private bool fLinesChanged = true;

        #endregion

        #region Constructor

        public BasicLines2Engine(EngineContainer cnt) : base(cnt)
        {
            // TODO: Construct any child components here
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get/set if should be using the graphic card to store the primitives
        /// </summary>
        public bool UseVertexBuffer
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

        #region DrawableGameComponent

        protected override void Initialize()
        {
            InitializeEffect();
            ScaleChanged();

            base.Initialize();
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
        }

        private void ScaleChanged()
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
            
            fBasicEffect.View = fViewMatrix;
            fBasicEffect.Projection = fProjectionMatrix;
        }


        /// <summary>
        /// Loads any component specific content
        /// </summary>
        protected override void LoadContent()
        {
            fSpriteBatch = new SpriteBatch(Game.GraphicsDevice);

            base.LoadContent();
        }

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
            else if (this.Manager.KeyPressed(Keys.C))
            {
                fManyLines = !fManyLines;
                fLinesChanged = true;
            }

            //if (this.Manager.IsKeyDown(Keys.Z))
            //{
            //    fAngleSpeed -= angleSpeedAdd;
            //    if (fAngleSpeed < -maxSpeedChange)
            //        fAngleSpeed = -maxSpeedChange;
            //}
            //else if (this.Manager.IsKeyDown(Keys.X))
            //{
            //    fAngleSpeed += angleSpeedAdd;
            //    if (fAngleSpeed >= maxSpeedChange)
            //        fAngleSpeed = maxSpeedChange;
            //}
            //else
            //{
            //    fAngleSpeed *= angleSpeedMult;
            //}
            //if (fAngleSpeed < -0.001f || fAngleSpeed > 0.001f)
            //{
            //    fAngle += timeFactor * fAngleSpeed;
            //    if (fAngle >= PI2)
            //        fAngle -= PI2;
            //    else if (fAngle < 0.0f)
            //        fAngle += PI2;
            //    fAngleChanged = true;
            //}
            fAngle += timeFactor * 0.1f;
            if (fAngle >= PI2)
                fAngle -= PI2;
            fAngleChanged = true;

            if (fLinesChanged)
                InitLines();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (fLines != null)
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
            }
            base.Draw(gameTime);
        }

        #endregion

        #region BaseEngine

        public override string GetName()
        {
            return "Basic Lines 2.0";
        }

        public override string GetHelp()
        {
            return "V Toggle UseVertexBuffer\nC Change number of lines";
        }

        public override string GetInfo()
        {
            return String.Format("Number of lines: {0}\nAngle: {1:D}\nUse vertex buffer: {2}", 
                fLines?.Lines,
                (int)(180.0f * fAngle / MathHelper.Pi),
                fUseVertexBuffer
                );
        }

        public override string GetAbout()
        {
            return @"Drawing unlimited lines with the DynamicPrimitiveLine class";
        }

        public override void DisplayChanged()
        {
            ScaleChanged();
            fAngleChanged = true;
        }

        #endregion

        #region Private methods

        private void InitLines()
        {            
            fLinesChanged = false;

            fLines = new DynamicPrimitiveLine(fSpriteBatch.GraphicsDevice);
                  
            int maxLines = 1000000;
            float rStep = 0.3f;
            int segments = 500;

            if (!fManyLines)
            {
                maxLines = 20000;
                rStep = 30.0f;
            }
                        
            float radius1 = 0;
            Color color1a = GetRandomColor();
            Color color1b = GetRandomColor();
            Color color2a = GetRandomColor();
            Color color2b = GetRandomColor();

            int colorStep = 0;

            while (fLines.Lines < maxLines)
            {
                Color colorA = GetColor(color1a, color2a, colorStep / 100.0f);
                Color colorB = GetColor(color1b, color2b, colorStep / 100.0f);

                float radius2 = radius1 + rStep;

                fLines.AddCircle(0, 0, 0, radius1, radius2, segments, colorA, colorB);

                colorStep++;
                if (colorStep>100)
                {
                    color1a = color2a;
                    color1b = color2b;
                    color2a = GetRandomColor();
                    color2b = GetRandomColor();
                    colorStep = 0;
                }
                radius1 = radius2;
            }
        }

        private Color GetRandomColor()
        {
            int r = fRandom.Next(256);
            int g = fRandom.Next(256);
            int b = fRandom.Next(256);
            return new Color(r, g, b, 255);
        }

        private Color GetColor(Color c1, Color c2, float f)
        {
            float f1 = 1.0f - f;

            int r = Convert.ToInt16(c1.R * f1 + c2.R * f);
            int g = Convert.ToInt16(c1.G * f1 + c2.G * f);
            int b = Convert.ToInt16(c1.B * f1 + c2.B * f);

            Color color = new Color(r, g, b, 255);
            return color;
        }


        #endregion

    }
}
