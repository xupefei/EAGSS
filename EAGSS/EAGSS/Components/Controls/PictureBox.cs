using System;

namespace EAGSS
{
    internal class PictureBox : FadeBox
    {
        public PictureBox(APNGTexture pic)
            : base(pic)
        {
            StopTime = TimeSpan.Zero;
            TransitionOnTime = TimeSpan.Zero;
            TransitionOffTime = TimeSpan.Zero;
            CanInterrupt = false;
        }
    }
}