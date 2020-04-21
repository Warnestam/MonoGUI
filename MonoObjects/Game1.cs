using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGUI.Controls;
using MonoGUI.Engine;
using System.Collections.Generic;
using MonoGUI.Graphics;
using System;
using MonoGUI.GameComponents;

// TODOS

// Label - TextAligned - center i fönster
// Sizeable - dra i border
// Window - SizeToContent
// Label - hoover
// Borde göra refactor ändå på resize och move. Alltid border?

namespace MonoObjects
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {

        public enum DrawMode { Fill, WireFrame };
        public enum ShaderMode { Basic, MyTechnique1, MyTechnique2, MyTechnique3, Last };
        public enum RenderObjects { All, PlaneOnly, ObjectsOnly, Last };

        #region Private members

        private GuiEngine fEngine;

        private GraphicsDeviceManager fGraphics;
        private SpriteBatch fSpriteBatch;

        private GuiWindow fWindow;
        private GuiLabel fCurrentElement;
        private GuiLabel fLabelDisplayMode;
        private GuiLabel fAdapterSize;
        private GuiLabel fWindowSize;
        private GuiLabel fAspectRatio;
        private GuiLabel fFPS;
        private GuiLabel fInfo;
        private GuiLabel lblDrawMode;
        private GuiLabel lblShaderMode;
        private GuiLabel lblRenderObjects;

        private Random fRandom = new Random();
        private PrimitivePlane fPlane;
        private List<PrimitivePlane> fPlanes;
        private DynamicPrimitiveTriangles fObject1;

        private bool fUpdatePlane;
        private RasterizerState fRenderSolid;
        private RasterizerState fRenderWireFrame;
        private DepthStencilState fStateDepth;

        private SimpleCameraController fCamera;

        private DrawMode fDrawMode = DrawMode.Fill;
        private ShaderMode fShaderMode = ShaderMode.Basic;
        private RenderObjects fRenderObjects = RenderObjects.All;

        private BasicEffect fBasicEffect;
        private Effect fPointLightEffect;


        private GuiEngineOptions fOptions = new GuiEngineOptions()
        {
            PreferredSize = new Rectangle(0, 0, 1024, 768)
        };

        #endregion

        #region Constructor

        public Game1()
        {
            fGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        #endregion

        #region Game

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            fEngine = new GuiEngine(this, fOptions, fGraphics);
            fEngine.OnResize += FEngine_OnResize;
            fLabelDisplayMode = new GuiLabel();
            fAdapterSize = new GuiLabel() { HorizontalAlignment = GuiHorizontalAlignment.Right, BackgroundColor = Color.Orange };
            fWindowSize = new GuiLabel();
            fAspectRatio = new GuiLabel();
            fFPS = new GuiLabel() { HorizontalAlignment = GuiHorizontalAlignment.Right };
            fInfo = new GuiLabel();
            lblDrawMode = new GuiLabel() { HorizontalAlignment = GuiHorizontalAlignment.Left, BackgroundColor = Color.Blue, ForegroundColor = Color.Yellow };
            lblShaderMode = new GuiLabel() { HorizontalAlignment = GuiHorizontalAlignment.Left, BackgroundColor = Color.Blue, ForegroundColor = Color.Yellow };
            lblRenderObjects = new GuiLabel() { HorizontalAlignment = GuiHorizontalAlignment.Left, BackgroundColor = Color.Blue, ForegroundColor = Color.Yellow };
            fCurrentElement = new GuiLabel();

            fWindow = new GuiWindow()
            {
                Dragable = true,
                Clickable = true,
                Title = new GuiDockChild()
                {
                    Dock = GuiDock.Top,
                    Control = new GuiLabel()
                    {
                        Text = "Lines! (Click, Size or Drag)",
                        //VerticalAlignment = GuiVerticalAlignment.Stretch,
                        //HorizontalAlignment = GuiHorizontalAlignment.Stretch,
                        BackgroundColor = Color.Black,
                        ForegroundColor = Color.Yellow,

                    }
                },
                Border = new GuiBorder()
                {
                    Border = new GuiThickness(15),
                    BorderColor = new Color(Color.Black, 0.3f),
                },
                //BackgroundColor = new Color(Color.Red, 0.9f),
                HorizontalAlignment = GuiHorizontalAlignment.Left,
                //VerticalAlignment = GuiVerticalAlignment.Top,
                WindowState = GuiWindowState.Normal,
                X = 100,
                Y = 100,
                Width = 600,
                Height = 500,

                Content = new GuiPanel()
                {
                    Margin = new GuiThickness(20),
                    Content = new GuiStackPanel()
                    {
                        Childs = new List<GuiStackChild>()
                        {
                            new GuiStackChild(){Control = fFPS },
                            new GuiStackChild(){Control = fLabelDisplayMode },
                            new GuiStackChild(){Control =fAdapterSize},
                            new GuiStackChild(){Control =fWindowSize},
                            new GuiStackChild(){Control =fAspectRatio},
                            new GuiStackChild(){Control =fInfo },
                            new GuiStackChild(){Control =fCurrentElement },
                            new GuiStackChild(){Control =lblDrawMode },
                            new GuiStackChild(){Control =lblShaderMode },
                            new GuiStackChild(){Control =lblRenderObjects }
                        },
                        HorizontalAlignment = GuiHorizontalAlignment.Left,
                        VerticalAlignment = GuiVerticalAlignment.Top,
                        BackgroundColor = new Color(Color.White, 0.2f),
                    },
                    BackgroundColor = new Color(Color.Red, 0.7f),
                }

            };

            fEngine.AddWindow(fWindow);

            if (fWindow.Title != null)
                fWindow.Title.Control.OnClick += WindowTitle_OnClick;
            lblDrawMode.OnClick += lblDrawMode_OnClick;
            lblShaderMode.OnClick += lblShaderMode_OnClick;
            lblRenderObjects.OnClick += LblRenderObjects_OnClick;
            this.Components.Add(fEngine);

            UpdateScale();

            base.Initialize();
        }

        private void LblRenderObjects_OnClick(object sender, EventArgs e)
        {
            fRenderObjects = fRenderObjects + 1;
            if (fRenderObjects == RenderObjects.Last)
                fRenderObjects = RenderObjects.All;
        }

        private void lblDrawMode_OnClick(object sender, EventArgs e)
        {
            if (fDrawMode == DrawMode.Fill)
                fDrawMode = DrawMode.WireFrame;
            else
                fDrawMode = DrawMode.Fill;
        }

        private void lblShaderMode_OnClick(object sender, EventArgs e)
        {
            fShaderMode = fShaderMode + 1;
            if (fShaderMode == ShaderMode.Last)
                fShaderMode = ShaderMode.Basic;
        }

        private void FEngine_OnResize(object sender, EventArgs e)
        {
            UpdateScale();
        }

        private void WindowTitle_OnClick(object sender, System.EventArgs e)
        {

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            fSpriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            IsMouseVisible = true;
            IsFixedTimeStep = false;

            this.Window.AllowAltF4 = true;
            this.Window.AllowUserResizing = true;
            fGraphics.PreferMultiSampling = false;
            fGraphics.SynchronizeWithVerticalRetrace = false;
            fGraphics.HardwareModeSwitch = false;
            fGraphics.ApplyChanges();

            fUpdatePlane = true;

            //fStateSolid = new RasterizerState() { FillMode = FillMode.Solid, CullMode = CullMode.CullClockwiseFace };

            fRenderWireFrame = new RasterizerState() { FillMode = FillMode.WireFrame, CullMode = CullMode.CullClockwiseFace };
            fRenderSolid = new RasterizerState() { FillMode = FillMode.Solid, CullMode = CullMode.CullClockwiseFace };

            fStateDepth = new DepthStencilState() { DepthBufferEnable = true };

            UpdateScale();

        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            fCamera.Update(gameTime);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (fEngine.InputManager.KeyPressed(Keys.P))
                fEngine.UpdatePaused = !fEngine.UpdatePaused;

            if (fEngine.InputManager.KeyPressed(Keys.M))
                fEngine.ToggleFullscreen();

            fLabelDisplayMode.Text = $"Mode={fEngine.DisplayMode}";
            fAdapterSize.Text = $"Adapter={fEngine.AdapterSize.Width}x{fEngine.AdapterSize.Height}";
            fWindowSize.Text = $"Window={fEngine.WindowSize.Width}x{fEngine.WindowSize.Height}";
            fAspectRatio.Text = $"AspectRatio={fEngine.AspectRatio}";
            fFPS.Text = $"FPS={fEngine.FPS.FrameRate}";
            int count = fPlane.NumberOfTriangles;
            foreach (var plane in fPlanes)
                count += plane.NumberOfTriangles;
            count += fObject1.NumberOfTriangles;

            fInfo.Text = $"#Triangles={count}";
            lblDrawMode.Text = $"Draw Mode = {fDrawMode}";
            lblShaderMode.Text = $"Shader Mode = {fShaderMode}";
            lblRenderObjects.Text = $"Render Objects = {fRenderObjects}";
            if (fEngine.MouseOverElement == null)
            {
                fCurrentElement.Text = "...";
            }
            else
            {
                GuiElement element = fEngine.MouseOverElement;
                string name = String.Empty;
                bool isFirst = true;

                while (element != null)
                {
                    if (!isFirst)
                        name = name + "/";
                    isFirst = false;
                    string n = element.GetType().Name;
                    if (n.StartsWith("Gui"))
                        name = name + n.Substring(3);
                    else
                        name = name + n;
                    element = element.Parent;
                }
                fCurrentElement.Text = name;
            }


            if (fUpdatePlane)
            {
                SetPlane();
            }
            int max = fPlane.Segments;
            double seconds = gameTime.TotalGameTime.TotalSeconds;
            for (int x = 0; x <= max; x++)
            {
                float nX = 2.0f * (float)x / max - 1.0f;// -1 to +1
                for (int z = 0; z <= max; z++)
                {
                    float nZ = 2.0f * (float)z / max - 1.0f;
                    double r = Math.Sqrt(nX * nX + nZ * nZ);
                    //double y = nX * Math.Cos(r * 12.0f - seconds * 4.2f) + nZ * Math.Sin((r - 0.5) * 3.1f - seconds * 1.9f);
                    double y = Math.Cos(r * 10.0f - seconds * 2.0f) + Math.Sin(r * 10.0f - seconds * 2.1f);
                    float height = 200.0f * (float)y;
                    fPlane.SetHeight(x, z, height);
                }
            }
            fPlane.GenerateNormals();

            base.Update(gameTime);

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            fSpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            GraphicsDevice.DepthStencilState = fStateDepth;

            Matrix planeMatrix = fCamera.Camera.WorldMatrix;
            //if (!fHalted)
            double seconds = gameTime.TotalGameTime.TotalSeconds;
            float angle = (float)seconds / 200;
            Matrix matrix = Matrix.CreateRotationY(angle);
            planeMatrix = matrix;


            Matrix objectMatrix;
            angle = (float)seconds * 1.3f * 0.01f;
            matrix = Matrix.CreateRotationY(angle) * Matrix.CreateRotationX(angle * 0.04f);
            objectMatrix = matrix;

            angle = (float)seconds * 0.8f;
            Vector3 lightPos1 = new Vector3(Convert.ToSingle(00000 + 2500 * Math.Sin(angle)), Convert.ToSingle(2500 * Math.Sin(angle*0.3f)), 00000 + Convert.ToSingle(2500 * Math.Cos(angle)));
            Vector3 lightPos3 = new Vector3(Convert.ToSingle(10000 + 2500 * Math.Sin(angle)), 500, 10000 + Convert.ToSingle(2500 * Math.Cos(angle)));
            Vector3 lightPos2 = new Vector3(Convert.ToSingle(00000 + 2500 * Math.Sin(angle + Math.PI)), 500, 00000 + Convert.ToSingle(2500 * Math.Cos(angle + Math.PI)));
            Vector3 lightPos4 = new Vector3(Convert.ToSingle(10000 + 2500 * Math.Sin(angle + Math.PI)), 500, Convert.ToSingle(10000 + 2500 * Math.Cos(angle + Math.PI)));

            Effect effect;
            if (fShaderMode == ShaderMode.Basic)
            {
                effect = fBasicEffect;
                fBasicEffect.View = fCamera.Camera.ViewMatrix;
                fBasicEffect.Projection = fCamera.Camera.ProjectionMatrix;
                fBasicEffect.World = planeMatrix;
            }
            else
            {
                effect = fPointLightEffect;
                effect.CurrentTechnique = effect.Techniques[fShaderMode.ToString()];
                effect.Parameters["World"].SetValue(planeMatrix);
                effect.Parameters["View"].SetValue(fCamera.Camera.ViewMatrix);
                effect.Parameters["Projection"].SetValue(fCamera.Camera.ProjectionMatrix);
                var worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(planeMatrix));
                effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                effect.Parameters["LightPos1"].SetValue(lightPos1);
                effect.Parameters["LightPos2"].SetValue(lightPos2);
                effect.Parameters["LightPos3"].SetValue(lightPos3);
                effect.Parameters["LightPos4"].SetValue(lightPos4);
                //effect.Parameters["xWorldViewProjection"].SetValue(Matrix.Identity * viewMatrix * projectionMatrix);
                //effect.Parameters["xTexture"].SetValue(streetTexture);

                //effect.Parameters["xWorld"].SetValue(Matrix.Identity);
                //effect.Parameters["xLightPos"].SetValue(lightPos);
                //effect.Parameters["xLightPower"].SetValue(lightPower);
                //effect.Parameters["xAmbient"].SetValue(ambientPower);
            }

            if (fDrawMode == DrawMode.WireFrame)
            {
                GraphicsDevice.RasterizerState = fRenderWireFrame;
            }
            else
            {
                GraphicsDevice.RasterizerState = fRenderSolid;
            }

            bool renderPlanes = false;
            bool renderObjects = false;
            switch (fRenderObjects)
            {
                case RenderObjects.All:
                    renderPlanes = true;
                    renderObjects = true;
                    break;
                case RenderObjects.ObjectsOnly:
                    renderObjects = true;
                    break;
                case RenderObjects.PlaneOnly:
                    renderPlanes = true;
                    break;
            }
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                if (renderPlanes)
                {
                    fPlane.RenderTriangles(fSpriteBatch);
                    foreach (var plane in fPlanes)
                        plane.RenderTriangles(fSpriteBatch);
                }
                if (renderObjects)
                {
                    fObject1.RenderTriangles(fSpriteBatch);
                }
            }
            fSpriteBatch.End();


            base.Draw(gameTime);

        }

        #endregion

        private void UpdateScale()
        {
            InitializeTransform();
            InitializeEffect();
            if (fSpriteBatch == null)
                return;

            SetPlane();
        }


        /// <summary>
        /// Init the plane
        /// </summary>
        private void SetPlane()
        {
            fUpdatePlane = false;
            fPlane = new PrimitivePlane(fSpriteBatch.GraphicsDevice, 180, -1500, 1500, -1500, 1500, 0, Color.White);
            SetColors(fPlane);
            fPlane.GenerateNormals();
            fPlanes = new List<PrimitivePlane>();
            for (int x = -1; x <= 1; x++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    int minX = 3000 * x - 1500;
                    int maxX = minX + 3000;
                    int minZ = 3000 * z - 1500;
                    int maxZ = minZ + 3000;
                    var plane = new PrimitivePlane(fSpriteBatch.GraphicsDevice, 180, minX, maxX, minZ, maxZ, 0, Color.White);
                    fPlanes.Add(plane);
                    SetColors(plane);
                    plane.GenerateNormals();

                    minX += 10000;
                    maxX += 10000;
                    minZ += 10000;
                    maxZ += 10000;

                    plane = new PrimitivePlane(fSpriteBatch.GraphicsDevice, 1, minX, maxX, minZ, maxZ, 0, Color.White);
                    fPlanes.Add(plane);
                    SetColors(plane);
                    plane.GenerateNormals();
                }
            }

            Color color = Color.Yellow;

            fObject1 = new DynamicPrimitiveTriangles();
            bool useBox = true;
            for (int x = -10; x <= 10; x++)
            {
                for (int y = -10; y <= 10; y++)
                {
                    for (int z = -10; z <= 10; z++)
                    {
                        Vector3 position = new Vector3(1000 * x, 1000 * y, 1000 * z);
                        int spread = 1000;
                        position += new Vector3(fRandom.Next(spread), fRandom.Next(spread), fRandom.Next(spread));
                        color = new Color((float)fRandom.NextDouble(), (float)fRandom.NextDouble(), (float)fRandom.NextDouble());
                        int size = 100 + fRandom.Next(300);

                        if (useBox)
                            fObject1.AddBox(3, position, size, color);
                        else
                            fObject1.AddSphere(9, position, size, color);
                        useBox = !useBox;
                    }
                }
            }
            //fObject1.AddBox(segments, new Vector3(0, 0, 0), 1500, color);
            //fObject1.AddBox(segments, new Vector3(0, 3000, 0), 1500, color);
            fObject1.GenerateNormals();
        }


        /// <summary>
        /// Set colors on points on the plane
        /// </summary>
        private void SetColors(PrimitivePlane plane)
        {
            //fUpdateColors = false;

            int max = plane.Segments;

            Color color1 = Color.SkyBlue;
            Color color2 = Color.DeepSkyBlue;

            bool useColor1 = true;

            for (int x = 0; x <= max; x++)
            {
                Color color;

                for (int z = 0; z <= max; z++)
                {
                    VertexPositionNormalColor p1 = plane.GetPoint(x, z);
                    if (useColor1)
                        color = color1;
                    else
                        color = color2;
                    if (useColor1)
                    {
                        color.R = (byte)(255 * z / max);
                    }
                    useColor1 = !useColor1;


                    p1.Color = color;
                    plane.SetPoint(x, z, p1);
                }
                useColor1 = !useColor1;
            }
        }



        /// <summary>
        /// Initializes the transforms used by the game.
        /// </summary>
        private void InitializeTransform()
        {
            fCamera = new SimpleCameraController(this, fEngine.InputManager, GraphicsDevice.Viewport);

            fCamera.Camera.Rotation = Quaternion.CreateFromYawPitchRoll(0, -2.6f, 0);
            fCamera.Camera.Position = new Vector3(0.0f, -3000.0f, -5000.0f);
            fCamera.Camera.Update();

            //fViewMatrix = Matrix.CreateLookAt(
            //   new Vector3(0.0f, 0.0f, 1.0f),
            //   Vector3.Zero,
            //   Vector3.Up
            //   );

            //fProjectionMatrix = Matrix.CreateOrthographicOffCenter(
            //    0,
            //    (float)GraphicsDevice.Viewport.Width,
            //    (float)GraphicsDevice.Viewport.Height,
            //    0,
            //    1.0f, 1000.0f);
        }

        /// <summary>
        /// Initializes the effect (loading, parameter setting, and technique selection)
        /// used by the game.
        /// </summary>
        private void InitializeEffect()
        {
            fBasicEffect = new BasicEffect(GraphicsDevice);
            fBasicEffect.VertexColorEnabled = true;

            fBasicEffect.PreferPerPixelLighting = true;
            fBasicEffect.DirectionalLight0.Enabled = true;
            fBasicEffect.DirectionalLight1.Enabled = true;
            fBasicEffect.LightingEnabled = true;

            fBasicEffect.DirectionalLight0.DiffuseColor = new Vector3(0.7f, 0.7f, 0.7f);
            fBasicEffect.DirectionalLight0.Direction = new Vector3(0.0f, -1.0f, 0.0f);

            fBasicEffect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);

            fPointLightEffect = Content.Load<Effect>("PointLight");

        }





    }
}
