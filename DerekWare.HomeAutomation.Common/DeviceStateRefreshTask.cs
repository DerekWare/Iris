using System;
using System.Threading;
using System.Threading.Tasks;

namespace DerekWare.HomeAutomation.Common
{
    public class DeviceStateRefreshTask : IDisposable
    {
        readonly IDevice Device;
        readonly TimeSpan Timeout;

        CancellationTokenSource CancellationTokenSource = new();
        Task Task;

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
            CancellationTokenSource?.Cancel();
            Task?.Wait();

            CancellationTokenSource = null;
            Task = null;
        }

        #endregion
    }
}
