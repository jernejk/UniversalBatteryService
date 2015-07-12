using System;

namespace UniversalBatteryService
{
    public interface IBatteryService
    {
        event EventHandler<BatteryStateChangedEventArgs> BatteryStateChanged;
        BatteryData GetCurrentState();

        void Start();
        void Stop();
    }
}
