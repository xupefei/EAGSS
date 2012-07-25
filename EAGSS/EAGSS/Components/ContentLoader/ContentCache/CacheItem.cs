namespace EAGSS.ContentCache
{
    internal struct CacheItem
    {
        public int Activity;
        public string AssetName;
        public object Content;

        public CacheItem(string assetName, object obj)
        {
            Activity = 1;
            AssetName = assetName;
            Content = obj;
        }
    }
}