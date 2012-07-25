using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EAGSS
{
    public class FadeBox : Control
    {
        #region Delegates

        /// <summary>
        /// 显示完成事件
        /// </summary>
        public delegate void OnFinishedEvent(FadeBox sender);

        #endregion Delegates

        private TimeSpan alreadyShownTime = TimeSpan.Zero;
        private bool canInterrupt = true;

        private TransitionStatus status = TransitionStatus.NotShown;

        private TimeSpan stopTime = TimeSpan.FromMilliseconds(2000);
        private TimeSpan transitionOffTime = TimeSpan.FromMilliseconds(1000);
        private TimeSpan transitionOnTime = TimeSpan.FromMilliseconds(1000);
        private float transitionPosition;

        /// <summary>
        /// 自动显隐纹理并在完成后发出通知
        /// </summary>
        /// <param name="textures">纹理</param>
        public FadeBox(params APNGTexture[] textures)
        {
            int temp = 0;
            Textures = textures.ToDictionary(texture => temp++.ToString());
        }

        /// <summary>
        /// 显示时间
        /// </summary>
        public TimeSpan TransitionOnTime
        {
            get { return transitionOnTime; }
            set { transitionOnTime = value; }
        }

        /// <summary>
        /// 隐藏时间
        /// </summary>
        public TimeSpan TransitionOffTime
        {
            get { return transitionOffTime; }
            set { transitionOffTime = value; }
        }

        /// <summary>
        /// 完全显示的保持时间
        /// </summary>
        public TimeSpan StopTime
        {
            get { return stopTime; }
            set { stopTime = value; }
        }

        /// <summary>
        /// 可否被打断
        /// </summary>
        public bool CanInterrupt
        {
            get { return canInterrupt; }
            set { canInterrupt = value; }
        }

        public event OnFinishedEvent OnFinish;

        public override void LoadContent(ContentLoader content, ScreenManager screenManager)
        {
            base.LoadContent(content, screenManager);
        }

        public override void Update(GameTime gameTime, ScreenManager screenManager)
        {
            if (!Enabled) return;

            //control finished!
            if (status == TransitionStatus.Dead)
            {
                //call OnFinish event
                if (OnFinish != null)
                    OnFinish(this);

                return;
            }

            if (status == TransitionStatus.Dead) return;
            if (status == TransitionStatus.NotShown) status = TransitionStatus.TransitioningIn;

            //now this.status must among [FadingIn, Shown, FadingOut]
            switch (status)
            {
                case TransitionStatus.TransitioningIn:
                    if (!UpdateTransition(gameTime, transitionOnTime, 1))
                    {
                        status = TransitionStatus.Shown;
                    }
                    break;

                case TransitionStatus.Shown:
                    alreadyShownTime += gameTime.ElapsedGameTime;
                    if (stopTime != TimeSpan.Zero && alreadyShownTime > stopTime)
                    {
                        // clear alreadyShownTime for next texture
                        alreadyShownTime = TimeSpan.Zero;
                        status = TransitionStatus.TransitioningOut;
                    }
                    break;

                case TransitionStatus.TransitioningOut:
                    if (!UpdateTransition(gameTime, transitionOffTime, -1))
                    {
                        Textures.Remove(Textures.ElementAt(0).Key);

                        // do we reach the end?
                        status = Textures.Count == 0
                                     ? TransitionStatus.Dead
                                     : TransitionStatus.TransitioningIn;
                    }
                    break;

                default:
                    throw new ArgumentException("Transition status error.");
            }

            base.Update(gameTime, screenManager);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, ScreenManager screenManager)
        {
            if (!Enabled) return;

            if (Textures.Count == 0
                || status == TransitionStatus.NotShown
                || status == TransitionStatus.Dead) return;

            spriteBatch.Begin();

            spriteBatch.Draw(
                Textures.ElementAt(0).Value.CurrentFrame, Bounds, Color.White * transitionPosition);

            spriteBatch.End();

            base.Draw(gameTime, spriteBatch, screenManager);
        }

        public override void HandleInput(InputState inputState, bool isTopMost, ScreenManager screenManager)
        {
            if (!Enabled) return;

            if (!canInterrupt) return;
            if (!inputState.IsNewMouseLeftButtonReleased(Bounds)) return;

            // we only handle Mouse1
            if (status == TransitionStatus.TransitioningIn) transitionPosition = 1;
            else if (status == TransitionStatus.Shown) status = TransitionStatus.TransitioningOut;
            else if (status == TransitionStatus.TransitioningOut) transitionPosition = 0;

            base.HandleInput(inputState, isTopMost, screenManager);
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

        #region Nested type: TransitionStatus

        internal enum TransitionStatus
        {
            NotShown,
            TransitioningIn,
            Shown,
            TransitioningOut,
            Dead,
        }

        #endregion Nested type: TransitionStatus
    }
}