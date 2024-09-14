using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Views.Accessibility;
using Android.Widget;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Java.Lang;
using System.Collections.Generic;
using Umaru.Core.Services;
using Umaru.Core;
using Java.IO;
using IOException = Java.IO.IOException;
using Process = Java.Lang.Process;
using Emgu.CV;
using Emgu.CV.Structure;
using Exception = Java.Lang.Exception;
using Android.Graphics;
using AndroidX.ConstraintLayout.Core.Motion.Utils;
using Java.Nio;
using System.Runtime.InteropServices;
using Emgu.CV.CvEnum;
using Org.Apache.Http.Conn;
using Path = System.IO.Path;
using Umaru.Core.Store;
using Android.Media.Projection;
using Umaru.Core.OpenCV;
using Emgu.CV.Ocl;
using Android.AccessibilityServices;
using Microsoft.Maui.Controls;
using System.Diagnostics;

namespace Umaru
{
	[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
	public class MainActivity : MauiAppCompatActivity
	{
		const int RequestPermissionsId = 0;
		const int RequestOverlayPermissionCode = 1000;

		readonly string[] Permissions =
		{
			"android.permission.ACCESS_SUPERUSER",
			Android.Manifest.Permission.BatteryStats,
			Android.Manifest.Permission.AccessNetworkState,
			Android.Manifest.Permission.Internet,
			Android.Manifest.Permission.BindAccessibilityService,
			Android.Manifest.Permission.SystemAlertWindow,
			Android.Manifest.Permission.AccessFineLocation,
			Android.Manifest.Permission.AccessWifiState,
			Android.Manifest.Permission.ReadExternalStorage,
			Android.Manifest.Permission.WriteExternalStorage,
			Android.Manifest.Permission.CaptureSecureVideoOutput,
			Android.Manifest.Permission.CaptureVideoOutput,
		};

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			try
			{
				FloatingService.Stop();
				RootUtils.Destroy();
				System.Environment.Exit(0);
			}
			catch
			{

			}

			try
			{
				System.Environment.Exit(0);
			}
			catch
			{

			}
		}

		protected override void OnCreate(Bundle? savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			try
			{
				RawUtils.WriteLocalAsync();

				// 检查并请求权限
				CheckAndRequestPermissions();

				if (RootUtils.IsRootAvailable())
				{
					Tools.Toast("已经具有 root 权限");

					//自动授权
					var enserver = "settings put secure enabled_accessibility_services " + "\"com.umaru.moper/crc6461fdc8ac6c9c5035.BarrierService\"" + "";
					RootUtils.ExecuteShell(enserver);
				}
				else
				{
					if (RootUtils.TryGetRootAccess())
					{
						Tools.Toast("成功获取 root 权限");
					}
					else
					{
						Tools.Toast("无法获取 root 权限");
					}
				}

				//// 检查并请求权限
				//CheckAndRequestPermissions();

				// 检查并请求 SYSTEM_ALERT_WINDOW 权限
				if (!Settings.CanDrawOverlays(this))
				{
					Intent intent = new Intent(Settings.ActionManageOverlayPermission,
						Android.Net.Uri.Parse("package:" + PackageName));
					StartActivityForResult(intent, RequestOverlayPermissionCode);

				}

				// 检查辅助功能服务是否启用
				if (!IsAccessibilityEnabled())
				{
					// 引导用户前往辅助功能设置页面
					PromptEnableAccessibility();
				}
			}
			catch (Exception ex)
			{
				Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
			}

		}

		void CheckAndRequestPermissions()
		{
			List<string> permissionsToRequest = new List<string>();

			foreach (var permission in Permissions)
			{
				if (ContextCompat.CheckSelfPermission(this, permission) != (int)Permission.Granted)
				{
					permissionsToRequest.Add(permission);
				}
			}

			if (permissionsToRequest.Count > 0)
			{
				ActivityCompat.RequestPermissions(this, permissionsToRequest.ToArray(), RequestPermissionsId);
			}
			else
			{
				// 所有权限已被授予
				Toast.MakeText(this, "已获取所有权限", ToastLength.Short).Show();
			}
		}

		bool IsAccessibilityEnabled()
		{
			string command = "settings get secure enabled_accessibility_services";
			var msg = RootUtils.ExecuteShell(command);
			if (msg.Contains("BarrierService") && msg.Contains(PackageName)) return true;
			return false;
		}


		void PromptEnableAccessibility()
		{
			AlertDialog.Builder builder = new AlertDialog.Builder(this);
			builder.SetTitle("开启小埋无障碍服务");
			builder.SetMessage("请根据引导开启小埋的无障碍服务");
			builder.SetPositiveButton("去开启", (sender, e) =>
			{
				Intent intent = new Intent(Settings.ActionAccessibilitySettings);
				StartActivity(intent);
			});
			builder.SetNegativeButton("取消", (sender, e) => { });
			builder.Show();
		}
	}
}
