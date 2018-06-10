using System;
using imBMW.iBus.Devices.Real;

namespace imBMW.iBus.Devices
{
    public static class NavigationModule
    {
        static Message MessageGetAnalogValues = new Message(DeviceAddress.Diagnostic, DeviceAddress.NavigationEurope, "Get voltage", 0x0B, 0x70);

        static double batteryVoltage;

        static NavigationModule()
        {
            Manager.AddMessageReceiverForSourceDevice(DeviceAddress.NavigationEurope, ProcessNaviMessage);
        }

        static void ProcessNaviMessage(Message m)
        {
            // 7F 21 A0 00 00 00 00 00 00 09 87 00 13 63 00 _35 5B_ 00 04 E3 00 00
            if (m.Data.Length >= 21 && m.Data[0] == 0xA0) // 0xA0 - DIAG data
            {
                var voltageValue = BitConverter.ToInt16(new byte[2] {m.Data[14], m.Data[13]}, 0);
                var voltage = Math.Round((float)voltageValue / 10) / 100;

                m.ReceiverDescription = "Analog values. Battery voltage = " + voltage + "V";
                BatteryVoltage = voltage;
            }
        }

        public static double BatteryVoltage
        {
            get { return batteryVoltage; }
            private set
            {
                batteryVoltage = value;

                var e = BatteryVoltageChanged;
                if (e != null)
                {
                    e(value);
                }
            }
        }

        public static void UpdateBatteryVoltage()
        {
            Manager.EnqueueMessage(MessageGetAnalogValues);
        }

        public static event VoltageEventHandler BatteryVoltageChanged;
    }
}