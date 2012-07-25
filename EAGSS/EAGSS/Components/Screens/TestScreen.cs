using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EAGSS
{
    internal class TestScreen : GameScreen
    {
        public TestScreen()
        {
            TransitionTime = new TimeSpan(0, 0, 0);
        }

        public override void InitializeControls()
        {
            var lb = new ListBox(new Rectangle(0, 0, 800, 600),
                                 ScreenManager.Content.Load<APNGTexture>(@"images\eagss.png"),
                                 ScreenManager.Content.Load<APNGTexture>(@"images\thumb.png"));
            lb.Items.Add(new ListBoxItem(""));

            AddControl(lb);

            var s = new Slider(new Vector3(300, 100, 400), false,
                               ScreenManager.Content.Load<APNGTexture>(@"images\thumb.png"),
                               ScreenManager.Content.Load<APNGTexture>(@"images\thumb_hover.png"))
                        {
                            ThumbLocation = 1f,
                        };

            s.OnDrag += sender =>
                            {
                                DebugScreen.Output(sender.ThumbLocation.ToString());
                            };

            AddControl(s);

            base.InitializeControls();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            var d = ScreenManager.Content.Load<Texture2D>(@"images\eagss_2.png");
        }
    }
}