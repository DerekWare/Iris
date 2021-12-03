using System.ComponentModel;

namespace DerekWare.Iris
{
    public static class Extensions
    {
        public static bool IsDesignMode()
        {
            return LicenseManager.UsageMode == LicenseUsageMode.Designtime;
        }
    }
}
