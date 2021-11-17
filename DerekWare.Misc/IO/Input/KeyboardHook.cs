using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace DerekWare.IO.Input
{
    public delegate void KeyboardHookEventHandler(object sender, KeyboardHookEventArgs e);

    /// <summary>
    ///     Installs a global keyboard hook to monitor key messages sent outside the application.
    /// </summary>
    public class KeyboardHook : IDisposable
    {
        public static readonly KeyboardHook Default = new KeyboardHook();

        readonly NativeMethods.WindowsHookProc _HookProc;
        IntPtr _HookHandle;

        public event KeyboardHookEventHandler KeyboardEvent
        {
            add
            {
                lock(SyncRoot)
                {
                    _KeyboardEvent += value;
                }
            }
            remove
            {
                lock(SyncRoot)
                {
                    _KeyboardEvent -= value;
                }
            }
        }

        event KeyboardHookEventHandler _KeyboardEvent;

        public KeyboardHook()
        {
            _HookProc = HookProc; // we must keep alive HookProc, because GC is not aware about SetWindowsHookEx behaviour.

            if((_HookHandle = NativeMethods.SetWindowsHookEx(13, _HookProc, NativeMethods.GetModuleHandle(), 0)) == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        ~KeyboardHook()
        {
            Dispose();
        }

        public object SyncRoot { get; } = new object();

        IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            var keyCode = Marshal.ReadInt32(lParam);
            var keyState = wParam.ToInt32();
            var handled = false;

            if(Enum.IsDefined(typeof(Keys), keyCode) && Enum.IsDefined(typeof(KeyState), keyState))
            {
                var e = new KeyboardHookEventArgs((Keys)keyCode, (KeyState)keyState);
                OnKeyboardEvent(this, e);
                handled = e.Handled;
            }

            return handled ? (IntPtr)1 : NativeMethods.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        #region IDisposable

        public void Dispose()
        {
            lock(SyncRoot)
            {
                // because we can unhook only in the same thread, not in garbage collector thread
                if(_HookHandle == IntPtr.Zero)
                {
                    return;
                }

                if(!NativeMethods.UnhookWindowsHookEx(_HookHandle))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                _HookHandle = IntPtr.Zero;
            }
        }

        #endregion

        #region Event Handlers

        protected virtual void OnKeyboardEvent(object sender, KeyboardHookEventArgs e)
        {
            lock(SyncRoot)
            {
                _KeyboardEvent?.Invoke(this, e);
            }
        }

        #endregion
    }

    public class KeyboardHookEventArgs : HandledEventArgs
    {
        public readonly Keys Key;
        public readonly KeyState State;

        public KeyboardHookEventArgs(Keys key, KeyState state)
        {
            Key = key;
            State = state;
        }
    }
}
