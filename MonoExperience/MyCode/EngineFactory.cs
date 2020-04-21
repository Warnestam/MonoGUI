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
using MonoGUI.GameComponents;
using MonoGUI.Graphics;

/*
 * File:		EngineFactory 
 * Purpose:		Create list of all engines
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2020-04-06  RW	
 * 
 * History:		2020-04-06  RW	Created
 * 
 * 
 */
namespace MonoExperience
{

    /// <summary>
    /// Create list of all engines
    /// </summary>
    public static class EngineFactory
    {

        public static List<BaseEngine> GetEngines(EngineContainer cnt)
        {
            return
                new List<BaseEngine>
                {
                new BasicLines1Engine(cnt),
                new BasicLines1bEngine(cnt),
                new BasicLines2Engine(cnt),
                new BasicPlaneEngine(cnt),
                new CountEngine(cnt),
                new DustEngine(cnt),
                new FamilyEngine(cnt),
                new FireworkEngine(cnt),
                new FractalPlaneEngine(cnt),
                new FractalPlaneEngine2(cnt),
                new GameEngine(cnt),
                new Gravity1Engine(cnt),
                new Gravity2Engine(cnt),
                new Gravity3Engine(cnt),
                new MyFirstModelEngine(cnt),
                new MyShaderEngine(cnt),
                new PointSprite1aEngine(cnt),
                new PointSprite1bEngine(cnt),
                new PointSprite1cEngine(cnt),
                new PointSprite2aEngine(cnt),
                new PolygonEngine(cnt),
                new RenderTargetEngine(cnt),
                new SpaceShipEngine1(cnt),
                new SpaceShipEngine2(cnt),
                new Star80Engine(cnt)
                };
        }

    }
}
