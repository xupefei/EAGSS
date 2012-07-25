﻿using System;
using System.Collections.Generic;
using System.IO;
using LibAPNG;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EAGSS
{
    public class APNGTexture
    {
        #region Constants and Fields

        private readonly List<APNGFrame> frameList = new List<APNGFrame>();
        private readonly GraphicsDevice graphicsDevice;
        private readonly bool isSimplePNG;
        private readonly int numPlays;
        private readonly List<Texture2D> renderedTextureList = new List<Texture2D>();
        private readonly SpriteBatch sb;
        private int alreadyPlays;
        private TimeSpan alreadyWaitTime = TimeSpan.Zero;
        private APNGFrame baseFrame;
        private int currentPlayedIndex;

        #endregion Constants and Fields

        #region Public Properties

        public Texture2D CurrentFrame { get; private set; }

        #endregion Public Properties

        #region Constructors and Destructors

        public APNGTexture(GraphicsDevice gd, string pngFile)
            : this(gd, File.ReadAllBytes(pngFile))
        {
        }

        public APNGTexture(GraphicsDevice gd, byte[] pngFile)
        {
            graphicsDevice = gd;

            var image = new APNG(pngFile);

            isSimplePNG = image.IsSimplePNG;
            baseFrame = new APNGFrame(gd, image.DefaultImage);

            if (isSimplePNG)
            {
                CurrentFrame = baseFrame.FrameTexture;
            }
            else
            {
                numPlays = (int) image.acTLChunk.NumPlays;

                foreach (Frame frame in image.Frames)
                {
                    frameList.Add(new APNGFrame(gd, frame));
                }

                sb = new SpriteBatch(graphicsDevice);

                RenderEachFrame();

                CurrentFrame = renderedTextureList[0];
            }
        }

        #endregion Constructors and Destructors

        #region Public Methods and Operators

        public void Update(GameTime gameTime)
        {
            if (isSimplePNG)
                return;

            if (CurrentFrame == null)
                CurrentFrame = baseFrame.FrameTexture;

            if (numPlays != 0 && alreadyPlays >= numPlays)
                CurrentFrame = renderedTextureList[0];

            if (alreadyWaitTime > frameList[currentPlayedIndex].DelayTime)
            {
                currentPlayedIndex = currentPlayedIndex < renderedTextureList.Count - 1
                                         ? currentPlayedIndex + 1
                                         : 0;

                CurrentFrame = renderedTextureList[currentPlayedIndex];

                alreadyWaitTime = TimeSpan.Zero;

                alreadyPlays++;
            }
            else
            {
                alreadyWaitTime += gameTime.ElapsedGameTime;
            }
        }

        #endregion Public Methods and Operators

        #region Methods

        private void RenderEachFrame()
        {
            for (int crtIndex = 0; crtIndex < frameList.Count; crtIndex++)
            {
                var currentTexture = new RenderTarget2D(
                    graphicsDevice, baseFrame.Width, baseFrame.Height);

                graphicsDevice.SetRenderTarget(currentTexture);
                graphicsDevice.Clear(Color.Transparent);

                // if this is the first frame, just draw.
                if (crtIndex == 0)
                {
                    goto LABEL_DRAW_NEW_FRAME;
                }

                // Restore previous texture
                sb.Begin();
                sb.Draw(renderedTextureList[crtIndex - 1], Vector2.Zero, Color.White);
                sb.End();

                APNGFrame crtFrame = frameList[crtIndex - 1];

                switch (crtFrame.DisposeOp)
                {
                        // Do nothing.
                    case DisposeOps.APNGDisposeOpNone:
                        break;

                        // Set current Rectangle to transparent.
                    case DisposeOps.APNGDisposeOpBackground:
                        LABEL_APNG_DISPOSE_OP_BACKGROUND:
                        var t2 = new Texture2D(graphicsDevice, 1, 1);
                        sb.Begin(SpriteSortMode.Deferred, BlendState.Opaque);
                        sb.Draw(
                            t2,
                            new Rectangle(crtFrame.X, crtFrame.Y, crtFrame.Width, crtFrame.Height),
                            Color.White);
                        sb.End();
                        break;

                        // Rollback to previous frame.
                    case DisposeOps.APNGDisposeOpPrevious:
                        // If the first `fcTL` chunk uses a `dispose_op` of APNG_DISPOSE_OP_PREVIOUS
                        // it should be treated as APNG_DISPOSE_OP_BACKGROUND.
                        if (crtIndex - 1 == 0)
                        {
                            goto LABEL_APNG_DISPOSE_OP_BACKGROUND;
                        }

                        APNGFrame prevFrame = frameList[crtIndex - 2];

                        sb.Begin(SpriteSortMode.Deferred, BlendState.Opaque);
                        sb.Draw(
                            prevFrame.FrameTexture,
                            new Rectangle(crtFrame.X, crtFrame.Y, crtFrame.Width, crtFrame.Height),
                            new Rectangle(crtFrame.X, crtFrame.Y, crtFrame.Width, crtFrame.Height),
                            Color.White);
                        sb.End();
                        break;
                }

                LABEL_DRAW_NEW_FRAME:
                // Now let's look at the new frame.
                if (crtIndex == 0)
                {
                    crtFrame = frameList[0];
                }
                else
                {
                    crtFrame = crtIndex < frameList.Count
                                   ? frameList[crtIndex]
                                   : frameList[0];
                }

                switch (crtFrame.BlendOp)
                {
                        // Do not apply alpha
                    case BlendOps.APNGBlendOpSource:
                        sb.Begin(SpriteSortMode.Deferred, BlendState.Opaque);
                        sb.Draw(
                            crtFrame.FrameTexture,
                            new Rectangle(crtFrame.X, crtFrame.Y, crtFrame.Width, crtFrame.Height),
                            Color.White);
                        sb.End();
                        break;

                        // Apply alpha
                    case BlendOps.APNGBlendOpOver:
                        sb.Begin();
                        sb.Draw(
                            crtFrame.FrameTexture,
                            new Rectangle(crtFrame.X, crtFrame.Y, crtFrame.Width, crtFrame.Height),
                            Color.White);
                        sb.End();
                        break;
                }

                renderedTextureList.Add(currentTexture);
            }

            // Okay it's all over now
            graphicsDevice.SetRenderTarget(null);
        }

        #endregion Methods
    }
}