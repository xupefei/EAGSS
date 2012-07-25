using System;
using System.Collections.Generic;
using System.IO;

namespace EAGSS.ContentCache
{
    internal class Cache
    {
        public static List<CacheItem> CacheContainer = new List<CacheItem>();

        public static long MaxMemoryUsage = 256*1024*1024;

        public Cache()
        {
        }

        public Cache(int memSize)
        {
            MaxMemoryUsage = memSize;
        }

        public void Add(string assetName, object obj)
        {
            if (Exists(assetName))
            {
                RaiseActivity(assetName);
            }

            Clean();

            CacheContainer.Add(new CacheItem(assetName, obj));
        }

        public T Get<T>(string assetName)
        {
            if (!Exists(assetName))
                throw new FileNotFoundException("cache item not found.");

            RaiseActivity(assetName);

            DebugScreen.Output(string.Format("Load {0} from cache", assetName));
            return (T) (CacheContainer.Find(item => item.AssetName == assetName).Content);
        }

        public bool Exists(string assetName)
        {
            return CacheContainer.Exists(item => item.AssetName == assetName);
        }

        public bool Clean()
        {
            while (GC.GetTotalMemory(false) > MaxMemoryUsage)
            {
                if (CacheContainer.Count == 0)
                    return false;

                DebugScreen.Output("Cleaning cache");

                Sort();

                CacheContainer.RemoveAt(CacheContainer.Count - 1);

                return true;
            }

            return false;
        }

        private void Sort()
        {
            CacheContainer.Sort((x, y) => x.Activity - y.Activity);
        }

        private void RaiseActivity(string assetName)
        {
            if (!Exists(assetName))
                return;

            int index = CacheContainer.FindIndex(cacheItem => cacheItem.AssetName == assetName);

            CacheItem i = CacheContainer[index];
            i.Activity++;
            CacheContainer[index] = i;
        }
    }
}