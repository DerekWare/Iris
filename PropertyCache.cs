using System;
using DerekWare.Diagnostics;
using DerekWare.Iris.Properties;

namespace DerekWare.Iris
{
    public static class PropertyCache
    {
        static readonly HomeAutomation.Common.PropertyCache Instance = new();

        static PropertyCache()
        {
            Deserialize();
        }

        public static void Deserialize()
        {
            try
            {
                Instance.Deserialize(Settings.Default.PropertyCache);
            }
            catch(Exception ex)
            {
                Debug.Warning(null, ex);
            }
        }

        public static void WriteToObject(object obj)
        {
            Instance.WriteToObject(obj);
        }

        public static void Serialize()
        {
            Settings.Default.PropertyCache = Instance.Serialize();
            Settings.Default.Save();
        }

        public static void ReadFromObject(object obj)
        {
            Instance.ReadFromObject(obj);
            Serialize();
        }
    }
}
