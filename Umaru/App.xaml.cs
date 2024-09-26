using Android.Content;
using Umaru.Core.Services;

namespace Umaru
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
            // 订阅 ProcessExit 事件
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            FloatingService.Start();
		}

        protected override void OnResume()
        {
            base.OnResume();
			FloatingService.Stop();
		}

        private void OnProcessExit(object? sender, EventArgs e)
        {

        }

    }
}
