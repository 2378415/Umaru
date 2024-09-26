using Android.App;
using Android.Content;
using Application = Android.App.Application;
using Exception = System.Exception;
using Umaru.Core.Services;
using Android.Widget;


[assembly: Dependency(typeof(SuperService))]
namespace Umaru.Core.Services
{
	public class SuperService : ISuperService
	{
		public string? GetPackageName()
		{
			Context context = Application.Context;
			return context.PackageName;
		}

		public void LaunchApp(string packageName)
		{
			var context = Application.Context;
			var pm = context.PackageManager;
			var launchIntent = pm?.GetLaunchIntentForPackage(packageName);
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
			RootUtils.Execute($"am force-stop {packageName}");
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
#pragma warning disable CS8602 // 解引用可能出现空引用。
				Android.Widget.Toast.MakeText(context, message, ToastLength.Short).Show();
#pragma warning restore CS8602 // 解引用可能出现空引用。
			});
		}

		public void ToHome()
		{
			try
			{
				FloatingService.Stop();
				// 获取包名
				//var packageName = Application.Context.PackageName;
				// 获取主活动的类名
				//var mainActivityClassName = "Umaru.MainActivity"; // 更新为你的主活动类名

				// 构建 Intent
				Intent intent = new Intent(Application.Context, typeof(MainActivity));
				intent.SetAction(Intent.ActionMain);
				intent.AddCategory(Intent.CategoryLauncher);
				intent.AddFlags(ActivityFlags.ReorderToFront | ActivityFlags.ClearTop | ActivityFlags.SingleTop);

				// 构建 PendingIntent
				var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.UpdateCurrent);

				// 发送 PendingIntent
				pendingIntent?.Send();
			}
			catch (Exception ex)
			{
				// 处理异常
				System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
			}
		}
	}
}
