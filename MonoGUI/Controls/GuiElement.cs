using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;

using MonoGUI.Graphics;
using MonoGUI.Engine;


/*
 * File:		GuiElement
 * Purpose:		The base element for all other GUI controls
 * 
 * Author(s):	RW: Robert Warnestam
 * 
 * History:		2018-11-14  RW  Created
 * 
 */
namespace MonoGUI.Controls
{

    /// <summary>
    /// The base element for all other GUI controls
    /// </summary>
    public class GuiElement
    {
        
        #region Private members

        private GuiSize fRenderSize = new GuiSize();
        private GuiSize fDesiredSize;
        private GuiSize fFinalSize;
        private GuiPoint fOffset;
        private GuiPoint fDrawPosition;
        private GuiPoint fContainerPosition;

        private bool fMeasureValid = false;
        private bool fArrangeValid = false;
        private int? fWidth;
        private int? fHeight;

        #endregion

        #region Public Properties

        public GuiEngine Engine { get; private set; }
        public GraphicsDevice Device { get; private set; }
        public GuiElement Parent { get; set; }
        public GuiVisibility Visibility { get; set; } = GuiVisibility.Visible;
        public GuiThickness Margin { get; set; }
        public GuiHorizontalAlignment HorizontalAlignment { get; set; } = GuiHorizontalAlignment.Stretch;
        public GuiVerticalAlignment VerticalAlignment { get; set; } = GuiVerticalAlignment.Stretch;
        public Color BackgroundColor { get; set; } = Color.Transparent;
        public object Tag { get; set; }

        /// <summary>
        /// Get the actual render size
        /// </summary>
        public GuiSize RenderSize
        {
            get
            {
                if (this.Visibility == GuiVisibility.Collapsed)
                    return new GuiSize();
                else
                    return fRenderSize;
            }
            private set
            {
                fRenderSize = value;
            }
        }

        /// <summary>
        /// Returns the size the element computed during the Measure pass.
        /// This is only valid if IsMeasureValid is true.
        /// </summary>
        public GuiSize DesiredSize
        {
            get
            {
                if (this.Visibility == GuiVisibility.Collapsed)
                    return new GuiSize(0, 0);
                else
                    return fDesiredSize;
            }
        }

        private GuiSize UnclippedDesiredSize { get; set; }

        public int? Width
        {
            get => fWidth;
            set
            {
                if (fWidth != value)
                {
                    fWidth = value;
                    InvalidateMeasure();
                }
            }
        }

        public int? Height
        {
            get => fHeight;
            set
            {
                if (fHeight != value)
                {
                    fHeight = value;
                    InvalidateMeasure();
                }
            }
        }

        public int MinWidth { get; set; } = 0;
        public int MinHeight { get; set; } = 0;

        public int MaxWidth { get; set; } = int.MaxValue;
        public int MaxHeight { get; set; } = int.MaxValue;

        public GuiSize FinalSize
        {
            get
            {
                return fFinalSize;
            }
        }

        public GuiPoint Offset
        {
            get
            {
                return fOffset;
            }
        }

        public GuiPoint DrawPosition
        {
            get
            {
                return fDrawPosition;
            }
        }
        
        public bool MeasureValid { get => fMeasureValid; }

        public bool ArrangeValid { get => fArrangeValid; }

        public string Name { get; set; }

        #endregion

        #region Private properties

        private bool IsClipped { get; set; }

        #endregion

        #region Monogame Methods
        

        public void Draw(SpriteBatch spriteBatch, GuiPoint point, Rectangle clipRect)
        {           
            fDrawPosition = point;
            if (this.Visibility == GuiVisibility.Visible)
            {
                bool useClipping = true;// IsClipped;

                if (useClipping)
                {
                    Rectangle r2 = GetClipRectangle(fFinalSize);
                    Rectangle r = new Rectangle(
                        point.X + Offset.X + r2.X,
                        point.Y + Offset.Y + r2.Y,
                        r2.Width,
                        r2.Height);

                    clipRect = Rectangle.Intersect(clipRect, r);
                    spriteBatch.GraphicsDevice.ScissorRectangle = clipRect;

                    DrawCore(spriteBatch, point, clipRect);
                }
                else
                {
                    DrawCore(spriteBatch, point, clipRect);
                }
            }
        }

        private Rectangle GetClipRectangle(GuiSize layoutSlotSize)
        {
            Rectangle result = new Rectangle();

            bool ClipToBounds = true;

            //......
            GuiMinMax mm = new GuiMinMax(this);
            GuiSize inkSize = this.RenderSize;

            int maxWidthClip = (mm.maxWidth == Int32.MaxValue) ? inkSize.Width : mm.maxWidth;
            int maxHeightClip = (mm.maxHeight == Int32.MaxValue) ? inkSize.Height : mm.maxHeight;


            //need to clip because the computed sizes exceed MaxWidth/MaxHeight/Width/Height
            bool needToClipLocally =
                 ClipToBounds //need to clip at bounds even if inkSize is less then maxSize
              || maxWidthClip < inkSize.Width
              || maxHeightClip < inkSize.Height;

            //now lets say we already clipped by MaxWidth/MaxHeight, lets see if further clipping is needed
            inkSize.Width = Math.Min(inkSize.Width, mm.maxWidth);
            inkSize.Height = Math.Min(inkSize.Height, mm.maxHeight);

            GuiThickness margin = Margin;
            int marginWidth = margin.Left + margin.Right;
            int marginHeight = margin.Top + margin.Bottom;

            GuiSize clippingSize = new GuiSize(Math.Max(0, layoutSlotSize.Width - marginWidth),
                                         Math.Max(0, layoutSlotSize.Height - marginHeight));

            bool needToClipSlot = (
                 ClipToBounds //forces clip at layout slot bounds even if reported sizes are ok
              || clippingSize.Width < inkSize.Width
              || clippingSize.Height < inkSize.Height);

            if (needToClipLocally && !needToClipSlot)
            {
                Rectangle clipRect = new Rectangle(0, 0, maxWidthClip, maxHeightClip);
                result = clipRect;
            }


            if (needToClipSlot)
            {
                GuiPoint offset = ComputeAlignmentOffset(clippingSize, inkSize);
                GuiRect slotRect = new GuiRect(-offset.X,
                                         -offset.Y,
                                          clippingSize.Width,
                                          clippingSize.Height);
                if (needToClipLocally) //intersect 2 rects
                {
                    GuiRect localRect = new GuiRect(0, 0, maxWidthClip, maxHeightClip);
                    slotRect.Intersect(localRect);
                }
                result = new Rectangle(slotRect.X, slotRect.Y, slotRect.Width, slotRect.Height);

            }

            return result;
        }

        protected void DrawCore(SpriteBatch spriteBatch, GuiPoint point, Rectangle clipRect)
        {
            DoDraw(spriteBatch, point, clipRect);
        }

        #endregion

        #region Measure - Calculate desired size

        public void InvalidateMeasure()
        {
            if (fMeasureValid)
            {
                fMeasureValid = false;
                fArrangeValid = false;
                DoInvalidateMeasure();

                GuiElement element = this.Parent;
                if (element != null)
                    element.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Calculate the desired size
        /// </summary>
        public void Measure(GuiSize availableSize)
        {
            GuiSize desiredSize = new GuiSize(0, 0);
            desiredSize = MeasureCore(availableSize);
            fMeasureValid = true;
            fDesiredSize = desiredSize;
        }

        protected GuiSize MeasureCore(GuiSize parentSize)
        {
            GuiThickness margin = Margin;

            GuiSize frameworkAvailableSize = new GuiSize(
                Math.Max(parentSize.Width - margin.Width, 0),
                Math.Max(parentSize.Height - margin.Height, 0));

            GuiMinMax mm = new GuiMinMax(this);

            frameworkAvailableSize.Width = Math.Max(mm.minWidth, Math.Min(frameworkAvailableSize.Width, mm.maxWidth));
            frameworkAvailableSize.Height = Math.Max(mm.minHeight, Math.Min(frameworkAvailableSize.Height, mm.maxHeight));

            GuiSize desiredSize = DoMeasure(frameworkAvailableSize);

            //  maximize desiredSize with user provided min size
            desiredSize = new GuiSize(
                Math.Max(desiredSize.Width, mm.minWidth),
                Math.Max(desiredSize.Height, mm.minHeight));

            UnclippedDesiredSize = desiredSize;

            if (desiredSize.Width > mm.maxWidth)
            {
                desiredSize.Width = mm.maxWidth;
            }

            if (desiredSize.Height > mm.maxHeight)
            {
                desiredSize.Height = mm.maxHeight;
            }


            int clippedDesiredWidth = desiredSize.Width + margin.Width;
            int clippedDesiredHeight = desiredSize.Height + margin.Height;

            if (clippedDesiredWidth > parentSize.Width)
            {
                clippedDesiredWidth = parentSize.Width;
            }
            if (clippedDesiredHeight > parentSize.Height)
            {
                clippedDesiredHeight = parentSize.Height;
            }

            return new GuiSize(Math.Max(0, clippedDesiredWidth), Math.Max(0, clippedDesiredHeight));
        }

        protected virtual GuiSize DoMeasure(GuiSize availableSize)
        {
            return new GuiSize(0, 0);
        }

        #endregion

        #region Arange - Arrange content

        public void Arrange(GuiSize finalSize)
        {
            if (this.Visibility == GuiVisibility.Collapsed)
            {
                fFinalSize = finalSize;
                return;
            }
            if (!fMeasureValid)
            {
                Measure(finalSize);
            }

            if (!fArrangeValid)
            {
                fFinalSize = finalSize;
                ArrangeCore(finalSize); //This has to update RenderSize
                fArrangeValid = true;
            }
        }

        protected virtual void ArrangeCore(GuiSize finalSize)
        {
            GuiSize arrangeSize = finalSize;// finalRect.Size;
            GuiThickness margin = Margin;
            arrangeSize.Width = Math.Max(0, arrangeSize.Width - margin.Width);
            arrangeSize.Height = Math.Max(0, arrangeSize.Height - margin.Height);

            bool clip = false;
            // Next, compare against unclipped, transformed size.
            GuiSize unclippedDesiredSize = UnclippedDesiredSize;
          
            if (arrangeSize.Width < unclippedDesiredSize.Width)
            {
                arrangeSize.Width = unclippedDesiredSize.Width;
                clip = true;
            }

            if (arrangeSize.Height < unclippedDesiredSize.Height)
            {
                arrangeSize.Height = unclippedDesiredSize.Height;
                clip = true;
            }

            // Alignment==Stretch --> arrange at the slot size minus margins
            // Alignment!=Stretch --> arrange at the unclippedDesiredSize
            if (HorizontalAlignment != GuiHorizontalAlignment.Stretch)
            {
                arrangeSize.Width = unclippedDesiredSize.Width;
            }

            if (VerticalAlignment != GuiVerticalAlignment.Stretch)
            {
                arrangeSize.Height = unclippedDesiredSize.Height;
            }


            GuiMinMax mm = new GuiMinMax(this);
            //we have to choose max between UnclippedDesiredSize and Max here, because
            //otherwise setting of max property could cause arrange at less then unclippedDS.
            //Clipping by Max is needed to limit stretch here
            int effectiveMaxWidth = Math.Max(unclippedDesiredSize.Width, mm.maxWidth);
            if (effectiveMaxWidth < arrangeSize.Width)
            {
                clip = true;
                arrangeSize.Width = effectiveMaxWidth;
            }

            int effectiveMaxHeight = Math.Max(unclippedDesiredSize.Height, mm.maxHeight);
            if (effectiveMaxHeight < arrangeSize.Height)
            {
                clip = true;
                arrangeSize.Height = effectiveMaxHeight;
            }


            GuiSize oldRenderSize = RenderSize;
            GuiSize innerInkSize = DoArrange(arrangeSize);
            //iSize innerInkSize = DoArrange(arrangeSize);

            //Here we use un-clipped InkSize because element does not know that it is
            //clipped by layout system and it shoudl have as much space to render as
            //it returned from its own ArrangeOverride
            RenderSize = innerInkSize;

            //clippedInkSize differs from InkSize only what MaxWidth/Height explicitly clip the
            //otherwise good arrangement. For ex, DS<clientSize but DS>MaxWidth - in this
            //case we should initiate clip at MaxWidth and only show Top-Left portion
            //of the element limited by Max properties. It is Top-left because in case when we
            //are clipped by container we also degrade to Top-Left, so we are consistent.
            GuiSize clippedInkSize = new GuiSize(Math.Min(innerInkSize.Width, mm.maxWidth),
                                           Math.Min(innerInkSize.Height, mm.maxHeight));


            //remember we have to clip if Max properties limit the inkSize
            clip |=
                    clippedInkSize.Width < innerInkSize.Width
                || clippedInkSize.Height < innerInkSize.Height;



            //Note that inkSize now can be bigger then layoutSlotSize-margin (because of layout
            //squeeze by the parent or LayoutConstrained=true, which clips desired size in Measure).

            // The client size is the size of layout slot decreased by margins.
            // This is the "window" through which we see the content of the child.
            // Alignments position ink of the child in this "window".
            // Max with 0 is neccessary because layout slot may be smaller then unclipped desired size.
            GuiSize clientSize = new GuiSize(Math.Max(0, finalSize.Width - margin.Width),
                                    Math.Max(0, finalSize.Height - margin.Height));


            //remember we have to clip if clientSize limits the inkSize
            clip |=
                    clientSize.Width < clippedInkSize.Width
                || clientSize.Height < clippedInkSize.Height;

            GuiPoint offset = ComputeAlignmentOffset(clientSize, clippedInkSize);

            //offset.X += finalRect.X + margin.Left;
            //offset.Y += finalRect.Y + margin.Top;
            offset.X += margin.Left;
            offset.Y += margin.Top;

            fOffset = offset;

            IsClipped = true;
        }

        protected virtual GuiSize DoArrange(GuiSize finalSize)
        {
            return finalSize;
        }

        #endregion

        #region OnClick

        public event EventHandler<EventArgs> OnClick;

        protected internal bool DoOnClick()
        {
            bool handled = false;
            if (OnClick == null)
            {
                if (Parent != null)
                {
                    handled = Parent.DoOnClick();
                }
            }
            else
            {
                OnClick(this, new EventArgs());
                handled = true;
            }
            return handled;
        }

        protected internal bool DoHasClickableElement()
        {
            bool handled = false;
            if (OnClick == null)
            {
                if (Parent != null)
                {
                    handled = Parent.DoHasClickableElement();
                }
            }
            else
            {
                handled = true;
            }
            return handled;
        }

        #endregion

        #region Private methods

        private GuiPoint ComputeAlignmentOffset(GuiSize clientSize, GuiSize inkSize)
        {
            GuiPoint offset = new GuiPoint();

            GuiHorizontalAlignment ha = HorizontalAlignment;
            GuiVerticalAlignment va = VerticalAlignment;

            //this is to degenerate Stretch to Top-Left in case when clipping is about to occur
            //if we need it to be Center instead, simply remove these 2 ifs
            if (ha == GuiHorizontalAlignment.Stretch
                && inkSize.Width > clientSize.Width)
            {
                ha = GuiHorizontalAlignment.Left;
            }

            if (va == GuiVerticalAlignment.Stretch
                && inkSize.Height > clientSize.Height)
            {
                va = GuiVerticalAlignment.Top;
            }
            //end of degeneration of Stretch to Top-Left

            if (ha == GuiHorizontalAlignment.Center
                || ha == GuiHorizontalAlignment.Stretch)
            {
                offset.X = (clientSize.Width - inkSize.Width) / 2;
            }
            else if (ha == GuiHorizontalAlignment.Right)
            {
                offset.X = clientSize.Width - inkSize.Width;
            }
            else
            {
                offset.X = 0;
            }

            if (va == GuiVerticalAlignment.Center
                || va == GuiVerticalAlignment.Stretch)
            {
                offset.Y = (clientSize.Height - inkSize.Height) / 2;
            }
            else if (va == GuiVerticalAlignment.Bottom)
            {
                offset.Y = clientSize.Height - inkSize.Height;
            }
            else
            {
                offset.Y = 0;
            }

            return offset;
        }

        #endregion

        #region Public methods

        public GuiElement GetTopParent()
        {
            if (this.Parent != null)
            {
                return this.Parent.GetTopParent();
            }
            else
            {
                return this;
            }
        }

        public GuiElement FindElement(Point point)
        {
            GuiElement result = null;
            if (fArrangeValid)
            {
                if (point.X >= fDrawPosition.X &&
                    point.X < (fDrawPosition.X + fFinalSize.Width) &&
                    point.Y >= fDrawPosition.Y &&
                    point.Y < (fDrawPosition.Y + fFinalSize.Height)
                    )
                {
                    result = this;
                    GuiElement subElement = DoFindElement(point);
                    if (subElement != null)
                        result = subElement;
                }
            }
            return result;
        }

        #endregion

        #region Virtual methods

        protected virtual GuiElement DoFindElement(Point point)
        {
            return null;
        }

        public virtual void Initialize(GraphicsDevice device)
        {
            Device = device;
        }

        public virtual void LoadContent(GuiEngine engine)
        {
            Engine = engine;
        }

        public virtual void UnloadContent()
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        protected virtual void DoDraw(SpriteBatch spriteBatch, GuiPoint point, Rectangle clipRect)
        {
        }

        protected virtual void DoInvalidateMeasure()
        {
        }


        #endregion

        // refactor?

        public IGuiDraggable DragElement { get; set; }

        public IGuiSizeable ResizeElement { get; set; }

    }
}