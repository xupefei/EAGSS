using System;
using Microsoft.Xna.Framework;

namespace EAGSS
{
    public class MathHelper
    {
        public static Vector2 MoveInCircle(GameTime gameTime, float speed)
        {
            double time = gameTime.TotalGameTime.TotalSeconds * speed;

            var x = (float)Math.Cos(time);
            var y = (float)Math.Sin(time);

            return new Vector2(x, y);
        }

        public static float RandomBetween(Random random, double min, double max)
        {
            return (float)(min + (float)random.NextDouble() * (max - min));
        }

        public static float SetPercent(float min, float max, float percent)
        {
            return (max - min) * percent + min;
        }

        public static float GetPercent(float min, float max, float current)
        {
            return (current - min) / (max - min);
        }
    }
}