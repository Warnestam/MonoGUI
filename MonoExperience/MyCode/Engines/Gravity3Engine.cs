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

using System.Threading;
using System.Threading.Tasks;
using MonoGUI.Graphics;
using MonoGUI.GameComponents;

/*
 * File:		Gravity3Engine
 * Purpose:		3D gravity engine - Change the room and let masses travel in this room
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2010-04-13  RW	
 * 
 * History:		2010-04-13  RW  Created
 *              2020-04-08  RW  Moved to MonoExperience
 * 
 * 
 */
namespace MonoExperience
{

    /// <summary>
    /// An engine that moves masses with a space calculation algorithm
    /// </summary>
    public class Gravity3Engine : BaseEngine
    {

        #region Enums & Classes

        class Particle
        {
            public Vector3 Position;
            public Vector3 Velocity;

            public float Mass;
            public Texture2D Texture;
            public float Size;
            public bool Remove;

            public Particle(Vector3 position, Vector3 velocity, float mass)
            {
                Position = position;
                Velocity = velocity;
                Mass = mass;
                Texture = null;
                Size = 1.0f;
            }

        }

        struct MySpace
        {
            public float ForcePositiveY;
            public float ForceNegativeY;
            public float ForcePositiveX;
            public float ForceNegativeX;
            public float ForcePositiveZ;
            public float ForceNegativeZ;
        }

        #endregion

        #region Constants

        private const int WORLD_BLOCKS = 30;
        private const int WORLD_BLOCKS_MAX = WORLD_BLOCKS - 1;

        private const float WORLD_SIZE = 4000.0f;
        // Convert particle position to world/block position
        private const float PARTICLE_TO_WORLD = WORLD_BLOCKS / WORLD_SIZE;
        private const float HALF_WORLD_SIZE = WORLD_SIZE / 2.0f;

        private const int INIT_RADIUS = 1000;
        private const int ADD_STARS = 10000;

        private DynamicPrimitiveLine fLines;

        #endregion

        #region Private members

        private SpriteBatch fSpriteBatch;

        private Texture2D[] fTexRed;
        private Texture2D[] fTexBlue;
        private Texture2D fTextureRed;
        private Texture2D fTextureBlue;

        private List<Particle> fParticles = new List<Particle>();

        private Random fRandom = new Random();
        private bool fAllowNegative = true;
        private bool fDrawGrid;
        private bool fDrawParticles = true;

        private BasicEffect fBasicEffect;
        private Effect fShader;
        private BlendState fBlendState;
        private SimpleCameraController fCamera;

        private int fCalculations;
        private MySpace[, ,] fSpace = new MySpace[WORLD_BLOCKS, WORLD_BLOCKS, WORLD_BLOCKS];
        private MySpace[, ,] fSpaceTemp = new MySpace[WORLD_BLOCKS, WORLD_BLOCKS, WORLD_BLOCKS];
        private float fGravitationEffect = 1.0f;
        private float fTotalMass = 0;

        #endregion

        #region Constructor

        /// <summary>
        /// Create the engine
        /// </summary>
        /// <param name="game"></param>
        public Gravity3Engine(EngineContainer cnt) : base(cnt)
        {
            ReInit();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get number of particles
        /// </summary>
        public int NumberOfParticles
        {
            get
            {
                return fParticles.Count;
            }
        }

        /// <summary>
        /// Get/set allow particles with negative masses
        /// </summary>
        public bool AllowNegative
        {
            get
            {
                return fAllowNegative;
            }
            set
            {
                fAllowNegative = value;
            }
        }

        /// <summary>
        /// Get/set draw grid
        /// </summary>
        public bool DrawGrid
        {
            get
            {
                return fDrawGrid;
            }
            set
            {
                fDrawGrid = value;
            }
        }

        /// <summary>
        /// Get/set draw particles
        /// </summary>
        public bool DrawParticles
        {
            get
            {
                return fDrawParticles;
            }
            set
            {
                fDrawParticles = value;
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
        /// Initializes the transforms used by the game.
        /// </summary>
        private void InitializeTransform()
        {
            fCamera = new SimpleCameraController(this.Game, this.Manager, GraphicsDevice.Viewport);

            fCamera.Camera.Rotation = Quaternion.CreateFromYawPitchRoll(0, 0.0f, 0);
            fCamera.Camera.Position = new Vector3(0.0f, 0.0f, 800.0f);
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
        }

        /// <summary>
        /// Loads any component specific content
        /// </summary>
        protected override void LoadContent()
        {
            fSpriteBatch = new SpriteBatch(Game.GraphicsDevice);

            fTexRed = new Texture2D[7];
            fTexBlue = new Texture2D[7];

            fTexRed[0] = Game.Content.Load<Texture2D>(@"Gravity\Red004");
            fTexRed[1] = Game.Content.Load<Texture2D>(@"Gravity\Red008");
            fTexRed[2] = Game.Content.Load<Texture2D>(@"Gravity\Red016");
            fTexRed[3] = Game.Content.Load<Texture2D>(@"Gravity\Red032");
            fTexRed[4] = Game.Content.Load<Texture2D>(@"Gravity\Red064");
            fTexRed[5] = Game.Content.Load<Texture2D>(@"Gravity\Red128");
            fTexRed[6] = Game.Content.Load<Texture2D>(@"Gravity\Red256");
            fTexBlue[0] = Game.Content.Load<Texture2D>(@"Gravity\Blue004");
            fTexBlue[1] = Game.Content.Load<Texture2D>(@"Gravity\Blue008");
            fTexBlue[2] = Game.Content.Load<Texture2D>(@"Gravity\Blue016");
            fTexBlue[3] = Game.Content.Load<Texture2D>(@"Gravity\Blue032");
            fTexBlue[4] = Game.Content.Load<Texture2D>(@"Gravity\Blue064");
            fTexBlue[5] = Game.Content.Load<Texture2D>(@"Gravity\Blue128");
            fTexBlue[6] = Game.Content.Load<Texture2D>(@"Gravity\Blue256");

            fTextureRed = Game.Content.Load<Texture2D>(@"Gravity\Red");
            fTextureBlue = Game.Content.Load<Texture2D>(@"Gravity\Blue");

            fShader = Game.Content.Load<Effect>("Gravity/BillboardShader");

            fBlendState = new BlendState();
            fBlendState.AlphaBlendFunction = BlendFunction.Add;
            fBlendState.AlphaDestinationBlend = fBlendState.ColorDestinationBlend = Blend.One;
            fBlendState.AlphaSourceBlend = fBlendState.ColorSourceBlend = Blend.SourceAlpha;

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
            HandleInput(gameTime);
            fCamera.Update(gameTime);

            double factor = gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            if (factor > 0)
            {
                fCalculations = UpdateSpaceAndParticles(factor);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (fDrawGrid)
            {
                RenderGrid();
            }
            if (fParticles.Count > 0 && fDrawParticles)
            {
                RenderParticles();
            }
            base.Draw(gameTime);
        }

        #endregion

        #region BaseEngine

        public override string GetName()
        {
            return "3D Gravity - Particles changes the room";
        }

        public override string GetHelp()
        {
            string text1 = @"R Reinit particles
N Toggle allow negative
G Toggle render grid
H Toggle render particles
+/- Add/remove particles
O/P Change gravitation effect";
            string text2 = fCamera.GetHelp();
            return String.Format("{0}\n{1}", text1, text2);

        }

        public override string GetInfo()
        {
            string text1 = String.Format(@"Number of particles: {0}
Allow negative: {1}
Calculations: {2}
Gravity effect: {3}
Total mass: {4}",
                fParticles.Count, fAllowNegative, fCalculations, fGravitationEffect, fTotalMass
                );
            string text2 = fCamera.GetInfo();
            return String.Format("{0}\n{1}", text1, text2);
        }

        public override string GetAbout()
        {
            return @"3D Gravity sample where objects affects the room";
        }


        private void HandleInput(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float timeFactor = 100.0f * delta;

            if (this.Manager.KeyPressed(Keys.R))
                this.ReInit();
            else if (this.Manager.KeyPressed(Keys.N))
                this.AllowNegative = !this.AllowNegative;
            else if (this.Manager.KeyPressed(Keys.G))
                this.DrawGrid = !this.DrawGrid;
            else if (this.Manager.KeyPressed(Keys.H))
                this.DrawParticles = !this.DrawParticles;
            else if (this.Manager.KeyPressed(Keys.O))
                fGravitationEffect /= 10.0f;
            else if (this.Manager.KeyPressed(Keys.P))
                fGravitationEffect *= 10.0f;

            if (this.Manager.IsKeyDown(Keys.Add))
                this.AddParticles(ADD_STARS);
            else if (this.Manager.IsKeyDown(Keys.Subtract))
                this.RemoveParticles(ADD_STARS);

        }

        public override void DisplayChanged()
        {
           
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Add particles
        /// </summary>
        /// <param name="particles"></param>
        public void AddParticles(int particles)
        {
            for (int i = 0; i < particles; i++)
                AddParticle();
        }

        /// <summary>
        /// Reinitialize particles
        /// </summary>
        /// <param name="particles"></param>
        public void ReInit()
        {
            InitSpace();

            fParticles.Clear();

            fParticles.Add(new Particle(new Vector3(0, 0, 0), new Vector3(0, 0, 0), 50000));
            AddParticles(10000);
        }

        /// <summary>
        /// Remove some particles
        /// </summary>
        /// <param name="particles"></param>
        public void RemoveParticles(int particles)
        {
            for (int i = 0; i < particles; i++)
                if (fParticles.Count > 0)
                    fParticles.RemoveAt(fParticles.Count - 1);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Add a single particle
        /// </summary>
        private void AddParticle()
        {
            Matrix m = Matrix.CreateFromYawPitchRoll(
                (float)fRandom.NextDouble() * MathHelper.TwoPi,
                (float)fRandom.NextDouble() * MathHelper.TwoPi,
                (float)fRandom.NextDouble() * MathHelper.TwoPi);
            float radius = (float)fRandom.Next(INIT_RADIUS);
            Vector3 position = m.Forward * radius;
            Vector3 velocity = Vector3.Zero;
            float mass;
            int size = fRandom.Next(20000);
            if (size < 10)
            {
                mass = 1000000.0f;
            }
            if (size < 100)
            {
                mass = 100000.0f;
            }
            else if (size < 1000)
            {
                mass = 5000.0f;
            }
            else if (size < 5000)
            {
                mass = 100.0f;
            }
            else if (size < 10000)
            {
                mass = 5.0f;
            }
            else
            {
                mass = 1.0f;
            }

            if (fAllowNegative)
            {
                if (fRandom.Next(2) == 0)
                {
                    mass = -mass;
                }
            }
            Particle particle = new Particle(position, velocity, mass);
            fParticles.Add(particle);
        }

        /// <summary>
        /// Set texture on particle
        /// </summary>
        /// <param name="data"></param>
        private void SetParticleTexture(Particle data)
        {
            double mass = Math.Abs(data.Mass);
            if (data.Mass >= 0)
                data.Texture = fTextureBlue;
            else
                data.Texture = fTextureRed;
            data.Size = (Single)Math.Pow(Math.Abs(data.Mass), 0.3333f);
        }

        #endregion

        #region Private methods: Draw space & particles

        /// <summary>
        /// Draw the space
        /// </summary>
        private void RenderGrid()
        {
            fLines = new DynamicPrimitiveLine(GraphicsDevice);

            float worldFactor = WORLD_SIZE / (WORLD_BLOCKS - 1);
            VertexPositionColor p1;
            VertexPositionColor p2;

            for (int x = 0; x < WORLD_BLOCKS; x++)
            {
                float sx1 = (x + 0.5f) * worldFactor - HALF_WORLD_SIZE;
                for (int y = 0; y < WORLD_BLOCKS; y++)
                {
                    float sy1 = (y + 0.5f) * worldFactor - HALF_WORLD_SIZE;
                    for (int z = 0; z < WORLD_BLOCKS; z++)
                    {
                        float sz1 = (z + 0.5f) * worldFactor - HALF_WORLD_SIZE;

                        float dx = 1024 * (fSpace[x, y, z].ForcePositiveX - fSpace[x, y, z].ForceNegativeX);
                        float dy = 1024 * (fSpace[x, y, z].ForcePositiveY - fSpace[x, y, z].ForceNegativeY);
                        float dz = 1024 * (fSpace[x, y, z].ForcePositiveZ - fSpace[x, y, z].ForceNegativeZ);
                        Vector3 d = new Vector3(dx, dy, dz);
                        float power = d.Length() / 1000000;
                        if (power > 0)
                        {
                            d.Normalize();
                            if (power > 200)
                                power = 200;

                            dx = d.X * power;
                            dy = d.Y * power;
                            dz = d.Z * power;

                            float sx2 = sx1 + dx;
                            float sy2 = sy1 + dy;
                            float sz2 = sz1 + dz;
                            int r = (int)(255f * Math.Abs(d.X));
                            int g = (int)(255f * Math.Abs(d.Y));
                            int b = (int)(255f * Math.Abs(d.Z));
                            Color c1 = new Color(r, g, b);
                            Color c2 = Color.Transparent;
                            p1 = new VertexPositionColor(new Vector3(sx1, sy1, sz1), c1);
                            p2 = new VertexPositionColor(new Vector3(sx2, sy2, sz2), c2);
                            fLines.AddLine(p1, p2);
                        }
                    }
                }
            }
            fBasicEffect.World = fCamera.Camera.WorldMatrix;
            fBasicEffect.View = fCamera.Camera.ViewMatrix;
            fBasicEffect.Projection = fCamera.Camera.ProjectionMatrix;
            foreach (EffectPass pass in fBasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                fLines.Render();
            }
        }

        /// <summary>
        /// Draw particles
        /// </summary>
        private void RenderParticles()
        {
            Dictionary<Texture2D, DynamicBillboard> billboards = new Dictionary<Texture2D, DynamicBillboard>();
            foreach (Particle particle in fParticles)
            {
                Texture2D texture = particle.Texture;
                if (texture == null)
                {
                    SetParticleTexture(particle);
                    texture = particle.Texture;
                }
                if (!billboards.ContainsKey(texture))
                    billboards.Add(texture, new DynamicBillboard(GraphicsDevice));
                DynamicBillboard billboard = billboards[texture];
                billboard.AddObject(particle.Position, Color.White, particle.Size);
            }
            //--------------------
            GraphicsDevice.BlendState = fBlendState;
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            Matrix vp = fCamera.Camera.ViewMatrix * fCamera.Camera.ProjectionMatrix;
            fShader.Parameters["world"].SetValue(fCamera.Camera.WorldMatrix);
            fShader.Parameters["vp"].SetValue(vp);
            int i = 0;
            foreach (KeyValuePair<Texture2D, DynamicBillboard> kvp in billboards)
            {
                fShader.Parameters["particleTexture"].SetValue(kvp.Key);
                for (int ps = 0; ps < fShader.CurrentTechnique.Passes.Count; ps++)
                {
                    fShader.CurrentTechnique.Passes[ps].Apply();
                    kvp.Value.Render();
                }
                i++;
            }
            //--------------------
        }

        #endregion

        #region Private methods: Space

        /// <summary>
        /// Init the room
        /// </summary>
        private void InitSpace()
        {
            for (int x = 0; x < WORLD_BLOCKS; x++)
                for (int y = 0; y < WORLD_BLOCKS; y++)
                    for (int z = 0; z < WORLD_BLOCKS; z++)
                    {
                        fSpace[x, y, z].ForcePositiveX = 0;
                        fSpace[x, y, z].ForcePositiveY = 0;
                        fSpace[x, y, z].ForcePositiveZ = 0;
                        fSpace[x, y, z].ForceNegativeX = 0;
                        fSpace[x, y, z].ForceNegativeY = 0;
                        fSpace[x, y, z].ForceNegativeZ = 0;
                    }
        }

        /// <summary>
        /// Update the space and particle velocity/direction
        /// </summary>
        private int UpdateSpaceAndParticles(double factor)
        {
            fSpaceTemp = new MySpace[WORLD_BLOCKS, WORLD_BLOCKS, WORLD_BLOCKS];
            int count = 0;
            float force;

            #region STEP: All particles changes the room structure

            Parallel.ForEach(fParticles, particle =>
            {
                int blockX = (int)((particle.Position.X + HALF_WORLD_SIZE) * PARTICLE_TO_WORLD);
                int blockY = (int)((particle.Position.Y + HALF_WORLD_SIZE) * PARTICLE_TO_WORLD);
                int blockZ = (int)((particle.Position.Z + HALF_WORLD_SIZE) * PARTICLE_TO_WORLD);
                if (blockX < 1 || blockY < 1 || blockZ < 1 ||
                    blockX >= WORLD_BLOCKS_MAX || blockY >= WORLD_BLOCKS_MAX || blockZ >= WORLD_BLOCKS_MAX)
                {
                    particle.Remove = true;
                }
                else
                {
                    force = particle.Mass;// * factor?
                    force *= 0.1f;
                    float force70 = 0.7f * force;
                    float force50 = 0.5f * force;
                    fSpace[blockX - 1, blockY, blockZ].ForcePositiveX += force;
                    fSpace[blockX + 1, blockY, blockZ].ForceNegativeX += force;
                    fSpace[blockX, blockY - 1, blockZ].ForcePositiveY += force;
                    fSpace[blockX, blockY + 1, blockZ].ForceNegativeY += force;
                    fSpace[blockX, blockY, blockZ - 1].ForcePositiveZ += force;
                    fSpace[blockX, blockY, blockZ + 1].ForceNegativeZ += force;

                    fSpace[blockX - 1, blockY - 1, blockZ].ForcePositiveX += force70;
                    fSpace[blockX - 1, blockY - 1, blockZ].ForcePositiveY += force70;
                    fSpace[blockX + 1, blockY - 1, blockZ].ForceNegativeX += force70;
                    fSpace[blockX + 1, blockY - 1, blockZ].ForcePositiveY += force70;
                    fSpace[blockX - 1, blockY + 1, blockZ].ForcePositiveX += force70;
                    fSpace[blockX - 1, blockY + 1, blockZ].ForceNegativeY += force70;
                    fSpace[blockX + 1, blockY + 1, blockZ].ForceNegativeX += force70;
                    fSpace[blockX + 1, blockY + 1, blockZ].ForceNegativeY += force70;

                    fSpace[blockX - 1, blockY, blockZ - 1].ForcePositiveX += force70;
                    fSpace[blockX - 1, blockY, blockZ - 1].ForcePositiveZ += force70;
                    fSpace[blockX + 1, blockY, blockZ - 1].ForceNegativeX += force70;
                    fSpace[blockX + 1, blockY, blockZ - 1].ForcePositiveZ += force70;
                    fSpace[blockX - 1, blockY, blockZ + 1].ForcePositiveX += force70;
                    fSpace[blockX - 1, blockY, blockZ + 1].ForceNegativeZ += force70;
                    fSpace[blockX + 1, blockY, blockZ + 1].ForceNegativeX += force70;
                    fSpace[blockX + 1, blockY, blockZ + 1].ForceNegativeZ += force70;

                    fSpace[blockX, blockY - 1, blockZ - 1].ForcePositiveY += force70;
                    fSpace[blockX, blockY - 1, blockZ - 1].ForcePositiveZ += force70;
                    fSpace[blockX, blockY + 1, blockZ - 1].ForceNegativeY += force70;
                    fSpace[blockX, blockY + 1, blockZ - 1].ForcePositiveZ += force70;
                    fSpace[blockX, blockY - 1, blockZ + 1].ForcePositiveY += force70;
                    fSpace[blockX, blockY - 1, blockZ + 1].ForceNegativeZ += force70;
                    fSpace[blockX, blockY + 1, blockZ + 1].ForceNegativeY += force70;
                    fSpace[blockX, blockY + 1, blockZ + 1].ForceNegativeZ += force70;

                    fSpace[blockX - 1, blockY - 1, blockZ - 1].ForcePositiveX += force50;
                    fSpace[blockX - 1, blockY - 1, blockZ - 1].ForcePositiveY += force50;
                    fSpace[blockX - 1, blockY - 1, blockZ - 1].ForcePositiveZ += force50;
                    fSpace[blockX + 1, blockY - 1, blockZ - 1].ForceNegativeX += force50;
                    fSpace[blockX + 1, blockY - 1, blockZ - 1].ForcePositiveY += force50;
                    fSpace[blockX + 1, blockY - 1, blockZ - 1].ForcePositiveZ += force50;
                    fSpace[blockX - 1, blockY + 1, blockZ - 1].ForcePositiveX += force50;
                    fSpace[blockX - 1, blockY + 1, blockZ - 1].ForceNegativeY += force50;
                    fSpace[blockX - 1, blockY + 1, blockZ - 1].ForcePositiveZ += force50;
                    fSpace[blockX + 1, blockY + 1, blockZ - 1].ForceNegativeX += force50;
                    fSpace[blockX + 1, blockY + 1, blockZ - 1].ForceNegativeY += force50;
                    fSpace[blockX + 1, blockY + 1, blockZ - 1].ForcePositiveZ += force50;
                    fSpace[blockX - 1, blockY - 1, blockZ + 1].ForcePositiveX += force50;
                    fSpace[blockX - 1, blockY - 1, blockZ + 1].ForcePositiveY += force50;
                    fSpace[blockX - 1, blockY - 1, blockZ + 1].ForceNegativeZ += force50;
                    fSpace[blockX + 1, blockY - 1, blockZ + 1].ForceNegativeX += force50;
                    fSpace[blockX + 1, blockY - 1, blockZ + 1].ForcePositiveY += force50;
                    fSpace[blockX + 1, blockY - 1, blockZ + 1].ForceNegativeZ += force50;
                    fSpace[blockX - 1, blockY + 1, blockZ + 1].ForcePositiveX += force50;
                    fSpace[blockX - 1, blockY + 1, blockZ + 1].ForceNegativeY += force50;
                    fSpace[blockX - 1, blockY + 1, blockZ + 1].ForceNegativeZ += force50;
                    fSpace[blockX + 1, blockY + 1, blockZ + 1].ForceNegativeX += force50;
                    fSpace[blockX + 1, blockY + 1, blockZ + 1].ForceNegativeY += force50;
                    fSpace[blockX + 1, blockY + 1, blockZ + 1].ForceNegativeZ += force50;
                }
                count++;
            });

            #endregion

            #region STEP: Copy SPACE to TEMP

            Parallel.For(1, WORLD_BLOCKS_MAX, x =>
            {
                for (int y = 1; y < WORLD_BLOCKS_MAX; y++)
                {
                    for (int z = 1; z < WORLD_BLOCKS_MAX; z++)
                    {
                        float forceX = fSpace[x, y, z].ForcePositiveX - fSpace[x, y, z].ForceNegativeX;
                        float forceY = fSpace[x, y, z].ForcePositiveY - fSpace[x, y, z].ForceNegativeY;
                        float forceZ = fSpace[x, y, z].ForcePositiveZ - fSpace[x, y, z].ForceNegativeZ;

                        if (forceX > 0)
                        {
                            fSpaceTemp[x, y, z].ForcePositiveX = forceX;
                        }
                        else
                        {
                            fSpaceTemp[x, y, z].ForceNegativeX = -forceX;
                        }
                        if (forceY > 0)
                        {
                            fSpaceTemp[x, y, z].ForcePositiveY = forceY;
                        }
                        else
                        {
                            fSpaceTemp[x, y, z].ForceNegativeY = -forceY;
                        }
                        if (forceZ > 0)
                        {
                            fSpaceTemp[x, y, z].ForcePositiveZ = forceZ;
                        }
                        else
                        {
                            fSpaceTemp[x, y, z].ForceNegativeZ = -forceZ;
                        }
                        count++;
                    }
                }
            });

            #endregion

            #region STEP: Every location affects it neighbours

            Parallel.For(1, WORLD_BLOCKS_MAX, x =>
            {
                for (int y = 1; y < WORLD_BLOCKS_MAX; y++)
                {
                    for (int z = 1; z < WORLD_BLOCKS_MAX; z++)
                    {
                        fSpace[x, y, z].ForcePositiveX = 0;
                        fSpace[x, y, z].ForcePositiveY = 0;
                        fSpace[x, y, z].ForcePositiveZ = 0;
                        fSpace[x, y, z].ForceNegativeX = 0;
                        fSpace[x, y, z].ForceNegativeY = 0;
                        fSpace[x, y, z].ForceNegativeZ = 0;
                        for (int dx = -1; dx < 2; dx++)
                        {
                            for (int dy = -1; dy < 2; dy++)
                            {
                                for (int dz = -1; dz < 2; dz++)
                                {
                                    float fx = fSpaceTemp[x + dx, y + dy, z + dz].ForcePositiveX - fSpaceTemp[x + dx, y + dy, z + dz].ForceNegativeX;
                                    float fy = fSpaceTemp[x + dx, y + dy, z + dz].ForcePositiveY - fSpaceTemp[x + dx, y + dy, z + dz].ForceNegativeY;
                                    float fz = fSpaceTemp[x + dx, y + dy, z + dz].ForcePositiveZ - fSpaceTemp[x + dx, y + dy, z + dz].ForceNegativeZ;
                                    if (fx > 0)
                                    {
                                        fSpace[x, y, z].ForcePositiveX += fx;
                                    }
                                    else
                                    {
                                        fSpace[x, y, z].ForceNegativeX -= fx;
                                    }
                                    if (fy > 0)
                                    {
                                        fSpace[x, y, z].ForcePositiveY += fy;
                                    }
                                    else
                                    {
                                        fSpace[x, y, z].ForceNegativeY -= fy;
                                    }
                                    if (fz > 0)
                                    {
                                        fSpace[x, y, z].ForcePositiveZ += fz;
                                    }
                                    else
                                    {
                                        fSpace[x, y, z].ForceNegativeZ -= fz;
                                    }
                                }
                            }
                        }
                        float m = 1 / 27f;
                        fSpace[x, y, z].ForcePositiveX *= m;
                        fSpace[x, y, z].ForcePositiveY *= m;
                        fSpace[x, y, z].ForcePositiveZ *= m;
                        fSpace[x, y, z].ForceNegativeX *= m;
                        fSpace[x, y, z].ForceNegativeY *= m;
                        fSpace[x, y, z].ForceNegativeZ *= m;
                        count++;
                    }
                }
            });

            for (int i = 0; i < WORLD_BLOCKS; i++)
            {
                for (int j = 0; j < WORLD_BLOCKS; j++)
                {
                    fSpace[i, j, 0] = new MySpace();
                    fSpace[i, j, WORLD_BLOCKS_MAX] = new MySpace();
                    fSpace[i, 0, j] = new MySpace();
                    fSpace[i, WORLD_BLOCKS_MAX, j] = new MySpace();
                    fSpace[0, i, j] = new MySpace();
                    fSpace[WORLD_BLOCKS_MAX, i, j] = new MySpace();

                }
            }

            #endregion

            #region STEP: Change velocity of every particle

            fTotalMass = 0;
            foreach (Particle particle in fParticles)
            {
                int blockX = (int)((particle.Position.X + HALF_WORLD_SIZE) * PARTICLE_TO_WORLD);
                int blockY = (int)((particle.Position.Y + HALF_WORLD_SIZE) * PARTICLE_TO_WORLD);
                int blockZ = (int)((particle.Position.Z + HALF_WORLD_SIZE) * PARTICLE_TO_WORLD);
                if (blockX < 1 || blockY < 1 || blockZ < 1 || blockX >= WORLD_BLOCKS_MAX || blockY >= WORLD_BLOCKS_MAX || blockZ >= WORLD_BLOCKS_MAX)
                {
                    particle.Remove = true;
                }
                else
                {
                    fTotalMass += particle.Mass;
                    float forceX = fSpace[blockX, blockY, blockZ].ForcePositiveX - fSpace[blockX, blockY, blockZ].ForceNegativeX;
                    float forceY = fSpace[blockX, blockY, blockZ].ForcePositiveY - fSpace[blockX, blockY, blockZ].ForceNegativeY;
                    float forceZ = fSpace[blockX, blockY, blockZ].ForcePositiveZ - fSpace[blockX, blockY, blockZ].ForceNegativeZ;
                    float effect = 0.01f;// 0.01f / particle.Mass;
                    forceX *= effect;
                    forceY *= effect;
                    forceZ *= effect;

                    particle.Velocity.X += (float)(fGravitationEffect * factor * forceX);
                    particle.Velocity.Y += (float)(fGravitationEffect * factor * forceY);
                    particle.Velocity.Z += (float)(fGravitationEffect * factor * forceZ);

                }
                count++;
            }

            #endregion

            #region Update position of every particle

            for (int i = 0; i < fParticles.Count; i++)
            {
                Particle particle = fParticles[i];
                if (particle.Remove)
                {
                    fParticles.RemoveAt(i--);
                }
                else
                {
                    particle.Position.X += Convert.ToSingle(factor * particle.Velocity.X);
                    particle.Position.Y += Convert.ToSingle(factor * particle.Velocity.Y);
                    particle.Position.Z += Convert.ToSingle(factor * particle.Velocity.Z);
                }
                count++;
            }

            #endregion

            return count;
        }

        #endregion

    }
}

