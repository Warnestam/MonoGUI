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
 * File:		SpaceShipEngine1
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
    public class SpaceShipEngine1 : BaseEngine
    {
        const int NUM_SHIPS = 1000;
        const int SPACE_SIZE = 5000;
        const int MODEL_SIZE = 30;
        const float PI = 3.141592653589f;

        enum MyMode { SteerCamera, SteerShips };

        #region Private members

        private SpriteBatch fSpriteBatch;
        private Random fRandom = new Random();
        private SimpleCameraController fViewCamera;
        private SimpleCameraController fShipCamera;
        private bool fHalted = false;
        private List<MyShip> fShips = new List<MyShip>();
        private Model fShipModel;
        private MyMode fMode = MyMode.SteerCamera;

        #endregion

        #region Constructor

        /// <summary>
        /// Create the engine
        /// </summary>
        /// <param name="game"></param>
        public SpaceShipEngine1(EngineContainer cnt) : base(cnt)
        {
            // TODO: Construct any child components here
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

            fShipCamera.Update(gameTime);
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
            return "Space Ship Engine 1";
        }

        public override string GetHelp()
        {
            string text1 = "H Toggle Halt\nM Toggle Mode";
            string text2 = fViewCamera.GetHelp();
            return String.Format("{0}\n{1}", text1, text2);
        }

        public override string GetInfo()
        {
            string text1 = $"Ships: {fShips.Count}\nMode: {fMode}";
            string text2 = fViewCamera.GetInfo();
            return String.Format("{0}\n{1}", text1, text2);
        }

        public override string GetAbout()
        {
            return "Camera vs Ship Movement";
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
            else if (this.Manager.KeyPressed(Keys.M))
            {
                if (fMode == MyMode.SteerCamera)
                {
                    fMode = MyMode.SteerShips;
                    fViewCamera.Disable();
                    fShipCamera.Enable();
                }
                else
                {
                    fMode = MyMode.SteerCamera;
                    fShipCamera.Disable();
                    fViewCamera.Enable();
                }
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

            fShipCamera = new SimpleCameraController(this.Game, this.Manager, GraphicsDevice.Viewport);
            fShipCamera.Camera.Rotation = Quaternion.CreateFromYawPitchRoll(0, 0.0f, 0);
            fShipCamera.Camera.Position = new Vector3(0.0f, 0.0f, 0.0f);
            fShipCamera.Camera.Update();
            fShipCamera.ForwardDirection = Vector3.Up;
            fShipCamera.UseInverseMovement = false;
            fShipCamera.Disable();// No input!
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
                Position = new Vector3(x, y, z)
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
            // All done in fShipCamera
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

            var rotation = Matrix.CreateFromQuaternion(fShipCamera.Camera.Rotation);// * Matrix.CreateRotationX(PI / -2);

            var world = Matrix.CreateScale(MODEL_SIZE) *
                        rotation *
                        Matrix.CreateTranslation(ship.Position+fShipCamera.Camera.Position);

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
            
            public MyShip()
            {
            }



        }

        #endregion

    }
}
