using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EAGSS
{
    public class EAGSS : Game
    {
        private readonly GraphicsDeviceManager graphics;

        private readonly ScreenManager screenManager;

        public new ContentLoader Content;
        private SpriteBatch spriteBatch;

        public EAGSS()
        {
            IsMouseVisible = true;

            graphics = new GraphicsDeviceManager(this)
                           {
                               PreferredBackBufferWidth = GameSettings.WindowWidth,
                               PreferredBackBufferHeight = GameSettings.WindowHeight,
                           };

            Components.Add(new DebugScreen(this) {DrawOrder = 65535});

            // Create the screen manager component.
            screenManager = new ScreenManager(this);

            Components.Add(screenManager);

            screenManager.AddScreen(new TestScreen());
        }

        protected override void Initialize()
        {
            Content = new ContentLoader(Services);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

            //blank = Content.Load<Texture2D>(@"images\blank.png");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState()[Keys.Escape] == KeyState.Down)
                Exit();
            else if (Keyboard.GetState()[Keys.F] == KeyState.Down)
            {
                graphics.IsFullScreen = !graphics.IsFullScreen;
                graphics.ApplyChanges();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // when in VS, it'll be jaggy if you comment the following lines...
            // O SHIT CAN SB TELL ME WHY THIS HAPPENS ??
            //if (System.Diagnostics.Debugger.IsAttached)
            //{
            //    spriteBatch.Begin();
            //    spriteBatch.Draw(blank, new Vector2(-10, -10), Color.White);
            //    spriteBatch.End();
            //}

            base.Draw(gameTime);
        }
    }
}