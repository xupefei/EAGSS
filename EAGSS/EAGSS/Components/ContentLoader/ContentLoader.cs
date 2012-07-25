using System;
using System.Collections.Generic;
using System.IO;
using EAGSS.ContentCache;
using EAGSS.DataPackage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace EAGSS
{
    public class ContentLoader : ContentManager
    {
        private static Dictionary<string, StructDescription.ContentInfo> dictContents =
            new Dictionary<string, StructDescription.ContentInfo>(StringComparer.OrdinalIgnoreCase);

        private static readonly Cache Cache = new Cache();
        private readonly GraphicsDeviceManager graphicsDeviceManager;

        private readonly bool isAreadyIndexed;

        public ContentLoader(IServiceProvider serviceProvider)
            : base(serviceProvider, GameSettings.DataFolderName)
        {
            graphicsDeviceManager =
                (GraphicsDeviceManager)serviceProvider.GetService(typeof(IGraphicsDeviceService));

            if (!isAreadyIndexed)
            {
                DebugScreen.Output("Indexing package");

                var pi = new PackageIndexer(GameSettings.DataFolderName);
                dictContents = pi.DictContents;

                isAreadyIndexed = true;
            }
        }

        public override T Load<T>(string assetName)
        {
            Type t = typeof(T);

            if (t == typeof(Texture2D))
            {
                if (Cache.Exists(assetName))
                    return Cache.Get<T>(assetName);

                Texture2D t2D = Texture2D.FromStream(
                    graphicsDeviceManager.GraphicsDevice, new MemoryStream(LoadFile(assetName)));

                MultiplyAlpha(t2D);

                Cache.Add(assetName, t2D);
                return (T)(t2D as object);
            }
            if (t == typeof(APNGTexture))
            {
                if (Cache.Exists(assetName))
                    return Cache.Get<T>(assetName);

                var apng = new APNGTexture(
                    graphicsDeviceManager.GraphicsDevice, LoadFile(assetName));

                Cache.Add(assetName, apng);
                return (T)(apng as object);
            }
            if (t == typeof(Effect))
            {
                if (Cache.Exists(assetName))
                    return Cache.Get<T>(assetName);

                var effect = new Effect(graphicsDeviceManager.GraphicsDevice, LoadFile(assetName));

                Cache.Add(assetName, effect);
                return (T)(effect as object);
            }
            if (t == typeof(byte[]))
            {
                if (Cache.Exists(assetName))
                    return Cache.Get<T>(assetName);

                byte[] bytes = LoadFile(assetName);

                Cache.Add(assetName, bytes);
                return (T)(bytes as object);
            }

            return base.Load<T>(assetName);
        }

        protected override Stream OpenStream(string assetName)
        {
            return base.OpenStream(assetName);
        }

        //private string FindAsset(string assetName)
        //{
        //    Directory.GetFiles(GameSettings.DataFolderName,)
        //}

        private byte[] LoadFile(string assetName)
        {
            // check outer file
            if (File.Exists(Path.Combine(GameSettings.DataFolderName, assetName)))
            {
                DebugScreen.Output(string.Format("Read {0} from file", assetName));
                return File.ReadAllBytes(Path.Combine(GameSettings.DataFolderName, assetName));
            }

            // then check data package
            StructDescription.ContentInfo contentInfo;

            if (dictContents.TryGetValue(assetName, out contentInfo))
            {
                DebugScreen.Output(string.Format("Read {0} from package", assetName));
                return PackageReader.GetContent(contentInfo).ToArray();
            }

            //on no...
            throw new FileNotFoundException(string.Format("asset {0} not found.", assetName));
        }

        private static void MultiplyAlpha(Texture2D ret)
        {
            var data = new Byte4[ret.Width * ret.Height];

            ret.GetData(data);
            for (int i = 0; i < data.Length; i++)
            {
                Vector4 vec = data[i].ToVector4();
                float alpha = vec.W / 255.0f;
                var a = (uint)(vec.W);
                var r = (uint)(alpha * vec.X);
                var g = (uint)(alpha * vec.Y);
                var b = (uint)(alpha * vec.Z);
                var packed = (a << 24) + (b << 16) + (g << 8) + r;

                data[i].PackedValue = packed;
            }

            ret.SetData(data);
        }
    }
}