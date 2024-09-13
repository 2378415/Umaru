using Android.OS;
using Android.Util;
using Emgu.CV.Freetype;
using Java.IO;
using Java.Lang;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IOException = Java.IO.IOException;
using Process = Java.Lang.Process;
using StringBuilder = Java.Lang.StringBuilder;

namespace Umaru.Core
{
	public static class RootUtils
	{
		private static readonly Runtime runtime = Runtime.GetRuntime();
		private static Process? suProcess;
		private static DataOutputStream? outputStream;

		static RootUtils()
		{
			TryBuildSuProcess();
		}

		public static void Destroy()
		{
			try
			{
				outputStream?.WriteBytes("exit\n");
				outputStream?.Flush();
				suProcess?.WaitFor();
				suProcess?.ExitValue();

				outputStream?.Close();
				suProcess = null;
				outputStream = null;
			}
			catch
			{ 
			
			}
		}

		private static void TryBuildSuProcess()
		{
			if (suProcess != null && outputStream != null) return;

			try
			{
				suProcess = runtime.Exec("su");
				outputStream = new DataOutputStream(suProcess.OutputStream);
			}
			catch (IOException e)
			{
				e.PrintStackTrace();
			}
		}

		/// <summary>
		/// 执行Root命令，推荐使用，因为他复用的su进程，效率更高
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		public static bool Execute(string command)
		{
			try
			{
				TryBuildSuProcess();

				outputStream?.WriteBytes(command + "\n");
				outputStream?.Flush();
				return true;
			}
			catch (IOException e)
			{
				e.PrintStackTrace();
				return false;
			}
		}

		/// <summary>
		/// 执行shell命令，不推荐，因为没有复用su进程，效率低
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
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
