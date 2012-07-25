using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EAGSS
{
    public struct ListBoxItem
    {
        public string Text;
        public string Vocal;

        public ListBoxItem(string text)
            : this(text, null)
        {
        }

        public ListBoxItem(string text, string vocal)
        {
            Text = text;
            Vocal = vocal;
        }
    }

    public class ListBox : Control
    {
        private List<ListBoxItem> items = new List<ListBoxItem>();
        private ListBoxItem[] displayItems;
        private Vector2 internalTextPadding = Vector2.Zero;
        private int displayedItemCount;
        private int firstDisplayedIndex = -1;

        public ListBox(Rectangle bounds, APNGTexture normalItemBackground, APNGTexture vocalItemBackground)
        {
            Textures.Add("normal_bg", normalItemBackground);
            Textures.Add("vocal_bg", vocalItemBackground);

            Bounds = bounds;

            displayedItemCount = Bounds.Height / (int)GameSettings.MessageWindowSize.Y;
        }

        /// <summary>
        /// 当前列表中的项目
        /// </summary>
        public List<ListBoxItem> Items
        {
            get { return items; }
            set { items = value; }
        }

        /// <summary>
        /// 文字显示参照点
        /// </summary>
        public Vector2 InternalTextPadding
        {
            get { return internalTextPadding; }
            set { internalTextPadding = value; }
        }

        /// <summary>
        /// 第一条显示的数据
        /// </summary>
        public int FirstDisplayedIndex
        {
            get { return firstDisplayedIndex; }
            set
            {
                if (items == null || FirstDisplayedIndex <= 0 || FirstDisplayedIndex == value)
                    return;

                firstDisplayedIndex = value;
            }
        }

        public override void LoadContent(ContentLoader content, ScreenManager screenManager)
        {
            base.LoadContent(content, screenManager);
        }

        public override void Update(GameTime gameTime, ScreenManager screenManager)
        {
            if (displayItems == null && items != null)
                FirstDisplayedIndex = 0;

            base.Update(gameTime, screenManager);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, ScreenManager screenManager)
        {
            spriteBatch.Begin();

            for (int i = 0; i < displayItems.Length; i++)
            {
                ;
            }

            spriteBatch.End();

            base.Draw(gameTime, spriteBatch, screenManager);
        }

        public override void HandleInput(InputState inputState, bool isTopMost, ScreenManager screenManager)
        {
            if (items.Count == 0)
                return;

            if (!inputState.IsMouseInRectangle(Bounds))
                return;

            var scroll = inputState.GetMouseWheelOffset(Bounds);
            if (scroll == 0)
                return;

            base.HandleInput(inputState, isTopMost, screenManager);
        }
    }
}