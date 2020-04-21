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
 * File:		SpaceShipEngine2
 * Purpose:		Render multiple space ships
 * 
 * Author(s):	RW: Robert Warnestam
 * 
 * History:		2020-04-19  RW  Created

 */
namespace MonoExperience
{

    /// <summary>
    /// Engine for playing with shaders
    /// </summary>
    public class SpaceShipEngine2 : BaseEngine
    {
        const int NUM_SHIPS = 2000;
        const int SPACE_SIZE = 10000;
        const int MODEL_SIZE = 200;
        const float PI = 3.141592653589f;

        #region Private members

        private SpriteBatch fSpriteBatch;
        private Random fRandom = new Random();
        private SimpleCameraController fViewCamera;
        private bool fHalted = false;
        private List<MyShip> fShips = new List<MyShip>();
        private Model fShipModel;

        #endregion

        #region Constructor

        /// <summary>
        /// Create the engine
        /// </summary>
        /// <param name="game"></param>
        public SpaceShipEngine2(EngineContainer cnt) : base(cnt)
        {
            this.BackColor = Color.Black;
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

            fShipModel = this.Game.Content.Load<Model>("MyShader/ship");

            InitializeTransform();
            InitializeEffect();
            InitShips();

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            HandleInput(gameTime);

            fViewCamera.Update(gameTime);

            if (!fHalted)
            {
                UpdateShips(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            RenderShips();

            base.Draw(gameTime);
        }

        #endregion

        #region BaseEngine

        public override string GetName()
        {
            return "Space Ship Engine 2";
        }

        public override string GetHelp()
        {
            string text1 = "H Toggle Halt";
            string text2 = fViewCamera.GetHelp();
            return String.Format("{0}\n{1}", text1, text2);
        }

        public override string GetInfo()
        {
            string text1 = $"Ships: {fShips.Count}";
            string text2 = fViewCamera.GetInfo();
            return String.Format("{0}\n{1}", text1, text2);
        }

        public override string GetAbout()
        {
            return "Auto movement";
        }



        /// <summary>
        /// Handle the input
        /// </summary>
        /// <param name="gameTime"></param>
        private void HandleInput(GameTime gameTime)
        {
            if (this.Manager.KeyPressed(Keys.H))
            {
                fHalted = !fHalted;
            }
        }

        public override void DisplayChanged()
        {
            InitShips();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Initializes the transforms used by the game.
        /// </summary>
        private void InitializeTransform()
        {
            fViewCamera = new SimpleCameraController(this.Game, this.Manager, GraphicsDevice.Viewport);
            fViewCamera.Camera.Rotation = Quaternion.CreateFromYawPitchRoll(0, 0.0f, 0);
            fViewCamera.Camera.Position = new Vector3(0.0f, 0.0f, 0.0f);
            fViewCamera.Camera.Update();
        }

        /// <summary>
        /// Initializes the effect (loading, parameter setting, and technique selection)
        /// used by the game.
        /// </summary>
        private void InitializeEffect()
        {
        }

        #endregion

        #region MyShip

        private void InitShips()
        {
            fShips.Clear();
            for (int i = 0; i < NUM_SHIPS; i++)
            {
                AddShip();
            }
        }

        private void AddShip()
        {
            int x = fRandom.Next(-SPACE_SIZE, SPACE_SIZE);
            int y = fRandom.Next(-SPACE_SIZE, SPACE_SIZE);
            int z = fRandom.Next(-SPACE_SIZE, SPACE_SIZE);
            var ship = new MyShip()
            {
                Position = new Vector3(x, y, z),
                Velocity = (float)(1.0f + 25.0 * fRandom.NextDouble()),
                AngleSpeedX = (float)(fRandom.NextDouble() - 0.5f) / 400.0f,
                AngleSpeedY = (float)(fRandom.NextDouble() - 0.5f) / 30.0f,
                AngleSpeedZ = (float)(fRandom.NextDouble() - 0.5f) / 400.0f,

            };

            fShips.Add(ship);
        }

        private void UpdateShips(GameTime gameTime)
        {
            foreach (var ship in fShips)
            {
                UpdateShip(gameTime, ship);
            }
        }

        private void RenderShips()
        {
            foreach (var ship in fShips)
            {
                RenderShip(ship);
            }
        }


        private void UpdateShip(GameTime gameTime, MyShip ship)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (delta > 0.1f)
                delta = 0.1f;
            float timeFactor = 100.0f * delta;

            float angleSpeedAdd = 0.0002f * timeFactor;
            float angleSpeedMult = 1.0f - (0.1f * timeFactor);
            float maxAngleChange = 0.05f;
            float velocitySpeedMult = 1.0f - (0.1f * timeFactor);
            float velocitySpeedAdd = 0.5f * timeFactor;

            bool updateCamera = false;
            bool angleChanged = false;

            if (ship.Velocity < -0.001f || ship.Velocity > 0.001f)
            {
                Quaternion quat = ship.Rotation;
                Vector3 forwardDirection = Vector3.Transform(Vector3.Up, quat);
                ship.Position += timeFactor * ship.Velocity * forwardDirection;
                updateCamera = true;
            }

            Quaternion rotationChange =
                    Quaternion.CreateFromAxisAngle(Vector3.Left, timeFactor * ship.AngleSpeedX) * //pitch
                    Quaternion.CreateFromAxisAngle(Vector3.Down, timeFactor * ship.AngleSpeedY) * // yaw
                    Quaternion.CreateFromAxisAngle(Vector3.Forward, timeFactor * ship.AngleSpeedZ); // roll


            ship.Rotation = rotationChange * ship.Rotation;


            // Move Ship
            //ship.Move(gameTime);

        }

        private void RenderShip(MyShip ship)
        {
            Model model = fShipModel;
            Matrix view = fViewCamera.Camera.ViewMatrix;
            Matrix projection = fViewCamera.Camera.ProjectionMatrix;

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

            var rotation = Matrix.CreateFromQuaternion(ship.Rotation);// * Matrix.CreateRotationX(PI / -2);

            var world = Matrix.CreateScale(MODEL_SIZE) *
                        rotation *
                        Matrix.CreateTranslation(ship.Position + ship.Position);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = view;
                    effect.Projection = projection;
                    effect.World = world;
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                }
                mesh.Draw();
            }

        }



        internal class MyShip
        {
            public Vector3 Position { get; set; }
            public float Velocity { get; set; }
            public float AngleSpeedX { get; set; }
            public float AngleSpeedY { get; set; }
            public float AngleSpeedZ { get; set; }
            public Quaternion Rotation { get; set; }

            public MyShip()
            {
                Rotation = Quaternion.Identity;
            }

        }

        #endregion

    }
}
