using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EAGSS
{
    public class Slider : Control
    {
        #region Delegates

        /// <summary>
        /// 拖动事件
        /// </summary>
        public delegate void OnDragEvent(Slider sender);

        #endregion Delegates

        private bool hovering;
        private bool isHorizontal;
        private float thumbLocation;

        private Rectangle thumbRectangle;

        public bool IsHorizontal
        {
            get { return isHorizontal; }
            set { isHorizontal = value; }
        }

        /// <summary>
        /// Thumb location from start.
        /// </summary>
        public float ThumbLocation
        {
            get { return thumbLocation; }
            set
            {
                if (value < 0 || value > 1.0)
                    throw new Exception("Value must within 0.00 and 1.00");

                thumbLocation = (float)Math.Round(value, 2);

                if (isHorizontal)
                {
                    thumbRectangle.X =
                        (int)MathHelper.SetPercent(Bounds.Left, (Bounds.Right - thumbRectangle.Width),
                                                    thumbLocation);
                }
                else
                {
                    thumbRectangle.Y =
                        (int)MathHelper.SetPercent(Bounds.Top, (Bounds.Bottom - thumbRectangle.Height),
                                                    thumbLocation);
                }
            }
        }

        public event OnDragEvent OnDrag;

        /// <summary>
        /// 带有一个滑块的滚动条，能发出滚动事件。
        /// </summary>
        public Slider(Vector3 size, bool isHorizontal, APNGTexture thumb, APNGTexture thumbHover)
        {
            this.isHorizontal = isHorizontal;

            Textures.Add("thumb", thumb);
            Textures.Add("thumb_hover", thumbHover);

            thumbRectangle = thumb.CurrentFrame.Bounds;

            thumbRectangle.Location = new Point((int)size.X, (int)size.Y);

            if (isHorizontal)
                Bounds = new Rectangle((int)size.X, (int)size.Y, (int)size.Z, thumb.CurrentFrame.Height);
            else
                Bounds = new Rectangle((int)size.X, (int)size.Y, thumb.CurrentFrame.Width, (int)size.Z);
        }

        public override void LoadContent(ContentLoader content, ScreenManager screenManager)
        {
            base.LoadContent(content, screenManager);
        }

        public override void Update(GameTime gameTime, ScreenManager screenManager)
        {
            base.Update(gameTime, screenManager);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, ScreenManager screenManager)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(Textures["thumb_hover"].CurrentFrame, Bounds, Color.White);

            spriteBatch.Draw(
                hovering ? Textures["thumb_hover"].CurrentFrame : Textures["thumb"].CurrentFrame,
                thumbRectangle, Color.White);

            spriteBatch.End();

            base.Draw(gameTime, spriteBatch, screenManager);
        }

        public override void HandleInput(InputState inputState, bool isTopMost, ScreenManager screenManager)
        {
            hovering = inputState.IsMouseInRectangle(thumbRectangle);

            // 单击
            if (inputState.IsMouseLeftButtonPressed(Bounds) || inputState.IsNewMouseLeftButtonReleased(Bounds))
            {
                if (MoveThumbTo(new Point(inputState.CurrentMouseState.X, inputState.CurrentMouseState.Y)))
                    if (OnDrag != null)
                        OnDrag(this);
            }

            base.HandleInput(inputState, isTopMost, screenManager);
        }

        private bool MoveThumbTo(Point p)
        {
            bool moved = false;

            if (isHorizontal)
            {
                thumbRectangle.X = p.X - thumbRectangle.Width / 2;

                if (thumbRectangle.Left < Bounds.Left)
                    thumbRectangle.X = Bounds.Left;
                else if (thumbRectangle.Right > Bounds.Right)
                    thumbRectangle.X = Bounds.Right - thumbRectangle.Width;

                float newValue =
                    (float)
                    Math.Round(MathHelper.GetPercent(Bounds.Left, Bounds.Right - thumbRectangle.Width,
                                                     thumbRectangle.X), 2);

                if (ThumbLocation != newValue)
                {
                    ThumbLocation = newValue;
                    moved = true;
                }
            }
            else
            {
                thumbRectangle.Y = p.Y - thumbRectangle.Height / 2;

                if (thumbRectangle.Top < Bounds.Top)
                    thumbRectangle.Y = Bounds.Top;
                else if (thumbRectangle.Bottom > Bounds.Bottom)
                    thumbRectangle.Y = Bounds.Bottom - thumbRectangle.Height;

                float newValue =
                    (float)
                    Math.Round(MathHelper.GetPercent(Bounds.Top, Bounds.Bottom - thumbRectangle.Height,
                                                     thumbRectangle.Y), 2);

                if (ThumbLocation != newValue)
                {
                    ThumbLocation = newValue;
                    moved = true;
                }
            }

            return moved;
        }
    }
}