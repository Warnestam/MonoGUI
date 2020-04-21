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
 * File:		DustEngine
 * Purpose:		Spray/brush the surface using the mouse 
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2010-07-16  RW	
 * 
 * History:		2010-07-16  RW  Created
 *              2020-04-01  RW  Moved to MonoExperience
 * 
 */
namespace MonoExperience
{

    /// <summary>
    /// Spray/brush the surface using the mouse 
    /// </summary>
    public class DustEngine : BaseEngine
    {

        #region Constant

        // ms between two "paint"
        private const int DURATION_BETWEEN_PAINT = 20;
        private const int DURATION_BETWEEN_BLUR_EFFECT = 10;


        #endregion

        #region Classes

        internal class MyPen
        {
            public string Name { get; set; }
            public string FileName { get; set; }
            public Texture2D Texture { get; set; }
            public MyPen(string fileName, string name)
            {
                Name = name;
                FileName = fileName;
            }
        }

        internal struct PixelNeighbours
        {
            public int TextureIndex;
        }

        internal class MySandEffect
        {
            public string Name { get; set; }
            public Action Action { get; set; }

            public MySandEffect(string name, Action action)
            {
                Name = name;
                Action = action;
            }
        }

        #endregion

        #region Private members

        private List<MyPen> fPens = new List<MyPen>(){
            new MyPen("Red064","Standard brush"),
            new MyPen("Sun","Sun brush"),
            new MyPen("Heart","Heart")
        };
        private int fPenIndex = 0;

        private List<MySandEffect> fMyEffects;

        private int fSandEffectIndex = 0;

        private SpriteBatch fSpriteBatch;
        private Random fRandom = new Random();

        private Matrix fWorldMatrix;
        private Matrix fViewMatrix;
        private Matrix fProjectionMatrix;

        private Matrix fTargetWorldMatrix;
        private Matrix fTargetViewMatrix;
        private Matrix fTargetProjectionMatrix;

        private RenderTarget2D fWorkTarget;
        private RenderTarget2D fTempWorkTarget;

        private Texture2D fBackground;
        private Texture2D fSandTexture;

        private Rectangle fScreenRectangle;
        private Rectangle fSandRectangle;
        private int fScreenWidth;
        private int fScreenHeight;
        private int fSandWidth;
        private int fSandHeight;

        private TimeSpan fLastPaint;
        private TimeSpan fLastBlur;
        private Effect fDustShader;
        private BlendState fAddDustState;
        private BlendState fRemoveDustState;
        private int fDustCount;
        private int fTotalDust;
        private bool fBlurEnabled = true;
        private bool fOnlySand = false;
        private int fPenAlphaAmount = 255;

      
        #endregion

        #region Constructor

        /// <summary>
        /// Create the engine
        /// </summary>
        /// <param name="game"></param>
        public DustEngine(EngineContainer cnt) : base(cnt)
        {
            // TODO: Construct any child components here
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

            //InitializeTransform();
        }

        protected override void Dispose(bool disposing)
        {
            // TODO
            base.Dispose(disposing);
        }

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


        private void ScaleChanged()
        {
            InitializeTransform();

            fScreenWidth = GraphicsDevice.Viewport.Width;
            fScreenHeight = GraphicsDevice.Viewport.Height;
            fSandWidth = fScreenWidth / 4;
            fSandHeight = fScreenHeight / 4;

            fScreenRectangle = new Rectangle(0, 0, fScreenWidth, fScreenHeight);
            // Eg. no problems to have the sand size as big as the screen
            // just have to optimize the blur effect first...
            fSandRectangle = new Rectangle(0, 0, fSandWidth, fSandHeight);
            fTempWorkTarget = new RenderTarget2D(GraphicsDevice, fSandWidth, fSandHeight);
            fWorkTarget = new RenderTarget2D(GraphicsDevice, fSandWidth, fSandHeight, false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.None,
                0,
                RenderTargetUsage.PreserveContents);

        }

        /// <summary>
        /// Loads any component specific content
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            fSpriteBatch = new SpriteBatch(Game.GraphicsDevice);

            fBackground = Game.Content.Load<Texture2D>("Dust/Penguins");
            fSandTexture = Game.Content.Load<Texture2D>("Dust/sand");
            fDustShader = Game.Content.Load<Effect>("Dust/DustShader");            

            foreach (MyPen pen in fPens)
                pen.Texture = this.Game.Content.Load<Texture2D>(String.Format("Dust/{0}", pen.FileName));

            fAddDustState = BlendState.Additive;
            fRemoveDustState = new BlendState()
            {
                ColorSourceBlend = Blend.SourceAlpha,
                ColorDestinationBlend = Blend.One,
                ColorBlendFunction = BlendFunction.ReverseSubtract,

                AlphaSourceBlend = Blend.SourceAlpha,
                AlphaDestinationBlend = Blend.One,
                AlphaBlendFunction = BlendFunction.ReverseSubtract
            };

            fMyEffects = new List<MySandEffect>(){
                new MySandEffect("Slow blur",new Action(SlowBlur)),
                new MySandEffect("Too much",new Action(BlurTooMuch)),
                new MySandEffect("Falling down",new Action(BlurDown)),
                new MySandEffect("Falling down II",new Action(BlurDownReverse)),
                new MySandEffect("SHADER BLUR",new Action(BlurWithShader))
        };


            ScaleChanged();

        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            HandleMouse(gameTime, this.Manager.GetMouseState());
            if (this.Manager.KeyPressed(Keys.B))
            {
                fBlurEnabled = !fBlurEnabled;
            }
            else if (this.Manager.KeyPressed(Keys.S))
            {
                fOnlySand = !fOnlySand;
            }
            else if (this.Manager.KeyPressed(Keys.E))
            {
                fSandEffectIndex++;
                if (fSandEffectIndex >= fMyEffects.Count)
                    fSandEffectIndex = 0;
            }
            else if (this.Manager.KeyPressed(Keys.P))
            {
                fPenIndex++;
                if (fPenIndex >= fPens.Count)
                    fPenIndex = 0;
            }
            else if (this.Manager.KeyPressed(Keys.A))
            {
                if (fPenAlphaAmount == 255)
                    fPenAlphaAmount = 16;
                else
                    fPenAlphaAmount = 255;
            }
            else if (this.Manager.KeyPressed(Keys.D1))
            {
                ClearSand();
            }
            else if (this.Manager.KeyPressed(Keys.D2))
            {
                FillSand();
            }
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            if (fBlurEnabled)
            {
                bool doBlur = false;
                // The shader effect can be very fast!
                double duration = DURATION_BETWEEN_BLUR_EFFECT;
                if (fLastBlur != null)
                    duration = gameTime.TotalGameTime.TotalMilliseconds - fLastBlur.TotalMilliseconds;
                doBlur = duration > DURATION_BETWEEN_BLUR_EFFECT;
                if (doBlur)
                {
                    fLastBlur = gameTime.TotalGameTime;
                    fMyEffects[fSandEffectIndex].Action();
                }
            }

            Effect effect = fDustShader;
            if (fOnlySand)
                effect.CurrentTechnique = effect.Techniques["PaintDustOnly"];
            else
                effect.CurrentTechnique = effect.Techniques["PaintDust"];

            if (effect.Parameters["Background"] != null)
                effect.Parameters["Background"].SetValue(fBackground);
            if (effect.Parameters["Sand"] != null)
                effect.Parameters["Sand"].SetValue(fSandTexture);

            fSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, fDustShader);
            fSpriteBatch.Draw(fWorkTarget, fScreenRectangle, Color.White);
            fSpriteBatch.End();

            //fSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            //fSpriteBatch.Draw(fBackground, new Rectangle(0, 0, 200, 200), Color.Yellow);
            //fSpriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion

        #region BaseEngine

        public override string GetName()
        {
            return "Dust Engine";
        }

        public override string GetHelp()
        {
            return @"B - Toggle blur effect
S - Toggle only sand
E - Change effect
P - Change pen
A - Change pen alpha
1 - Clear sand
2 - Fill sand";
        }

        public override string GetInfo()
        {
            if (fBlurEnabled)
            {
                return String.Format("Effect: {0}\nPen: {1}\nPen alpha: {2}\nPixels affected: {3}\nTotal amount of sand: {4}",
                    fPens[fPenIndex].Name, fMyEffects[fSandEffectIndex].Name, fPenAlphaAmount,
                    fDustCount, fTotalDust);
            }
            return String.Format("Effect: {0}\nPen: {1}\nPen alpha: {2}",
                     fPens[fPenIndex].Name, fMyEffects[fSandEffectIndex].Name, fPenAlphaAmount);
        }

        public override string GetAbout()
        {
            return @"Interact with the surface using the mouse and clicking on the left or right mouse button";
        }


        public override void DisplayChanged()
        {
            ScaleChanged();
        }

        #endregion

        #region Painting

        /// <summary>
        /// Handle mouse: paint
        /// </summary>
        /// <param name="state"></param>
        private void HandleMouse(GameTime gameTime, MouseState state)
        {
            double duration = DURATION_BETWEEN_PAINT;
            if (fLastPaint != null)
                duration = gameTime.TotalGameTime.TotalMilliseconds - fLastPaint.TotalMilliseconds;
            if (duration < DURATION_BETWEEN_PAINT)
                return;

            Vector2 position = new Vector2(state.X, state.Y);
            Vector2 mouseScaled = position;/// fParent.ScaleVector;

            if (state.LeftButton == ButtonState.Pressed)
            {
                fLastPaint = gameTime.TotalGameTime;
                PaintWithTexture(fPens[fPenIndex], (int)mouseScaled.X, (int)mouseScaled.Y, true);
            }
            else if (state.RightButton == ButtonState.Pressed)
            {
                fLastPaint = gameTime.TotalGameTime;
                PaintWithTexture(fPens[fPenIndex], (int)mouseScaled.X, (int)mouseScaled.Y, false);
            }
        }

        /// <summary>
        /// Paint with a texture
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void PaintWithTexture(MyPen pen, int x, int y, bool addDust)
        {
            if (x >= 0 && y >= 0 && x < fScreenWidth && y < fScreenWidth)
            {
                int sx = x * fSandWidth / fScreenWidth;
                int sy = y * fSandHeight / fScreenHeight;
                RenderTargetBinding[] previousRenderTargets = GraphicsDevice.GetRenderTargets();
                GraphicsDevice.SetRenderTarget(fWorkTarget);
                // Using built in additive effect
                BlendState state;
                if (addDust)
                    state = fAddDustState;
                else
                    state = fRemoveDustState;
                fSpriteBatch.Begin(SpriteSortMode.Immediate, state);
                fSpriteBatch.Draw(pen.Texture,
                    new Vector2(sx - pen.Texture.Width / 2, sy - pen.Texture.Height / 2),
                    new Color(255, 255, 255, fPenAlphaAmount));
                fSpriteBatch.End();

                GraphicsDevice.SetRenderTargets(previousRenderTargets);
            }
        }

        /// <summary>
        /// Clear all sand
        /// </summary>
        private void ClearSand()
        {
            Texture2D texture = CreateTexture(1, 1, Color.Black);

            RenderTargetBinding[] previousRenderTargets = GraphicsDevice.GetRenderTargets();
            GraphicsDevice.SetRenderTarget(fWorkTarget);
            fSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            fSpriteBatch.Draw(texture, fSandRectangle, Color.White);
            fSpriteBatch.End();
            GraphicsDevice.SetRenderTargets(previousRenderTargets);
        }

        /// <summary>
        /// Fill with sand
        /// </summary>
        private void FillSand()
        {
            Texture2D texture = CreateTexture(1, 1, Color.Red);

            RenderTargetBinding[] previousRenderTargets = GraphicsDevice.GetRenderTargets();
            GraphicsDevice.SetRenderTarget(fWorkTarget);
            fSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            fSpriteBatch.Draw(texture, fSandRectangle, Color.White);
            fSpriteBatch.End();
            GraphicsDevice.SetRenderTargets(previousRenderTargets);
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

        #region Blur/sand effect

        /// <summary>
        /// BlurSand
        /// </summary>
        private void SlowBlur()
        {
            fDustCount = 0;
            fTotalDust = 0;

            // TODO: Could this code be moved to a shader?
            // maybe by:
            // Shader A: create a texture showing all pixels that is on a higher level of its neightbours
            // Shader B: create a texture showing all pixels that is on a lower level of its neighbours
            // Shader C: combine results...

            byte DUST_TO_MOVE = 2;
            int MIN_HEIGHT_DIFF = DUST_TO_MOVE;

            Color[] data = new Color[fSandWidth * fSandHeight];
            fWorkTarget.GetData<Color>(data);
            PixelNeighbours[] lowNeighbours = new PixelNeighbours[8];
            for (int y = 1; y < (fSandHeight - 1); y++)
            {
                int indexY = y * fSandWidth;
                for (int x = 1; x < (fSandWidth - 1); x++)
                {
                    int index = indexY + x;
                    int height = data[index].R;
                    fTotalDust += height;
                    if (height > 0)
                    {
                        // Find lowest level of sand
                        int lowHeight = 999;
                        int countNeightbours = 0;
                        for (int dy = -1; dy < 2; dy++)
                        {
                            for (int dx = -1; dx < 2; dx++)
                            {
                                if (dy != 0 && dx != 0)
                                {
                                    int currentIndex = index + dy * fSandWidth + dx;
                                    int currentHeight = data[currentIndex].R;
                                    if (currentHeight < lowHeight)
                                    {
                                        lowHeight = currentHeight;
                                        countNeightbours = 0;
                                        lowNeighbours[countNeightbours].TextureIndex = currentIndex;
                                        //lowNeighbours[countNeightbours].Height = currentHeight;
                                        countNeightbours++;
                                    }
                                    else if (currentHeight == lowHeight)
                                    {
                                        lowNeighbours[countNeightbours].TextureIndex = currentIndex;
                                        // not needed lowNeighbours[countNeightbours].Height = currentHeight;
                                        countNeightbours++;
                                    }
                                }
                            }
                        }
                        if (countNeightbours > 0)
                        {
                            if ((height - lowHeight) > MIN_HEIGHT_DIFF)
                            {
                                fDustCount++;
                                // Only if current pixel has more sand than neighbours
                                // Take a random neighbour
                                int neighbourIndex = lowNeighbours[fRandom.Next(countNeightbours)].TextureIndex;
                                if (lowHeight < (255 - DUST_TO_MOVE))
                                    data[neighbourIndex].R += DUST_TO_MOVE;
                                else
                                    data[neighbourIndex].R = 255;

                                if (height < DUST_TO_MOVE)
                                    data[index].R = 0;
                                else
                                {
                                    data[index].R -= DUST_TO_MOVE;
                                }
                            }
                        }
                    }
                }
            }
            fWorkTarget.SetData<Color>(data);
        }

        /// <summary>
        /// BlurSand
        /// </summary>
        private void BlurTooMuch()
        {
            fDustCount = 0;
            fTotalDust = 0;

            // TODO: Could this code be moved to a shader?
            // maybe by:
            // Shader A: create a texture showing all pixels that is on a higher level of its neightbours
            // Shader B: create a texture showing all pixels that is on a lower level of its neighbours
            // Shader C: combine results...

            byte DUST_TO_MOVE = 30;
            int MIN_HEIGHT_DIFF = 1;

            Color[] data = new Color[fSandWidth * fSandHeight];
            fWorkTarget.GetData<Color>(data);
            PixelNeighbours[] lowNeighbours = new PixelNeighbours[8];
            for (int y = 1; y < (fSandHeight - 1); y++)
            {
                int indexY = y * fSandWidth;
                for (int x = 1; x < (fSandWidth - 1); x++)
                {
                    int index = indexY + x;
                    int height = data[index].R;
                    fTotalDust += height;
                    if (height > 0)
                    {
                        // Find lowest level of sand
                        int lowHeight = 999;
                        int countNeightbours = 0;
                        for (int dy = -1; dy < 2; dy++)
                        {
                            for (int dx = -1; dx < 2; dx++)
                            {
                                if (dy != 0 && dx != 0)
                                {
                                    int currentIndex = index + dy * fSandWidth + dx;
                                    int currentHeight = data[currentIndex].R;
                                    if (currentHeight < lowHeight)
                                    {
                                        lowHeight = currentHeight;
                                        countNeightbours = 0;
                                        lowNeighbours[countNeightbours].TextureIndex = currentIndex;
                                        //lowNeighbours[countNeightbours].Height = currentHeight;
                                        countNeightbours++;
                                    }
                                    else if (currentHeight == lowHeight)
                                    {
                                        lowNeighbours[countNeightbours].TextureIndex = currentIndex;
                                        // not needed lowNeighbours[countNeightbours].Height = currentHeight;
                                        countNeightbours++;
                                    }
                                }
                            }
                        }
                        if (countNeightbours > 0)
                        {
                            if ((height - lowHeight) > MIN_HEIGHT_DIFF)
                            {
                                fDustCount++;
                                // Only if current pixel has more sand than neighbours
                                // Take a random neighbour
                                int neighbourIndex = lowNeighbours[fRandom.Next(countNeightbours)].TextureIndex;
                                if (lowHeight < (255 - DUST_TO_MOVE))
                                    data[neighbourIndex].R += DUST_TO_MOVE;
                                else
                                    data[neighbourIndex].R = 255;

                                if (height < DUST_TO_MOVE)
                                    data[index].R = 0;
                                else
                                {
                                    data[index].R -= DUST_TO_MOVE;
                                }
                            }
                        }
                    }
                }
            }
            fWorkTarget.SetData<Color>(data);
        }

        /// <summary>
        /// BlurSand
        /// </summary>
        private void BlurDown()
        {
            fDustCount = 0;
            fTotalDust = 0;

            Color[] data = new Color[fSandWidth * fSandHeight];
            fWorkTarget.GetData<Color>(data);
            for (int y = 1; y < (fSandHeight - 1); y++)
            {
                int indexY = y * fSandWidth;
                for (int x = 1; x < (fSandWidth - 1); x++)
                {
                    int index = indexY + x;
                    int indexBelow = index + fSandWidth;
                    int indexLeft = index - 1;
                    int indexRight = index + 1;

                    int height = data[index].R;
                    int heightBelow = data[indexBelow].R;
                    int heightLeft = data[indexLeft].R;
                    int heightRight = data[indexRight].R;

                    fTotalDust += height;
                    int delta;
                    int amount;
                    bool changed = false;
                    // Move some dust down
                    delta = 255 - heightBelow;
                    if (height < delta)
                        delta = height;
                    if (delta > 0)
                    {
                        amount = delta / 2;
                        height -= amount;
                        heightBelow += amount;
                        changed = true;
                    }
                    // Move some dust left or right
                    if (fRandom.Next(2) == 0)
                    {
                        delta = height - heightLeft;
                        if (delta > 0)
                        {
                            amount = delta / 2;
                            height -= amount;
                            heightLeft += amount;
                            changed = true;
                        }
                    }
                    else
                    {
                        delta = height - heightRight;
                        if (delta > 0)
                        {
                            amount = delta / 2;
                            height -= amount;
                            heightRight += amount;
                            changed = true;
                        }
                    }
                    if (changed)
                    {
                        fDustCount++;
                        data[index].R = (byte)height;
                        data[indexBelow].R = (byte)heightBelow;
                        data[indexLeft].R = (byte)heightLeft;
                        data[indexRight].R = (byte)heightRight;
                    }
                }
            }
            fWorkTarget.SetData<Color>(data);
        }

        /// <summary>
        /// BlurSand
        /// </summary>
        private void BlurDownReverse()
        {
            // using a temp array so that current changes does not interfere with algorithm
            fDustCount = 0;
            fTotalDust = 0;

            Color[] data = new Color[fSandWidth * fSandHeight];
            Color[] newData = new Color[fSandWidth * fSandHeight];
            int[] rowDiff = new int[fSandWidth];

            int lastIndexLine = (fSandHeight - 1) * fSandWidth;
            for (int x = 0; x < fSandWidth; x++)
                newData[lastIndexLine + x].R = 255;

            fWorkTarget.GetData<Color>(data);

            // STEP 1: Move down
            for (int y = 1; y < (fSandHeight - 1); y++)
            {
                int indexY = y * fSandWidth;
                for (int x = 0; x < fSandWidth; x++)
                {
                    int index = indexY + x;
                    int indexAbove = index - fSandWidth;
                    int indexBelow = index + fSandWidth;
                    int height = data[index].R;
                    int heightAbove = data[indexAbove].R;
                    int heightBelow = data[indexBelow].R;
                    fTotalDust += height;
                    int oldHeight = height;
                    int deltaAbove = 255 - height;
                    if (deltaAbove > heightAbove)
                        deltaAbove = heightAbove;
                    int deltaBelow = 255 - heightBelow;
                    if (deltaBelow > height)
                        deltaBelow = height;

                    height += deltaAbove;
                    height -= deltaBelow;
                    if (oldHeight != height)
                        fDustCount++;
                    newData[index].R = (byte)height;
                }
            }
            // STEP 2: Left/right
            for (int y = 0; y < fSandHeight; y++)
            {
                int indexY = y * fSandWidth;
                for (int x = 0; x < fSandWidth; x++)
                    rowDiff[x] = 0;
                for (int x = 0; x < fSandWidth; x++)
                {
                    int index = indexY + x;
                    int height = newData[index].R;

                    int totalHeight = 0;
                    int count = 0;
                    for (int dx = -1; dx < 2; dx++)
                    {
                        int ix = x + dx;
                        if (ix >= 0 && ix < fSandWidth)
                        {
                            totalHeight += newData[index + dx].R;
                            count++;
                        }
                    }
                    int avgHeight = totalHeight / count;
                    int diff = 0;
                    for (int dx = -1; dx < 2; dx++)
                    {
                        int ix = x + dx;
                        if (dx != 0 && ix >= 0 && ix < fSandWidth)
                        {
                            int currentHeight = newData[index + dx].R;
                            diff = currentHeight - avgHeight;
                            rowDiff[x + dx] -= diff / 2;// cant move all...
                            rowDiff[x] += diff / 2;
                        }
                    }
                }
                for (int x = 0; x < fSandWidth; x++)
                {
                    int index = indexY + x;
                    int height = newData[index].R;
                    int oldHeight = height;
                    height += rowDiff[x];
                    if (oldHeight != height)
                        fDustCount++;
                    //if (height < 0 || height > 255)
                    //    Console.WriteLine(String.Format("{0}: {1}, {2}", x, height, rowDiff[x]));
                    data[index].R = (byte)height;
                }
            }
            fWorkTarget.SetData<Color>(data);
        }


        /// <summary>
        /// BlurSand
        /// </summary>
        private void BlurWithShader()
        {
            // Blur the WorkTarget
            // We have to use a separate render target for this

            RenderTargetBinding[] previousRenderTargets = GraphicsDevice.GetRenderTargets();
            GraphicsDevice.SetRenderTarget(fTempWorkTarget);

            Effect effect = fDustShader;
            effect.CurrentTechnique = effect.Techniques["BlurDownEffect"];
            fSpriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.Opaque,
                SamplerState.LinearWrap,
                DepthStencilState.None,
                RasterizerState.CullNone,
                fDustShader);
            //effect.Parameters["ScreenTexture"].SetValue(fWorkTarget);// is done by Draw() method
            effect.Parameters["BlurDownFX"].SetValue(1.0f / fSandWidth);
            effect.Parameters["BlurDownFY"].SetValue(1.0f / fSandHeight);

            fSpriteBatch.Draw(fWorkTarget, fSandRectangle, Color.White);
            fSpriteBatch.End();


            // copy again
            GraphicsDevice.SetRenderTarget(fWorkTarget);

            fSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            fSpriteBatch.Draw(fTempWorkTarget, fSandRectangle, Color.White);
            fSpriteBatch.End();

            GraphicsDevice.SetRenderTargets(previousRenderTargets);


        }

        #endregion

    }
}
