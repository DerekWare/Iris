using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using DerekWare.HomeAutomation.Lifx.Lan;
using DerekWare.Iris.Properties;

namespace DerekWare.Iris
{
    public static class Program
    {
        public static Version Version => new(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion);

        [STAThread]
        static void Main()
        {
            Client.Instance.Start();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
            Settings.Default.Save();
            Client.Instance.Dispose();
        }
    }
}
