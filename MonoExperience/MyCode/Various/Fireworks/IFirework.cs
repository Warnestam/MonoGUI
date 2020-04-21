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



/*
 * File:		IFireEngine
 * Purpose:		Interface for a single pices of fireworks
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2009-05-24  RW	
 * 
 * History:		2009-05-24  RW  Created
 * 
 */

namespace MonoExperience.Fireworks
{
    
    public interface IFirework
    {

        bool HasStarted();

        bool HasEnded();

        void IgniteFirework();

        void SetStartPosition(Vector2 position, Vector2 velocity);

        void Update(GameTime gameTime);

        void Draw(GameTime gameTime);

    }

}
