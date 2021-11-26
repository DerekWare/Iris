using DerekWare.Iris.Properties;

namespace DerekWare.Iris
{
    public static class PropertyCache
    {
        static PropertyCache()
        {
            Deserialize();
        }

        public static void Deserialize()
        {
            HomeAutomation.Common.PropertyCache.Instance.Deserialize(Settings.Default.PropertyCache);
        }

        public static void Read(object obj)
        {
            HomeAutomation.Common.PropertyCache.Instance.Read(obj);
        }

        public static void Serialize()
        {
            Settings.Default.PropertyCache = HomeAutomation.Common.PropertyCache.Instance.Serialize();
            Settings.Default.Save();
        }

        public static void Write(object obj)
        {
            HomeAutomation.Common.PropertyCache.Instance.Write(obj);
            Serialize();
        }
    }
}
