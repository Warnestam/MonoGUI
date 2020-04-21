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
 * File:		FractalPlaneEngine
 * Purpose:		A simple engine that draws a fractal landscape
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2010-02-28  RW	
 * 
 * History:		2010-02-28  RW  Created
 *              2020-04-01  RW  Moved to MonoExperience
 * 
 * 
 */
namespace MonoExperience
{

    /// <summary>
    /// A simple engine that draws a fractal landscape
    /// </summary>
    public class FractalPlaneEngine : BaseEngine
    {

        #region Enums

        public enum DrawMode { Solid, WireFrame, Grid };

        #endregion

        #region Private members

        private SpriteBatch fSpriteBatch;
        private Random fRandom = new Random();
        private BasicEffect fBasicEffect;
        //private PrimitivePlaneTexture/*Extra*/ fPlane;
        private PrimitivePlaneGeneric<PlaneTexture> fPlane;
        private SimpleCameraController fCamera;

        private bool fHalted = false;
        private float fAngle;
        private bool fUpdatePlane;
        private bool fGenerateFractal;
        private bool fAddZeroPlane = false;

        private DrawMode fDrawMode = DrawMode.Solid;
        private RasterizerState fStateSolid;
        private RasterizerState fStateWire;
        private DepthStencilState fStateDepth;

        private const int FRACTAL_SIZE = 1024;
        private float[,] fFractalHeight = new float[FRACTAL_SIZE + 1, FRACTAL_SIZE + 1];
        private const int SEGMENTS = 180;
        private const int WORLD_SIZE = 5000;
        private Texture2D fTextureGrass;
        private Texture2D fTextureRock;
        private Texture2D fTextureSand;
        private Texture2D fTextureSnow;

        #endregion

        #region Constructor

        /// <summary>
        /// Create the engine
        /// </summary>
        /// <param name="game"></param>
        public FractalPlaneEngine(EngineContainer cnt) : base(cnt)
        {
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
            // TODO
            base.Dispose(disposing);
        }

        /// <summary>
        /// Loads any component specific content
        /// </summary>
        protected override void LoadContent()
        {
            fSpriteBatch = new SpriteBatch(Game.GraphicsDevice);

                fGenerateFractal = true;

            fStateSolid = new RasterizerState() { FillMode = FillMode.Solid, CullMode = CullMode.None };
            fStateWire = new RasterizerState() { FillMode = FillMode.WireFrame, CullMode = CullMode.None };
            fStateDepth = new DepthStencilState() { DepthBufferEnable = true };


            fTextureGrass = Game.Content.Load<Texture2D>(@"FractalPlane\grass");
            //fTextureGrass = Game.Content.Load<Texture2D>(@"background");
            fTextureRock = Game.Content.Load<Texture2D>(@"FractalPlane\rock");
            fTextureSand = Game.Content.Load<Texture2D>(@"FractalPlane\sand");
            fTextureSnow = Game.Content.Load<Texture2D>(@"FractalPlane\snow");


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
            else if (this.Manager.KeyPressed(Keys.F))
            {
                fGenerateFractal = true;
            }
            else if (this.Manager.KeyPressed(Keys.W))
            {
                fAddZeroPlane = !fAddZeroPlane;
                fUpdatePlane = true;
            }
            else if (this.Manager.KeyPressed(Keys.H))
            {
                fHalted = !fHalted;
            }

            fCamera.Update(gameTime);

            double seconds = gameTime.TotalGameTime.TotalSeconds;

            if (fGenerateFractal)
            {
                GenerateFractal();
            }
            if (fUpdatePlane)
            {
                UpdatePlane();
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.DepthStencilState = fStateDepth;
            //GraphicsDevice.BlendState = BlendState.AlphaBlend;

            fBasicEffect.View = fCamera.Camera.ViewMatrix;
            fBasicEffect.Projection = fCamera.Camera.ProjectionMatrix;

            Matrix planeMatrix = fCamera.Camera.WorldMatrix;
            if (!fHalted)
            {
                double seconds = gameTime.ElapsedGameTime.TotalSeconds;
                fAngle = fAngle + (float)(seconds / 10.0f);
            }
            Matrix matrix = Matrix.CreateRotationY(fAngle);
            planeMatrix = matrix;


            Effect effect = fBasicEffect;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            if (fDrawMode == DrawMode.WireFrame)
            {
                GraphicsDevice.RasterizerState = fStateWire;
            }
            else
            {
                GraphicsDevice.RasterizerState = fStateSolid;
            }

            ShowReflectionGraphicsDevice(GraphicsDevice);
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
            base.Draw(gameTime);
        }

        #endregion

        #region BaseEngine

        public override string GetName()
        {
            return "Fractal Plane 1.0";
        }

        public override string GetHelp()
        {
            string text1 = @"M - Change Draw Mode
H - Halt
W - Add zero plane";
            string text2 = fCamera.GetHelp();
            return String.Format("{0}\n{1}", text1, text2);
        }

        public override string GetInfo()
        {
            string text1 = String.Format("Draw mode: {0}", fDrawMode);
            string text2 = fCamera.GetInfo();
            return String.Format("{0}\n{1}", text1, text2);
        }

        public override string GetAbout()
        {
            return @"Drawing a fractal landscape";
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
            fCamera.Camera.Rotation = Quaternion.CreateFromYawPitchRoll(0, 3.14f, 3.14f);
            fCamera.Camera.Position = new Vector3(0.0f, 500.0f, -4000.0f);
            fCamera.Camera.Update();
        }

        /// <summary>
        /// Initializes the effect (loading, parameter setting, and technique selection)
        /// used by the game.
        /// </summary>
        private void InitializeEffect()
        {
            fBasicEffect = new BasicEffect(GraphicsDevice);
            fBasicEffect.VertexColorEnabled = false;
            fBasicEffect.EnableDefaultLighting();
            fBasicEffect.SpecularPower = 100.0f;
            fBasicEffect.Texture = fTextureGrass;
            fBasicEffect.PreferPerPixelLighting = false;
            fBasicEffect.TextureEnabled = true;
        }

        /// <summary>
        /// Generate the fractal
        /// </summary>
        private void UpdatePlane()
        {
            int worldSize2 = WORLD_SIZE;

            fUpdatePlane = false;

            // Init plane and copy data
            fPlane = new PrimitivePlaneGeneric<PlaneTexture>(fSpriteBatch.GraphicsDevice, SEGMENTS,
                -worldSize2, worldSize2, -worldSize2, worldSize2, 0, 3.0f);
            for (int x = 0; x <= SEGMENTS; x++)
            {
                int fractalX = (int)((float)x / SEGMENTS * FRACTAL_SIZE);
                for (int z = 0; z <= SEGMENTS; z++)
                {
                    int fractalY = (int)((float)z / SEGMENTS * FRACTAL_SIZE);
                    float fractalHeight = fFractalHeight[fractalX, fractalY];
                    if (fAddZeroPlane && fractalHeight < 0.0f)
                        fractalHeight = 0.0f;
                    fPlane.SetHeight(x, z, fractalHeight);
                }
            }

            fPlane.GenerateNormals();
        }

        /// <summary>
        /// Generate the fractal landscape
        /// </summary>
        private void GenerateFractal()
        {
            fGenerateFractal = false;
            fUpdatePlane = true;

            int height = 1500;
            int granularity = 0;

            for (int x = 0; x <= FRACTAL_SIZE; x++)
                for (int y = 0; y <= FRACTAL_SIZE; y++)
                    fFractalHeight[x, y] = 0;

            int step = FRACTAL_SIZE;
            step = step >> granularity;
            float maxHeight = height >> granularity;

            for (int x = 0; x <= FRACTAL_SIZE; x += step)
            {
                for (int y = 0; y <= FRACTAL_SIZE; y += step)
                {
                    fFractalHeight[x, y] +=
                        maxHeight * (float)(fRandom.NextDouble() - 0.5f);
                }
            }

            while (step > 1)
            {
                int halfStep = step / 2;

                for (int x = 0; x < FRACTAL_SIZE; x += step)
                {
                    for (int y = 0; y <= FRACTAL_SIZE; y += step)
                    {
                        float h1 = fFractalHeight[x, y];
                        float h2 = fFractalHeight[x + step, y];
                        fFractalHeight[x + halfStep, y] = (h1 + h2) / 2.0f +
                            maxHeight * (float)(fRandom.NextDouble() - 0.5f);
                    }
                }
                for (int y = 0; y < FRACTAL_SIZE; y += step)
                {
                    for (int x = 0; x <= FRACTAL_SIZE; x += step)
                    {
                        float h1 = fFractalHeight[x, y];
                        float h2 = fFractalHeight[x, y + step];
                        fFractalHeight[x, y + halfStep] = (h1 + h2) / 2.0f +
                             maxHeight * (float)(fRandom.NextDouble() - 0.5f);
                    }
                }
                for (int x = 0; x < FRACTAL_SIZE; x += step)
                {
                    for (int y = 0; y < FRACTAL_SIZE; y += step)
                    {
                        float h1 = fFractalHeight[x, y];
                        float h2 = fFractalHeight[x + step, y];
                        float h3 = fFractalHeight[x + step, y + step];
                        float h4 = fFractalHeight[x, y + step];
                        fFractalHeight[x + halfStep, y + halfStep] = (h1 + h2 + h3 + h4) / 4.0f +
                             maxHeight * (float)(fRandom.NextDouble() - 0.5f);
                    }
                }
                halfStep = step;
                step = step / 2;
                maxHeight = maxHeight / 2;
            }
        }

        #endregion

        public bool _deviceInfoShown = false;
        public void ShowReflectionGraphicsDevice(GraphicsDevice device)
        {
            if (_deviceInfoShown)
                return;
            _deviceInfoShown = true;
            ShowReflectionInfo("GraphicsDevice", device);
            ShowReflectionInfo("BlendState", device.BlendState);
            ShowReflectionInfo("DepthStencilState", device.DepthStencilState);
            ShowReflectionInfo("DisplayMode", device.DisplayMode);
            ShowReflectionInfo("GraphicsProfile", device.GraphicsProfile);
            ShowReflectionInfo("PresentationParameters", device.PresentationParameters);
            ShowReflectionInfo("RasterizerState", device.RasterizerState);
            ShowReflectionInfo("SamplerStates", device.SamplerStates);
            ShowReflectionInfo("VertexSamplerStates", device.VertexSamplerStates);

        }
        public void ShowReflectionInfo(string caption, object o)
        {
            foreach (var kvp in ReflectionInfo(o))
            {
                System.Console.WriteLine($"{caption}.{kvp.Key}:{kvp.Value}");
            }
        }
        public static SortedDictionary<string, object> ReflectionInfo(object o)
        {
            SortedDictionary<string, object> result = new SortedDictionary<string, object>();
            var t = o.GetType();
            var properties = t.GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                if (property.CanRead)
                {
                    if (property.GetIndexParameters().Length == 0)
                    {
                        var subObject = property.GetValue(o, null);
                        if (o != subObject)
                        {
                            object value = property.GetValue(o, new object[] { });
                            result.Add(property.Name, value);
                            //Console.WriteLine($"{t}.{property.Name}: {subObject.ToString()}");
                            //ReflectionInfo(subObject);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"{t}.{property.Name}: INDEX TODO");
                    }
                }
            }
            return result;
        }


    }
}
