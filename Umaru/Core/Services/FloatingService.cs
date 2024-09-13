using Android.Animation;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.OS;
using Android.Runtime;
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using AndroidX.Core.View;
using Java.Lang;
using System.Diagnostics;
using Application = Android.App.Application;
using Button = Android.Widget.Button;
using Color = Android.Graphics.Color;
using Exception = Java.Lang.Exception;
using Process = Java.Lang.Process;
using Resource = Microsoft.Maui.Resource;
using View = Android.Views.View;
using WebView = Android.Webkit.WebView;
using Window = Android.Views.Window;

namespace Umaru.Core.Services
{
	[Service(Enabled = true, Exported = false)]
	public class FloatingService : Service
	{
		private IWindowManager _windowManager;
		private View _avatarView;
		private WindowManagerLayoutParams _layoutParams;
		private float _initialX;
		private float _initialY;
		private float _touchX;
		private float _touchY;
		private LinearLayout _menuLayout;
		private bool _isMenuLayoutVisible;
		private DisplayMetrics _displayMetrics = null;
		private ScreenOrientationReceiver _receiver;

		public static bool IsRun
		{
			get;
			set;
		} = false;

		public override IBinder OnBind(Intent intent)
		{
			return null;
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			// 移除并销毁菜单视图
			if (_menuLayout != null && _menuLayout.IsAttachedToWindow)
			{
				_windowManager.RemoveView(_menuLayout);
				_menuLayout = null;
			}

			if (_avatarView != null && _avatarView.IsAttachedToWindow)
			{
				_windowManager.RemoveView(_avatarView);
				_avatarView = null;
			}

			// 注销广播接收器
			if (_receiver != null) UnregisterReceiver(_receiver);
		}

		public override void OnCreate()
		{
			base.OnCreate();
			try
			{
				//得到窗口管理器
				_windowManager = GetSystemService(WindowService).JavaCast<IWindowManager>();

				BuildAvatar();

				// 注册广播接收器
				_receiver = new ScreenOrientationReceiver(this);
				RegisterReceiver(_receiver, new IntentFilter(Intent.ActionConfigurationChanged));
			}
			catch
			{

			}

		}

		public int GetMenuX(int menuX)
		{
			if (_displayMetrics == null) return menuX;
			var w = _displayMetrics.WidthPixels;
			if (menuX > (w - 90)) menuX = w - 90;
			//var x = menuX > 0 ? (menuX + 10) : 10;
			return menuX;
		}

		public int GetMenuY(int menuY)
		{
			if (_displayMetrics == null) return menuY;
			var h = _displayMetrics.HeightPixels;
			menuY = menuY > h ? h : menuY;

			// 判断屏幕方向
			bool isLandscape = Resources.Configuration.Orientation == Android.Content.Res.Orientation.Landscape;
			var offset = (isLandscape ? 0 : 50) + 320;

			// 如果存在菜单，则菜单的 Y 坐标应该比头像的 Y 坐标低
			if (_isMenuLayoutVisible && _menuLayout != null)
			{
				if (menuY > (h - offset)) menuY = h - offset;
			}

			return menuY > 0 ? menuY : 0;
		}

		public void LoadDisplayMetrics()
		{
			_displayMetrics = new DisplayMetrics();
			_windowManager.DefaultDisplay.GetMetrics(_displayMetrics);
		}

		public void BuildAvatar()
		{

			//构建浮动头像
			_avatarView = new ImageView(this)
			{

				LayoutParameters = new ViewGroup.LayoutParams(90, 90),
				ContentDescription = "Avatar",
			};


			((ImageView)_avatarView).SetImageResource(Resource.Drawable.avatar_c);

			//头像宽高
			_layoutParams = new WindowManagerLayoutParams(90, 90, Build.VERSION.SdkInt >= BuildVersionCodes.O ? WindowManagerTypes.ApplicationOverlay : WindowManagerTypes.Phone, WindowManagerFlags.NotFocusable, Format.Translucent);
			_layoutParams.Gravity = GravityFlags.Top | GravityFlags.Left;
			_layoutParams.X = 0;
			_layoutParams.Y = 100;

			_windowManager = GetSystemService(WindowService).JavaCast<IWindowManager>();
			_windowManager.AddView(_avatarView, _layoutParams);

			_avatarView.Touch += OnTouch;
			_avatarView.Click += OnMenuClick;
		}

		public void ReBuildMenu()
		{
			if (_avatarView != null)
			{
				_layoutParams.X = GetMenuX(_layoutParams.X);
				_layoutParams.Y = GetMenuY(_layoutParams.Y);

				_windowManager.UpdateViewLayout(_avatarView, _layoutParams);
			}

			// 更新 _buttonLayout 的位置
			if (_isMenuLayoutVisible && _menuLayout != null)
			{
				var menuLayoutParams = (WindowManagerLayoutParams)_menuLayout.LayoutParameters;
				menuLayoutParams.X = _layoutParams.X > 0 ? (_layoutParams.X + 10) : 10;
				menuLayoutParams.Y = _layoutParams.Y + 100; // 调整 Y 位置以显示在浮动视图下方
				_windowManager.UpdateViewLayout(_menuLayout, menuLayoutParams);
			}

		}

		private bool _isDragging;
		private void OnTouch(object sender, View.TouchEventArgs e)
		{
			LoadDisplayMetrics();
			switch (e.Event.Action)
			{
				case MotionEventActions.Down:
					_initialX = _layoutParams.X;
					_initialY = _layoutParams.Y;
					_touchX = e.Event.RawX;
					_touchY = e.Event.RawY;
					_isDragging = false;
					return;
				case MotionEventActions.Move:
					_layoutParams.X = (int)(_initialX + (e.Event.RawX - _touchX));
					_layoutParams.Y = (int)(_initialY + (e.Event.RawY - _touchY));
					ReBuildMenu();
					_isDragging = true;
					return;
				case MotionEventActions.Up:
					if (!_isDragging) _avatarView.PerformClick();
					return;
			}
		}

		private void OnMenuClick(object sender, EventArgs e)
		{
			if (_isMenuLayoutVisible)
			{
				HideMenu();
			}
			else
			{
				ShowMenu();
			}
		}

		private void OnMenuItemClick(object sender, EventArgs e)
		{
			if (sender is ImageView button)
			{
				int drawableId = (int)button.Tag;

				switch (drawableId)
				{
					case Resource.Drawable.run:
						// 处理运行按钮点击事件
						HandleRun(button);
						break;
					case Resource.Drawable.runing:
						// 处理运行中按钮点击事件
						HandleRuning(button);
						break;
					case Resource.Drawable.home:
						// 处理主页按钮点击事件
						ToHome();
						break;
					case Resource.Drawable.stop:
						// 处理停止按钮点击事件
						System.Environment.Exit(0);
						break;
					default:
						// 处理未知按钮点击事件
						break;
				}
			}
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
				Intent intent = new Intent(this, typeof(MainActivity));
				intent.SetAction(Intent.ActionMain);
				intent.AddCategory(Intent.CategoryLauncher);
				intent.AddFlags(ActivityFlags.ReorderToFront | ActivityFlags.ClearTop | ActivityFlags.SingleTop);

				// 构建 PendingIntent
				PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.UpdateCurrent);

				// 发送 PendingIntent
				pendingIntent.Send();
			}
			catch (Exception ex)
			{
				// 处理异常
				System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
			}
		}

		private void ShowMenu()
		{
			if (_menuLayout != null)
			{
				if (!_isMenuLayoutVisible)
				{
					var tempmenuLayoutParams = (WindowManagerLayoutParams)_menuLayout.LayoutParameters;
					_windowManager.AddView(_menuLayout, tempmenuLayoutParams);
					_isMenuLayoutVisible = true;
				}

				ReBuildMenu();
				return;
			}

			//构建菜单
			_menuLayout = new LinearLayout(this)
			{
				Orientation = Orientation.Vertical,
				LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
			};

			//构建菜单背景
			var gradientDrawable = new GradientDrawable();
			gradientDrawable.SetShape(ShapeType.Rectangle);
			gradientDrawable.SetColor(Color.Argb(60, 128, 128, 128)); // 50% transparent gray
			gradientDrawable.SetCornerRadius(30); // Set the corner radius to make it rounded
			_menuLayout.Background = gradientDrawable;

			var buttons = new int[] { IsRun ? Resource.Drawable.runing : Resource.Drawable.run, Resource.Drawable.home, Resource.Drawable.stop };

			foreach (var img in buttons)
			{
				var button = BuildMenuItem(img);
				_menuLayout.AddView(button);
			}

			//构建样式
			var menuLayoutParams = new WindowManagerLayoutParams(
				ViewGroup.LayoutParams.WrapContent,
				ViewGroup.LayoutParams.WrapContent,
				Build.VERSION.SdkInt >= BuildVersionCodes.O ? WindowManagerTypes.ApplicationOverlay : WindowManagerTypes.Phone,
				WindowManagerFlags.NotFocusable,
				Format.Translucent)
			{
				Gravity = GravityFlags.Top | GravityFlags.Left,
				X = _layoutParams.X > 0 ? (_layoutParams.X + 10) : 10, // Adjust X position to center relative to _floatingView
				Y = _layoutParams.Y + 100 // Adjust Y position to show below the floating view
			};

			_windowManager.AddView(_menuLayout, menuLayoutParams);
			_isMenuLayoutVisible = true;

			ReBuildMenu();
		}

		private void HideMenu()
		{
			if (_menuLayout != null)
			{
				_windowManager.RemoveView(_menuLayout);
				_isMenuLayoutVisible = false;
			}
		}

		public ImageView BuildMenuItem(int drawable)
		{
			var button = new ImageView(this)
			{
				LayoutParameters = new ViewGroup.LayoutParams(60, 60),
				ContentDescription = $"MenuItem_{drawable}",
			};

			button.SetImageResource(drawable);

			var layoutParams = new LinearLayout.LayoutParams(button.LayoutParameters)
			{
				LeftMargin = 5,
				RightMargin = 5,
				TopMargin = 5,
				BottomMargin = 5,
			};
			button.Tag = drawable;
			button.LayoutParameters = layoutParams;
			button.Click += OnMenuItemClick;
			return button;
		}

		private void HandleRun(ImageView button)
		{
			if (!ScriptUtils.IsCanRun()) return;

			// 将运行按钮替换成运行中按钮
			button.SetImageResource(Resource.Drawable.runing);
			button.Tag = Resource.Drawable.runing;
			IsRun = true;

			ScriptUtils.RunScript();
			// 其他运行按钮点击事件处理逻辑
		}

		private void HandleRuning(ImageView button)
		{
			// 将运行按钮替换成运行按钮
			button.SetImageResource(Resource.Drawable.run);
			button.Tag = Resource.Drawable.run;
			IsRun = false;
			ScriptUtils.StopScript();
			// 其他运行按钮点击事件处理逻辑
		}

		private class ScreenOrientationReceiver : BroadcastReceiver
		{
			private readonly FloatingService _service;

			public ScreenOrientationReceiver(FloatingService service)
			{
				_service = service;
			}

			public override void OnReceive(Context context, Intent intent)
			{
				if (intent.Action == Intent.ActionConfigurationChanged)
				{
					// 处理屏幕方向变化
					_service.OnScreenOrientationChanged();
				}
			}
		}

		private void OnScreenOrientationChanged()
		{
			// 重新加载显示指标
			LoadDisplayMetrics();

			// 重新构建菜单
			ReBuildMenu();
		}


		public static void Start()
		{
			try
			{

				var isOverlay = Android.Provider.Settings.CanDrawOverlays(Android.App.Application.Context);
				var isAlert = ContextCompat.CheckSelfPermission(Application.Context, Android.Manifest.Permission.SystemAlertWindow) != (int)Permission.Granted;
				if (isAlert && isOverlay)
				{
					Stop();

					var intent = new Intent(Android.App.Application.Context, typeof(FloatingService));
					Android.App.Application.Context.StartService(intent);
				}
			}
			catch
			{ 
			
			}
		}

		public static void Stop()
		{
			try
			{
				var isOverlay = Android.Provider.Settings.CanDrawOverlays(Android.App.Application.Context);
				var isAlert = ContextCompat.CheckSelfPermission(Application.Context, Android.Manifest.Permission.SystemAlertWindow) != (int)Permission.Granted;
				if (isAlert && isOverlay)
				{
					var intent = new Intent(Android.App.Application.Context, typeof(FloatingService));
					Android.App.Application.Context.StopService(intent);
				}
			}
			catch
			{ 
			
			}
		}
	}
}
