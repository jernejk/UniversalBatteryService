using System;
using Windows.UI.Xaml;

namespace UniversalBatteryService
{
    public class BatteryService : IBatteryService
    {
        private BatteryData lastBatteryData;
        private DispatcherTimer timer = new DispatcherTimer();

        public event EventHandler<BatteryStateChangedEventArgs> BatteryStateChanged;

        public BatteryService()
        {
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Tick += TimerTick;
        }

        public BatteryData GetCurrentState()
        {
            var status = BatteryNativeUtils.GetStatus();

            BatteryData data = new BatteryData();
            data.BatteryLife = status.BatteryLifePercent;

            if (status.BatteryLifeTime > 0)
            {
                data.BatteryLifeTime = TimeSpan.FromSeconds(status.BatteryLifeTime);
            }

            if ((status.BatteryFlag & BatteryNativeUtils.BatteryFlag.Charging) > 0)
            {
                data.BatteryState = BatteryState.Charging;
            }
            else if ((status.BatteryFlag & BatteryNativeUtils.BatteryFlag.NoSystemBattery) > 0)
            {
                data.BatteryState = BatteryState.NotPresent;
            }
            else if (status.BatteryFlag != BatteryNativeUtils.BatteryFlag.Unknown)
            {
                data.BatteryState = BatteryState.Discharging;
            }
            else
            {
                data.BatteryState = BatteryState.Unknown;
            }

            if (lastBatteryData == null || data.BatteryLife != lastBatteryData.BatteryLife || data.BatteryLifeTime != lastBatteryData.BatteryLifeTime || data.BatteryState != lastBatteryData.BatteryState)
            {
                BatteryStateChanged?.Invoke(this, new BatteryStateChangedEventArgs(data));
            }

            lastBatteryData = data;

            return data;
        }

        public void Start()
        {
            if (!timer.IsEnabled)
            {
                timer.Start();
            }

            GetCurrentState();
        }

        public void Stop()
        {
            timer.Stop();
        }

        private void TimerTick(object sender, object e)
        {
            GetCurrentState();
        }
    }
}
