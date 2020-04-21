using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

using MonoGUI.Controls;
using MonoGUI.Engine;
namespace MonoGUISampleShared
{

    public class MainWindow3 : GuiWindow
    {

        private GuiEngine fEngine;
        private GuiLabel fMyFpsLabel;
        
        public MainWindow3(GuiEngine engine)
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

            fMyFpsLabel.Text = new string('-', size1) + $">fps {fEngine.FPS.FrameRate}<" + new string('-', size1);
        }

        private void InitOther()
        {
            fMyFpsLabel = new GuiLabel()
            {
                Name = "FPS",
                Margin = new GuiThickness(5),
                BackgroundColor = Color.DarkGreen,
                ForegroundColor = Color.LightGreen,
                HorizontalAlignment=GuiHorizontalAlignment.Center,
                VerticalAlignment = GuiVerticalAlignment.Center
            };
        }

        private void InitWindow()
        {
            Title = new GuiDockChild()
            {
                Dock = GuiDock.Bottom,
                Control = new GuiLabel() { Text = "My Window 3" }
            };
            Border = new GuiBorder() { Border = new GuiThickness(2), BorderColor = Color.Black };

            X = 300;
            Y = 200;
            Width = 500;
            Height = 400;

            Margin = new GuiThickness(0);
            Name = "W3";
            Content = new GuiTabControl()
            {
                Name = "MainTab",
                BackgroundColor = Color.White,
                Childs = new List<GuiTabControlChild>()
                    {
                       new GuiTabControlChild()
                       {
                           Header =new GuiLabel(){BackgroundColor=Color.Orange,Text="FPS", Margin=new GuiThickness(2) },
                           Control = new GuiPanel(){BackgroundColor=Color.DarkMagenta,Content = fMyFpsLabel }
                       },
                       new GuiTabControlChild()
                       {
                           Header=new GuiLabel(){BackgroundColor=Color.Orange,Text="StackPanel", Margin=new GuiThickness(2) },
                           Control=new GuiPanel()
                           {
                               Content = new GuiTabControl()
                               {
                                    Name = "SubTab",
                                    Childs = new List<GuiTabControlChild>()
                                    {
                                        new GuiTabControlChild()
                                        {
                                            Header =new GuiLabel(){BackgroundColor=Color.Orange,Text="100x100", Margin=new GuiThickness(2) },
                                            Control = new GuiStackPanel(){
                                                BackgroundColor =Color.Black,
                                                Childs =new List<GuiStackChild>()
                                                {
                                                    new GuiStackChild(){Control=new GuiBox(){BackgroundColor=Color.Yellow, Width=100,Height=100} },
                                                    new GuiStackChild(){Control=new GuiBox(){BackgroundColor=Color.Aquamarine, Width=100,Height=100} },
                                                    new GuiStackChild(){Control=new GuiBox(){BackgroundColor=Color.Blue, Width=100,Height=100} },
                                                    new GuiStackChild(){Control=new GuiBox(){BackgroundColor=Color.CadetBlue, Width=100,Height=100} }
                                                }
                                            }
                                        },
                                        new GuiTabControlChild()
                                        {
                                            Header =new GuiLabel(){BackgroundColor=Color.Orange,Text="H.Stretch", Margin=new GuiThickness(2) },
                                            Control = new GuiStackPanel(){
                                                BackgroundColor =Color.Red,
                                                Orientation=GuiStackPanelOrientation.Horizontal,
                                                VerticalAlignment=GuiVerticalAlignment.Stretch,
                                                HorizontalAlignment=GuiHorizontalAlignment.Stretch,
                                                Childs =new List<GuiStackChild>()
                                                {
                                                    new GuiStackChild(){Control=new GuiBox(){BackgroundColor=Color.Yellow, Width=100,Height=100} },
                                                    new GuiStackChild(){Control=new GuiBox(){BackgroundColor=Color.Aquamarine, Width=100,Height=100} },
                                                    new GuiStackChild(){Control=new GuiBox(){BackgroundColor=Color.Blue, Width=100,Height=100} },
                                                    new GuiStackChild(){Control=new GuiBox(){BackgroundColor=Color.CadetBlue, Width=100,Height=100} }
                                                }
                                            }
                                        },
                                        new GuiTabControlChild()
                                        {
                                            Header =new GuiLabel(){BackgroundColor=Color.Orange,Text="H.Center", Margin=new GuiThickness(2) },
                                            Control = new GuiStackPanel(){
                                                BackgroundColor =Color.Red,
                                                Name ="H.Center",
                                                Orientation=GuiStackPanelOrientation.Horizontal,
                                                VerticalAlignment=GuiVerticalAlignment.Center,
                                                HorizontalAlignment=GuiHorizontalAlignment.Center,
                                                Childs =new List<GuiStackChild>()
                                                {
                                                    new GuiStackChild(){Control=new GuiBox(){BackgroundColor=Color.Yellow, Width=100,Height=100} },
                                                    new GuiStackChild(){Control= new GuiBox(){BackgroundColor=Color.Aquamarine, Width=100,Height=100} },
                                                    new GuiStackChild(){Control=new GuiBox(){BackgroundColor=Color.Blue, Width=100,Height=100} },
                                                    new GuiStackChild(){Control=new GuiBox(){BackgroundColor=Color.CadetBlue, Width=100,Height=100} }
                                                }
                                            }
                                        }
                                        ,
                                        new GuiTabControlChild()
                                        {
                                            Header =new GuiLabel(){BackgroundColor=Color.Orange,Text="Height20", Margin=new GuiThickness(2) },
                                            Control = new GuiStackPanel(){
                                                BackgroundColor =Color.Black,
                                                Childs =new List<GuiStackChild>()
                                                {
                                                    new GuiStackChild(){Control=new GuiBox(){BackgroundColor=Color.Yellow, Height=20} },
                                                    new GuiStackChild(){Control=new GuiBox(){BackgroundColor=Color.Aquamarine, Height=20} },
                                                    new GuiStackChild(){Control=new GuiBox(){BackgroundColor=Color.Blue, Height=20} },
                                                    new GuiStackChild(){Control=new GuiBox(){BackgroundColor=Color.CadetBlue, Height=20} }
                                                }
                                            }
                                        },
                                        new GuiTabControlChild()
                                        {
                                            Header =new GuiLabel(){BackgroundColor=Color.Orange,Text="Width20", Margin=new GuiThickness(2) },
                                            Control = new GuiStackPanel(){
                                                BackgroundColor =Color.Black,
                                                Childs =new List<GuiStackChild>()
                                                {
                                                    new GuiStackChild(){Control=new GuiBox(){BackgroundColor=Color.Yellow, Width=20} },
                                                    new GuiStackChild(){Control=new GuiBox(){BackgroundColor=Color.Aquamarine, Width=20} },
                                                    new GuiStackChild(){Control=new GuiBox(){BackgroundColor=Color.Blue, Width=20} },
                                                    new GuiStackChild(){Control=new GuiBox(){BackgroundColor=Color.CadetBlue, Width=20} }
                                                }
                                            }
                                        },
                                        new GuiTabControlChild()
                                        {
                                            Header =new GuiLabel(){BackgroundColor=Color.Orange,Text="H.Alignment", Margin=new GuiThickness(2) },
                                            Control = new GuiStackPanel(){
                                                BackgroundColor =Color.Black,
                                                Childs =new List<GuiStackChild>()
                                                {
                                                    new GuiStackChild(){Control=new GuiBox(){BackgroundColor=Color.Yellow, Width=25, Height=25, HorizontalAlignment=GuiHorizontalAlignment.Right} },
                                                    new GuiStackChild(){Control=new GuiBox(){BackgroundColor=Color.Aquamarine, Width=25, Height=25, HorizontalAlignment=GuiHorizontalAlignment.Left} },
                                                    new GuiStackChild(){Control=new GuiBox(){BackgroundColor=Color.Blue, Width=25, Height=25, HorizontalAlignment=GuiHorizontalAlignment.Stretch} },
                                                    new GuiStackChild(){Control=new GuiBox(){BackgroundColor=Color.CadetBlue, Width=25, Height=25, HorizontalAlignment=GuiHorizontalAlignment.Center} }
                                                }
                                            }
                                        },
                                        new GuiTabControlChild()
                                        {
                                            Header =new GuiLabel(){BackgroundColor=Color.Orange,Text="Text", Margin=new GuiThickness(2) },
                                            Control = new GuiStackPanel(){
                                                BackgroundColor =Color.White,
                                                Childs =new List<GuiStackChild>()
                                                {
                                                    new GuiStackChild(){Control=new GuiLabel(){Width=50,Text="Text 1" }},
                                                    new GuiStackChild(){Control=new GuiLabel(){Text="Text 2" }},
                                                    new GuiStackChild(){Control=new GuiLabel(){Text="Text 3" }},
                                                    new GuiStackChild(){Control=new GuiPanel(){
                                                        BackgroundColor =Color.Black,
                                                        HorizontalAlignment =GuiHorizontalAlignment.Stretch,
                                                        VerticalAlignment =GuiVerticalAlignment.Stretch,
                                                        Content=new GuiLabel()
                                                        {
                                                            Text ="..."
                                                        }
                                                    }},
                                                    new GuiStackChild(){Control=new GuiLabel(){Text="Text 4" }},
                                                    new GuiStackChild(){Control=new GuiLabel(){Text="Text 5" }}
                                                }
                                            }
                                        },
                                        new GuiTabControlChild()
                                        {
                                            Header =new GuiLabel(){BackgroundColor=Color.Orange,Text="MY TEST", Margin=new GuiThickness(2) },
                                            Control = new GuiStackPanel(){
                                                BackgroundColor =Color.DarkSlateGray,
                                                Childs =new List<GuiStackChild>()
                                                {
                                                    new GuiStackChild(){Control=new GuiLabel(){Margin=new GuiThickness(1), BackgroundColor=Color.LightBlue, Text="Line 1" }},
                                                    new GuiStackChild(){Control=new GuiLabel(){Margin=new GuiThickness(1), BackgroundColor=Color.AliceBlue, Text="Line 2" }},
                                                    new GuiStackChild(){Control=new GuiDockPanel()
                                                    {
                                                        Margin=new GuiThickness(10),
                                                        BackgroundColor=Color.Yellow,
                                                        Childs=new List<GuiDockChild>()
                                                        {
                                                            new GuiDockChild()
                                                            {
                                                                Dock=GuiDock.Left,
                                                                Control=new GuiLabel(){BackgroundColor=Color.Orange, Text="Box A3", HorizontalAlignment=GuiHorizontalAlignment.Stretch}
                                                            }
                                                        }
                                                    }},
                                                    new GuiStackChild(){Control=new GuiBox(){Margin=new GuiThickness(5), BackgroundColor=Color.Red, Width=20,Height=20}},
                                                    new GuiStackChild(){Control=new GuiLabel(){Margin=new GuiThickness(1), BackgroundColor=Color.LightBlue, Text="Line 3" }},
                                                }
                                            }
                                        }
                                    },
                                },
                           }

                       },
                       new GuiTabControlChild()
                       {
                           Header =new GuiLabel(){BackgroundColor=Color.Orange,Text="Test", Margin=new GuiThickness(2) },
                           Control = new GuiTabControl()
                           {
                                Childs=new List<GuiTabControlChild>()
                                {
                                    new GuiTabControlChild()
                                    {
                                        Header =new GuiLabel(){BackgroundColor=Color.Orange,Text="Test", Margin=new GuiThickness(2) },
                                        Control = new GuiDockPanel()
                                        {
                                            BackgroundColor=Color.Yellow,
                                            Margin=new GuiThickness(5),
                                            Childs=new List<GuiDockChild>()
                                            {
                                                new GuiDockChild()
                                                {
                                                    Dock=GuiDock.Bottom,
                                                    Control=new GuiLabel(){Text="Box A3"}
                                                }
                                            }
                                        }
                                    },
                                    new GuiTabControlChild()
                                    {
                                        Header =new GuiLabel(){BackgroundColor=Color.Orange,Text="SP1", Margin=new GuiThickness(2) },
                                        Control = new GuiStackPanel()
                                        {
                                            BackgroundColor=Color.Orange,
                                            Orientation=GuiStackPanelOrientation.Horizontal,
                                            VerticalAlignment=GuiVerticalAlignment.Center,
                                            HorizontalAlignment=GuiHorizontalAlignment.Center,
                                            Margin=new GuiThickness(5),
                                            Width=200,
                                            Childs=new List<GuiStackChild>()
                                            {
                                                new GuiStackChild(){Control=new GuiBox(){BackgroundColor=Color.Yellow,Width=50,Height=50}},
                                                new GuiStackChild(){Control=new GuiBox(){BackgroundColor=Color.Aquamarine,Width=50,Height=50}}
                                            }
                                        }
                                    },
                                    new GuiTabControlChild()
                                    {
                                        Header =new GuiLabel(){BackgroundColor=Color.Orange,Text="SP2", Margin=new GuiThickness(2) },
                                        Control = new GuiStackPanel()
                                        {
                                            BackgroundColor=Color.Orange,
                                            Orientation=GuiStackPanelOrientation.Horizontal,
                                            VerticalAlignment=GuiVerticalAlignment.Center,
                                            HorizontalAlignment=GuiHorizontalAlignment.Center,
                                            Margin=new GuiThickness(5),
                                            Childs=new List<GuiStackChild>()
                                            {
                                                new GuiStackChild(){Control=new GuiBox(){BackgroundColor=Color.Yellow,Width=50,Height=50}},
                                                new GuiStackChild(){Control=new GuiBox(){BackgroundColor=Color.Aquamarine,Width=50,Height=50}}
                                            }
                                        }
                                    } ,
                                    new GuiTabControlChild()
                                    {
                                        Header =new GuiLabel(){BackgroundColor=Color.Orange,Text="Border", Margin=new GuiThickness(2) },
                                        Control = new GuiBorder()
                                        {
                                            BackgroundColor=Color.LightBlue,
                                            BorderColor=Color.Red,
                                            Border = new GuiThickness(20,2,20,2),
                                            Margin=new GuiThickness(25),
                                            //Content = new GuiLabel(){ Text="In a border"}
                                            Content = new GuiPanel()
                                            {
                                                BackgroundColor =Color.Blue,
                                                Margin =new GuiThickness(5),

                                                Content = new GuiBorder()
                                                {
                                                    Border=new GuiThickness(5),
                                                    BorderColor=Color.Orange,
                                                    Content=new GuiLabel(){ Text="In a panel"}
                                                }
                                            }
                                        }
                                    },
                                    new GuiTabControlChild()
                                    {
                                        Header =new GuiLabel(){BackgroundColor=Color.Orange,Text="Expandable", Margin=new GuiThickness(2) },
                                        Control = new GuiStackPanel()
                                        {
                                           BackgroundColor=Color.GreenYellow,
                                           Childs=new List<GuiStackChild>()
                                           {
                                               new GuiStackChild()
                                               {
                                                   Control=new GuiLabel()
                                                   {
                                                       Text="Label first"
                                                   }
                                               },
                                               new GuiStackChild()
                                               {
                                                   Control=new GuiExpandablePanel()
                                                   {
                                                       Title = new GuiDockChild(){Dock=GuiDock.Top,Control=new GuiLabel(){ Text="Expander 1"} },
                                                       BackgroundColor=Color.LightGreen,
                                                       Border = new GuiBorder(){Border=new GuiThickness(2),BorderColor=Color.Black},
                                                       Content=new GuiLabel(){
                                                           VerticalAlignment=GuiVerticalAlignment.Top,
                                                           HorizontalAlignment=GuiHorizontalAlignment.Stretch,
                                                           Text ="Inside an expandable panel",
                                                           BackgroundColor =Color.Yellow}
                                                   }
                                               },
                                               new GuiStackChild()
                                               {
                                                   Control=new GuiExpandablePanel()
                                                   {
                                                       Title = new GuiDockChild(){Dock=GuiDock.Top,Control=new GuiLabel(){ Text="Expander 2"} },
                                                       BackgroundColor=Color.LightGray,
                                                       Border = new GuiBorder(){Border=new GuiThickness(2),BorderColor=Color.Black},
                                                       Content=new GuiLabel(){Text="Inside an expandable panel", BackgroundColor=Color.Yellow}
                                                   }
                                               },
                                               new GuiStackChild()
                                               {
                                                   Control=new GuiExpandablePanel()
                                                   {
                                                       Title = new GuiDockChild(){Dock=GuiDock.Top,Control=new GuiLabel(){ Text="Expander 3"} },
                                                       BackgroundColor=Color.LightGray,
                                                       Border = new GuiBorder(){Border=new GuiThickness(2),BorderColor=Color.Black},
                                                       Content=new GuiBox(){BackgroundColor=Color.Yellow, Width=100, Height=100}
                                                   }
                                               }
                                           }
                                        }
                                    }
                                }
                           }
                       },
                    }
            };
        }


    }

}
