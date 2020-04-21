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
 * File:		Gravity2Engine
 * Purpose:		2D gravity engine - Change the room and let masses travel in this room
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2010-04-09  RW	
 * 
 * History:		2010-04-09  RW  Created
 *              2020-04-08  RW  Moved to MonoExperience
 * 
 * 
 */
namespace MonoExperience
{

    /// <summary>
    /// An engine that moves masses with a space calculation algorithm
    /// </summary>
    public class Gravity2Engine : BaseEngine
    {

        #region Enums & Classes

        class Particle
        {
            public Vector2 Position;
            public Vector2 Velocity;

            public float Mass;
            public Texture2D Texture;
            public float Size;
            public bool Remove;

            public Particle(Vector2 position, Vector2 velocity, float mass)
            {
                Position = position;
                Velocity = velocity;
                Mass = mass;
                Texture = null;
                Size = 1.0f;
            }

            public void SetTexture(Texture2D texture)
            {
                if (Texture != texture)
                {
                    Texture = texture;
                    Size = 2.0f * texture.Width;
                }
            }
        }

        struct MySpace
        {
            public float ForceUp;
            public float ForceDown;
            public float ForceRight;
            public float ForceLeft;
        }

        #endregion

        #region Constants

        private const int WORLD_BLOCKS = 300;
        private const float WORLD_SIZE = 5000.0f;
        // Convert particle position to world/block position
        private const float PARTICLE_TO_WORLD = WORLD_BLOCKS / WORLD_SIZE;
        private const float HALF_WORLD_SIZE = WORLD_SIZE / 2.0f;
        private const int WORLD_BLOCKS_MAX = WORLD_BLOCKS - 1;
        private const float MAX_LINE_SIZE = 15;

        private const int INIT_RADIUS = 400;
        private const int ADD_STARS = 1000;

        private DynamicPrimitiveLine fLines;

        #endregion

        #region Private members

        private SpriteBatch fSpriteBatch;

        private Texture2D[] fTexRed;
        private Texture2D[] fTexBlue;

        private List<Particle> fParticles = new List<Particle>();

        private Random fRandom = new Random();
        private bool fAllowNegative;
        private bool fDrawGrid;
        private bool fDrawParticles = true;

        private BasicEffect fBasicEffect;
        private Effect fShader;
        private BlendState fBlendState;
        private SimpleCameraController fCamera;

        private int fCalculations;
        private MySpace[,] fSpace = new MySpace[WORLD_BLOCKS, WORLD_BLOCKS];
        private MySpace[,] fSpaceTemp = new MySpace[WORLD_BLOCKS, WORLD_BLOCKS];
        private float fGravitationEffect = 1.0f;
        private float fTotalMass = 0;

        #endregion

        #region Constructor

        /// <summary>
        /// Create the engine
        /// </summary>
        /// <param name="game"></param>
        public Gravity2Engine(EngineContainer cnt) : base(cnt)
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
            return "2D Gravity - Particles changes the room";
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
            return @"2D Gravity sample where objects affects the room";
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
            double radius = fRandom.Next(INIT_RADIUS);
            double v = fRandom.Next(360) / 180.0f * Math.PI;
            int x = Convert.ToInt32(radius * Math.Cos(v));
            int y = Convert.ToInt32(radius * Math.Sin(v));

            Vector2 position = new Vector2(x, y);
            Vector2 velocity = Vector2.Zero;// new Vector2(-y / 5, x / 5);
            float mass;

            int size = fRandom.Next(10000);
            //if (size < 5)
            //{
            //    mass = 100000.0f;
            //}
            if (size < 10)
            {
                mass = 10000.0f;
            }
            else if (size < 100)
            {
                mass = 1000.0f;
            }
            else if (size < 1000)
            {
                mass = 100.0f;
            }
            else if (size < 5000)
            {
                mass = 10.0f;
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
            int index;
            if (mass >= 100000.0)
                index = 6;
            else if (mass > 10000.0)
                index = 5;
            else if (mass > 1000.0)
                index = 4;
            else if (mass > 100.0)
                index = 3;
            else if (mass > 10.0)
                index = 2;
            else if (mass > 5.0)
                index = 1;
            else
                index = 0;
            if (data.Mass >= 0)
                data.SetTexture(fTexBlue[index]);
            else
                data.SetTexture(fTexRed[index]);
        }

        #endregion

        #region Private methods: Draw space & particles

        /// <summary>
        /// Draw the space
        /// </summary>
        private void RenderGrid()
        {
            fLines = new DynamicPrimitiveLine(fSpriteBatch.GraphicsDevice);
            float worldFactor = WORLD_SIZE / (WORLD_BLOCKS - 1);

            for (int x = 0; x < WORLD_BLOCKS; x++)
            {
                //int blockX = (int)((particle.Position.X + HALF_WORLD_SIZE) * PARTICLE_TO_WORLD);
                //int blockY = (int)((particle.Position.Y + HALF_WORLD_SIZE) * PARTICLE_TO_WORLD);
                float sx1 = (x + 0.5f) * worldFactor - HALF_WORLD_SIZE;
                for (int y = 0; y < WORLD_BLOCKS; y++)
                {
                    float sy1 = (y + 0.5f) * worldFactor - HALF_WORLD_SIZE;
                    float dx = 1024 * 1024 * 1024 * (fSpace[x, y].ForceRight - fSpace[x, y].ForceLeft);
                    float dy = 1024 * 1024 * 1024 * (fSpace[x, y].ForceUp - fSpace[x, y].ForceDown);
                    float f = Convert.ToSingle(Math.Sqrt(dx * dx + dy * dy));
                    float f1 = f;
                    Color c1;
                    Color c2;
                    int b1;
                    float size = 1.0f;
                    if (f1 < 1024.0f)
                    {
                        size = f1 / 1024.0f;
                        b1 = (byte)(f1 / 4.0f);
                        c1 = new Color(0,0, b1, 255);
                        c2 = new Color(0, 0, b1, 255);
                    }
                    else
                    {
                        f1 /= 1024.0f;
                        if (f1 < 1024.0f)
                        {
                            size = 1.0f;// 0.5f + f1 / 512.0f;
                            b1 = (byte)(f1 / 4.0f);
                            c1 = new Color(0, b1, 255 - b1, 255);
                            c2 = new Color(0, 255, 0, 128);
                        }
                        else
                        {
                            f1 /= 1024.0f;
                            if (f1 < 1024.0f)
                            {
                                size = 1.0f;//0.5f + f1 / 512.0f;
                                b1 = (byte)(f1 / 4.0f);
                                c1 = new Color(b1, 255, 0, 255);
                                c2 = new Color(255, 255, 0, 128);
                            }
                            else
                            {
                                f1 /= 1024.0f;
                                if (f1 < 1024.0f)
                                {
                                    size = 1.0f;//0.5f + f1 / 512.0f;
                                    b1 = (byte)(f1 / 4.0f);
                                    c1 = new Color(255, 255 - b1, 0, 255);
                                    c2 = new Color(255, 0, 0, 128);
                                }
                                else
                                {
                                    size = 1.0f;
                                    c1 = new Color(255, 0, 255, 255);
                                    c2 = new Color(255, 0, 255, 255);
                                }
                            }
                        }
                    }

                    dx = MAX_LINE_SIZE * (dx / f) * size;
                    dy = MAX_LINE_SIZE * (dy / f) * size;
                    float sx2 = sx1 + dx;
                    float sy2 = sy1 + dy;
                    VertexPositionColor p1 = new VertexPositionColor(new Vector3(sx1, sy1, 0), c1);
                    VertexPositionColor p2 = new VertexPositionColor(new Vector3(sx2, sy2, 0), c2);
                    fLines.AddLine(p1, p2);
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
            Dictionary<Texture2D, Billboard> billboards = new Dictionary<Texture2D, Billboard>();
            foreach (Particle particle in fParticles)
            {
                Texture2D texture = particle.Texture;
                if (texture == null)
                {
                    SetParticleTexture(particle);
                    texture = particle.Texture;
                }
                if (!billboards.ContainsKey(texture))
                    billboards.Add(texture, new Billboard(GraphicsDevice));
                Billboard billboard = billboards[texture];
                billboard.AddObject(
                    new Vector3(particle.Position.X, particle.Position.Y, 0),
                    Color.White, particle.Size);
            }
            //--------------------
            GraphicsDevice.BlendState = fBlendState;
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            Matrix vp = fCamera.Camera.ViewMatrix * fCamera.Camera.ProjectionMatrix;
            fShader.Parameters["world"].SetValue(fCamera.Camera.WorldMatrix);
            fShader.Parameters["vp"].SetValue(vp);
            int i = 0;
            foreach (KeyValuePair<Texture2D, Billboard> kvp in billboards)
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

        #region Private methods: Particles

        #endregion

        #region Private methods: Space

        /// <summary>
        /// Init the room
        /// </summary>
        private void InitSpace()
        {
            for (int x = 0; x < WORLD_BLOCKS; x++)
                for (int y = 0; y < WORLD_BLOCKS; y++)
                {
                    fSpace[x, y].ForceLeft = 0;
                    fSpace[x, y].ForceRight = 0;
                    fSpace[x, y].ForceUp = 0;
                    fSpace[x, y].ForceDown = 0;
                }
        }

        /// <summary>
        /// Update the space and particle velocity/direction
        /// </summary>
        private int UpdateSpaceAndParticles(double factor)
        {
            int count = 0;
            float force;

            // -----------------------------------------------------------------------
            // STEP: All particles changes the room structure
            // -----------------------------------------------------------------------
            
            Parallel.ForEach(fParticles, particle =>
            {
                int blockX = (int)((particle.Position.X + HALF_WORLD_SIZE) * PARTICLE_TO_WORLD);
                int blockY = (int)((particle.Position.Y + HALF_WORLD_SIZE) * PARTICLE_TO_WORLD);
                if (blockX < 1 || blockY < 1 || blockX >= WORLD_BLOCKS_MAX || blockY >= WORLD_BLOCKS_MAX)
                {
                    particle.Remove = true;
                }
                else
                {
                    force = particle.Mass;// * factor?
                    force *= 0.1f;
                    float force70 = 0.7f * force;
                    fSpace[blockX - 1, blockY].ForceRight += force;
                    fSpace[blockX + 1, blockY].ForceLeft += force;
                    fSpace[blockX, blockY - 1].ForceUp += force;
                    fSpace[blockX, blockY + 1].ForceDown += force;
                    fSpace[blockX - 1, blockY - 1].ForceRight += force70;
                    fSpace[blockX - 1, blockY - 1].ForceUp += force70;
                    fSpace[blockX + 1, blockY - 1].ForceLeft += force70;
                    fSpace[blockX + 1, blockY - 1].ForceUp += force70;
                    fSpace[blockX - 1, blockY + 1].ForceRight += force70;
                    fSpace[blockX - 1, blockY + 1].ForceDown += force70;
                    fSpace[blockX + 1, blockY + 1].ForceLeft += force70;
                    fSpace[blockX + 1, blockY + 1].ForceDown += force70;
                }
                count++;
            });

            // -----------------------------------------------------------------------
            // STEP: Copy SPACE to TEMP
            // -----------------------------------------------------------------------
            
            fSpaceTemp = new MySpace[WORLD_BLOCKS, WORLD_BLOCKS];
            Parallel.For(1, WORLD_BLOCKS_MAX, x =>
            {
                for (int y = 1; y < WORLD_BLOCKS_MAX; y++)
                {
                    float forceX = fSpace[x, y].ForceRight - fSpace[x, y].ForceLeft;
                    float forceY = fSpace[x, y].ForceUp - fSpace[x, y].ForceDown;
                    if (forceX > 0)
                    {
                        fSpaceTemp[x, y].ForceRight = forceX;
                    }
                    else
                    {
                        fSpaceTemp[x, y].ForceLeft = -forceX;
                    }
                    if (forceY > 0)
                    {
                        fSpaceTemp[x, y].ForceUp = forceY;
                    }
                    else
                    {
                        fSpaceTemp[x, y].ForceDown = -forceY;
                    }
                    count++;
                }
            });
            for (int i = 0; i < WORLD_BLOCKS; i++)
            {
                fSpaceTemp[0, i] = new MySpace();
                fSpaceTemp[WORLD_BLOCKS_MAX, i] = new MySpace();
                fSpaceTemp[i, 0] = new MySpace();
                fSpaceTemp[i, WORLD_BLOCKS_MAX] = new MySpace();
            }
            
            // -----------------------------------------------------------------------
            // STEP: Update every location with forces from neighbours
            // -----------------------------------------------------------------------
            
            Parallel.For(1, WORLD_BLOCKS_MAX, x =>
            {
                for (int y = 1; y < WORLD_BLOCKS_MAX; y++)
                {
                    fSpace[x, y].ForceRight = 0;
                    fSpace[x, y].ForceLeft = 0;
                    fSpace[x, y].ForceUp = 0;
                    fSpace[x, y].ForceDown = 0;
                    for (int dx = -1; dx < 2; dx++)
                    {
                        for (int dy = -1; dy < 2; dy++)
                        {
                            float fx = fSpaceTemp[x + dx, y + dy].ForceRight - fSpaceTemp[x + dx, y + dy].ForceLeft;
                            float fy = fSpaceTemp[x + dx, y + dy].ForceUp - fSpaceTemp[x + dx, y + dy].ForceDown;
                            if (fx > 0)
                                fSpace[x, y].ForceRight += fx;                                
                            else
                               fSpace[x, y].ForceLeft -= fx;
                            if (fy > 0)
                                fSpace[x, y].ForceUp += fy;
                            else
                                fSpace[x, y].ForceDown -= fy;
                        }
                    }
                    float m = 1 / 10f;
                    fSpace[x, y].ForceRight *= m;
                    fSpace[x, y].ForceLeft *= m;
                    fSpace[x, y].ForceUp *= m;
                    fSpace[x, y].ForceDown *= m;
                    count++;
                }
            });
            for (int i = 0; i < WORLD_BLOCKS; i++)
            {
                fSpace[0, i] = new MySpace();
                fSpace[WORLD_BLOCKS_MAX, i] = new MySpace();
                fSpace[i, 0] = new MySpace();
                fSpace[i, WORLD_BLOCKS_MAX] = new MySpace();
            }

            // -----------------------------------------------------------------------
            // STEP: Room changes velocity of all particles
            // -----------------------------------------------------------------------

            fTotalMass = 0;
            foreach (Particle particle in fParticles)
            {
                int blockX = (int)((particle.Position.X + HALF_WORLD_SIZE) * PARTICLE_TO_WORLD);
                int blockY = (int)((particle.Position.Y + HALF_WORLD_SIZE) * PARTICLE_TO_WORLD);
                if (blockX < 1 || blockY < 1 || blockX >= WORLD_BLOCKS_MAX || blockY >= WORLD_BLOCKS_MAX)
                {
                    particle.Remove = true;
                }
                else
                {
                    fTotalMass += particle.Mass;

                    float forceX = fSpace[blockX, blockY].ForceRight - fSpace[blockX, blockY].ForceLeft;
                    float forceY = fSpace[blockX, blockY].ForceUp - fSpace[blockX, blockY].ForceDown;
                    float effect = 0.0100f;
                    forceX *= effect;
                    forceY *= effect;
                    particle.Velocity.X += (float)(fGravitationEffect * factor * forceX);
                    particle.Velocity.Y += (float)(fGravitationEffect * factor * forceY);

                }
                count++;
            }

            // -----------------------------------------------------------------------
            // STEP: Update position of all particles
            // -----------------------------------------------------------------------

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
                }
                count++;
            }

            return count;
        }

        #endregion
   
    }
}

