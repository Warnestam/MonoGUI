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
 * File:		FractalPlaneEngine2
 * Purpose:		A simple engine that draws a fractal landscape
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2010-06-23  RW	
 * 
 * History:		2010-06-23  RW  Created
 *              2020-04-01  RW  Moved to MonoExperience
 *  
 */
namespace MonoExperience
{

    /// <summary>
    /// A simple engine that draws a fractal landscape
    /// </summary>
    public class FractalPlaneEngine2 : BaseEngine
    {

        #region Enums

        public enum DrawMode { Solid, WireFrame, Grid };

        #endregion

        #region Private members

        private SpriteBatch fSpriteBatch;
        private Random fRandom = new Random();
        private BasicEffect fBasicEffect;
        private PrimitivePlaneGeneric<PlaneMultiTexture> fPlane;
        private SimpleCameraController fCamera;

        private bool fHalted = true;
        private float fAngle;
        private bool fUpdatePlane;
        private bool fGenerateFractal;

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
        private Texture2D fCloudMap;
        private Texture2D fWaterBumpMap;
        private Effect fShader;
        private Model fSkyDome;

        private bool fShowWater = true; // Key 1
        private bool fShowTerrain = true; // Key 2 
        private bool fShowSkyBox = true; // Key 3
        private bool fShowReflection = true; // Key 4
        private bool fClipReflection = true; // Key 5
        private bool fShowRefraction = true; // Key 6
        private bool fClipRefraction = true; // Key 7
        private bool fShowSkyBoxInReflection = true; // Key 8
        private bool fShowWaterBumpMap = true;  // Key 9

        private VertexBuffer fWaterVertexBuffer;
        private Matrix fReflectionViewMatrix;
        private Vector3 fReflectionCameraPosition;

        private const float WATER_HEIGHT = 5.0f;
        private RenderTarget2D fRefractionRenderTarget;
        private RenderTarget2D fReflectionRenderTarget;

        #endregion

        #region Constructor

        /// <summary>
        /// Create the engine
        /// </summary>
        /// <param name="game"></param>
        public FractalPlaneEngine2(EngineContainer cnt) : base(cnt)
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

            InitializeTransform();
            InitializeEffect();

            fGenerateFractal = true;

            fStateSolid = new RasterizerState() { FillMode = FillMode.Solid, CullMode = CullMode.None };
            fStateWire = new RasterizerState() { FillMode = FillMode.WireFrame, CullMode = CullMode.None };
            fStateDepth = new DepthStencilState() { DepthBufferEnable = true };

            fTextureGrass = Game.Content.Load<Texture2D>(@"FractalPlane/grass");
            fTextureRock = Game.Content.Load<Texture2D>(@"FractalPlane/rock");
            fTextureSand = Game.Content.Load<Texture2D>(@"FractalPlane/sand");
            fTextureSnow = Game.Content.Load<Texture2D>(@"FractalPlane/snow");
            fShader = Game.Content.Load<Effect>("FractalPlane/multitextureWithBlendAndClip");
            fSkyDome = Game.Content.Load<Model>("FractalPlane/dome");
            fCloudMap = Game.Content.Load<Texture2D>(@"FractalPlane/cloudmap");
            fWaterBumpMap = Game.Content.Load<Texture2D>(@"FractalPlane/waterbump");

            fSkyDome.Meshes[0].MeshParts[0].Effect = fShader.Clone();

            fRefractionRenderTarget = new RenderTarget2D(Game.GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, false, GraphicsDevice.DisplayMode.Format, DepthFormat.Depth16);
            fReflectionRenderTarget = new RenderTarget2D(Game.GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, false, GraphicsDevice.DisplayMode.Format, DepthFormat.Depth16);

            SetUpWaterVertices();



            base.LoadContent();
        }

        private void SetUpWaterVertices()
        {
            VertexPositionTexture[] waterVertices = new VertexPositionTexture[6];
            int worldSizeX = WORLD_SIZE / 2;
            //int worldSizeX = WORLD_SIZE * 2;
            waterVertices[0] = new VertexPositionTexture(new Vector3(-worldSizeX, WATER_HEIGHT, worldSizeX), new Vector2(0, 1));
            waterVertices[2] = new VertexPositionTexture(new Vector3(worldSizeX, WATER_HEIGHT, -worldSizeX), new Vector2(1, 0));
            waterVertices[1] = new VertexPositionTexture(new Vector3(-worldSizeX, WATER_HEIGHT, -worldSizeX), new Vector2(0, 0));
            waterVertices[3] = new VertexPositionTexture(new Vector3(-worldSizeX, WATER_HEIGHT, worldSizeX), new Vector2(0, 1));
            waterVertices[5] = new VertexPositionTexture(new Vector3(worldSizeX, WATER_HEIGHT, worldSizeX), new Vector2(1, 1));
            waterVertices[4] = new VertexPositionTexture(new Vector3(worldSizeX, WATER_HEIGHT, -worldSizeX), new Vector2(1, 0));

            fWaterVertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionTexture), 6, BufferUsage.WriteOnly);
            fWaterVertexBuffer.SetData(waterVertices);
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
            else if (this.Manager.KeyPressed(Keys.H))
            {
                fHalted = !fHalted;
            }

            else if (this.Manager.KeyPressed(Keys.D1))
            {
                fShowWater = !fShowWater;
            }
            else if (this.Manager.KeyPressed(Keys.D2))
            {
                fShowTerrain = !fShowTerrain;
            }
            else if (this.Manager.KeyPressed(Keys.D3))
            {
                fShowSkyBox = !fShowSkyBox;
            }
            else if (this.Manager.KeyPressed(Keys.D4))
            {
                fShowReflection = !fShowReflection;
            }
            else if (this.Manager.KeyPressed(Keys.D5))
            {
                fClipReflection = !fClipReflection;
            }
            else if (this.Manager.KeyPressed(Keys.D6))
            {
                fShowRefraction = !fShowRefraction;
            }
            else if (this.Manager.KeyPressed(Keys.D7))
            {
                fClipRefraction = !fClipRefraction;
            }
            else if (this.Manager.KeyPressed(Keys.D8))
            {
                fShowSkyBoxInReflection = !fShowSkyBoxInReflection;
            }
            else if (this.Manager.KeyPressed(Keys.D9))
            {
                fShowWaterBumpMap = !fShowWaterBumpMap;
            }

            fCamera.Update(gameTime);
            double seconds = gameTime.TotalGameTime.TotalSeconds;

            Vector3 cameraPosition = fCamera.Camera.Position;

            Quaternion quat = fCamera.Camera.Rotation;
            quat = Quaternion.Inverse(quat);
            Vector3 cameraForwardDirection = Vector3.Transform(Vector3.Forward, quat);
            Vector3 cameraUpDirection = Vector3.Transform(Vector3.Up, quat);
            Vector3 cameraRightDirection = Vector3.Transform(Vector3.Right, quat);
            Vector3 cameraTarget = cameraForwardDirection + cameraPosition;

            fReflectionCameraPosition = fCamera.Camera.Position;
            fReflectionCameraPosition.Y = -fCamera.Camera.Position.Y + WATER_HEIGHT * 2;
            Vector3 reflTargetPos = cameraTarget;
            reflTargetPos.Y = -cameraTarget.Y + WATER_HEIGHT * 2;
            Vector3 cameraRight = Vector3.Transform(Vector3.Right, quat);
            Vector3 invUpVector = Vector3.Cross(cameraRight, reflTargetPos - fReflectionCameraPosition);
            fReflectionViewMatrix = Matrix.CreateLookAt(fReflectionCameraPosition, reflTargetPos, invUpVector);


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
            float time = (float)gameTime.TotalGameTime.TotalMilliseconds * 0.0000001f;

            Matrix world = fCamera.Camera.WorldMatrix;
            Matrix view = fCamera.Camera.ViewMatrix;

            if (!fHalted)
            {
                double seconds = gameTime.ElapsedGameTime.TotalSeconds;
                fAngle = fAngle + (float)(seconds / 10.0f);
            }
            world = Matrix.CreateRotationY(fAngle);
            //DrawSkyDome(view, world, fCamera.Camera.Position);


            if (fShowRefraction)
                DrawRefractionMap(view, world);
            if (fShowReflection)
                DrawReflectionMap(view, world);


            if (fShowSkyBox)
                DrawSkyDome(view, world, fCamera.Camera.Position);
            if (fShowTerrain)
                DrawTerrain(view, world);
            if (fShowWater)
                DrawWater(view, world, time);

            fSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            int width = GraphicsDevice.Viewport.Width / 4;
            int height = GraphicsDevice.Viewport.Height / 4;
            int x1 = width - width / 2;
            int x2 = 3 * width - width / 2;

            if (fShowReflection /*&& fReflectionRenderTarget!=null*/)
                fSpriteBatch.Draw(fReflectionRenderTarget, new Rectangle(x1, 50, width, height), Color.White);
            if (fShowRefraction && fRefractionRenderTarget != null)
                fSpriteBatch.Draw(fRefractionRenderTarget, new Rectangle(x2, 50, width, height), Color.White);


            fSpriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawTerrain(Matrix view, Matrix world)
        {
            DrawTerrain(view, world, new Plane(), false);
        }

        private void DrawTerrain(Matrix view, Matrix world, Plane clippingPlane, bool usePlane)
        {
            GraphicsDevice.DepthStencilState = fStateDepth;
            GraphicsDevice.BlendState = BlendState.Opaque;

            if (usePlane)
            {
                fShader.CurrentTechnique = fShader.Techniques["MultiTexturedClipping"];
                fShader.Parameters["xTexture0"].SetValue(fTextureSand);
                fShader.Parameters["xTexture1"].SetValue(fTextureGrass);
                fShader.Parameters["xTexture2"].SetValue(fTextureRock);
                fShader.Parameters["xTexture3"].SetValue(fTextureSnow);
                fShader.Parameters["xWorld"].SetValue(world);
                fShader.Parameters["xView"].SetValue(view);
                fShader.Parameters["xProjection"].SetValue(fCamera.Camera.ProjectionMatrix);
                fShader.Parameters["xEnableLighting"].SetValue(true);
                fShader.Parameters["xAmbient"].SetValue(0.2f);
                fShader.Parameters["xLightDirection"].SetValue(new Vector3(-0.5f, -0.5f, -0.5f));
                fShader.Parameters["xClipPlane0"].SetValue(new Vector4(clippingPlane.Normal, clippingPlane.D));
            }
            else
            {
                fShader.CurrentTechnique = fShader.Techniques["MultiTextured"];
                fShader.Parameters["xTexture0"].SetValue(fTextureSand);
                fShader.Parameters["xTexture1"].SetValue(fTextureGrass);
                fShader.Parameters["xTexture2"].SetValue(fTextureRock);
                fShader.Parameters["xTexture3"].SetValue(fTextureSnow);
                fShader.Parameters["xWorld"].SetValue(world);
                fShader.Parameters["xView"].SetValue(view);
                fShader.Parameters["xProjection"].SetValue(fCamera.Camera.ProjectionMatrix);
                fShader.Parameters["xEnableLighting"].SetValue(true);
                fShader.Parameters["xAmbient"].SetValue(0.2f);
                fShader.Parameters["xLightDirection"].SetValue(new Vector3(-0.5f, -0.5f, -0.5f));
            }

            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            if (fDrawMode == DrawMode.WireFrame)
            {
                GraphicsDevice.RasterizerState = fStateWire;
            }
            else
            {
                GraphicsDevice.RasterizerState = fStateSolid;
            }
            Effect effect = fShader;

            // Using basic effect
            //fBasicEffect.View = view;
            //fBasicEffect.Projection = fCamera.Camera.ProjectionMatrix;
            //fBasicEffect.World = world;
            //effect = fBasicEffect;
            

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
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
        }

        private void DrawSkyDome(Matrix view, Matrix world, Vector3 position)
        {
            DrawSkyDome(view, world, position, new Plane(), false);
        }


        private void DrawSkyDome(Matrix view, Matrix world, Vector3 position, Plane clippingPlane, bool usePlane)
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            Matrix wMatrix = world * Matrix.CreateTranslation(0, -0.3f, 0) * Matrix.CreateScale(100) * Matrix.CreateTranslation(position);
            Matrix[] modelTransforms = new Matrix[fSkyDome.Bones.Count];
            fSkyDome.CopyAbsoluteBoneTransformsTo(modelTransforms);

            foreach (ModelMesh mesh in fSkyDome.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    Matrix worldMatrix = modelTransforms[mesh.ParentBone.Index] * wMatrix;
                    if (usePlane)
                    {
                        currentEffect.CurrentTechnique = currentEffect.Techniques["TexturedClipping"];
                        currentEffect.Parameters["xWorld"].SetValue(worldMatrix);
                        currentEffect.Parameters["xView"].SetValue(view);
                        currentEffect.Parameters["xProjection"].SetValue(fCamera.Camera.ProjectionMatrix);
                        currentEffect.Parameters["xTexture"].SetValue(fCloudMap);
                        currentEffect.Parameters["xEnableLighting"].SetValue(false);
                        fShader.Parameters["xClipPlane0"].SetValue(new Vector4(clippingPlane.Normal, clippingPlane.D));
                    }
                    else
                    {
                        currentEffect.CurrentTechnique = currentEffect.Techniques["Textured"];
                        currentEffect.Parameters["xWorld"].SetValue(worldMatrix);
                        currentEffect.Parameters["xView"].SetValue(view);
                        currentEffect.Parameters["xProjection"].SetValue(fCamera.Camera.ProjectionMatrix);
                        currentEffect.Parameters["xTexture"].SetValue(fCloudMap);
                        currentEffect.Parameters["xEnableLighting"].SetValue(false);
                    }

                }
                mesh.Draw();
            }
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        }

        private Plane CreatePlane(float height, Vector3 planeNormalDirection, Matrix currentViewMatrix, bool clipSide)
        {
            planeNormalDirection.Normalize();
            Vector4 planeCoeffs = new Vector4(planeNormalDirection, height);
            if (clipSide)
                planeCoeffs *= -1;
            Matrix worldViewProjection = currentViewMatrix * fCamera.Camera.ProjectionMatrix;
            Matrix inverseWorldViewProjection = Matrix.Invert(worldViewProjection);
            inverseWorldViewProjection = Matrix.Transpose(inverseWorldViewProjection);

            planeCoeffs = Vector4.Transform(planeCoeffs, inverseWorldViewProjection);
            Plane finalPlane = new Plane(planeCoeffs);

            return finalPlane;
        }


        private void DrawRefractionMap(Matrix view, Matrix world)
        {
            RenderTargetBinding[] previousRenderTargets = GraphicsDevice.GetRenderTargets();

            Plane refractionPlane = CreatePlane(WATER_HEIGHT + 1.5f, new Vector3(0, -1, 0), view, false);

            GraphicsDevice.SetRenderTarget(fRefractionRenderTarget);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
            DrawTerrain(view, world, refractionPlane, fClipRefraction);

            GraphicsDevice.SetRenderTargets(previousRenderTargets);
        }

        private void DrawReflectionMap(Matrix view, Matrix world)
        {
            RenderTargetBinding[] previousRenderTargets = GraphicsDevice.GetRenderTargets();

            Plane reflectionPlane = CreatePlane(WATER_HEIGHT - 0.5f, new Vector3(0, -1, 0), fReflectionViewMatrix, true);
            GraphicsDevice.SetRenderTarget(fReflectionRenderTarget);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            if (fShowSkyBoxInReflection)
                DrawSkyDome(fReflectionViewMatrix, world, fReflectionCameraPosition, reflectionPlane, fClipReflection);
            DrawTerrain(fReflectionViewMatrix, world, reflectionPlane, fClipReflection);

            GraphicsDevice.SetRenderTargets(previousRenderTargets);
        }

        private void DrawWater(Matrix view, Matrix world, float time)
        {
            Vector3 windDirection = new Vector3(1, 0, 0);

            fShader.CurrentTechnique = fShader.Techniques["Water"];
            Matrix worldMatrix = Matrix.Identity;
            fShader.Parameters["xWorld"].SetValue(world);
            fShader.Parameters["xView"].SetValue(view);
            fShader.Parameters["xReflectionView"].SetValue(fReflectionViewMatrix);
            fShader.Parameters["xProjection"].SetValue(fCamera.Camera.ProjectionMatrix);



            RenderTarget2D target = null;
            if (fShowReflection)
                fShader.Parameters["xReflectionMap"].SetValue(fReflectionRenderTarget);
            else
                fShader.Parameters["xReflectionMap"].SetValue(target);

            if (fShowRefraction)
                fShader.Parameters["xRefractionMap"].SetValue(fRefractionRenderTarget);
            else
                fShader.Parameters["xRefractionMap"].SetValue(target);

            if (fShowWaterBumpMap)
            {
                fShader.Parameters["xWaterBumpMap"].SetValue(fWaterBumpMap);
                fShader.Parameters["xWaveLength"].SetValue(0.1f);
                fShader.Parameters["xWaveHeight"].SetValue(0.3f);
            }
            else
            {
                fShader.Parameters["xWaterBumpMap"].SetValue(fWaterBumpMap);
                fShader.Parameters["xWaveLength"].SetValue(1.0f);
                fShader.Parameters["xWaveHeight"].SetValue(0.0f);
            }
            fShader.Parameters["xCamPos"].SetValue(fCamera.Camera.Position);
            fShader.Parameters["xTime"].SetValue(time);
            fShader.Parameters["xWindForce"].SetValue(20f);
            fShader.Parameters["xWindDirection"].SetValue(windDirection);
            foreach (EffectPass pass in fShader.CurrentTechnique.Passes)
            {
                pass.Apply();
                int noVertices = 6;
                GraphicsDevice.SetVertexBuffer(fWaterVertexBuffer);
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, noVertices / 3);
            }
        }

        #endregion

        #region BaseEngine

        public override string GetName()
        {
            return "Fractal Plane 2.0";
        }

        public override string GetHelp()
        {
            string text1 = @"M - Change Draw Mode
H - Halt
1 - Toggle water 
2 - Toggle terrain
3 - Toggle sky box
4 - Toggle reflection
5 - Toggle clip reflection
6 - Toggle refraction
7 - Toggle clip refraction
8 - Toggle skybox in reflection
9 - Toggle water bump map
F - Genereate fractal";


            string text2 = fCamera.GetHelp();
            return String.Format("{0}\n{1}", text1, text2);
        }

        public override string GetInfo()
        {
            string text1 = String.Format(@"Draw mode: {0}", fDrawMode);
            string text2 = fCamera.GetInfo();
            return String.Format("{0}\n{1}", text1, text2);
        }

        public override string GetAbout()
        {
            return "Drawing a fractal landscape\n\nOriginal author: Riemer Grootjans\nhttp://www.riemers.net";
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
            fBasicEffect.Texture = fTextureSnow;
            fBasicEffect.PreferPerPixelLighting = false;
            fBasicEffect.TextureEnabled = true;
        }

        /// <summary>
        /// Generate the fractal
        /// </summary>
        private void UpdatePlane()
        {
            //int numberOfSegments = 400;
            int worldSize2 = WORLD_SIZE / 2;

            fUpdatePlane = false;

            // Init plane and copy data
            fPlane = new PrimitivePlaneGeneric<PlaneMultiTexture>(fSpriteBatch.GraphicsDevice, SEGMENTS,
                -worldSize2, worldSize2, -worldSize2, worldSize2, 0);

            // MIN/MAX
            float minHeight = 100000;
            float maxHeight = -minHeight;
            for (int x = 0; x <= FRACTAL_SIZE; x++)
            {
                for (int z = 0; z <= FRACTAL_SIZE; z++)
                {
                    //fFractalHeight[x, z] = z;
                    float fractalHeight = fFractalHeight[x, z];
                    if (fractalHeight < minHeight)
                        minHeight = fractalHeight;
                    else if (fractalHeight > maxHeight)
                        maxHeight = fractalHeight;
                }
            }
            float deltaHeight = maxHeight - minHeight;

            for (int x = 0; x <= SEGMENTS; x++)
            {
                float fx = (float)x / SEGMENTS;
                int fractalX = (int)(fx * FRACTAL_SIZE);
                for (int z = 0; z <= SEGMENTS; z++)
                {
                    float fz = (float)z / SEGMENTS;
                    int fractalZ = (int)(fz * FRACTAL_SIZE);
                    float fractalHeight = fFractalHeight[fractalX, fractalZ];
                    float testHeight = fractalHeight - minHeight;
                    float fh = testHeight / deltaHeight;
                    PlaneMultiTexture vertex = fPlane.GetPoint(x, z);
                    vertex.Position.Y = fractalHeight;
                    vertex.TextureCoordinate = new Vector2(5.0f * fx, 5.0f * fz);
                    vertex.TextureWeights.X = MathHelper.Clamp(1.0f - 8.0f * Math.Abs(fh - 1.0f / 8.0f), 0, 1);
                    vertex.TextureWeights.Y = MathHelper.Clamp(1.0f - 8.0f * Math.Abs(fh - 3.0f / 8.0f), 0, 1);
                    vertex.TextureWeights.Z = MathHelper.Clamp(1.0f - 8.0f * Math.Abs(fh - 5.0f / 8.0f), 0, 1);
                    vertex.TextureWeights.W = MathHelper.Clamp(1.0f - 8.0f * Math.Abs(fh - 7.0f / 8.0f), 0, 1);
                    float total = vertex.TextureWeights.X +
                        vertex.TextureWeights.Y +
                        vertex.TextureWeights.Z +
                        vertex.TextureWeights.W;
                    if (total == 0)
                    {
                        //vertex.TextureWeights.X = 1.0f;
                        total = 1.0f;
                    }
                    vertex.TextureWeights.X /= total;
                    vertex.TextureWeights.Y /= total;
                    vertex.TextureWeights.Z /= total;
                    vertex.TextureWeights.W /= total;
                    fPlane.SetPoint(x, z, vertex);
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

            int granularity = fRandom.Next(4);

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

    }
}
