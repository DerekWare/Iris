using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;
using DerekWare.Diagnostics;

namespace DerekWare
{
    public static class COM
    {
        public static void FinalReleaseComObject(object obj)
        {
            if(null == obj)
            {
                return;
            }

            try
            {
                Marshal.FinalReleaseComObject(obj);
            }
            catch(InvalidComObjectException ex)
            {
                Debug.Error(typeof(COM), ex);
            }
        }

        public static void FinalReleaseComObject<T>(ref T obj)
            where T : class
        {
            FinalReleaseComObject(Interlocked.Exchange(ref obj, null));
        }

        public static void FinalReleaseComObjects(IEnumerable items)
        {
            foreach(var i in items)
            {
                FinalReleaseComObject(i);
            }
        }

        public static void ReleaseComObject(object obj)
        {
            if(null == obj)
            {
                return;
            }

            try
            {
                Marshal.ReleaseComObject(obj);
            }
            catch(InvalidComObjectException ex)
            {
                Debug.Error(typeof(COM), ex);
            }
        }

        public static void ReleaseComObject<T>(ref T obj)
            where T : class
        {
            ReleaseComObject(Interlocked.Exchange(ref obj, null));
        }

        public static void ReleaseComObjects(IEnumerable items)
        {
            foreach(var item in items)
            {
                ReleaseComObject(item);
            }
        }
    }

    public static class HRESULT
    {
        public const int E_UNEXPECTED = -2147418113;
        public const int S_OK = 0;

        public static bool Failed(int hr)
        {
            return hr < 0;
        }

        public static bool Succeeded(int hr)
        {
            return hr >= 0;
        }

        public static void Throw(int hr)
        {
            if(Failed(hr))
            {
                throw new COMException("COM failure", hr);
            }
        }
    }
}
