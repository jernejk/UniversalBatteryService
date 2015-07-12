using System;

namespace UniversalBatteryService
{
    public class BatteryStateChangedEventArgs : EventArgs
    {
        public BatteryStateChangedEventArgs(BatteryData data)
        {
            Data = data;
        }

        public BatteryData Data { get; set; }
    }
}
