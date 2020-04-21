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
 * File:		Gravity1Engine
 * Purpose:		2D gravity engine - Calculate forces between masses
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
    /// An engine that moves masses with a gravity calculation algorithm
    /// </summary>
    public class Gravity1Engine : BaseEngine
    {

        #region Enums & Classes

        class Particle
        {
            public Vector2 Position;
            public Vector2 Velocity;

            public double Mass;
            public Texture2D Texture;
            public float Size;
            public bool Remove;

            public Particle(Vector2 position, Vector2 velocity, double mass)
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
                    Size = 2.0f*texture.Width;
                }
            }
        }

        #endregion

        #region Constants

        private const double GRAVITY = 200.0f;
        private const double MAX_FORCE = 200f;
        private const double MIN_RADIUS = 0.1f;
        private const int INIT_RADIUS = 200;

        #endregion

        #region Private members

        private SpriteBatch fSpriteBatch;

        private Texture2D[] fTexRed;
        private Texture2D[] fTexBlue;

        private List<Particle> fParticles = new List<Particle>();

        private Random fRandom = new Random();
        private bool fAllowNegative;

        private BasicEffect fBasicEffect;
        private SimpleCameraController fCamera;
        private Effect fShader;
        private BlendState fBlendState;
      
        private int fCalculations;

        #endregion

        #region Constructor

        /// <summary>
        /// Create the engine
        /// </summary>
        /// <param name="game"></param>
        public Gravity1Engine(EngineContainer cnt) : base(cnt)
        {  
            for (int i = 0; i < 100; i++)
                AddParticle();
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
                fCalculations = UpdateStandard(factor);
            }
            MoveParticles(factor);

            base.Update(gameTime);
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
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
                     new Vector3(particle.Position.X, particle.Position.Y,0), 
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
            base.Draw(gameTime);
        }

        #endregion

        #region BaseEngine

        public override string GetName()
        {
            return "2D Gravity with Newtons formula";
        }

        public override string GetHelp()
        {
            string text1 = "R Reinit particles\nN Toggle allow negative\n+ Add particles\n- Remove particles";
            string text2 = fCamera.GetHelp();
            return String.Format("{0}\n{1}", text1, text2);

        }

        public override string GetInfo()
        {
            string text1 = String.Format("Number of particles: {0}\nAllow negative: {1}\nCalculations: {2}",
                fParticles.Count, fAllowNegative, fCalculations
                );
            string text2 = fCamera.GetInfo();
            return String.Format("{0}\n{1}", text1, text2);
        }

        public override string GetAbout()
        {
            return @"2D Gravity sample where objects affects eachother with Newtons gravity formula";
        }


        private void HandleInput(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float timeFactor = 100.0f * delta;

            if (this.Manager.KeyPressed(Keys.R))
                this.ReInit(100);
            else if (this.Manager.KeyPressed(Keys.Add))
                this.AddParticles(100);
            else if (this.Manager.KeyPressed(Keys.Subtract))
                this.RemoveParticles(100);
            else if (this.Manager.KeyPressed(Keys.N))
                this.AllowNegative = !this.AllowNegative;
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
        public void ReInit(int particles)
        {
            fParticles.Clear();
            for (int i = 0; i < particles; i++)
                AddParticle();
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
            Vector2 velocity = new Vector2(-y/5, x/5);
            double mass;

            int size = fRandom.Next(1000);
            if (size < 5)
            {
                mass = 100000.0;
            }
            else if (size < 10)
            {
                mass = 10000.0f;
            }
            else if (size < 25)
            {
                mass = 1000.0f;
            }
            else if (size < 100)
            {
                mass = 100.0f;
            }
            else if (size < 250)
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

        /// <summary>
        /// Move all particles
        /// </summary>
        /// <param name="factor"></param>
        private void MoveParticles(double factor)
        {
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
            }
        }

        #endregion

        #region Private methods: Calculate gravity

        /// <summary>
        /// Everybode affect everybody (O=n^2)
        /// </summary>
        /// <param name="factor"></param>
        private int UpdateStandard(double factor)
        {
            int count = 0;

            Parallel.For(1, fParticles.Count - 1, i =>
            //for (int i = 1; i < fParticles.Count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (UpdateParticles(fParticles[i], fParticles[j], factor))
                        count++;
                }
            });
            return count;
        }

        /// <summary>
        /// Update forces between two particles
        /// </summary>
        /// <param name="particle1"></param>
        /// <param name="particle2"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        private bool UpdateParticles(Particle particle1, Particle particle2, double factor)
        {
            if (particle1 != particle2 && !particle1.Remove && !particle2.Remove)
            {
                double deltaX = particle1.Position.X - particle2.Position.X;
                double deltaY = particle1.Position.Y - particle2.Position.Y;
                double dx2 = deltaX * deltaX;
                double dy2 = deltaY * deltaY;
                double radius = Math.Sqrt(dx2 + dy2);

                bool remove = false;
                if (Math.Abs(radius) < MIN_RADIUS)
                {
                    radius = MIN_RADIUS;
                    remove = true;
                }

                double dxRadius = deltaX / radius;
                double dyRadius = deltaY / radius;
                double r2 = radius * radius;

                double force = GRAVITY * factor * particle1.Mass * particle2.Mass / r2;
                if (force > MAX_FORCE)
                    force = MAX_FORCE;
                else if (force < -MAX_FORCE)
                    force = -MAX_FORCE;

                double force1 = -force / Math.Abs(particle1.Mass);
                double force2 = +force / Math.Abs(particle2.Mass);

                particle1.Velocity.X += Convert.ToSingle(force1 * dxRadius);
                particle1.Velocity.Y += Convert.ToSingle(force1 * dyRadius);
                particle2.Velocity.X += Convert.ToSingle(force2 * dxRadius);
                particle2.Velocity.Y += Convert.ToSingle(force2 * dyRadius);

                if (remove)
                {
                    double m1 = Math.Abs(particle1.Mass);
                    double m2 = Math.Abs(particle2.Mass);
                    double mt = m1 + m2;
                    Vector2 v1 = particle1.Velocity;
                    Vector2 v2 = particle2.Velocity;
                    Vector2 vt = v1 * (float)(m1 / mt) + v2 * (float)(m2 / mt);                    
                    particle1.Remove = true;
                    particle2.Mass += particle1.Mass;
                    SetParticleTexture(particle2);
                    particle2.Velocity = vt;
                }
                return true;
            }
            return false;
        }

        #endregion

    }
}

