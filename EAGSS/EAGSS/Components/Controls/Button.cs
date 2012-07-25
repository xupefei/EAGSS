using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EAGSS
{
    public class Button : Control
    {
        #region Delegates

        /// <summary>
        /// 单击事件
        /// </summary>
        public delegate void OnClickEvent(Button sender);

        #endregion Delegates

        private string currentState = string.Empty;

        /// <summary>
        /// 响应用户鼠标点击，显示对应材质并发出事件。
        /// </summary>
        public Button(APNGTexture normal, APNGTexture hover, APNGTexture pressed, APNGTexture disabled)
        {
            if (normal == null) throw new ArgumentException("normal texture must not null.");

            Textures.Add("normal", normal);
            Textures.Add("hover", hover);
            Textures.Add("pressed", pressed);
            Textures.Add("disabled", disabled);
        }

        public event OnClickEvent OnClick;

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

            if (Enabled)
                spriteBatch.Draw(
                    Textures[currentState] != null
                        ? Textures[currentState].CurrentFrame
                        : Textures["hover"] != null
                              ? Textures["hover"].CurrentFrame
                              : Textures["normal"].CurrentFrame,
                    Bounds,
                    Color.White);
            else
                spriteBatch.Draw(
                    Textures["disabled"] != null
                        ? Textures["disabled"].CurrentFrame
                        : Textures["normal"].CurrentFrame,
                    Bounds,
                    Color.White);

            spriteBatch.End();

            base.Draw(gameTime, spriteBatch, screenManager);
        }

        public override void HandleInput(InputState inputState, bool isTopMost, ScreenManager screenManager)
        {
            if (!Enabled)
            {
                currentState = "disabled";
                return;
            }

            if (inputState.IsMouseInRectangle(Bounds))
            {
                //2-按下，1-经过
                currentState = inputState.IsMouseLeftButtonPressed(Bounds) ? "pressed" : "hover";

                //松开按键时立刻引发事件
                if (inputState.IsNewMouseLeftButtonReleased(Bounds))
                {
                    if (OnClick != null) OnClick(this);
                }
            }
            else
            {
                //通常
                currentState = "normal";
            }

            base.HandleInput(inputState, isTopMost, screenManager);
        }
    }
}