using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umaru.Core.Services
{
	public interface IBatteryOptimizationService
	{
		void StartMonitoring();
		void StopMonitoring();
		event EventHandler<BatteryOptimizationEventArgs> BatteryStatusChanged;
	}

	public class BatteryOptimizationEventArgs : EventArgs
	{
		public double BatteryLevel { get; set; }
		public BatteryState BatteryState { get; set; }
	}

	public class BatteryOptimizationService : IBatteryOptimizationService
	{
		public event EventHandler<BatteryOptimizationEventArgs> BatteryStatusChanged;

		public void StartMonitoring()
		{
			Battery.BatteryInfoChanged += OnBatteryInfoChanged;
		}

		public void StopMonitoring()
		{
			Battery.BatteryInfoChanged -= OnBatteryInfoChanged;
		}

		private void OnBatteryInfoChanged(object? sender, BatteryInfoChangedEventArgs e)
		{
			var args = new BatteryOptimizationEventArgs
			{
				BatteryLevel = e.ChargeLevel,
				BatteryState = e.State
			};

			BatteryStatusChanged?.Invoke(this, args);
		}
	}
}
