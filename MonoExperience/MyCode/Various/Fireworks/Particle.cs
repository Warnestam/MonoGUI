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
 * File:		Particle
 * Purpose:		A single particle used in different fireworks
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2009-05-24  RW	
 * 
 * History:		2009-05-24  RW  Created
 * 
 */

namespace MonoExperience.Fireworks
{

    class Particle
    {
        public const float GRAVITY = 10.0f;

        public Vector2 Position;
        public Vector2 Velocity;
        public Texture2D Texture;
        public DateTime StartDate;
        public Color Color;

        public Particle(Vector2 position, Vector2 velocity, Texture2D texture)
        {
            Position = position;
            Velocity = velocity; ;
            Texture = texture;
            StartDate = DateTime.Now;
        }

        public void Move(float factor)
        {
            Position.X = Position.X + 3.0f * factor * Velocity.X;
            Position.Y = Position.Y + 3.0f * factor * Velocity.Y;
            //Velocity.X = particle.Velocity.X * 0.99f;
            Velocity.Y = Velocity.Y + GRAVITY * factor;
        }

    }

}
