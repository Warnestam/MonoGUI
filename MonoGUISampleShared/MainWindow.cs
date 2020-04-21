using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

using MonoGUI.Controls;
using MonoGUI.Engine;
namespace MonoGUISampleShared
{

    public class MainWindow : GuiWindow
    {

        private List<GuiCanvasChild> fMyBoxes;
        private GuiLabel fMyLabel1;
        private GuiLabel fMyLabel2;
        private GuiBox fMyBox1;
        private GuiBox fMyBox2;
        private GuiEngine fEngine;

        public MainWindow(GuiEngine engine)
        {
            fEngine = engine;
            InitOther();
            InitWindow();
        }

        public override void Update(GameTime gameTime)
        {
            double f = gameTime.TotalGameTime.TotalMilliseconds / 3000;

            int size1 = (int)(1 + 25 + 25 * System.Math.Sin(2 * System.Math.PI * gameTime.TotalGameTime.TotalMilliseconds / 3000));
            int size2 = (int)(1 + 100 + 100 * System.Math.Sin(2 * System.Math.PI * gameTime.TotalGameTime.TotalMilliseconds / 5000));
            int size3 = (int)(1 + 100 + 100 * System.Math.Sin(2 * System.Math.PI * gameTime.TotalGameTime.TotalMilliseconds / 11000));

            fMyLabel1.Text = $"fps {fEngine.FPS.FrameRate}";
            fMyLabel2.Text = new string('.', size1);
            fMyBox1.Width = fMyBox1.Height = size2;
            fMyBox2.Width = fMyBox2.Height = size3;
          
            //double boxAngle = System.Math.Sin(2 * System.Math.PI * gameTime.TotalGameTime.TotalMilliseconds / 9000);
            double boxAngle = gameTime.TotalGameTime.TotalMilliseconds / 300;
            int radius = 100;
            float alpha = 1.0f;
            foreach (var box in fMyBoxes)
            {
                alpha = alpha * 0.9f;
                box.X = 100 + (int)(radius * System.Math.Cos(boxAngle));
                box.Y = 100 + (int)(radius * System.Math.Sin(boxAngle));
                radius -= 2;
                boxAngle += 0.25;
                box.Control.BackgroundColor = new Color(Color.White, alpha);
            }
        }

        private void InitOther()
        {
            fMyLabel1 = new GuiLabel()
            {
                Name = "My Label 1",
                BackgroundColor = Color.LightPink,
                Text = "XXX",
                Margin = new GuiThickness(0),
            };
            fMyLabel2 = new GuiLabel()
            {
                Name = "My Label 2",
                BackgroundColor = Color.LightPink,
                Text = "XXX",
                Margin = new GuiThickness(0),
            };
            fMyBox1 = new GuiBox()
            {
                Name = "My Box 1",
                Width = 20,
                Height = 20,
                Margin = new GuiThickness(3),
                BackgroundColor = Color.White
            };
            fMyBox2 = new GuiBox()
            {
                Name = "My Box 2",
                Width = 20,
                Height = 20,
                Margin = new GuiThickness(3),
                BackgroundColor = Color.White
            };
            fMyBoxes = new List<GuiCanvasChild>();
            for (int i = 0; i < 50; i++)
            {
                fMyBoxes.Add(new GuiCanvasChild()
                {
                    X = 0,
                    Y = 0,
                    Control = new GuiBox() { Width = 10, Height = 10, BackgroundColor = Color.FloralWhite }
                });
            }
        }

        private void InitWindow()
        {
            Title = new GuiDockChild()
            {
                Dock = GuiDock.Top,
                Control = new GuiLabel() { Text = "My Window 1" }
            };
            Border = new GuiBorder() { Border = new GuiThickness(5), BorderColor = Color.White };

            Margin = new GuiThickness(0);
            BackgroundColor = new Color(Color.Green, 0.5f);
            Name = "W1";
            X = 0;
            Y = 0;
            Width = 500;
            Height = 400; 
            Content = new GuiPanel()
            {
                Name = "Panel 1",
                Margin = new GuiThickness(60),
                BackgroundColor = Color.Orange,
                Content = new GuiStackPanel()
                {
                    Name = "Stack",
                    Margin = new GuiThickness(15),
                    BackgroundColor = Color.DarkSlateGray,
                    Orientation = GuiStackPanelOrientation.Horizontal,
                    Childs = new List<GuiStackChild>
                        {
                            new GuiStackChild(){Control=new GuiBox()
                            {
                                Name = "PNL Box 1",
                                BackgroundColor=Color.Black,
                                Width=100,
                                Height=100
                            } },
                            new GuiStackChild(){Control=new GuiCanvasPanel()
                            {
                                Name = "PNL Canvas",
                                BackgroundColor=Color.DarkBlue,
                                Childs = fMyBoxes
                            } },
                            new GuiStackChild(){Control=new GuiLabel()
                            {
                                Name = "PNL Clipped Label",
                                BackgroundColor=Color.DarkBlue,
                                Text = "This should be a clipped label",
                                HorizontalAlignment=GuiHorizontalAlignment.Right,
                                VerticalAlignment = GuiVerticalAlignment.Top,
                                Width=50,
                                Height=5
                            } },
                            new GuiStackChild(){Control=new GuiBox()
                            {
                                Name = "PNL Box 2",
                                BackgroundColor=Color.Gray,
                                Width=100,
                                Height=100
                            } },
                            new GuiStackChild(){Control=new GuiDockPanel()
                            {
                                Name = "PNL Dock",
                                BackgroundColor = Color.Black,
                                LastChildFill = true,
                                Childs=new List<GuiDockChild>()
                                {
                                    new GuiDockChild(){Dock=GuiDock.Left, Control = new GuiLabel(){Text="Left", BackgroundColor=Color.LightPink}},
                                    new GuiDockChild(){Dock=GuiDock.Top, Control = new GuiLabel(){Text="Top", BackgroundColor=Color.LightGreen}},
                                    new GuiDockChild(){Dock=GuiDock.Right, Control = new GuiLabel(){Text="Right", BackgroundColor=Color.LightYellow}},
                                    new GuiDockChild(){Dock=GuiDock.Bottom, Control = new GuiLabel(){Text="Bottom", BackgroundColor=Color.LightSkyBlue}},
                                    new GuiDockChild(){Control = new GuiLabel(){Text="LAST", BackgroundColor=Color.White}},
                                }
                            } },
                            new GuiStackChild(){Control=fMyLabel1 },
                            new GuiStackChild(){Control=new GuiBox()
                            {
                                Name = "PNL Box 3",
                                BackgroundColor =Color.Red,
                                Width=20,
                                Height=20
                            } },
                            new GuiStackChild(){Control=fMyLabel2 },
                            new GuiStackChild(){Control=new GuiStackPanel()
                            {
                                Name = "Another Stacker",
                                Orientation=GuiStackPanelOrientation.Horizontal,
                                BackgroundColor=Color.DarkRed,
                                Margin=new GuiThickness(10),
                                Childs =new List<GuiStackChild>()
                                {
                                    new GuiStackChild()
                                    {
                                        Control=new GuiBox() {Name = "Box A1",Width=20,Height=20,Margin=new GuiThickness(3),BackgroundColor=Color.Black}
                                    },
                                    new GuiStackChild()
                                    {
                                        Control=new GuiBox()
                                        {
                                            Name = "Box A1",Width=30,Height=30,Margin=new GuiThickness(3),BackgroundColor=Color.Red
                                        }
                                    },
                                    new GuiStackChild()
                                    {
                                        Control=new GuiBox()
                                        {
                                            Name = "Box A1",Width=20,Height=20,Margin=new GuiThickness(3),BackgroundColor=Color.Green
                                        }
                                    },
                                    new GuiStackChild()
                                    {
                                        Control=new GuiBox()
                                        {
                                            Name = "Box A1",Width=20,Height=20,Margin=new GuiThickness(3),BackgroundColor=Color.Blue
                                        }
                                    },
                                    new GuiStackChild(){Control=fMyBox1 }
                                }
                            } },
                            new GuiStackChild(){Control=new GuiStackPanel()
                            {
                                Name = "Another Stacker",
                                Orientation=GuiStackPanelOrientation.Vertical,
                                BackgroundColor=Color.DarkBlue,
                                Margin=new GuiThickness(10),
                                HorizontalAlignment = GuiHorizontalAlignment.Center,
                                VerticalAlignment = GuiVerticalAlignment.Center,
                                Childs=new List<GuiStackChild>()
                                {
                                    new GuiStackChild(){ Control=new GuiBox()
                                    {
                                        Name = "Box A1",Width=20,Height=20,Margin=new GuiThickness(3),BackgroundColor=Color.Black
                                    }},
                                    new GuiStackChild(){ Control=new GuiBox()
                                    {
                                        Name = "Box A1",Width=30,Height=30,Margin=new GuiThickness(3),BackgroundColor=Color.Red
                                    }},
                                    new GuiStackChild(){ Control=new GuiBox()
                                    {
                                        Name = "Box A1",Width=20,Height=20,Margin=new GuiThickness(3),BackgroundColor=Color.Green
                                    }},
                                    new GuiStackChild(){ Control=new GuiBox()
                                    {
                                        Name = "Box A1",Width=20,Height=20,Margin=new GuiThickness(3),BackgroundColor=Color.Blue
                                    }},
                                    new GuiStackChild(){ Control=fMyBox2}
                                }
                            } },
                            new GuiStackChild(){Control=new GuiBox()
                            {
                                Name = "PNL Box 4",
                                BackgroundColor =Color.Red,
                                Width=20,
                                Height=20
                            } },
                            new GuiStackChild(){Control=new GuiBox()
                            {
                                Name = "PNL Box 5",
                                BackgroundColor =Color.Yellow,
                                Width=20,
                                Height=20
                            } }
                        }
                }
            };
        }


    }

}
