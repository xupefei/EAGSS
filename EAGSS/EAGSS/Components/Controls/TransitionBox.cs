using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EAGSS
{
    public class TransitionBox : Control
    {
        #region Delegates

        /// <summary>
        /// 显示完成事件
        /// </summary>
        public delegate void OnFinishedEvent(TransitionBox sender);

        #endregion Delegates

        private Effect transitionEffect;
        private float transitionPosition;
        private Texture2D transitionShader;

        private TimeSpan transitionTime = new TimeSpan(0, 0, 0, 1);

        public TransitionBox(APNGTexture current)
        {
            Textures.Add("current", current);
            Textures.Add("new", null);
        }

        /// <summary>
        /// 当前稳定显示的纹理
        /// </summary>
        public APNGTexture CurrentTexture
        {
            get { return Textures["current"]; }
            set { Textures["current"] = value; }
        }

        /// <summary>
        /// 新纹理
        /// </summary>
        public APNGTexture NewTexture
        {
            get { return Textures["new"]; }
            set { Textures["new"] = value; }
        }

        /// <summary>
        /// 过渡效果
        /// </summary>
        public Effect TransitionEffect
        {
            get { return transitionEffect; }
            set { transitionEffect = value; }
        }

        /// <summary>
        /// 过渡遮罩
        /// </summary>
        public Texture2D TransitionShader
        {
            get { return transitionShader; }
            set { transitionShader = value; }
        }

        /// <summary>
        /// 过渡时间
        /// </summary>
        public TimeSpan TransitionTime
        {
            get { return transitionTime; }
            set { transitionTime = value; }
        }

        public event OnFinishedEvent OnFinish;

        public override void Update(GameTime gameTime, ScreenManager screenManager)
        {
            if (!Enabled) return;

            if (Textures["new"] == null) return;

            if (!UpdateTransition(gameTime, transitionTime, 1))
            {
                Textures["current"] = Textures["new"];
                Textures["new"] = null;

                transitionPosition = 0;

                //call OnFinish event
                if (OnFinish != null)
                    OnFinish(this);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, ScreenManager screenManager)
        {
            if (!Enabled) return;

            spriteBatch.GraphicsDevice.Textures[1] = transitionShader;

            if (Textures["current"] != null)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(Textures["current"].CurrentFrame, Bounds, Color.White);
                spriteBatch.End();
            }
            if (Textures["new"] != null)
            {
                spriteBatch.Begin(0, null, null, null, null, transitionEffect);
                spriteBatch.Draw(Textures["new"].CurrentFrame, Bounds, Color.White * transitionPosition);
                spriteBatch.End();
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
    }
}