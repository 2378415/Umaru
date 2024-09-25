using Android.Graphics;
using Android.OS;
using Android.Util;
using Emgu.CV.Freetype;
using Java.IO;
using Java.Lang;
using Kotlin.System;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exception = Java.Lang.Exception;
using IOException = Java.IO.IOException;
using Process = Java.Lang.Process;
using StringBuilder = System.Text.StringBuilder;

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
		public static string Execute(string command)
		{
			try
			{
				TryBuildSuProcess();

				// 添加一个特殊的标记命令
				string endMarker = "END_OF_COMMAND";
				outputStream?.WriteBytes(command + "\n");
				outputStream?.WriteBytes($"echo {endMarker}\n");
				outputStream?.Flush();

				// 读取输出直到看到标记
				StringBuilder output = new StringBuilder();
				using (var reader = new BufferedReader(new InputStreamReader(suProcess.InputStream)))
				{
					string line;
					while ((line = reader.ReadLine()) != null)
					{
						if (line.Contains(endMarker))
						{
							break;
						}
						output.AppendLine(line);
					}
				}

				return output.ToString();
			}
			catch (IOException e)
			{
				e.PrintStackTrace();
				return string.Empty;
			}
		}

		public static string Screencap(string path)
		{
			return Execute($"screencap -p {System.IO.Path.Combine(FileSystem.AppDataDirectory, path)}");
		}

		public static Bitmap ReadImg(string path)
		{
			try
			{

				var tempPath = System.IO.Path.Combine(FileSystem.AppDataDirectory, path);
				// 读取文件内容的命令
				string readCommand = $"cat {tempPath} | base64";
				string fileContent = Execute(readCommand);

				// 将Base64字符串转换为字节数组
				byte[] fileBytes = Convert.FromBase64String(fileContent);

				// 使用字节数组创建 Bitmap
				using (var memoryStream = new MemoryStream(fileBytes))
				{
					return BitmapFactory.DecodeStream(memoryStream);
				}
			}
			catch (Exception ex)
			{
				return null;
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
