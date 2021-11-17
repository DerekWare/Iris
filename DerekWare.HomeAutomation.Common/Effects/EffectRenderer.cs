using System;
using System.ComponentModel;
using DerekWare.Diagnostics;
using DerekWare.Reflection;
using DerekWare.Threading;
using DoWorkEventArgs = DerekWare.Threading.DoWorkEventArgs;

namespace DerekWare.HomeAutomation.Common.Effects
{
    public abstract class EffectRenderer : Effect
    {
        protected abstract void Update(RenderState state);

        protected bool FirstRun = true;
        protected Thread Thread;

        [Description("True if the effect runs on the device as opposed to running in this application.")]
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
            return RefreshRate;
        }

        #region Event Handlers

        protected virtual void DoWork(Thread sender, DoWorkEventArgs e)
        {
            var state = new RenderState();
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

                // Update the scene by giving it the position within the cycle (e.g. if we're 30 seconds through
                // a 60-second cycle, the position is 0.5. Likewise if we're 90 seconds through a 60-second cycle.
                state.TotalElapsed = currentTime - startTime;
                state.UpdateElapsed = currentTime - lastUpdateTime;

                var cycleTime = state.TotalElapsed.TotalSeconds / Duration.TotalSeconds;
                var cycleCount = (int)cycleTime;
                var cyclePosition = cycleTime - cycleCount;
                var cycleIncrement = cyclePosition - state.CyclePosition;

                while(cycleIncrement < 0)
                {
                    cycleIncrement += 1;
                }

                state.CycleCount = cycleCount;
                state.CyclePosition = cyclePosition;
                state.CycleIncrement = cycleIncrement;

                Debug.Trace(this, state);
                Update(state);

                lastUpdateTime = currentTime;
                FirstRun = false;
            }
        }

        #endregion

        public class RenderState : ICloneable<RenderState>
        {
            // The number of cycles that have elapsed since the effect started
            public int CycleCount { get; set; }

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
