using Android.AccessibilityServices;
using Microsoft.Extensions.Logging;
using Umaru.Core.OpenCV;
using Umaru.Core.Services;
using MudBlazor.Services;


namespace Umaru
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>();
				//.ConfigureFonts(fonts =>
				//{
				//	fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				//});

			// 注册 ISuperService 的实现
			//builder.Services.AddSingleton<AccessibilityService, BarrierService>();
			builder.Services.AddSingleton<ISuperService, SuperService>();

			//添加httpclient 忽略ssl
			builder.Services.AddHttpClient("IgnoreSSL").ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
			{
				ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
			});

			builder.Services.AddMauiBlazorWebView();
            builder.Services.AddMudServices();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
			builder.Logging.AddDebug();
#endif

			var app = builder.Build();

			// 在此处进行任何需要的初始化操作
			ServiceLocator.Registry(app.Services);

			return app;
		}
	}
}
