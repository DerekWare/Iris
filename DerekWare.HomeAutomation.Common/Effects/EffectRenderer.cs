using System;
using System.ComponentModel;
using DerekWare.Diagnostics;
using DerekWare.Reflection;
using DerekWare.Threading;
using Newtonsoft.Json;
using DoWorkEventArgs = DerekWare.Threading.DoWorkEventArgs;

namespace DerekWare.HomeAutomation.Common.Effects
{
    public abstract class EffectRenderer : Effect
    {
        protected abstract void Update(RenderState state);

        protected Thread Thread;

        [Description("True if the effect runs on the device as opposed to running in this application."), Browsable(false), JsonIgnore]
        public override bool IsFirmware => false;

        [Description("The time it takes for the effect to complete a full cycle and start over.")]
        public virtual TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(30);

        [Description("The time it takes for the colors to change.")]
        public virtual TimeSpan RefreshRate { get; set; } = TimeSpan.FromSeconds(5);

        public override void Dispose()
        {
            Thread?.Dispose();
            Thread = null;
        }

        protected override void StartEffect()
        {
            Thread = new Thread { Name = $"{GetType().Name}", SupportsCancellation = true };
            Thread.DoWork += DoWork;
            Thread.Start();
        }

        protected override void StopEffect(bool wait)
        {
            Thread?.Stop(wait);
        }

        protected virtual TimeSpan ValidateRefreshRate()
        {
            return RefreshRate.Max(Device.Client.MinMessageInterval);
        }

        #region Event Handlers

        protected virtual void DoWork(Thread sender, DoWorkEventArgs e)
        {
            var renderState = new RenderState { CycleCount = -1 };
            var startTime = DateTime.Now;
            var lastUpdateTime = DateTime.MinValue;

            RefreshRate = ValidateRefreshRate();

            while(!sender.CancellationPending)
            {
                var currentTime = DateTime.Now;
                var nextUpdateTime = lastUpdateTime + RefreshRate;
                var timeout = nextUpdateTime - currentTime;

                // Sleep until the next time we're supposed to render
                if(timeout > TimeSpan.Zero)
                {
                    if(sender.CancelEvent.WaitOne(timeout))
                    {
                        break;
                    }

                    continue;
                }

                // Update the theme by giving it the position within the cycle (e.g. if we're 30 seconds through
                // a 60-second cycle, the position is 0.5. Likewise if we're 90 seconds through a 60-second cycle.
                renderState.TotalElapsed = currentTime - startTime;
                renderState.UpdateElapsed = currentTime - lastUpdateTime;

                var cycleTime = renderState.TotalElapsed.TotalSeconds / Duration.TotalSeconds;
                var cycleCount = (int)cycleTime;
                var cyclePosition = cycleTime - cycleCount;
                var cycleIncrement = cyclePosition >= renderState.CyclePosition
                    ? cyclePosition - renderState.CyclePosition
                    : (1 - renderState.CyclePosition) + cyclePosition;

                renderState.CycleCountChanged = renderState.CycleCount != cycleCount;
                renderState.CycleCount = cycleCount;
                renderState.CyclePosition = cyclePosition;
                renderState.CycleIncrement = cycleIncrement;

                Debug.Trace(this, renderState);
                Update(renderState);

                lastUpdateTime = currentTime;
            }
        }

        #endregion

        public class RenderState : ICloneable<RenderState>
        {
            // The number of cycles that have elapsed since the effect started
            public int CycleCount { get; set; }

            // Has the cycle count changed since the last update?
            public bool CycleCountChanged { get; set; }

            // The position (0-1) the cycle position moved since the last update
            public double CycleIncrement { get; set; }

            // The position (0-1) within the current cycle
            public double CyclePosition { get; set; }

            // The total amount of time elapsed since the effect started
            public TimeSpan TotalElapsed { get; set; }

            // The amount of time elapsed since the last update
            public TimeSpan UpdateElapsed { get; set; }

            public override string ToString()
            {
                return
                    $"{{ CycleCount:{CycleCount}, CycleIncrement:{CycleIncrement}, CyclePosition:{CyclePosition}, TotalElapsed:{TotalElapsed}, UpdateElapsed:{UpdateElapsed} }}";
            }

            #region ICloneable

            object ICloneable.Clone()
            {
                return Clone();
            }

            #endregion

            #region ICloneable<RenderState>

            public RenderState Clone()
            {
                return (RenderState)MemberwiseClone();
            }

            #endregion
        }
    }
}
