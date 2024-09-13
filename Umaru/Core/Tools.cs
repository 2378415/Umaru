using Umaru.Core.Services;
using Application = Android.App.Application;
using Umaru.Core.OpenCV;
using Umaru.Core.Http;
using GoogleGson;
using Point = System.Drawing.Point;
using Umaru.Core.Store;
using static Android.Provider.ContactsContract.CommonDataKinds;

namespace Umaru.Core
{
	public static class Tools
	{
		private static ISuperService? _superService = ServiceLocator.Get<ISuperService>();

		/// <summary>
		/// 启动app
		/// </summary>
		/// <param name="packageName"></param>
		public static void LaunchApp(string packageName)
		{
			_superService?.LaunchApp(packageName);
		}

		/// <summary>
		/// 关闭app
		/// </summary>
		/// <param name="packageName"></param>
		public static void CloseApp(string packageName)
		{
			_superService?.CloseApp(packageName);
		}

		/// <summary>
		/// 点击
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public static void Tap(int x, int y)
		{
			_superService.Tap(x, y);
		}

		/// <summary>
		/// 滑动
		/// </summary>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		/// <param name="x2"></param>
		/// <param name="y2"></param>
		/// <param name="duration"></param>
		public static void Swipe(int x1, int y1, int x2, int y2, int duration = 500)
		{
			_superService.Swipe(x1, y1, x2, y2, duration);
		}

		/// <summary>
		/// 滚动
		/// </summary>
		/// <param name="index"></param>
		/// <param name="count"></param>
		public static void Roll(int index, int count)
		{
			_superService.Roll(index, count);
		}

		/// <summary>
		/// 事件
		/// </summary>
		/// <param name="event"></param>
		public static void KeyEvent(string @event)
		{
			_superService.KeyEvent(@event);
		}

		/// <summary>
		/// 提示
		/// </summary>
		/// <param name="message"></param>
		public static void Toast(string message)
		{
			_superService.Toast(message);
		}

		/// <summary>
		/// Post请求
		/// </summary>
		/// <param name="url"></param>
		/// <param name="json"></param>
		/// <returns></returns>
		public static string Post(string url, string json = "")
		{
			return SuperHttp.Post(url, json);
		}

		/// <summary>
		/// Get请求
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static string Get(string url)
		{
			return SuperHttp.Get(url);
		}

		/// <summary>
		/// 找图，低于0.9不能信
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="w"></param>
		/// <param name="h"></param>
		/// <param name="pic_name"></param>
		/// <param name="sim"></param>
		/// <returns></returns>
		public static Point FindPic(int x, int y, int w, int h, string pic_name, float sim)
		{
			return SuperImage.FindPic(x, y, w, h, pic_name, sim);
		}

		/// <summary>
		/// 获取存储的值
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static SqliteModel GetStoreItem(string key)
		{
			return SuperSqlite.Instance.GetItem(key);
		}

		/// <summary>
		/// 保存需要存储的值
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public static int SaveStoreItem(SqliteModel item)
		{
			return SuperSqlite.Instance.SaveItem(item);
		}
	}
}
