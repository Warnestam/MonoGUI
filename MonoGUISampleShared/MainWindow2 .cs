using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

using MonoGUI.Controls;
using MonoGUI.Engine;
namespace MonoGUISampleShared
{

    public class MainWindow2 : GuiWindow
    {

        private GuiEngine fEngine;

        public MainWindow2(GuiEngine engine)
        {
            fEngine = engine;
            InitOther();
            InitWindow();
        }

        public override void Update(GameTime gameTime)
        {

        }

        private void InitOther()
        {

        }

        private void InitWindow()
        {
            Title = new GuiDockChild()
            {
                Dock = GuiDock.Top,
                Control = new GuiLabel() { Text = "My Window 2" }
            };
            X = 150;
            Y = 100;
            Width = 500;
            Height = 400;
            Margin = new GuiThickness(50);
            Name = "W2";
            Content = new GuiStackPanel()
            {
                Name = "Stack",
                Margin = new GuiThickness(0),
                Orientation = GuiStackPanelOrientation.Horizontal,
                Childs = new List<GuiStackChild>
                    {
                        new GuiStackChild() {
                            Control = new GuiPanel()
                            {
                                Name = "Panel Left",
                                BackgroundColor = Color.White,
                                Width = 200,
                                HorizontalAlignment = GuiHorizontalAlignment.Left,
                                Content = new GuiStackPanel()
                                {
                                    Name = "Stack",
                                    Margin = new GuiThickness(0),
                                    Orientation = GuiStackPanelOrientation.Vertical,
                                    Childs = new List<GuiStackChild>()
                                    {
                                        new GuiStackChild(){Control=new GuiLabel(){Text="Line 1", Padding=new GuiThickness(2), BackgroundColor=Color.Yellow}},
                                        new GuiStackChild(){Control=new GuiLabel(){Text="Line 2", Padding=new GuiThickness(4), BackgroundColor=Color.Yellow}},
                                        new GuiStackChild(){Control=new GuiLabel(){Text="Line 3", Padding=new GuiThickness(6), BackgroundColor=Color.Yellow}},
                                        new GuiStackChild(){Control=new GuiLabel(){Text="Line 4", Padding=new GuiThickness(8), BackgroundColor=Color.Yellow}},
                                        new GuiStackChild(){Control=new GuiLabel(){Text="Line 5", Padding=new GuiThickness(10), BackgroundColor=Color.Yellow}},
                                        new GuiStackChild(){Control=new GuiLabel(){Text="Line 6", Padding=new GuiThickness(2), BackgroundColor=Color.Pink, HorizontalAlignment=GuiHorizontalAlignment.Left}},
                                        new GuiStackChild(){Control=new GuiLabel(){Text="Line 7", Padding=new GuiThickness(2), BackgroundColor=Color.Pink, HorizontalAlignment=GuiHorizontalAlignment.Right}},
                                        new GuiStackChild(){Control=new GuiLabel(){Text="Line 8", Padding=new GuiThickness(2), BackgroundColor=Color.Pink, HorizontalAlignment=GuiHorizontalAlignment.Center}},
                                        new GuiStackChild(){Control=new GuiLabel(){Text="Line 9", Padding=new GuiThickness(2), BackgroundColor=Color.Pink, HorizontalAlignment=GuiHorizontalAlignment.Stretch}},

                                      }
                                }
                            }
                        },
                        new GuiStackChild() {
                            Control = new GuiPanel()
                            {
                                Name = "Panel Right",
                                BackgroundColor = Color.Blue,
                                HorizontalAlignment = GuiHorizontalAlignment.Left,
                            }
                        }
                    }
            };
        }


    }

}
