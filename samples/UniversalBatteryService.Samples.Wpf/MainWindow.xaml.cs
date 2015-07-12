using System;
using System.Windows;

namespace UniversalBatteryService.Samples.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IBatteryService batteryService = new BatteryService();

        public MainWindow()
        {
            InitializeComponent();

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

        private void BatteryService_BatteryStateChanged(object sender, BatteryStateChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => UpdateBattery(e.Data)));
        }

        private void UpdateBattery(BatteryData data)
        {
            BatteryLife.Text = (data.BatteryLife.HasValue ? data.BatteryLife.Value.ToString() : "-") + " %";
            BatteryLifeTime.Text = data.BatteryLifeTime.HasValue ? data.BatteryLifeTime.Value.ToString() : "-";
            BatteryStatus.Text = data.BatteryState.ToString();
        }
    }
}
