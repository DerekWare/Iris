using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DerekWare.Strings;
using Microsoft.Win32;
using Debug = DerekWare.Diagnostics.Debug;
using Path = DerekWare.IO.Path;

namespace DerekWare
{
    public static class Process
    {
        /// <summary>
        ///     Runs a given console executable file, capturing output.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="args"></param>
        /// <returns>STDERR output or STDIN if STDERR is empty.</returns>
        public static string ConsoleExecute(this Path path, params object[] args)
        {
            Console.WriteLine(args.Prepend(path.SafeToString()).Join(' '));

            var stdout = new StringWriter();
            var stderr = new StringWriter();

            using(var p = new System.Diagnostics.Process())
            {
                p.StartInfo.FileName = path;
                p.StartInfo.Arguments = args.Select(i => i.SafeToString()).Join(" ");
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;

                p.OutputDataReceived += (sender, e) =>
                {
                    Debug.Trace(path, $"{e.Data}{Environment.NewLine}");
                    Console.WriteLine(e.Data);
                    stdout.WriteLine(e.Data);
                };

                p.ErrorDataReceived += (sender, e) =>
                {
                    Debug.Trace(path, $"{e.Data}{Environment.NewLine}");
                    Console.WriteLine(e.Data);
                    stderr.WriteLine(e.Data);
                };

                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                p.WaitForExit();
            }

            var a = stdout.ToString();
            var b = stderr.ToString();

            // TODO be smarter than this
            return a.Length > b.Length ? a : b;
        }

        public static bool IsAutoAppStart(string executablePath, string appName)
        {
            var rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", false);
            var value = rk?.GetValue(appName) as string;
            return string.Equals(executablePath, value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     Runs a given executable file to completion.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="args"></param>
        /// <returns>The exit code of the process.</returns>
        public static int Run(this Path path, params object[] args)
        {
            using(var p = Start(path, args))
            {
                p.WaitForExit();
                return p.ExitCode;
            }
        }

        public static void SetAutoAppStart(string executablePath, string appName, bool autoStart)
        {
            var rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if(autoStart)
            {
                rk.SetValue(appName, executablePath);
            }
            else
            {
                rk.DeleteValue(appName, false);
            }
        }

        /// <summary>
        ///     Runs a given executable file.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="args"></param>
        /// <returns>The started process.</returns>
        public static System.Diagnostics.Process Start(this Path path, params object[] args)
        {
            return System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = path, Arguments = args.Select(i => i.SafeToString()).Join(" "), UseShellExecute = true
            });
        }
    }
}
