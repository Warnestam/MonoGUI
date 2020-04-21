using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


/*
 * File:		TextBox
 * Purpose:		Class for rendering text with a background
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2010-07-14  RW	
 * 
 * History:		2010-07-14  RW  Created
 * 
 * FUTURE:      Replace List<string> with a TextBoxLines class + TextBoxLineClass?
 */
namespace MonoGUI.Graphics
{

    /// <summary>
    /// Class for rendering text with a background
    /// </summary>
    public class TextBox
    {

        #region Enums

        public enum TextBoxLocation { TopLeft, TopRight, BotttomLeft, BottomRight }
        public enum TextBoxMode { List, AutoWrap }

        #endregion

        #region Private membets

        private SpriteFont fFont;
        private TextBoxLocation fLocation;
        private TextBoxMode fMode;
        private List<string> fText;
        private List<string> fWrappedText;
        private int fOffsetX = 0;
        private int fOffsetY = 0;
        private int fBorderSize = 0;
        private int fPadding = 0;
        private Color fBorderColor = Color.Red;
        private Color fBackgroundColor = Color.Transparent;
        private Color fPenColor = Color.Yellow;
        private GraphicsDevice fDevice;
        private int fMaxWidth = 200;
        private bool fTextChanged;

        private Texture2D fTexture;

        // Calculated later
        private Point fPosition;
        private Rectangle fTextBoxBorder;
        private Rectangle fTextBoxPadding;
        private Rectangle fTextBoxText;
        private int fTextHeight;
        private int fTextWidth;

        #endregion

        #region Properties

        /// <summary>
        /// Get/set the font used
        /// </summary>
        public SpriteFont Font
        {
            get
            {
                return fFont;
            }
            set
            {
                if (fFont != value)
                {
                    fFont = value;
                    fTextChanged = true;
                }
            }
        }

        /// <summary>
        /// Get/set the lines of text to draw
        /// </summary>
        public List<string> Text
        {
            get
            {
                return fText;
            }
            set
            {
                if (fText != value)
                {
                    bool changed = false;
                    if (fText == null || value==null)
                        changed = true;
                    if (!changed)
                    {
                        if (fText.Count != value.Count)
                            changed = true;
                        else
                        {
                            for (int i = 0; i < fText.Count; i++)
                            {
                                if (!fText[i].Equals(value[i]))
                                {
                                    changed = true;
                                    break;
                                }
                            }
                        }
                    }
                    fText = value;
                    fTextChanged = changed;
                }
            }
        }

        /// <summary>
        /// Get/set the location of the text box
        /// </summary>
        public TextBoxLocation Location
        {
            get
            {
                return fLocation;
            }
            set
            {
                if (fLocation != value)
                {
                    fLocation = value;
                    fTextChanged = true;
                }
            }
        }

        /// <summary>
        /// Get/set the mode of the text box
        /// </summary>
        public TextBoxMode Mode
        {
            get
            {
                return fMode;
            }
            set
            {
                if (fMode != value)
                {
                    fMode = value;
                    fTextChanged = true;
                }
            }
        }

        /// <summary>
        /// Get/set the x-offset to the textbox 
        /// </summary>
        public int OffsetX
        {
            get
            {
                return fOffsetX;
            }
            set
            {
                if (fOffsetX != value)
                {
                    fOffsetX = value;
                    fTextChanged = true;
                }
            }
        }

        /// <summary>
        /// Get/set the y-offset to the textbox 
        /// </summary>
        public int OffsetY
        {
            get
            {
                return fOffsetY;
            }
            set
            {
                if (fOffsetY != value)
                {
                    fOffsetY = value;
                    fTextChanged = true;
                }
            }
        }

        /// <summary>
        /// Get/set the width of the border
        /// </summary>
        public int BorderSize
        {
            get
            {
                return fBorderSize;
            }
            set
            {
                if (fBorderSize != value)
                {
                    fBorderSize = value;
                    fTextChanged = true;
                }
            }
        }

        /// <summary>
        /// Get/set the size of the padding
        /// </summary>
        public int Padding
        {
            get
            {
                return fPadding;
            }
            set
            {
                if (fPadding != value)
                {
                    fPadding = value;
                    fTextChanged = true;
                }
            }
        }

        /// <summary>
        /// Get/set the color of the border
        /// </summary>
        public Color BorderColor
        {
            get
            {
                return fBorderColor;
            }
            set
            {
                if (fBorderColor != value)
                {
                    fBorderColor = value;
                    //fTextChanged = true;
                }
            }
        }

        /// <summary>
        /// Get/set the color of the background
        /// </summary>
        public Color BackgroundColor
        {
            get
            {
                return fBackgroundColor;
            }
            set
            {
                if (fBackgroundColor != value)
                {
                    fBackgroundColor = value;
                    //fTextChanged = true;
                }
            }
        }

        /// <summary>
        /// Get/set the color of the text
        /// </summary>
        public Color PenColor
        {
            get
            {
                return fPenColor;
            }
            set
            {
                if (fPenColor != value)
                {
                    fPenColor = value;
                    //fTextChanged = true;
                }
            }
        }

        /// <summary>
        /// Get/set the max width of the textbox 
        /// Used only in TextBoxMode.AutoWrap
        /// </summary>
        public int MaxWidth
        {
            get
            {
                return fMaxWidth;
            }
            set
            {
                if (fMaxWidth != value)
                {
                    fMaxWidth = value;
                    fTextChanged = true;
                }
            }
        }


        #endregion

        #region Constructor / destructor

        /// <summary>
        /// Creates a new textbox object.
        /// </summary>
        public TextBox(GraphicsDevice graphicsDevice, SpriteFont font)
        {
            fDevice = graphicsDevice;
            fFont = font;
            fTexture = CreateTexture(1, 1, Color.White);
        }

        /// <summary>
        /// Called when the billboard object is destroyed.
        /// </summary>
        ~TextBox()
        {
        }

        #endregion

        #region Static creators

        /// <summary>
        /// Create a textbox with x lines of text
        /// </summary>
        public static TextBox CreateTextBox(GraphicsDevice device, SpriteFont font,
            TextBoxLocation location, int borderSize, int padding, int offsetX, int offsetY,
            Color borderColor, Color backgroundColor, Color penColor)
        {
            TextBox tb = new TextBox(device, font);
            tb.Location = location;
            tb.Mode = TextBoxMode.List;
            tb.BorderSize = borderSize;
            tb.Padding = padding;
            tb.OffsetX = offsetX;
            tb.OffsetY = offsetY;
            tb.BorderColor = borderColor;
            tb.BackgroundColor = backgroundColor;
            tb.PenColor = penColor;
            return tb;
        }

        /// <summary>
        /// Create a wrapped textbox 
        /// </summary>
        public static TextBox CreateWrappedTextBox(GraphicsDevice device, SpriteFont font,
            TextBoxLocation location, int borderSize, int padding, int offsetX, int offsetY,
            Color borderColor, Color backgroundColor, Color penColor,
            int width)
        {
            TextBox tb = new TextBox(device, font);
            tb.Location = location;
            tb.Mode = TextBoxMode.AutoWrap;
            tb.BorderSize = borderSize;
            tb.Padding = padding;
            tb.OffsetX = offsetX;
            tb.OffsetY = offsetY;
            tb.BorderColor = borderColor;
            tb.BackgroundColor = backgroundColor;
            tb.PenColor = penColor;
            tb.MaxWidth = width;
            return tb;
        }

        #endregion

        #region Private/protected methods

        /// <summary>
        /// Calculate positions and dimensions
        /// </summary>
        private void Update()
        {
            if (fFont != null && fTextChanged)
            {
                fTextChanged = false;
                fTextWidth = fMaxWidth - 2 * fBorderSize - 2 * fPadding;// used in GetDisplayText...
                fWrappedText = GetDisplayText();
             
                int maxLineWidth = 0;
                int maxLineHeight = 0;
                foreach (string line in fWrappedText)
                {
                    Vector2 size = fFont.MeasureString(line);
                    if (size.X > maxLineWidth)
                        maxLineWidth = Convert.ToInt32(size.X);
                    if (size.Y > maxLineHeight)
                        maxLineHeight = Convert.ToInt32(size.Y);
                }
                fTextHeight = maxLineHeight;
                if (fMode == TextBoxMode.AutoWrap)
                    fTextWidth = fMaxWidth - 2 * fBorderSize - 2 * fPadding;
                else
                    fTextWidth = maxLineWidth;
               
                int totalTextHeight = fTextHeight * fWrappedText.Count;
                int totalTextWidth = fTextWidth;
                int paddingWidth = totalTextWidth + 2 * fPadding;
                int paddingHeight = totalTextHeight + 2 * fPadding;
                int borderWidth = paddingWidth + 2 * fBorderSize;
                int borderHeight = paddingHeight + 2 * fBorderSize;
                int screenWidth = fDevice.Viewport.Width;
                int screenHeight = fDevice.Viewport.Height;

                Point origo;
                switch (fLocation)
                {
                    case TextBoxLocation.BottomRight:
                        origo = new Point(screenWidth - borderWidth - fOffsetX, screenHeight - borderHeight - fOffsetY);
                        break;
                    case TextBoxLocation.BotttomLeft:
                        origo = new Point(fOffsetX, screenHeight - borderHeight - fOffsetY);
                        break;
                    case TextBoxLocation.TopLeft:
                        origo = new Point(fOffsetX, fOffsetY);
                        break;
                    case TextBoxLocation.TopRight:
                        origo = new Point(screenWidth - borderWidth - fOffsetX, fOffsetY);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                fPosition = origo;
                fTextBoxBorder = new Rectangle(
                    origo.X,
                    origo.Y,
                    borderWidth, borderHeight);
                fTextBoxPadding = new Rectangle(
                    origo.X + fBorderSize,
                    origo.Y + fBorderSize,
                    paddingWidth, paddingHeight);
                fTextBoxText = new Rectangle(
                    origo.X + fBorderSize + fPadding,
                    origo.Y + fBorderSize + fPadding,
                    totalTextWidth, totalTextHeight);
            }

        }

        /// <summary>
        /// Get text to be displayed
        /// </summary>
        private List<String> GetDisplayText()
        {
            List<String> outLines = new List<string>();
            foreach (string line in fText)
            {
                string[] linesInLine = line.Split('\n');
                foreach (string newLine in linesInLine)
                    outLines.Add(newLine);
            }

            if (fMode == TextBoxMode.AutoWrap)
                return GetWrappedText(outLines);
            return outLines;
        }

        /// <summary>
        /// Get text to be wrapped
        /// </summary>
        private List<String> GetWrappedText(List<String> text)
        {
            List<String> result = new List<string>();
            foreach (string line in text)
            {
                WrapLine(result, line);
            }
            return result;
        }

        /// <summary>
        /// Wrap a single line
        /// </summary>
        private void WrapLine(List<String> lines, string line)
        {
            if (fTextWidth < 32)
            {
                // To lazy for this...
                lines.Add(line);
            }
            else
            {
                Vector2 size = fFont.MeasureString(line);
                if (size.X <= fTextWidth)
                {
                    lines.Add(line);
                }
                else
                {
                    size = fFont.MeasureString(" ");
                    int spaceSize = Convert.ToInt32(size.X);
                    string[] words = line.Split(' ');
                    int[] wordWidths = new int[words.Length];
                    for (int i = 0; i < words.Length; i++)
                    {
                        size = fFont.MeasureString(words[i]);
                        wordWidths[i] = Convert.ToInt32(size.X);
                    }
                    // Try to add as many words as possible
                    int current = 0;
                    int totalWidth = 0;
                    string totalLine = String.Empty;
                    while (current < words.Length)
                    {
                        int tempWidth = totalWidth + spaceSize + wordWidths[current];
                        string tempLine;
                        if (totalWidth == 0)
                            tempLine = words[current];
                        else
                            tempLine = totalLine + " " + words[current];
                        if (tempWidth < fTextWidth)
                        {
                            totalWidth = tempWidth;
                            totalLine = tempLine;
                            current++;
                        }
                        else
                        {
                            if (totalWidth == 0)
                            {
                                // To lazy to split a word
                                lines.Add(tempLine);
                                current++;
                            }
                            else
                            {
                                lines.Add(totalLine);
                                totalWidth = 0;
                                totalLine = String.Empty;
                            }
                        }
                        bool lastWord = current >= words.Length;
                        if (lastWord && totalWidth >= 0)
                        {
                            lines.Add(totalLine);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Create a width x height texture
        /// </summary>
        /// <returns></returns>
        private Texture2D CreateTexture(int width, int height, Color color)
        {
            Texture2D texture = new Texture2D(fDevice, width, height, false, SurfaceFormat.Color);

            Color[] colorArray = new Color[width * height];
            for (int i = 0; i < colorArray.Length; i++)
            {
                colorArray[i] = color;
            }
            texture.SetData(colorArray);
            return texture;
        }

        #endregion

        #region Draw method

        /// <summary>
        /// Renders the textbox object.
        /// </summary>
        public void Render(SpriteBatch spriteBatch)
        {
            if (fTextChanged)
                Update();
            if (fFont == null)
                return;

            Vector2 position = new Vector2(fTextBoxText.X, fTextBoxText.Y);

            if (fBorderSize > 0 && fBorderColor != Color.Transparent)
            {
                Rectangle rec1 = new Rectangle(fPosition.X, fPosition.Y, fTextBoxBorder.Width, fBorderSize);
                Rectangle rec2 = new Rectangle(fPosition.X, fPosition.Y + fTextBoxBorder.Height - fBorderSize, fTextBoxBorder.Width, fBorderSize);
                Rectangle rec3 = new Rectangle(fPosition.X, fPosition.Y + fBorderSize, fBorderSize, fTextBoxPadding.Height);
                Rectangle rec4 = new Rectangle(fPosition.X + fTextBoxBorder.Width - fBorderSize, fPosition.Y + fBorderSize, fBorderSize, fTextBoxPadding.Height);
                spriteBatch.Draw(fTexture, rec1, fBorderColor);
                spriteBatch.Draw(fTexture, rec2, fBorderColor);
                spriteBatch.Draw(fTexture, rec3, fBorderColor);
                spriteBatch.Draw(fTexture, rec4, fBorderColor);
            }
            if (fBackgroundColor != Color.Transparent)
            {
                spriteBatch.Draw(fTexture, fTextBoxPadding, fBackgroundColor);
            }
            foreach (string line in fWrappedText)
            {
                spriteBatch.DrawString(fFont, line, position, fPenColor);
                position.Y += fTextHeight;
            }
        }

        public void Invalidate()
        {
            fTextChanged = true;
         }
                #endregion

    }


}
