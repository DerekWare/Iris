using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using DerekWare.Collections;

namespace DerekWare.Threading
{
    public partial class ThreadPool
    {
        public class Thread : BackgroundThread
        {
            protected AutoResetEvent WakeEvent = new AutoResetEvent(true);

            public Thread()
            {
                SupportsCancellation = true;
            }

            public override void Start()
            {
                if(IsEnabled)
                {
                    WakeEvent.Set();
                    return;
                }

                base.Start();
            }

            protected override void OnDoWork()
            {
                WaitHandle[] wait = { CancelEvent.WaitHandle, WakeEvent };

                while(true)
                {
                    WaitHandle.WaitAny(wait);

                    if(CancellationPending)
                    {
                        break;
                    }

                    base.OnDoWork();
                }
            }
        }
    }
}
