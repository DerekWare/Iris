using System;
using System.Threading;
using System.Threading.Tasks;

namespace DerekWare.HomeAutomation.Common
{
    public class DeviceStateRefreshTask : IDisposable
    {
        readonly CancellationTokenSource CancellationTokenSource = new();
        readonly IDevice Device;
        readonly Task Task;
        readonly TimeSpan Timeout;

        public DeviceStateRefreshTask(IDevice device)
            : this(device, TimeSpan.FromSeconds(10))
        {
        }

        public DeviceStateRefreshTask(IDevice device, TimeSpan timeout)
        {
            Device = device;
            Timeout = timeout;
            Task = Task.Run(RefreshState, CancellationTokenSource.Token);
        }

        protected virtual async void RefreshState()
        {
            while(!CancellationTokenSource.IsCancellationRequested)
            {
                Device.RefreshState();
                await Task.Delay(Timeout, CancellationTokenSource.Token);
            }
        }

        #region IDisposable

        public virtual void Dispose()
        {
            CancellationTokenSource.Cancel();
            Task.Wait();
        }

        #endregion
    }
}
