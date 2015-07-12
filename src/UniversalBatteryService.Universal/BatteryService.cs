using System;
using Windows.Devices.Power;

namespace UniversalBatteryService
{
    public class BatteryService : IBatteryService
    {
        private BatteryData lastBatteryData;

        public event EventHandler<BatteryStateChangedEventArgs> BatteryStateChanged;

        public BatteryData GetCurrentState()
        {
            BatteryReport report = Battery.AggregateBattery.GetReport();

            BatteryData data = new BatteryData();
            data.BatteryLife = report.BatteryLevelInPercentage();
            data.BatteryLifeTime = report.EstimateTimeToDischarge();

            switch (report.Status)
            {
                case Windows.System.Power.BatteryStatus.NotPresent:
                    data.BatteryState = BatteryState.NotPresent;
                    break;
                case Windows.System.Power.BatteryStatus.Discharging:
                    data.BatteryState = BatteryState.Discharging;
                    break;
                case Windows.System.Power.BatteryStatus.Charging:
                    data.BatteryState = BatteryState.Charging;
                    break;
                default:
                    data.BatteryState = BatteryState.Unknown;
                    break;
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
            // Ensure there are no duplicate event handling.
            Battery.AggregateBattery.ReportUpdated -= AggregateBattery_ReportUpdated;
            Battery.AggregateBattery.ReportUpdated += AggregateBattery_ReportUpdated;

            // Invoke changed event if no state is yet present or battery state has changed.
            GetCurrentState();
        }

        public void Stop()
        {
            Battery.AggregateBattery.ReportUpdated -= AggregateBattery_ReportUpdated;
        }

        private void AggregateBattery_ReportUpdated(Battery sender, object args)
        {
            GetCurrentState();
        }
    }
}
