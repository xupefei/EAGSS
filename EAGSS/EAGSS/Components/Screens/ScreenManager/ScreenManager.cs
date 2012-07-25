using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EAGSS
{
    public class ScreenManager : DrawableGameComponent
    {
        private readonly InputState inputState = new InputState();
        private readonly List<GameScreen> screens = new List<GameScreen>();
        private readonly List<GameScreen> screensToUpdate = new List<GameScreen>();

        private BitmapFont bitmapFont;
        private ContentLoader content;
        private bool isInitialized;
        private SpriteBatch spriteBatch;
        private Effect transitionEffect;
        private Texture2D transitionShader;

        public ScreenManager(EAGSS game)
            : base(game)
        {
            Game = game;
        }

        public new EAGSS Game { get; set; }

        /// <summary>
        /// 默认的 Shader
        /// </summary>
        public Texture2D TransitionShader
        {
            get { return transitionShader; }
        }

        /// <summary>
        /// 渐变 Effect
        /// </summary>
        public Effect TransitionEffect
        {
            get { return transitionEffect; }
        }

        /// <summary>
        /// 各 Screen 共用的 SpriteBatch
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        /// <summary>
        /// 各 Screen 共用的 BitmapFont
        /// </summary>
        public BitmapFont BitmapFont
        {
            get { return bitmapFont; }
        }

        /// <summary>
        /// 各 Screen 共用的 SpriteBatch
        /// </summary>
        public ContentLoader Content
        {
            get { return content; }
        }

        public InputState InputState
        {
            get { return inputState; }
        }

        public override void Initialize()
        {
            base.Initialize();

            isInitialized = true;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            content = Game.Content;

            bitmapFont = new BitmapFont(Game, "fonts\\FZHei_17_Bold.xml");
            bitmapFont.Reset(Game.GraphicsDevice);

            transitionEffect = content.Load<Effect>("effects\\disappear.fxc");
            transitionShader = content.Load<Texture2D>("shaders\\default.png");

            // Tell each of the screens to load their content.
            foreach (GameScreen screen in screens)
            {
                screen.InitializeControls();
                screen.LoadContent();
            }
        }

        protected override void UnloadContent()
        {
            // Tell each of the screens to unload their content.
            foreach (GameScreen screen in screens)
            {
                screen.UnloadContent();
            }
        }

        public override void Update(GameTime gameTime)
        {
            // update input status
            inputState.Update();

            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others.
            screensToUpdate.Clear();

            foreach (GameScreen screen in screens)
                screensToUpdate.Add(screen);

            bool coveredByOtherScreen = false;

            // Loop as long as there are screens waiting to be updated.
            while (screensToUpdate.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                GameScreen screen = screensToUpdate[screensToUpdate.Count - 1];

                screensToUpdate.RemoveAt(screensToUpdate.Count - 1);

                // Update the screen.
                screen.Update(gameTime, coveredByOtherScreen);

                if (screen.ScreenState == ScreenState.TransitionOn ||
                    screen.ScreenState == ScreenState.Active)
                {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if (!coveredByOtherScreen)
                    {
                        if (Game.IsActive)
                            screen.HandleInput(inputState);
                    }

                    // If this is an active non-popup, inform any subsequent
                    // screens that they are covered by it.
                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (GameScreen screen in screens)
            {
                screen.Draw(gameTime);
            }
        }

        /// <summary>
        /// Adds a new screen to the screen manager.
        /// </summary>
        public void AddScreen(GameScreen screen)
        {
            screen.ScreenManager = this;

            // If we have a graphics device, tell the screen to load content.
            if (isInitialized)
            {
                screen.InitializeControls();
                screen.LoadContent();
            }

            screens.Add(screen);
        }

        /// <summary>
        /// Removes a screen from the screen manager instantly
        /// </summary>
        public void RemoveScreen(GameScreen screen)
        {
            screens.Remove(screen);
            screensToUpdate.Remove(screen);
        }
    }
}