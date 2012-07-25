using System;
using EAGSS.ContentCache;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EAGSS
{
    public class DebugScreen : DrawableGameComponent
    {
        private static string debugText = string.Empty;
        private static string fps;
        private static string cache;
        private readonly EAGSS game;

        private TimeSpan elapsedTime = TimeSpan.Zero;
        private BitmapFont font;
        private int frameCounter;
        private int frameRate;

        public DebugScreen(EAGSS game)
            : base(game)
        {
            this.game = game;
        }

        public static void Output(string str)
        {
            debugText = string.Format("<{0}> {1}\n{2}", DateTime.Now.ToString("HH:mm:ss"), str, debugText);
        }

        protected override void LoadContent()
        {
            font = new BitmapFont(game, "fonts\\courier.xml");
            font.Reset(GraphicsDevice);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime = TimeSpan.Zero;
                frameRate = frameCounter;
                frameCounter = 0;
                fps = string.Format("Current FPS: {0}", frameRate);

                // read cache status
                cache = string.Format(
                    "Memory usage: {0}/{1} bytes in {2} item(s)",
                    GC.GetTotalMemory(false),
                    Cache.MaxMemoryUsage,
                    Cache.CacheContainer.Count);
            }

            if (debugText.Length > 2048)
                debugText = debugText.Substring(0, 2048);
        }

        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = new SpriteBatch(GraphicsDevice);

            //calc current fps
            frameCounter++;

            spriteBatch.Begin();
            font.DrawString(
                new Vector2(5, 5), Color.White, fps + "\n" + cache + "\n" + debugText);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}