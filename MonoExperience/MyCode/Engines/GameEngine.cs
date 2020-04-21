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

/*
 * File:		GameEngine
 * Purpose:		Simple 2D game engine
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2016-04-11  RW	
 * 
 * History:		2016-04-11  RW  Created
 *              2020-04-01  RW  Moved to MonoExperience
 * 
 */
namespace MonoExperience
{

    /// <summary>
    /// Simple 2D game engine
    /// </summary>
    public class GameEngine : BaseEngine
    {

        #region Classes

        class Shot
        {
            public Vector2 Position { get; set; }

            public Vector2 Velocity { get; set; }

            public float Angle { get; set; }
            public Color Color { get; set; }

            public bool Hit { get; set; }

            public bool FireFromShip { get; set; }

        }

        class Enemy
        {
            public Vector2 Position { get; set; }

            public int InvaderType { get; set; }

            public bool Alive { get; set; }

        }

        #endregion

        #region Constant

        private const int SHIP_FRONT_X = 0;// to center
        private const int SHIP_FRONT_Y = -40;
        private const int SHIP_CENTER_X = 63;
        private const int SHIP_CENTER_Y = 59;
        private const int SHIP_SATELLITE_RADIUS = 100;

        private const int SHIP_SIZE = 64;
        private const int SHOT_SIZE = 64;
        private const int SHOT_CENTER_X = 63;
        private const int SHOT_CENTER_Y = 64;

        private const int INVADER_TYPES = 15;


        #endregion

        #region Private members

        private SpriteBatch fSpriteBatch;
        private Random fRandom = new Random();

        private Matrix fWorldMatrix;
        private Matrix fViewMatrix;
        private Matrix fProjectionMatrix;

        private Matrix fTargetWorldMatrix;
        private Matrix fTargetViewMatrix;
        private Matrix fTargetProjectionMatrix;

        private RenderTarget2D fWorkTarget;

        private Rectangle fScreenRectangle;
        private int fScreenWidth;
        private int fScreenHeight;

        private Texture2D fShipTexture;
        private Texture2D fShipSatelliteTexture;
        private Texture2D fShotTexture;
        private Texture2D fOskarTexture1;
        private Texture2D[] fInvaderTextures;

        private float fShipX;
        private float fShipY;
        private float fShipAngle;
        private float fShipSpeed;
        private float fShipSatelliteAngle;
        private float fShipSatelliteAngle1;
        private float fShipSatelliteAngle2;

        private List<Shot> fShots;
        private TimeSpan fShotTime = TimeSpan.MinValue;
        private TimeSpan fShotTime1 = TimeSpan.MinValue;

        private bool fHalted;
        private List<Enemy> fEnemies;


        #endregion

        #region Constructor

        /// <summary>
        /// Create the engine
        /// </summary>
        /// <param name="game"></param>
        public GameEngine(EngineContainer cnt) : base(cnt)
        {
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

            InitApp();
        }

        protected override void Dispose(bool disposing)
        {
            //
            base.Dispose(disposing);
        }


       

        /// <summary>
        /// Loads any component specific content
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            fSpriteBatch = new SpriteBatch(Game.GraphicsDevice);

                        
            fShipTexture = Game.Content.Load<Texture2D>("Game/ship");
            fShotTexture = Game.Content.Load<Texture2D>("Game/shot");
            fShipSatelliteTexture = Game.Content.Load<Texture2D>("Game/ship_s");
            fOskarTexture1 = Game.Content.Load<Texture2D>("Game/oskar_1");
            fInvaderTextures = new Texture2D[INVADER_TYPES];
            for (int i = 0; i < INVADER_TYPES; i++)
            {
                fInvaderTextures[i] = Game.Content.Load<Texture2D>(String.Format("Game/invader{0:D2}", i + 1));
            }

            UpdateScale();

        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            HandleMouse(gameTime, this.Manager.GetMouseState());

            if (!fHalted)
            {
                if (this.Manager.IsKeyDown(Keys.Left))
                {
                    fShipAngle = fShipAngle - (float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.005f;
                }
                if (this.Manager.IsKeyDown(Keys.Right))
                {
                    fShipAngle = fShipAngle + (float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.005f;
                }

                if (this.Manager.IsKeyDown(Keys.A))
                {
                    fShipSpeed = fShipSpeed + (float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f;
                    if (fShipSpeed > 0.5f)
                        fShipSpeed = 0.5f;
                }
                else /*if (this.Manager.IsKeyDown(Keys.Z))*/
                {
                    fShipSpeed = fShipSpeed - (float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f;
                    if (fShipSpeed < 0)
                        fShipSpeed = 0;
                }

                if (this.Manager.IsKeyDown(Keys.X))
                {
                    TryFire(gameTime);
                }
                TryFireSatellite(gameTime);

            }

            if (this.Manager.KeyPressed(Keys.H))
            {
                fHalted = !fHalted;
            }

            if (!fHalted)
            {
                UpdateShip(gameTime);
                CheckShotCollisions();
                UpdateFire(gameTime);
                UpdateEnemies(gameTime);
            }


        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {


            Rectangle? sourceR = null;

            Vector2 shipPos = new Vector2(fShipX, fShipY);
            Vector2 origin = new Vector2(SHIP_CENTER_X, SHIP_CENTER_Y);
            Vector2 originSatellite = new Vector2(32, 32);

            Vector2 s1Pos = new Vector2(
                (float)(fShipX + SHIP_SATELLITE_RADIUS * Math.Sin( fShipSatelliteAngle)),
                (float)(fShipY - SHIP_SATELLITE_RADIUS * Math.Cos(fShipSatelliteAngle)));
            Vector2 s2Pos = new Vector2(
                (float)(fShipX + SHIP_SATELLITE_RADIUS * Math.Sin(fShipSatelliteAngle + Math.PI)),
                (float)(fShipY - SHIP_SATELLITE_RADIUS * Math.Cos(fShipSatelliteAngle + Math.PI)));


            fSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

            int maxW = fScreenWidth + 2 * SHIP_SIZE;
            int maxH = fScreenHeight + 2 * SHIP_SIZE;

            for (int x = -10; x <= 10; x++)
            {
                for (int y = -10; y <= 10; y++)
                {
                    Vector2 pos = new Vector2(fShipX + maxW * x / 10.0f, fShipY + maxH * y / 10.0f);
                    fSpriteBatch.Draw(fShipTexture, pos, sourceR, Color.Red, fShipAngle, origin, 0.5f, SpriteEffects.None, 0.0f);
                }
            }

            fSpriteBatch.End();

            RenderFire();
            RenderEnemies();

            fSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            fSpriteBatch.Draw(fShipTexture, shipPos, sourceR, Color.White, fShipAngle, origin, 1.0f, SpriteEffects.None, 0.0f);
            fSpriteBatch.Draw(fShipSatelliteTexture, s1Pos, sourceR, Color.White, fShipSatelliteAngle1, originSatellite, 1.0f, SpriteEffects.None, 0.0f);
            fSpriteBatch.Draw(fShipSatelliteTexture, s2Pos, sourceR, Color.White, fShipSatelliteAngle2, originSatellite, 1.0f, SpriteEffects.None, 0.0f);



            fSpriteBatch.End();


            base.Draw(gameTime);
        }

        #endregion

        #region BaseEngine

        public override string GetName()
        {
            return "Game Engine";
        }

        public override string GetHelp()
        {
            return @"A - Forward
LEFT - Rotate left
RIGHT - Rotate right
X - Fire
H - Halt";
        }

        public override string GetInfo()
        {
            return "";//
            //if (fBlurEnabled)
            //{
            //    return String.Format("Effect: {0}\nPen: {1}\nPen alpha: {2}\nPixels affected: {3}\nTotal amount of sand: {4}",
            //        fPens[fPenIndex].Name, fMyEffects[fSandEffectIndex].Name, fPenAlphaAmount,
            //        fDustCount, fTotalDust);
            //}
            //return String.Format("Effect: {0}\nPen: {1}\nPen alpha: {2}",
            //         fPens[fPenIndex].Name, fMyEffects[fSandEffectIndex].Name, fPenAlphaAmount);
        }

        public override string GetAbout()
        {
            return @"A simple 2D game";
        }

       

        public override void DisplayChanged()
        {
            UpdateScale();
        }

        private void UpdateScale()
        {
            fScreenWidth = GraphicsDevice.Viewport.Width;
            fScreenHeight = GraphicsDevice.Viewport.Height;

            fScreenRectangle = new Rectangle(0, 0, fScreenWidth, fScreenHeight);

            fWorkTarget = new RenderTarget2D(GraphicsDevice, fScreenWidth, fScreenHeight, false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.None,
                0,
                RenderTargetUsage.PreserveContents);

            fShipX = fScreenWidth / 2;
            fShipY = fScreenHeight / 2;
            fShipAngle = (float)Math.PI / 2.0f; ;
            fShipSatelliteAngle = 0;
            fShipSatelliteAngle1 = (float)Math.PI / 2.0f; ;
            fShipSatelliteAngle2 = (float)Math.PI / 2.0f; ;

            InitEnemies();
            fShots = new List<Shot>();
        }

        #endregion

        #region Private methods: INIT

        /// <summary>
        /// Initializes the transforms used by the game.
        /// </summary>
        private void InitializeTransform()
        {

            fWorldMatrix = Matrix.Identity;

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

            fTargetWorldMatrix = fWorldMatrix;
            fTargetViewMatrix = fViewMatrix;
            fTargetProjectionMatrix = fProjectionMatrix;

        }

        /// <summary>
        /// Initialize the app
        /// </summary>
        private void InitApp()
        {
            this.BackColor = Color.Black;

            InitializeTransform();
       
            
            fHalted = false;

        }

        /// <summary>
        /// Initialize the enemies
        /// </summary>
        private void InitEnemies()
        {
            fEnemies = new List<Enemy>();
            for (int x = 0; x < fScreenWidth; x += 48)
            {
                for (int y = 0; y < fScreenHeight; y += 48)
                {
                    Enemy enemy = new Enemy()
                    {
                        InvaderType = fRandom.Next(INVADER_TYPES),
                        Position = new Vector2(x, y),
                        Alive = true
                    };
                    fEnemies.Add(enemy);
                }
            }
        }


        #endregion

        #region Private methods: MOVE

        private void UpdateShip(GameTime gameTime)
        {
            float speed = (float)(fShipSpeed * gameTime.ElapsedGameTime.TotalMilliseconds);

            float dx = (float)(Math.Sin(fShipAngle) * speed);
            float dy = (float)(-Math.Cos(fShipAngle) * speed);

            fShipX += dx;
            fShipY += dy;

            if (fShipX < -SHIP_SIZE)
                fShipX = fShipX + fScreenWidth + 2 * SHIP_SIZE;
            else if (fShipX >= (fScreenWidth + SHIP_SIZE))
                fShipX = fShipX - fScreenWidth - 2 * SHIP_SIZE;

            if (fShipY < -SHIP_SIZE)
                fShipY = fShipY + fScreenHeight + 2 * SHIP_SIZE;
            else if (fShipY >= (fScreenHeight + SHIP_SIZE))
                fShipY = fShipY - fScreenHeight - 2 * SHIP_SIZE;

            fShipSatelliteAngle = fShipSatelliteAngle + (float)(0.0002f * gameTime.ElapsedGameTime.TotalMilliseconds);
            fShipSatelliteAngle1 = fShipSatelliteAngle1 + 0.001f + (float)(0.002f * Math.Sin(0.04f * gameTime.TotalGameTime.TotalMilliseconds));
            fShipSatelliteAngle2 = fShipSatelliteAngle2 + 0.001f + (float)(0.002f * Math.Cos(0.04f * gameTime.TotalGameTime.TotalMilliseconds));

        }

        private void CheckShotCollisions()
        {
            foreach (Shot shot in fShots)
            {
                if (!shot.Hit)
                {
                    foreach (Enemy enemy in fEnemies)
                    {
                        if (enemy.Alive && shot.FireFromShip)
                        {
                            if (CheckCollision(enemy, shot))
                            {
                                enemy.Alive = false;
                                shot.Hit = true;
                            }
                        }
                        else if (!enemy.Alive && !shot.FireFromShip)
                        {
                            if (CheckCollision(enemy, shot))
                            {
                                enemy.Alive = true;
                                shot.Hit = true;
                            }
                        }
                    }
                }
            }
        }

        private bool CheckCollision(Enemy enemy, Shot shot)
        {
            bool result = false;
            int dx = (int)(enemy.Position.X - shot.Position.X);
            int dy = (int)(enemy.Position.Y - shot.Position.Y);
            if (dx>=-15 && dx<=15 && dy>=-15 && dy<=15)
            {
                result = true;
            }
            return result;
        }

        private void UpdateFire(GameTime gameTime)
        {
            //float speed = (float)(fShipSpeed * gameTime.ElapsedGameTime.TotalMilliseconds);


            List<Shot> removeList = new List<Shot>();
            foreach (Shot shot in fShots)
            {
                if (shot.Hit)
                {
                    removeList.Add(shot);
                }
                else
                {
                    shot.Position = new Vector2(
                        (float)(shot.Position.X + gameTime.ElapsedGameTime.TotalMilliseconds * shot.Velocity.X),
                        (float)(shot.Position.Y + gameTime.ElapsedGameTime.TotalMilliseconds * shot.Velocity.Y));

                    if ((shot.Position.X < -SHOT_SIZE) ||
                        (shot.Position.X >= (fScreenWidth + 2 * SHOT_SIZE)) ||
                        (shot.Position.Y < -SHOT_SIZE) ||
                        (shot.Position.Y >= (fScreenHeight + 2 * SHOT_SIZE)))
                    {
                        removeList.Add(shot);
                    }
                }
            }
            foreach (Shot shot in removeList)
            {
                fShots.Remove(shot);
            }
        }

        private void UpdateEnemies(GameTime gameTime)
        {
        }

        #endregion

        #region Private methods: DRAW

        private void RenderFire()
        {
            fSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            foreach (Shot shot in fShots)
            {
                Vector2 shotPos = shot.Position;
                Vector2 origin = new Vector2(SHOT_CENTER_X, SHOT_CENTER_Y);
                float angle = shot.Angle;
                fSpriteBatch.Draw(fShotTexture, shotPos, null, shot.Color, angle, origin, 1.0f, SpriteEffects.None, 0.0f);
            }
            fSpriteBatch.End();
        }

        private void RenderEnemies()
        {
            fSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            foreach (Enemy enemy in fEnemies)
            {
                if (enemy.Alive)
                {
                    Vector2 pos = enemy.Position;
                    fSpriteBatch.Draw(fInvaderTextures[enemy.InvaderType], pos, Color.White);
                }
            }
            fSpriteBatch.End();
        }

        #endregion

        #region Fire


        private void TryFire(GameTime gameTime)
        {
            if (fShotTime == TimeSpan.MinValue || (gameTime.TotalGameTime - fShotTime).TotalMilliseconds > 10.0f)
            {
                fShotTime = gameTime.TotalGameTime;
                for (int i = 0; i <= fRandom.Next(10); i++)
                {
                    Fire(gameTime);
                    //FireSatellite();
                }
                //for (int i = 0; i < 1000; i++)
                //{
                //    FireOskar();
                //}                
            }
            //else
            //    FireSatellite();
        }

        private void TryFireSatellite(GameTime gameTime)
        {
            if (fShotTime1 == TimeSpan.MinValue || (gameTime.TotalGameTime - fShotTime1).TotalMilliseconds > 100.0f)
            {
                fShotTime1 = gameTime.TotalGameTime;
                FireSatellite(gameTime);
            }
        }

        private void Fire(GameTime gameTime)
        {
            Shot shot = new Shot();
            // shot.Position = new Vector2(fShipX + SHIP_FRONT_X, fShipY + SHIP_FRONT_Y);
            shot.Position = new Vector2(fShipX, fShipY);
            float angleAdjust = (float)(0.2 * (fRandom.NextDouble() - 0.5f));
            shot.Angle = fShipAngle + angleAdjust;
            shot.Hit = false;
            shot.FireFromShip = true;

            float speed = (float)(1.0f + fRandom.NextDouble());
            float dx = (float)(Math.Sin(shot.Angle) * speed);
            float dy = (float)(-Math.Cos(shot.Angle) * speed);
            shot.Velocity = new Vector2(dx, dy);
            switch (fRandom.Next(5))
            {
                case 0: shot.Color = Color.Green; break;
                case 1: shot.Color = Color.Yellow; break;
                case 2: shot.Color = Color.Red; break;
                case 3: shot.Color = Color.Purple; break;
                case 4: shot.Color = Color.Cyan; break;
            }

            fShots.Add(shot);
        }

        private void FireSatellite(GameTime gameTime)
        {
            Shot shot1 = new Shot();
            Shot shot2 = new Shot();
            Vector2 s1Pos = new Vector2(
                  (float)(fShipX + SHIP_SATELLITE_RADIUS * Math.Sin( fShipSatelliteAngle)),
                  (float)(fShipY - SHIP_SATELLITE_RADIUS * Math.Cos( fShipSatelliteAngle)));
            Vector2 s2Pos = new Vector2(
                (float)(fShipX + SHIP_SATELLITE_RADIUS * Math.Sin( fShipSatelliteAngle + Math.PI)),
                (float)(fShipY - SHIP_SATELLITE_RADIUS * Math.Cos( fShipSatelliteAngle + Math.PI)));

            shot1.Position = s1Pos;
            shot2.Position = s2Pos;
            shot1.Hit = false;
            shot2.Hit = false;
            shot1.FireFromShip = false;
            shot2.FireFromShip = false;

            float angleAdjust = (float)(0 * (fRandom.NextDouble() - 0.5f));
            shot1.Angle = (float)(fShipSatelliteAngle1 + angleAdjust);
            shot2.Angle = (float)(fShipSatelliteAngle2 + angleAdjust);

            float speed = 2.01f;// (float)(2.0f + 1.5f * Math.Cos(gameTime.TotalGameTime.TotalMilliseconds));
            float dx = (float)(Math.Sin(shot1.Angle) * speed);
            float dy = (float)(-Math.Cos(shot1.Angle) * speed);
            shot1.Velocity = new Vector2(dx, dy);
            dx = (float)(Math.Sin(shot2.Angle) * speed);
            dy = (float)(-Math.Cos(shot2.Angle) * speed);
            shot2.Velocity = new Vector2(dx, dy);

            shot1.Color = Color.Purple;
            shot2.Color = Color.Cyan;

            fShots.Add(shot1);
            fShots.Add(shot2);
        }

        private void FireOskar()
        {
            Shot shot = new Shot();
            // shot.Position = new Vector2(fShipX + SHIP_FRONT_X, fShipY + SHIP_FRONT_Y);
            shot.Position = new Vector2(500, 350);
            float angleAdjust = (float)(0.8 * (fRandom.NextDouble() - 0.5f));
            shot.Angle =  angleAdjust;

            float speed = 1.5f;
            float dx = (float)(Math.Sin(shot.Angle) * speed);
            float dy = (float)(-Math.Cos(shot.Angle) * speed);
            shot.Velocity = new Vector2(dx, dy);
            switch (fRandom.Next(2))
            {
                case 0: shot.Color = Color.DarkGreen; break;
                case 1: shot.Color = Color.Orange; break;
            }
            fShots.Add(shot);
        }

        #endregion

        #region Painting

        /// <summary>
        /// Handle mouse: paint
        /// </summary>
        /// <param name="state"></param>
        private void HandleMouse(GameTime gameTime, MouseState state)
        {
            //double duration = DURATION_BETWEEN_PAINT;
            //if (fLastPaint != null)
            //    duration = gameTime.TotalGameTime.TotalMilliseconds - fLastPaint.TotalMilliseconds;
            //if (duration < DURATION_BETWEEN_PAINT)
            //    return;

            //if (state.LeftButton == ButtonState.Pressed)
            //{
            //    fLastPaint = gameTime.TotalGameTime;
            //    PaintWithTexture(fPens[fPenIndex], state.X, state.Y, true);
            //}
            //else if (state.RightButton == ButtonState.Pressed)
            //{
            //    fLastPaint = gameTime.TotalGameTime;
            //    PaintWithTexture(fPens[fPenIndex], state.X, state.Y, false);
            //}
        }


        /// <summary>
        /// Create a width x height texture
        /// </summary>
        /// <returns></returns>
        private Texture2D CreateTexture(int width, int height, Color color)
        {
            Texture2D texture = new Texture2D(GraphicsDevice, width, height, false, SurfaceFormat.Color);

            Color[] colorArray = new Color[width * height];
            for (int i = 0; i < colorArray.Length; i++)
            {
                colorArray[i] = color;
            }
            texture.SetData(colorArray);
            return texture;
        }

        #endregion

    }
}
