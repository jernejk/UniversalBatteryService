﻿using System;
using Windows.Phone.Devices.Power;
using Windows.UI.Xaml;

namespace UniversalBatteryService
{
    public class BatteryService : IBatteryService
    {
        private BatteryData lastBatteryData;
        private Battery battery;

        public event EventHandler<BatteryStateChangedEventArgs> BatteryStateChanged;
        
        public BatteryData GetCurrentState()
        {
            if (battery == null)
            {
                battery = Battery.GetDefault();
            }


            BatteryData data = new BatteryData();
            data.BatteryLife = battery.RemainingChargePercent;
            TimeSpan dischargeTime = battery.RemainingDischargeTime;

            if (dischargeTime.Days < 1000)
            {
                data.BatteryLifeTime = battery.RemainingDischargeTime;
                data.BatteryState = BatteryState.Discharging;
            }
            else
            {
                data.BatteryState = BatteryState.Charging;
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
            if (battery == null)
            {
                battery = Battery.GetDefault();
            }

            battery.RemainingChargePercentChanged += BatteryRemainingChargePercentChanged;

            GetCurrentState();
        }

        public void Stop()
        {
            if (battery != null)
            {
                battery.RemainingChargePercentChanged -= BatteryRemainingChargePercentChanged;
            }
        }

        private void BatteryRemainingChargePercentChanged(object sender, object e)
        {
            GetCurrentState();
        }
    }
}
