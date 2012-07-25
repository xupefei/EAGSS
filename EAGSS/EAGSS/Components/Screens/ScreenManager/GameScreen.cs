using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EAGSS
{
    public abstract class GameScreen
    {
        private List<Control> controls = new List<Control>();
        private List<Control> controlsToUpdate = new List<Control>();
        private RenderTarget2D currentTexture;
        private bool isExiting;
        private ScreenManager screenManager;
        private ScreenState screenState = ScreenState.TransitionOn;
        private Effect transitionEffect;
        private float transitionPosition;
        private Texture2D transitionShader;
        private TimeSpan transitionTime = TimeSpan.Zero;

        /// <summary>
        /// 当前的 ScreenManager
        /// </summary>
        public ScreenManager ScreenManager
        {
            get { return screenManager; }
            internal set { screenManager = value; }
        }

        /// <summary>
        /// 渐变时间
        /// </summary>
        public TimeSpan TransitionTime
        {
            get { return transitionTime; }
            protected set { transitionTime = value; }
        }

        /// <summary>
        /// Shader
        /// </summary>
        public Texture2D TransitionShader
        {
            get { return transitionShader; }
            protected set { transitionShader = value; }
        }

        /// <summary>
        /// Effect
        /// </summary>
        public Effect TransitionEffect
        {
            get { return transitionEffect; }
            protected set { transitionEffect = value; }
        }

        /// <summary>
        /// 当前透明度：0-全透明, 1-不透明
        /// </summary>
        public float TransitionPosition
        {
            get { return transitionPosition; }
            protected set { transitionPosition = value; }
        }

        /// <summary>
        /// 当前状态
        /// </summary>
        public ScreenState ScreenState
        {
            get { return screenState; }
            protected set { screenState = value; }
        }

        /// <summary>
        /// 本窗口是否为弹出式窗口，如果是，则此窗口以下的窗口不会被自动退出
        /// </summary>
        public bool IsPopup { get; protected set; }

        public void AddControl(Control control)
        {
            control.LoadContent(screenManager.Content, screenManager);
            controls.Add(control);
        }

        /// <summary>
        /// Initialize controls of the screen.
        /// </summary>
        public virtual void InitializeControls()
        {
        }

        /// <summary>
        /// Load graphics content for the screen.
        /// </summary>
        public virtual void LoadContent()
        {
            //if (transitionShader == null)
            transitionShader = screenManager.TransitionShader;
            //if (transitionEffect == null)
            transitionEffect = screenManager.TransitionEffect;

            currentTexture = new RenderTarget2D(
                screenManager.GraphicsDevice,
                screenManager.GraphicsDevice.Viewport.Bounds.Width,
                screenManager.GraphicsDevice.Viewport.Bounds.Height);

            foreach (Control control in controls)
            {
                control.LoadContent(ScreenManager.Content, ScreenManager);
            }
        }

        /// <summary>
        /// Unload content for the screen.
        /// </summary>
        public virtual void UnloadContent()
        {
        }

        public virtual void Update(GameTime gameTime, bool coveredByOtherScreen)
        {
            // Make a copy of the master control list, to avoid confusion if
            // the process of updating one control adds or removes others.
            controlsToUpdate.Clear();

            foreach (Control c in controls)
                controlsToUpdate.Add(c);

            // Loop as long as there are controls waiting to be updated.
            while (controlsToUpdate.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                Control control = controlsToUpdate[controlsToUpdate.Count - 1];

                controlsToUpdate.RemoveAt(controlsToUpdate.Count - 1);

                control.Update(gameTime, ScreenManager);
            }

            // Draw every control onto RenderTarget2D so
            // we can apply some transition shader onto it
            screenManager.GraphicsDevice.SetRenderTarget(currentTexture);
            screenManager.GraphicsDevice.Clear(Color.Transparent);

            foreach (var control in controls)
            {
                control.Draw(gameTime, screenManager.SpriteBatch, ScreenManager);
            }

            screenManager.GraphicsDevice.SetRenderTarget(null);

            if (coveredByOtherScreen || isExiting)
            {
                // If the screen is going away to die, it should transition off.
                screenState = ScreenState.TransitionOff;

                if (!UpdateTransition(gameTime, transitionTime, -1))
                {
                    // When the transition finishes, remove the screen.
                    ScreenManager.RemoveScreen(this);
                }
            }
            else
            {
                // Otherwise the screen should transition on and become active.
                screenState = UpdateTransition(gameTime, transitionTime, 1)
                                  ? ScreenState.TransitionOn
                                  : ScreenState.Active;
            }
        }

        /// <summary>
        /// This is called when the screen should draw itself.
        /// </summary>
        public virtual void Draw(GameTime gameTime)
        {
            screenManager.SpriteBatch.GraphicsDevice.Textures[1] = transitionShader;

            screenManager.SpriteBatch.Begin(0, null, null, null, null, transitionEffect);

            screenManager.SpriteBatch.Draw(
                currentTexture,
                screenManager.GraphicsDevice.Viewport.Bounds,
                Color.White * transitionPosition);

            screenManager.SpriteBatch.End();
        }

        /// <summary>
        /// Handle input
        /// </summary>
        public virtual void HandleInput(InputState inputState)
        {
            for (int i = 0; i < controls.Count; i++)
            {
                controls[i].HandleInput(inputState, i == 0, ScreenManager);
            }
        }

        /// <summary>
        /// Helper for updating the screen transition position.
        /// direction=1 is not transparent, -1 is fully transparent.
        /// </summary>
        /// <returns>True is busy transitioning, otherwise False.</returns>
        private bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            // How much should we move by?
            float transitionDelta;

            if (time == TimeSpan.Zero)
                transitionDelta = 1;
            else
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / time.TotalMilliseconds);

            // Update the transition position.
            transitionPosition += transitionDelta * direction;

            // Did we reach the end of the transition?
            if (((direction < 0) && (transitionPosition <= 0)) ||
                ((direction > 0) && (transitionPosition >= 1)))
            {
                transitionPosition = Microsoft.Xna.Framework.MathHelper.Clamp(transitionPosition, 0, 1);
                return false;
            }

            // Otherwise we are still busy transitioning.
            return true;
        }

        /// <summary>
        /// Tells the screen to go away. Unlike ScreenManager.RemoveScreen, which
        /// instantly kills the screen, this method respects the transition timings
        /// and will give the screen a chance to gradually transition off.
        /// </summary>
        public void ExitScreen()
        {
            if (transitionTime == TimeSpan.Zero)
            {
                // If the screen has a zero transition time, remove it immediately.
                ScreenManager.RemoveScreen(this);
            }
            else
            {
                // Otherwise flag that it should transition off and then exit.
                isExiting = true;
            }
        }
    }
}