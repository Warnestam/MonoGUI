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
 * File:		BasicPlaneEngine
 * Purpose:		A simple engine that draws a plane
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2010-02-28  RW	
 * 
 * History:		2010-02-28  RW  Created
 *              2020-04-06  RW  Moved to MonoExperience
 * 
 * 
 */
namespace MonoExperience
{

    /// <summary>
    /// A simple engine that draws a plane
    /// </summary>
    public class BasicPlaneEngine : BaseEngine
    {

        #region Enums

        public enum DrawMode { Solid, WireFrame, Grid };

        public enum ColorMode { Points, SkyBlue };

        public enum NormalMode { Standard, Crazy };

        #endregion

        #region Private members

        private SpriteBatch fSpriteBatch;
        private Random fRandom = new Random();
        private BasicEffect fBasicEffect;
        private PrimitivePlane fPlane;
        private SimpleCameraController fCamera;

        private RasterizerState fStateSolid;
        private RasterizerState fStateWire;
        private DepthStencilState fStateDepth;


      
        private bool fHalted = false;
        private bool fUpdatePlane;
        private bool fUpdateDiagram;
        private bool fUpdateColors;
        private bool fUpdateNormals;

        private DrawMode fDrawMode = DrawMode.Solid;
        private ColorMode fColorMode = ColorMode.SkyBlue;
        private NormalMode fNormalMode = NormalMode.Standard;

        #endregion

        #region Constructor

        /// <summary>
        /// Create the engine
        /// </summary>
        /// <param name="game"></param>
        public BasicPlaneEngine(EngineContainer cnt) : base(cnt)
        {
            // TODO: Construct any child components here
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get/set how to render the plane
        /// </summary>
        public DrawMode CurrentDrawMode
        {
            get
            {
                return fDrawMode;
            }
            set
            {
                fDrawMode = value;
            }
        }

        /// <summary>
        /// Get/set how to colorize the plane
        /// </summary>
        public ColorMode CurrentColorMode
        {
            get
            {
                return fColorMode;
            }
            set
            {
                fColorMode = value;
                fUpdateColors = true;
            }
        }

        /// <summary>
        /// Get/set how to calculate the normals
        /// </summary>
        public NormalMode CurrentNormalMode
        {
            get
            {
                return fNormalMode;
            }
            set
            {
                fNormalMode = value;
                fUpdateNormals = true;
            }
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

            fUpdateColors = true;
            fUpdatePlane = true;
            fUpdateDiagram = true;
            fUpdateNormals = true;

            fStateSolid = new RasterizerState() { FillMode = FillMode.Solid, CullMode=CullMode.None };
            fStateWire = new RasterizerState() { FillMode = FillMode.WireFrame, CullMode = CullMode.None };
            fStateDepth = new DepthStencilState() { DepthBufferEnable = true };           
            
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (this.Manager.KeyPressed(Keys.M))
            {
                switch (fDrawMode)
                {
                    case DrawMode.Grid:
                        CurrentDrawMode = DrawMode.Solid;
                        break;
                    case DrawMode.Solid:
                        CurrentDrawMode = DrawMode.WireFrame;
                        break;
                    case DrawMode.WireFrame:
                        CurrentDrawMode = DrawMode.Grid;
                        break;
                }
            }
            else if (this.Manager.KeyPressed(Keys.C))
            {
                switch (fColorMode)
                {
                    case ColorMode.Points:
                        CurrentColorMode = ColorMode.SkyBlue;
                        break;
                    case ColorMode.SkyBlue:
                        CurrentColorMode = ColorMode.Points;
                        break;
                }
            }
            else if (this.Manager.KeyPressed(Keys.N))
            {
                switch (fNormalMode)
                {
                    case NormalMode.Crazy:
                        CurrentNormalMode = NormalMode.Standard;
                        break;
                    case NormalMode.Standard:
                        CurrentNormalMode = NormalMode.Crazy;
                        break;
                }
            }

            fCamera.Update(gameTime);
            
            double seconds = gameTime.TotalGameTime.TotalSeconds;

            if (!fHalted)
                fUpdateDiagram = true;

            fUpdateDiagram = fUpdateDiagram || fUpdatePlane;
            fUpdateColors = fUpdateColors || fUpdateDiagram;
            fUpdateNormals = fUpdateNormals || fUpdateDiagram;

            if (fUpdatePlane)
            {
                SetPlane();
            }
            int max = fPlane.Segments;

            if (fUpdateDiagram)
            {
                fUpdateDiagram = false;
                for (int x = 0; x <= max; x++)
                {
                    float nX = 2.0f * (float)x / max - 1.0f;// -1 to +1
                    for (int z = 0; z <= max; z++)
                    {
                        float nZ = 2.0f * (float)z / max - 1.0f;
                        double r = Math.Sqrt(nX * nX + nZ * nZ);
                        // function
                        double y = nX * Math.Cos(r * 7.0f - seconds * 4.2f) +
                            nZ * Math.Sin((r - 0.5) * 10.0f - seconds * 1.9f);

                        float height = 200.0f * (float)y;
                        fPlane.SetHeight(x, z, height);
                    }
                }

            }
            if (fUpdateColors)
                SetColors();
            if (fUpdateNormals)
            {
                switch (fNormalMode)
                {
                    case NormalMode.Crazy:
                        for (int x = 0; x <= max; x++)
                        {
                            float nX = 2.0f * (float)x / max - 1.0f;// -1 to +1
                            for (int z = 0; z <= max; z++)
                            {
                                float nZ = 2.0f * (float)z / max - 1.0f;
                                // function
                                double r = Math.Sqrt(nX * nX + nZ * nZ);
                                VertexPositionNormalColor p = fPlane.GetPoint(x, z);
                                p.Normal = Vector3.Normalize(new Vector3(
                                    Convert.ToSingle(Math.Cos(r * 1.0f + 0.5f - seconds * 0.1f)),
                                    Convert.ToSingle(Math.Cos(r * 2.0f + 2.1f - seconds * 0.2f)),
                                   Convert.ToSingle(Math.Cos(r * 5.1f * seconds))));
                                fPlane.SetPoint(x, z, p);
                            }
                        }
                        break;
                    case NormalMode.Standard:
                        fPlane.GenerateNormals();
                        break;
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            fSpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            fBasicEffect.View = fCamera.Camera.ViewMatrix;
            fBasicEffect.Projection = fCamera.Camera.ProjectionMatrix;
            GraphicsDevice.DepthStencilState = fStateDepth;

            Matrix planeMatrix = fCamera.Camera.WorldMatrix;
            if (!fHalted)
            {
                double seconds = gameTime.TotalGameTime.TotalSeconds;
                float angle = (float)seconds / 3;
                Matrix matrix = Matrix.CreateRotationY(angle);
                planeMatrix = matrix;
            }


            Effect effect = fBasicEffect;

            if (fDrawMode == DrawMode.WireFrame)
            {
                GraphicsDevice.RasterizerState = fStateWire;
            }
            else
            {
                GraphicsDevice.RasterizerState = fStateSolid;
            }

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                fBasicEffect.World = planeMatrix;
                pass.Apply();

                switch (fDrawMode)
                {
                    case DrawMode.Grid:
                        fPlane.RenderLines(fSpriteBatch);
                        break;
                    case DrawMode.Solid:
                    case DrawMode.WireFrame:
                        fPlane.RenderTriangles(fSpriteBatch);
                        break;
                }
            }
            //GraphicsDevice.RasterizerState.FillMode = FillMode.Solid;
            fSpriteBatch.End();
            base.Draw(gameTime);
        }

        #endregion

        #region BaseEngine

        public override string GetName()
        {
            return "Basic Plane 1.0";
        }

        public override string GetHelp()
        {
            string text1 = @"M - Change Draw Mode
C - Change Color Mode
N - Change Normal Mode";
            string text2 = fCamera.GetHelp();
            return String.Format("{0}\n{1}", text1, text2);
        }

        public override string GetInfo()
        {
            string text1 = String.Format("Draw mode: {0}\nColor mode: {1}\nNormal mode: {2}",
               fDrawMode, fColorMode, fNormalMode);
            string text2 = fCamera.GetInfo();
            return String.Format("{0}\n{1}", text1, text2);
        }

        public override string GetAbout()
        {
            return @"Drawing a plane using the PrimitivePlane class";
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

            fCamera.Camera.Rotation = Quaternion.CreateFromYawPitchRoll(0, -2.6f, 0);
            fCamera.Camera.Position = new Vector3(0.0f, -3000.0f, -5000.0f);
            //fCamera.Position = new Vector3(0.0f, -300.0f, -500.0f);
            fCamera.Camera.Update();
        }

        /// <summary>
        /// Initializes the effect (loading, parameter setting, and technique selection)
        /// used by the game.
        /// </summary>
        private void InitializeEffect()
        {
            fBasicEffect = new BasicEffect(GraphicsDevice);
            fBasicEffect.VertexColorEnabled = true;

            // Lightning
            fBasicEffect.Alpha = 1.0f;
            fBasicEffect.DiffuseColor = Vector3.One;
            fBasicEffect.AmbientLightColor = Vector3.Zero;
            fBasicEffect.DirectionalLight0.Enabled = false;
            fBasicEffect.DirectionalLight1.Enabled = false;
            fBasicEffect.LightingEnabled = true;
            fBasicEffect.EnableDefaultLighting();
        }

        /// <summary>
        /// Init the plane
        /// </summary>
        private void SetPlane()
        {
            fUpdatePlane = false;
            fPlane = new PrimitivePlane(fSpriteBatch.GraphicsDevice, 50,
                -1500, 1500, -1500, 1500, 0, Color.White);
        }

        /// <summary>
        /// Set colors on points on the plane
        /// </summary>
        private void SetColors()
        {
            fUpdateColors = false;

            int max = fPlane.Segments;

            Color color1 = Color.White;
            Color color2 = Color.White;
            switch (fColorMode)
            {
                case ColorMode.Points:
                    color1 = Color.White;
                    color2 = Color.Black;
                    break;
                case ColorMode.SkyBlue:
                    color1 = Color.SkyBlue;
                    color2 = Color.SkyBlue;
                    break;
            }

            Color startColor = color1;
            for (int x = 0; x <= max; x++)
            {
                Color color = startColor;
                for (int z = 0; z <= max; z++)
                {
                    VertexPositionNormalColor p = fPlane.GetPoint(x, z);
                    p.Color = color;
                    fPlane.SetPoint(x, z, p);
                    if (startColor != color2)
                    {
                        if (color == color1)
                            color = color2;
                        else if (color == color2)
                            color = color1;
                    }
                }
                if (startColor == color1)
                    startColor = color2;
                else if (startColor == color2)
                    startColor = color1;
            }
        }

        #endregion

    }
}
