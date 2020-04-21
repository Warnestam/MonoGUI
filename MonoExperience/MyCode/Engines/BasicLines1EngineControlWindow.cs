using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

using MonoGUI.Controls;
using MonoGUI.Engine;

namespace MonoExperience
{

    public class BasicLines1EngineControlWindow : GuiWindow
    {
        private GuiEngine fGuiEngine;
        private BasicLines1Engine fEngine;
        private GuiLabel fInfo;
        private GuiLabel fButtonReset;

        public BasicLines1EngineControlWindow(GuiEngine guiEngine, BasicLines1Engine engine)
        {
            fGuiEngine = guiEngine;
            fEngine = engine;
            InitOther();
            InitWindow();

            fButtonReset.OnClick += FButtonReset_OnClick;
        }

        private void FButtonReset_OnClick(object sender, EventArgs e)
        {
            fEngine.Reset();
        }

        

        public override void Update(GameTime gameTime)
        {
            fInfo.Text = $"TEST";
        }

        private void InitOther()
        {
            fInfo = new GuiLabel();            
            fButtonReset = new GuiLabel() { BackgroundColor = Color.Orange, Text = "Reset" };
        }

        private void InitWindow()
        {
            Dragable = true;
            Clickable = true;
            Title = new GuiDockChild()
            {
                Dock = GuiDock.Top,
                Control = new GuiLabel()
                {
                    Text = "BasicLines1",
                    BackgroundColor = Color.Black,
                    ForegroundColor = Color.Yellow,

                }
            };
            Border = new GuiBorder() { Border = new GuiThickness(15)  };
            HorizontalAlignment = GuiHorizontalAlignment.Left;
            //VerticalAlignment = GuiVerticalAlignment.Top,
            WindowState = GuiWindowState.Normal;
            X = 300;
            Y = 100;
            //Width = 600;
            //Height = 500;

            Content = new GuiPanel()
            {
                Margin = new GuiThickness(0),
                Content = new GuiStackPanel()
                {
                    Childs = new List<GuiStackChild>()
                    {
                        new GuiStackChild()
                        {                            
                            Control = new GuiBorder()
                            {
                                Border=new GuiThickness(2),
                                BorderColor=Color.Black,
                                Content=new GuiStackPanel()
                                {
                                    Childs = new List<GuiStackChild>()
                                    {
                                        new GuiStackChild(){Control =fInfo }
                                    },
                                    HorizontalAlignment = GuiHorizontalAlignment.Center,
                                    VerticalAlignment = GuiVerticalAlignment.Center,
                                    BackgroundColor = new Color(Color.White, 1f),
                                    Orientation=GuiStackPanelOrientation.Vertical
                                }
                            }                            
                        },
                        new GuiStackChild()
                        {
                            Control = new GuiBorder()
                            {
                                Border=new GuiThickness(2),
                                BorderColor=Color.Black,
                                Content = new GuiStackPanel()
                                {
                                    Childs = new List<GuiStackChild>()
                                    {
                                        new GuiStackChild(){Control = fButtonReset}
                                    },
                                    HorizontalAlignment = GuiHorizontalAlignment.Center,
                                    VerticalAlignment = GuiVerticalAlignment.Center,
                                    BackgroundColor = new Color(Color.Yellow, 1f),
                                    Orientation=GuiStackPanelOrientation.Vertical
                                }
                            }
                        }
                    },
                    HorizontalAlignment = GuiHorizontalAlignment.Center,
                    VerticalAlignment = GuiVerticalAlignment.Center,
                    BackgroundColor = new Color(Color.LightGreen, 1f),
                    Orientation=GuiStackPanelOrientation.Horizontal
                },
                BackgroundColor = new Color(Color.Red, 0.7f),
            };

        }

        private void WindowTitle_OnClick(object sender, System.EventArgs e)
        {
        }

    }

}
