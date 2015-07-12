using System;

namespace UniversalBatteryService
{
    public class BatteryData
    {
        public double? BatteryLife { get; set; }

        public TimeSpan? BatteryLifeTime { get; set; }

        public BatteryState BatteryState { get; set; }
    }
}
