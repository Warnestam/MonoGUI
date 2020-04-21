using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

using MonoGUI.Controls;
using MonoGUI.Engine;

namespace MonoExperience
{

    public class MainWindow : GuiWindow
    {
        private GuiEngine fEngine;
        private GuiLabel fCurrentElement;
        private GuiLabel fLabelDisplayMode;
        private GuiLabel fAdapterSize;
        private GuiLabel fWindowSize;
        private GuiLabel fAspectRatio;
        private GuiLabel fFPS;
        private GuiLabel fInfo;
        private GuiLabel fGoWindow;
        private GuiLabel fGoWindowAspect;
        private GuiLabel fGoFullscreen;

        public MainWindow(GuiEngine engine)
        {
            fEngine = engine;
            InitOther();
            InitWindow();


            if (this.Title != null)
                this.Title.Control.OnClick += WindowTitle_OnClick;
            fGoWindow.OnClick += FGoWindow_OnClick;
            fGoWindowAspect.OnClick += FGoWindowAspect_OnClick;
            fGoFullscreen.OnClick += FGoFullscreen_OnClick;
        }

        private void FGoFullscreen_OnClick(object sender, EventArgs e)
        {
            fEngine.DisplayMode = GuiDisplayMode.Fullscreen;
        }

        private void FGoWindowAspect_OnClick(object sender, EventArgs e)
        {
            fEngine.DisplayMode = GuiDisplayMode.WindowKeepAspectRatio;
        }

        private void FGoWindow_OnClick(object sender, EventArgs e)
        {
            fEngine.DisplayMode = GuiDisplayMode.Window;
        }

        public override void Update(GameTime gameTime)
        {
            fLabelDisplayMode.Text = $"Mode={fEngine.DisplayMode}";
            fAdapterSize.Text = $"Adapter={fEngine.AdapterSize.Width}x{fEngine.AdapterSize.Height}";
            fWindowSize.Text = $"Window={fEngine.WindowSize.Width}x{fEngine.WindowSize.Height}";
            fAspectRatio.Text = $"AspectRatio={fEngine.AspectRatio}";
            fFPS.Text = $"FPS={fEngine.FPS.FrameRate}";

            //if (fEngine.MouseOverElement == null)
            //{
            //    fCurrentElement.Text = "...";
            //}
            //else
            //{
            //    GuiElement element = fEngine.MouseOverElement;
            //    string name = String.Empty;
            //    bool isFirst = true;

            //    while (element != null)
            //    {
            //        if (!isFirst)
            //            name = name + "/";
            //        isFirst = false;
            //        string n = element.GetType().Name;
            //        if (n.StartsWith("Gui"))
            //            name = name + n.Substring(3);
            //        else
            //            name = name + n;
            //        element = element.Parent;
            //    }
            //    fCurrentElement.Text = name;
            //}

        }

        private void InitOther()
        {
            fLabelDisplayMode = new GuiLabel();
            fAdapterSize = new GuiLabel();
            fWindowSize = new GuiLabel();
            fAspectRatio = new GuiLabel();
            fFPS = new GuiLabel();
            fInfo = new GuiLabel();
            //lblDrawMode = new GuiLabel() { HorizontalAlignment = GuiHorizontalAlignment.Left, BackgroundColor = Color.Blue, ForegroundColor = Color.Yellow };
            fCurrentElement = new GuiLabel();

            fGoWindow = new GuiLabel() { BackgroundColor = Color.Orange, Text = "Window Mode" };
            fGoWindowAspect = new GuiLabel() { BackgroundColor = Color.Orange, Text = "Keep Aspect" };
            fGoFullscreen = new GuiLabel() { BackgroundColor = Color.Orange, Text = "Fullscreen" };
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
                    Text = "Move Me!",
                    BackgroundColor = Color.Black,
                    ForegroundColor = Color.Yellow,

                }
            };
            Border = new GuiBorder()
            {
                Border = new GuiThickness(15),
                BorderColor = new Color(Color.Black, 0.3f),
            };
            //BackgroundColor = new Color(Color.Red, 0.9f),
            HorizontalAlignment = GuiHorizontalAlignment.Left;
            //VerticalAlignment = GuiVerticalAlignment.Top,
            WindowState = GuiWindowState.Normal;
            X = 100;
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
                                        new GuiStackChild(){Control = fFPS },
                                        new GuiStackChild(){Control = fLabelDisplayMode },
                                        new GuiStackChild(){Control =fAdapterSize},
                                        new GuiStackChild(){Control =fWindowSize},
                                        new GuiStackChild(){Control =fAspectRatio},
                                        new GuiStackChild(){Control =fInfo },
                                        new GuiStackChild(){Control =fCurrentElement }
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
                                        new GuiStackChild(){Control = fGoWindow },
                                        new GuiStackChild(){Control = fGoWindowAspect },
                                        new GuiStackChild(){Control = fGoFullscreen }
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
                    BackgroundColor = new Color(Color.White, 1f),
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
