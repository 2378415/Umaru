using Android.Util;
using Java.IO;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IOException = Java.IO.IOException;
using StringBuilder = Java.Lang.StringBuilder;

namespace Umaru.Core
{
    public static class RootUtils
    {
        private static readonly Runtime runtime = Runtime.GetRuntime();

        public static bool Execute(string command)
        {
            try
            {
                var process = runtime.Exec("su");
                using (var outputStream = new DataOutputStream(process.OutputStream))
                {
                    outputStream.WriteBytes(command + "\n");
                    outputStream.WriteBytes("exit\n");
                    outputStream.Flush();
                }
                process.WaitFor();
                return process.ExitValue() == 0;
            }
            catch (IOException e)
            {
                e.PrintStackTrace();
                return false;
            }
            catch (InterruptedException e)
            {
                e.PrintStackTrace();
                return false;
            }
        }

        public static string ExecuteShell(string command)
        {
            try
            {
                var processInfo = new ProcessStartInfo("/system/bin/sh", "-c \"su -c '" + command + "'\"")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var process = new System.Diagnostics.Process
                {
                    StartInfo = processInfo
                };

                process.Start();

                string output = process.StandardOutput.ReadToEnd();

                process.WaitForExit();
                return output;
            }
            catch
            {
                return string.Empty;
            }
        }


        public static bool TryGetRootAccess()
        {
            try
            {
                var process = runtime.Exec("su");
                using (var outputStream = new DataOutputStream(process.OutputStream))
                {
                    outputStream.WriteBytes("exit\n");
                    outputStream.Flush();
                }
                process.WaitFor();
                return process.ExitValue() == 0;
            }
            catch (IOException)
            {
                return false;
            }
            catch (InterruptedException)
            {
                return false;
            }
        }

        public static bool IsRootAvailable()
        {
            try
            {
                var process = runtime.Exec("su");
                using (var outputStream = new DataOutputStream(process.OutputStream))
                {
                    outputStream.WriteBytes("exit\n");
                    outputStream.Flush();
                }
                process.WaitFor();
                return process.ExitValue() == 0;
            }
            catch (IOException)
            {
                return false;
            }
            catch (InterruptedException)
            {
                return false;
            }
        }


    }
}
