using DerekWare.Iris.Properties;

namespace DerekWare.Iris
{
    public static class PropertyCache
    {
        static PropertyCache()
        {
            Load();
        }

        public static void Load()
        {
            HomeAutomation.Common.PropertyCache.Load(Settings.Default.PropertyCache);
        }

        public static void LoadProperties(object obj)
        {
            HomeAutomation.Common.PropertyCache.LoadProperties(obj);
        }

        public static void Save()
        {
            Settings.Default.PropertyCache = HomeAutomation.Common.PropertyCache.Save();
            Settings.Default.Save();
        }

        public static void SaveProperties(object obj)
        {
            HomeAutomation.Common.PropertyCache.SaveProperties(obj);
            Save();
        }
    }
}
