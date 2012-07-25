using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EAGSS
{
    public abstract class Control
    {
        private Rectangle bounds = new Rectangle(0, 0, 100, 100);
        private bool enabled = true;
        private Dictionary<string, APNGTexture> textures = new Dictionary<string, APNGTexture>();

        /// <summary>
        /// 得到或设置当前 Control 所使用的 APNGTexture 字典
        /// </summary>
        public Dictionary<string, APNGTexture> Textures
        {
            get { return textures; }
            set { textures = value; }
        }

        /// <summary>
        /// 得到或设置 Control 的位置和边界
        /// </summary>
        public Rectangle Bounds
        {
            get { return bounds; }
            set { bounds = value; }
        }

        /// <summary>
        /// 得到或设置 Control 是否启用
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        /// <summary>
        /// 得到或设置 Control 的状态
        /// </summary>
        public ControlStatus Status { get; set; }

        public virtual void LoadContent(ContentLoader content, ScreenManager screenManager)
        {
        }

        public virtual void Update(GameTime gameTime, ScreenManager screenManager)
        {
            foreach (var texture in textures.Where(texture => texture.Value != null))
            {
                texture.Value.Update(gameTime);
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, ScreenManager screenManager)
        {
        }

        public virtual void HandleInput(InputState inputState, bool isTopMost, ScreenManager screenManager)
        {
        }
    }
}