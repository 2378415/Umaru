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
            bool success = RootUtils.Execute(command);

            if (!success)
            {
                System.Diagnostics.Debug.WriteLine("Error executing tap command with root.");
            }
        }

        public void Toast(string message)
        {
            var context = Android.App.Application.Context;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Android.Widget.Toast.MakeText(context, message, ToastLength.Short).Show();
            });
        }
    }
}
