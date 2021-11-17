using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace DerekWare
{
    public static class NativeMethods
    {
        [Flags]
        public enum LLKHF
        {
            /// <summary>
            ///     Test the extended-key flag.
            /// </summary>
            LLKHF_EXTENDED = 1 << 0,

            /// <summary>
            ///     Test the event-injected (from a process running at lower integrity level) flag.
            /// </summary>
            LLKHF_LOWER_IL_INJECTED = 1 << 1,

            /// <summary>
            ///     Test the event-injected (from any process) flag.
            /// </summary>
            LLKHF_INJECTED = 1 << 4,

            /// <summary>
            ///     Test the context code.
            /// </summary>
            LLKHF_ALTDOWN = 1 << 5,

            /// <summary>
            ///     Test the transition-state flag.
            /// </summary>
            LLKHF_UP = 1 << 7
        }

        public enum ShowWindowCommands
        {
            Hide = 0,
            ShowNormal = 1,
            Normal = 1,
            ShowMinimized = 2,
            ShowMaximized = 3,
            Maximize = 3,
            ShowNoActivate = 4,
            Show = 5,
            Minimize = 6,
            ShowMinNoActive = 7,
            ShowNA = 8,
            Restore = 9,
            ShowDefault = 10,
            ForceMinimize = 11
        }

        public enum SIGDN : uint
        {
            NORMALDISPLAY = 0,
            PARENTRELATIVEPARSING = 0x80018001,
            PARENTRELATIVEFORADDRESSBAR = 0x8001c001,
            DESKTOPABSOLUTEPARSING = 0x80028000,
            PARENTRELATIVEEDITING = 0x80031001,
            DESKTOPABSOLUTEEDITING = 0x8004c000,
            FILESYSPATH = 0x80058000,
            URL = 0x80068000
        }

        public enum SIIGBF
        {
            SIIGBF_RESIZETOFIT = 0x00,
            SIIGBF_BIGGERSIZEOK = 0x01,
            SIIGBF_MEMORYONLY = 0x02,
            SIIGBF_ICONONLY = 0x04,
            SIIGBF_THUMBNAILONLY = 0x08,
            SIIGBF_INCACHEONLY = 0x10
        }

        [Flags]
        public enum ThreadExecutionState : uint
        {
            Continuous = 0x80000000,
            SystemRequired = 0x00000001,
            DisplayRequired = 0x00000002
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SIZE
        {
            public int cx;
            public int cy;
        }

        public delegate IntPtr WindowsHookProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool BringWindowToTop(this IntPtr hWnd);

        /// <summary>
        ///     The CallNextHookEx function passes the hook information to the next hook procedure in the current hook chain.
        ///     A hook procedure can call this function either before or after processing the hook information.
        /// </summary>
        /// <param name="hHook">handle to current hook</param>
        /// <param name="code">hook code passed to hook procedure</param>
        /// <param name="wParam">value passed to hook procedure</param>
        /// <param name="lParam">value passed to hook procedure</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hHook, int code, IntPtr wParam, IntPtr lParam);

        [DllImport("gdi32.dll", ExactSpelling = true, PreserveSig = true, SetLastError = true)]
        public static extern int DeleteObject(this IntPtr hObject);

        public static void EnableScreenSaver(bool enable)
        {
            var state = ThreadExecutionState.Continuous;

            if(!enable)
            {
                state |= ThreadExecutionState.SystemRequired | ThreadExecutionState.DisplayRequired;
            }

            state.SetThreadExecutionState();
        }

        public static Point GetCursorPos()
        {
            var pt = new POINT();
            GetCursorPos(ref pt);
            return new Point(pt.x, pt.y);
        }

        [DllImport("user32.dll", PreserveSig = true, SetLastError = true)]
        public static extern int GetCursorPos(ref POINT pt);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName = null);

        [DllImport("user32.dll")]
        public static extern bool IsHungAppWindow(this IntPtr hWnd);

        [DllImport("kernel32.dll")]
        public static extern ThreadExecutionState SetThreadExecutionState(this ThreadExecutionState esFlags);

        /// <summary>
        ///     The SetWindowsHookEx function installs an application-defined hook procedure into a hook chain.
        ///     You would install a hook procedure to monitor the system for certain types of events. These events are
        ///     associated either with a specific thread or with all threads in the same desktop as the calling thread.
        /// </summary>
        /// <param name="idHook">hook type</param>
        /// <param name="lpfn">hook procedure</param>
        /// <param name="hMod">handle to application instance</param>
        /// <param name="dwThreadId">thread identifier</param>
        /// <returns>If the function succeeds, the return value is the handle to the hook procedure.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, WindowsHookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(this IntPtr hWnd, ShowWindowCommands nCmdShow = ShowWindowCommands.ShowDefault);

        public static void SwitchToThisWindow(this IntPtr window, bool fAltTab = false)
        {
            SwitchToThisWindow(window, fAltTab ? 1 : 0);
        }

        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(this IntPtr hWnd, int fAltTab);

        /// <summary>
        ///     The UnhookWindowsHookEx function removes a hook procedure installed in a hook chain by the SetWindowsHookEx
        ///     function.
        /// </summary>
        /// <param name="hhk">handle to hook procedure</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);
    }
}
