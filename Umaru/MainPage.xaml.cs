using Umaru.Core.Services;

namespace Umaru
{
	public partial class MainPage : ContentPage
	{
		private readonly IBatteryOptimizationService? _batteryOptimizationService;

		public MainPage()
		{
			InitializeComponent();
			_batteryOptimizationService = ServiceLocator.Get<IBatteryOptimizationService>();
			if (_batteryOptimizationService != null)
			{
				_batteryOptimizationService.BatteryStatusChanged += OnBatteryStatusChanged;
				_batteryOptimizationService.StartMonitoring();
			}
		}

		private void OnBatteryStatusChanged(object? sender, BatteryOptimizationEventArgs e)
		{
			// 根据电池状态进行优化
			if (e.BatteryLevel < 0.2)
			{
				// 例如：降低应用的刷新频率
			}
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			_batteryOptimizationService?.StopMonitoring();
		}
	}
}
