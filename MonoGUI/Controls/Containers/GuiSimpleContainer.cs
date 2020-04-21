using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGUI.Graphics;
using MonoGUI.GameComponents;
using MonoGUI.Engine;
using MonoGUI.Themes;


//TODO ta bort GuiStackPanel ersätter


namespace MonoGUI.Controls
{

    public class GuiSimpleContainer : GuiContainerControl
    {
        
        public override void Initialize(GraphicsDevice device)
        {
            base.Initialize(device);
            if (Childs != null)
            {
                foreach (var child in Childs)
                {
                    child.HorizontalAlignment = GuiHorizontalAlignment.Stretch;
                    child.VerticalAlignment = GuiVerticalAlignment.Stretch;
                    child.Initialize(device);
                }
            }
        }

        public override void LoadContent(GuiEngine engine)
        {
            base.LoadContent(engine);
            if (Childs != null)
            {
                foreach (var child in Childs)
                {
                    child.LoadContent(engine);
                }
            }
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            if (Childs != null)
            {
                foreach (var child in Childs)
                {
                    child.UnloadContent();
                }
            }
        }

        protected override GuiSize DoMeasure(GuiSize availableSize)
        {
            GuiSize frameworkAvailableSize = new GuiSize(availableSize.Width, availableSize.Height);
            GuiMinMax mm = new GuiMinMax(this);
            frameworkAvailableSize.Width = Math.Max(mm.minWidth, Math.Min(frameworkAvailableSize.Width, mm.maxWidth));
            frameworkAvailableSize.Height = Math.Max(mm.minHeight, Math.Min(frameworkAvailableSize.Height, mm.maxHeight));
            GuiSize desiredSize = new GuiSize(frameworkAvailableSize.Width, 0); ;
            
            if (Childs != null && Childs.Count>0)
            {
                int childLeft = Childs.Count;
                int heightLeft = availableSize.Height;
                foreach (var child in Childs)
                {
                    int childHeight;
                    if (childLeft>1)
                    {
                        childHeight = heightLeft / childLeft;
                    }
                    else
                    {
                        childHeight = heightLeft;
                    }
                    //GuiSize childSize = new GuiSize(frameworkAvailableSize.Width, 0);
                    GuiSize childSize = new GuiSize(availableSize.Width, childHeight);
                    child.Measure(childSize);
                    desiredSize.Height += child.DesiredSize.Height;
                    heightLeft -= child.DesiredSize.Height;

                    childLeft--;
                }                
            }
            

            //  maximize desiredSize with user provided min size
            desiredSize = new GuiSize(
                Math.Max(desiredSize.Width, mm.minWidth),
                Math.Max(desiredSize.Height, mm.minHeight));

            return desiredSize;
        }

        protected override GuiSize DoArrange(GuiSize arrangeSize)
        {
            GuiMinMax mm = new GuiMinMax(this);

            arrangeSize.Width = Math.Max(mm.minWidth, Math.Min(arrangeSize.Width, mm.maxWidth));
            arrangeSize.Height = Math.Max(mm.minHeight, Math.Min(arrangeSize.Height, mm.maxHeight));

            if (Childs != null && Childs.Count>0)
            {
                int offsetY = 0;
                int childLeft = Childs.Count;
                int heightLeft = arrangeSize.Height;
                foreach (var child in Childs)
                {
                    int childHeight = child.DesiredSize.Height;
                    //if (childLeft > 1)
                    //{
                    //    childHeight = heightLeft / childLeft;
                    //}
                    //else
                    //{
                    //    childHeight = heightLeft;
                    //}
                    GuiRect childRect = new GuiRect(FinalRect.X + Margin.Left, FinalRect.Y + Margin.Top + offsetY, arrangeSize.Width, childHeight);
                    child.Arrange(childRect);
                    offsetY += childHeight;
                    heightLeft -= childHeight;
                    childLeft--;
                }
            }

            return arrangeSize;
        }

        protected override void DoDraw(SpriteBatch spriteBatch)
        {
            if (Childs != null)
            {
                foreach (var child in Childs)
                {
                    child.Draw(spriteBatch);
                }
            }

        }

    }

}
