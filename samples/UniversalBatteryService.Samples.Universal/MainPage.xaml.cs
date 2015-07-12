using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UniversalBatteryService.Samples.Universal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private IBatteryService batteryService = new BatteryService();

        public MainPage()
        {
            this.InitializeComponent();

            Loaded += PageLoaded;
            Unloaded += PageUnloaded;
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            batteryService.BatteryStateChanged += BatteryService_BatteryStateChanged;
            batteryService.Start();
        }

        private void PageUnloaded(object sender, RoutedEventArgs e)
        {
            batteryService.Stop();
            batteryService.BatteryStateChanged -= BatteryService_BatteryStateChanged;
        }

        private async void BatteryService_BatteryStateChanged(object sender, BatteryStateChangedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => UpdateBattery(e.Data));
        }

        private void UpdateBattery(BatteryData data)
        {
            BatteryLife.Text = (data.BatteryLife.HasValue ? data.BatteryLife.Value.ToString() : "-") + " %";
            BatteryLifeTime.Text = data.BatteryLifeTime.HasValue ? data.BatteryLifeTime.Value.ToString() : "-";
            BatteryStatus.Text = data.BatteryState.ToString();
        }
    }
}
