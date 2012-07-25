using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EAGSS
{
    public class FallingBox : Control
    {
        private readonly FallingElement[] fallingElements;

        public FallingBox(int elementCount, float mindeltaX, float mindeltaY,
                          float maxdeltaX, float maxdeltaY, float deltaAngle, bool scale,
                          Rectangle bounds, params APNGTexture[] textures)
        {
            Bounds = bounds;

            int temp = 0;
            Textures = textures.ToDictionary(texture => temp++.ToString());

            fallingElements = new FallingElement[elementCount];

            var random = new Random((int)DateTime.UtcNow.Ticks * fallingElements.Length);
            for (int i = 0; i < elementCount; i++)
            {
                fallingElements[i].TextureIndex = random.Next(0, textures.Length);
                fallingElements[i].StartPostion =
                    new Vector2(
                        MathHelper.RandomBetween(random, Bounds.X, Bounds.Width),
                        -Textures[fallingElements[i].TextureIndex.ToString()].CurrentFrame.Height);
                fallingElements[i].CurrentPostion =
                    new Vector2(
                        MathHelper.RandomBetween(random, Bounds.X, Bounds.Width),
                        -Textures[fallingElements[i].TextureIndex.ToString()].CurrentFrame.Height);
                fallingElements[i].DeltaPostion =
                    new Vector2(
                        MathHelper.RandomBetween(random, mindeltaX, maxdeltaX),
                        MathHelper.RandomBetween(random, mindeltaY, maxdeltaY));
                fallingElements[i].DeltaAngle = MathHelper.RandomBetween(random, -deltaAngle, deltaAngle);
                fallingElements[i].Scale = scale ? MathHelper.RandomBetween(random, 0.1, 1) : 1.0f;
            }
        }

        public override void LoadContent(ContentLoader content, ScreenManager screenManager)
        {
            base.LoadContent(content, screenManager);
        }

        public override void Update(GameTime gameTime, ScreenManager screenManager)
        {
            if (!Enabled) return;

            //update each element
            for (int i = 0; i < fallingElements.Length; i++)
            {
                //do we reach the end?
                if (fallingElements[i].CurrentPostion.Y
                    > Bounds.Height + Textures[fallingElements[i].TextureIndex.ToString()].CurrentFrame.Height)
                {
                    fallingElements[i].CurrentPostion = fallingElements[i].StartPostion;
                    fallingElements[i].CurrentAngle = 0;
                }

                fallingElements[i].CurrentPostion += fallingElements[i].DeltaPostion;
                fallingElements[i].CurrentAngle = (fallingElements[i].CurrentAngle
                                                   + fallingElements[i].DeltaAngle) % (2 * (float)Math.PI);
            }

            base.Update(gameTime, screenManager);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, ScreenManager screenManager)
        {
            if (!Enabled) return;

            spriteBatch.Begin();

            foreach (FallingElement element in fallingElements)
            {
                spriteBatch.Draw(
                    Textures[element.TextureIndex.ToString()].CurrentFrame,
                    element.CurrentPostion,
                    null,
                    Color.White,
                    element.CurrentAngle,
                    new Vector2(
                        Textures[element.TextureIndex.ToString()].CurrentFrame.Width / 2,
                        Textures[element.TextureIndex.ToString()].CurrentFrame.Height / 2),
                    element.Scale,
                    SpriteEffects.None,
                    0f);
            }

            spriteBatch.End();

            base.Draw(gameTime, spriteBatch, screenManager);
        }

        #region Nested type: FallingElement

        public struct FallingElement
        {
            internal float CurrentAngle;
            internal Vector2 CurrentPostion;
            internal float DeltaAngle;
            internal Vector2 DeltaPostion;
            internal float Scale;
            internal Vector2 StartPostion;
            internal int TextureIndex;
        }

        #endregion Nested type: FallingElement
    }
}