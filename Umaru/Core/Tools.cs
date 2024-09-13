using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls;
using Android.AccessibilityServices;
using Android.App;
using Android.Util;
using Android.Views.Accessibility;
using Android.Content;
using Android.Nfc;
using static Android.Views.Accessibility.AccessibilityNodeInfo;
using Action = Android.Views.Accessibility.Action;
using Umaru.Core.Node;
using Umaru.Core.Services;
using Application = Android.App.Application;
using Umaru.Core.OpenCV;
namespace Umaru.Core
{
	public static class Tools
	{
		private static ISuperService? _superService = ServiceLocator.Get<ISuperService>();

		public static void LaunchApp(string packageName)
		{
			_superService?.LaunchApp(packageName);
		}

		public static void CloseApp(string packageName)
		{
			_superService?.CloseApp(packageName);
		}

		public static void Tap(int x, int y)
		{
			_superService.Tap(x, y);
		}

		public static void Swipe(int x1, int y1, int x2, int y2, int duration = 500)
		{
			_superService.Swipe(x1, y1, x2, y2, duration);
		}

		public static void Roll(int index, int count)
		{
			_superService.Roll(index, count);
		}

		public static void KeyEvent(string @event)
		{
			_superService.KeyEvent(@event);
		}

		public static void Toast(string message)
		{
			_superService.Toast(message);
		}
	}
}
