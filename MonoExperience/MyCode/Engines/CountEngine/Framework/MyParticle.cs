using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// RW 2019-03-10


namespace MonoExperience.Engines.CountEngine
{

    class MyParticle
    {

        private MyParticles fParent;

        private Vector2 fInnerDestination;
        private Vector2 fInnerSource;
        public Vector2 fInnerPosition;
        public Vector2 fInnerDelta;
        public double fInnerCurrentTime;
        public int fInnerEndTime;

        public Vector2 DrawPosition { get; private set; }

        public Vector2 Position { get; set; }
        public Vector2 Destination { get; set; }
        public Vector2 Velocity { get; private set; }


        public MyParticle(MyParticles parent)
        {
            fParent = parent;
            GetNewInnerDestination();
        }

        public MyParticle(MyParticles parent, Vector2 position)
        {
            fParent = parent;
            GetNewInnerDestination();
            Position = position;
            Destination = position;
        }

        public void Update(double milliseconds)
        {
            // Inner movement
            if ((fParent.Settings & ParticleSettings.Shaky) != 0)
            {
                fInnerCurrentTime += milliseconds;
                if (fInnerCurrentTime >= fInnerEndTime)
                {
                    GetNewInnerDestination();
                }
                else
                {
                    float factor = Convert.ToSingle(fInnerCurrentTime) / fInnerEndTime;// 0..1
                    //factor = Convert.ToSingle(Math.Sin(factor * Math.PI));//0..1
                    fInnerPosition.X = fInnerSource.X + fInnerDelta.X * factor;
                    fInnerPosition.Y = fInnerSource.Y + fInnerDelta.Y * factor;
                }
            }

            // Outer movement

            // https://www.red3d.com/cwr/steer/gdc99/

            float timeFactor = (float)milliseconds / 400.0f;
            float slowingDistance = 500.0f;
            float maxSpeed = 10000.0f;
            Vector2 targetOffset = Destination - Position;
            float distance = targetOffset.Length();
            float rampedSpeed = maxSpeed * (distance / slowingDistance);
            float clippedSpeed = Math.Min(rampedSpeed, maxSpeed);
            Vector2 desiredVelocity = (clippedSpeed / distance) * targetOffset;
            if (float.IsNaN(desiredVelocity.X) || float.IsNaN(desiredVelocity.Y))
            {
            }
            else
            {
                Vector2 steering = desiredVelocity - Velocity;
                Vector2 steeringMS = steering * timeFactor;
                Velocity += steeringMS;
                //Console.WriteLine($"Steer {steering.X};{steering.Y} Velocity {Velocity.X};{Velocity.Y} Position {Position.X};{Position.Y}");

                if (distance < 300.0f)
                {
                    // Remove to soft
                    //Velocity = new Vector2();
                    Velocity = Velocity * (0.95f - timeFactor);
                }


                /*
                if (distance < 2.0f)
                {
                    Velocity = new Vector2();
                }
                else
                {
                    Vector2 steering = desiredVelocity - Velocity;
                    Vector2 steeringMS = steering * timeFactor;
                    Velocity += steeringMS;
                }*/
            }

            Position += (Velocity * timeFactor);

            // Final position
            DrawPosition = Position + fInnerPosition;
        }

        private float GetCurrentStoppingDistance(float velocity, float acceleration)
        {
            return (velocity / acceleration) * velocity * 0.5f;
        }



        private void GetNewInnerDestination()
        {
            double angle = fParent.Random.NextDouble() * Math.PI * 2.0f;
            double radius = 10;// fParent.Random.Next(50);
            fInnerDestination.X = Convert.ToSingle(Math.Cos(angle) * radius);
            fInnerDestination.Y = Convert.ToSingle(Math.Sin(angle) * radius);

            fInnerSource = fInnerPosition;
            fInnerEndTime = 20 + fParent.Random.Next(500);
            fInnerDelta = new Vector2(
                fInnerDestination.X - fInnerSource.X,
                fInnerDestination.Y - fInnerSource.Y);
            fInnerCurrentTime = 0;
        }

        public int GetRandom(int max)
        {
            return fParent.Random.Next(-max, max);
        }




    }

}
