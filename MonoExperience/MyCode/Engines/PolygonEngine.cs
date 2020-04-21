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
using MonoExperience.Engines.CountEngine;
using MonoExperience.Engines.PolygonEngine;
using MonoGUI.Graphics;
using TexturePackerLoader;


/*
 * File:		PolygonEngine
 * Purpose:		Render rectangles surrounded by a polygon
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2020-04-16  RW	
 * 
 * History:		2020-04-16  RW  Created
 * 
 */
namespace MonoExperience
{

    /// <summary>
    /// Render rectangles surrounded by a polygon
    /// </summary>
    public class PolygonEngine : BaseEngine
    {

        #region Private members

        private List<MyRectangle> fRectangles;

        private double fAngle;
        private DynamicPrimitiveLine fLines;
        private BasicEffect fBasicEffect;
        private Matrix fWorldMatrix;
        private Matrix fViewMatrix;
        private Matrix fProjectionMatrix;


        private SpriteBatch fSpriteBatch;
        private Random fRandom = new Random();

   
        private Rectangle fScreenRectangle;
        private int fScreenWidth;
        private int fScreenHeight;

        private bool fHalted;

        #endregion

        #region Constructor

        /// <summary>
        /// Create the engine
        /// </summary>
        public PolygonEngine(EngineContainer cnt) : base(cnt)
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

            UpdateScale();
        }

        private void UpdateScale()
        {
            fScreenWidth = GraphicsDevice.Viewport.Width;
            fScreenHeight = GraphicsDevice.Viewport.Height;
            fScreenRectangle = new Rectangle(0, 0, fScreenWidth, fScreenHeight);

            fViewMatrix = Matrix.CreateLookAt(
              new Vector3(0.0f, 0.0f, 1.0f),
              Vector3.Zero,
              Vector3.Up
              );

            fProjectionMatrix = Matrix.CreateOrthographicOffCenter(0,fScreenWidth,fScreenHeight,0,1.0f, 1000.0f);

            fBasicEffect.View = fViewMatrix;
            fBasicEffect.Projection = fProjectionMatrix;

            fWorldMatrix = Matrix.CreateTranslation(
                    GraphicsDevice.Viewport.Width / 2f,
                    GraphicsDevice.Viewport.Height / 2f, 0);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            HandleInput(gameTime);
            base.Update(gameTime);

            if (!fHalted)
            {
                double angle = gameTime.ElapsedGameTime.TotalMilliseconds;
                fAngle = fAngle + angle * 0.001;

                bool newRectangles = fAngle > 3.1f;
                UpdateRectangles(newRectangles);
            }


        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            fBasicEffect.World = fWorldMatrix;
            foreach (EffectPass pass in fBasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                fLines.Render();
            }
            base.Draw(gameTime);
        }

        #endregion

        #region BaseEngine

        public override string GetName()
        {
            return "Polygon Engine";
        }

        public override string GetHelp()
        {
            return @"
H - Halt";
        }

        public override string GetInfo()
        {
            return String.Format("...");
        }

        public override string GetAbout()
        {
            return @"Render rectangles surrounded by a polygon";
        }

        private void HandleInput(GameTime gameTime)
        {
            if (this.Manager.KeyPressed(Keys.H))
            {
                fHalted = !fHalted;
            };

        }

        public override void DisplayChanged()
        {
            UpdateScale();
        }

        #endregion

        #region Private methods: INIT

        /// <summary>
        /// Initialize the app
        /// </summary>
        private void InitApp()
        {
            fBasicEffect = new BasicEffect(GraphicsDevice);
            fBasicEffect.VertexColorEnabled = true;
            fHalted = false;
        }


        #endregion

        private void SetPixel(Texture2D texture, int x, int y, Color c)
        {
            Rectangle r = new Rectangle(x, y, 1, 1);
            Color[] color = new Color[] { c };
            texture.SetData<Color>(0, r, color, 0, 1);
        }

        private void SetPixelsVertical(Texture2D texture, int x, int height, Color[] colors)
        {
            Rectangle r = new Rectangle(x, 0, 1, height);
            texture.SetData<Color>(0, r, colors, 0, height);
        }

        private void SetPixelsHorizontal(Texture2D texture, int y, int width, Color[] colors)
        {
            Rectangle r = new Rectangle(0, y, width, 1);
            texture.SetData<Color>(0, r, colors, 0, width);
        }

        private void UpdateRectangles(bool createNewRectangles)
        {
            if (fRectangles == null || createNewRectangles)
            {
                fRectangles = MyFactory.GetRandomRectangles();
                fAngle = 0;
            }
            else
            {
                foreach (var rectangle in fRectangles)
                    rectangle.Angle = fAngle;
            }
            int numberOfPoints = 4 * fRectangles.Count;
            MyPoint[] points = new MyPoint[numberOfPoints];
            int pointIndex = 0;
            foreach (var rectangle in fRectangles)
            {
                var corners = rectangle.CalculateCorners();
                points[pointIndex++] = corners[0];
                points[pointIndex++] = corners[1];
                points[pointIndex++] = corners[2];
                points[pointIndex++] = corners[3];
            }
            var polygon = CalculateConvexPolygon(points);

            if (fLines == null)
            {
                fLines = new DynamicPrimitiveLine(fSpriteBatch.GraphicsDevice);
                fLines.UseVertexBuffer = false;
            }
            else
            {
                fLines.Clear();
            }
            double boardArea = 0;
            foreach (var rectangle in fRectangles)
            {
                var corners = rectangle.CalculateCorners();
                fLines.AddLine(ToVertexPositionColor(corners[0], Color.White), ToVertexPositionColor(corners[1], Color.White));
                fLines.AddLine(ToVertexPositionColor(corners[1], Color.White), ToVertexPositionColor(corners[2], Color.White));
                fLines.AddLine(ToVertexPositionColor(corners[2], Color.White), ToVertexPositionColor(corners[3], Color.White));
                fLines.AddLine(ToVertexPositionColor(corners[3], Color.White), ToVertexPositionColor(corners[0], Color.White));
                boardArea += rectangle.Width * rectangle.Height;
            }

            double polyArea = 0;
            int numberOfPolyLines = polygon.Length;
            for (int i = 0; i < numberOfPolyLines; i++)
            {
                int j = (i + 1) % numberOfPolyLines;
                MyPoint p1 = polygon[i];
                MyPoint p2 = polygon[j];
                polyArea += MyPoint.Cross(p1, p2);
                fLines.AddLine(ToVertexPositionColor(p1, Color.Red), ToVertexPositionColor(p2, Color.Red));
            }
            polyArea = polyArea / 2;
            double ratio = 100.0 * boardArea / polyArea;
            //System.Console.WriteLine($"Area: {polyArea} PolyPoints: {numberOfPolyLines}, Ratio: {ratio:F1} %");

        }

        private VertexPositionColor ToVertexPositionColor(MyPoint point, Color color)
        {
            float x = (float)point.X;
            float y = (float)point.Y;
            float z = 0;
            return new VertexPositionColor(new Vector3(x, y, z), color);
        }








        static MyPoint[] CalculateConvexPolygon(MyPoint[] points)
        {
            Array.Sort(points);

            int numPoints = points.Length;
            MyPoint[] result = new MyPoint[numPoints * 2];
            int currentIndex = 0;
            int minIndex = 2;
            for (int i = 0; i < numPoints; i++)
            {
                while (currentIndex >= minIndex)
                {
                    MyPoint p1 = MyPoint.Sub(result[currentIndex - 1], result[currentIndex - 2]);
                    MyPoint p2 = MyPoint.Sub(points[i], result[currentIndex - 2]);
                    if (MyPoint.Cross(p1, p2) <= 0)
                    {
                        currentIndex--;
                    }
                    else
                    {
                        break;
                    }
                }
                result[currentIndex++] = points[i];
            }
            minIndex = currentIndex + 1;
            for (int i = numPoints - 2; i >= 0; i--)
            {
                while (currentIndex >= minIndex)
                {
                    MyPoint p1 = MyPoint.Sub(result[currentIndex - 1], result[currentIndex - 2]);
                    MyPoint p2 = MyPoint.Sub(points[i], result[currentIndex - 2]);
                    if (MyPoint.Cross(p1, p2) <= 0)
                    {
                        currentIndex--;
                    }
                    else
                    {
                        break;
                    }
                }
                result[currentIndex++] = points[i];
            }
            Array.Resize<MyPoint>(ref result, currentIndex - 1);
            return result;
        }

    }
}
