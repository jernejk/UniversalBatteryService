using System;
using System.Threading;
using System.Threading.Tasks;

namespace UniversalBatteryService
{
    public class BatteryService : IBatteryService
    {
        private BatteryData lastBatteryData;
        private Thread runningThread;

        public event EventHandler<BatteryStateChangedEventArgs> BatteryStateChanged;

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
            if (runningThread == null)
            {
                runningThread = new Thread(BackgroundThread);
                runningThread.Priority = ThreadPriority.BelowNormal;
                runningThread.Start();
            }
        }

        public void Stop()
        {
            if (runningThread != null)
            {
                runningThread.Interrupt();
            }
        }

        private async void BackgroundThread()
        {
            while (true)
            {
                GetCurrentState();

                // Update every 10 seconds.
                await Task.Delay(10000);
            }
        }
    }
}
