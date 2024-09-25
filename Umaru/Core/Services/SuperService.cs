using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Java.Lang;
using Java.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application = Android.App.Application;
using Exception = System.Exception;
using Umaru.Core.Services;
using Android.Widget;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Platform = Microsoft.Maui.ApplicationModel.Platform;
using Android.AccessibilityServices;


[assembly: Dependency(typeof(SuperService))]
namespace Umaru.Core.Services
{
	public class SuperService : ISuperService
	{
		public void LaunchApp(string packageName)
		{
			var context = Application.Context;
			var pm = context.PackageManager;
			var launchIntent = pm.GetLaunchIntentForPackage(packageName);
			if (launchIntent != null)
			{
				context.StartActivity(launchIntent);
			}
			else
			{
				// 应用程序未安装
			}
		}

		public void CloseApp(string packageName)
		{
			try
			{
				// 使用 root 权限执行命令
				var process = Runtime.GetRuntime().Exec(new string[] { "su", "-c", "am force-stop " + packageName });
				process.WaitFor();
			}
			catch (Exception ex)
			{
				// 处理异常
				System.Diagnostics.Debug.WriteLine("Error closing app: " + ex.Message);
			}
		}

		public void Tap(int x, int y)
		{
			string command = $"input tap {x} {y}";
			RootUtils.Execute(command);
		}

		public void Swipe(int x1, int y1, int x2, int y2, int duration = 500)
		{
			string command = $"input swipe {x1} {y1} {x2} {y2} {duration}";
			RootUtils.Execute(command);
		}

		public void Roll(int index, int count)
		{
			string command = $"input roll {index} {count}";
			RootUtils.Execute(command);
		}

		public void KeyEvent(string @event)
		{
			string command = $"input keyevent {@event}";
			RootUtils.Execute(command);
		}


		public void Toast(string message)
		{
			var context = Android.App.Application.Context;
			MainThread.BeginInvokeOnMainThread(() =>
			{
				Android.Widget.Toast.MakeText(context, message, ToastLength.Short).Show();
			});
		}

		public void ToHome()
		{
			try
			{
				// 获取包名
				string packageName = Application.Context.PackageName;
				// 获取主活动的类名
				string mainActivityClassName = "Umaru.MainActivity"; // 更新为你的主活动类名

				// 构建 Intent
				Intent intent = new Intent(Application.Context, typeof(MainActivity));
				intent.SetAction(Intent.ActionMain);
				intent.AddCategory(Intent.CategoryLauncher);
				intent.AddFlags(ActivityFlags.ReorderToFront | ActivityFlags.ClearTop | ActivityFlags.SingleTop);

				// 构建 PendingIntent
				PendingIntent pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.UpdateCurrent);

				// 发送 PendingIntent
				pendingIntent.Send();
			}
			catch (Exception ex)
			{
				// 处理异常
				System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
			}
		}
	}
}
