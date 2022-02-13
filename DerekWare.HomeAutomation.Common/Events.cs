using System;
using DerekWare.HomeAutomation.Common;

namespace DerekWare.HomeAutomation.Common
{
    public class DeviceEventArgs : EventArgs
    {
        public IDevice Device { get; set; }
    }
}
