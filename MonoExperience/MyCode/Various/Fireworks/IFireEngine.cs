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
 * Purpose:		Interface for different kind of fireworks
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2009-05-24  RW	
 * 
 * History:		2009-05-24  RW  Created
 * 
 */

namespace MonoExperience.Fireworks
{
   
    public interface IFireEngine 
    {

        IFirework CreateFirework();

        void LoadContent(Game game, SpriteBatch spriteBatch);

    }

}
