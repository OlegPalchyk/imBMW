using imBMW.iBus;
using imBMW.iBus.Devices.Real;

namespace imBMW.Tools
{
    public static class EnumConverter
    {
        /**
        * :) Sorry, it's .NET MF,
        * so there is no pretty way to print enums
        */

        public static BordmonitorFields GetBordmonitorFieldFromIndex(byte index)
        {
            switch (index)
            {
                case 1: return BordmonitorFields.T1;
                case 2: return BordmonitorFields.T2;
                case 3: return BordmonitorFields.T3;
                case 4: return BordmonitorFields.T4;
                case 5: return BordmonitorFields.T5;
                case 6: return BordmonitorFields.Status;
            }
            return BordmonitorFields.Title;
        }

        public static string ToStringValue(this MFLButton e)
        {
            switch (e)
            {
                case MFLButton.Next: return "Next";
                case MFLButton.Prev: return "Prev";
                case MFLButton.VolumeUp: return "VolumeUp";
                case MFLButton.VolumeDown: return "VolumeDown";
                case MFLButton.RT: return "RT";
                case MFLButton.ModeRadio: return "ModeRadio";
                case MFLButton.ModeTelephone: return "ModeTelephone";
                case MFLButton.Dial: return "Dial";
                case MFLButton.DialLong: return "DialLong";
            }
            return "NotSpecified(" + e.ToString() + ")";
        }

        public static string ToStringValue(this IgnitionState e)
        {
            switch (e)
            {
                case IgnitionState.Off: return "Off";
                case IgnitionState.Acc: return "Acc";
                case IgnitionState.Ign: return "Ign";
                case IgnitionState.Starting: return "Starting";
            }
            return "NotSpecified(" + e.ToString() + ")";
        }

        public static string ToStringValue(this RemoteKeyButton e)
        {
            switch (e)
            {
                case RemoteKeyButton.Lock: return "Lock";
                case RemoteKeyButton.Trunk: return "Trunk";
                case RemoteKeyButton.Unlock: return "Unlock";
            }
            return "NotSpecified(" + e.ToString() + ")";
        }

        public static string ToStringValue(this DeviceAddress e)
        {
            switch (e)
            {
                case DeviceAddress.BodyModule: return "ZKE";
                case DeviceAddress.ElectronicBodyModule: return "ElectronicBodyModule";
                case DeviceAddress.SunroofControl: return "SHD";
                case DeviceAddress.DME: return "DME";
                case DeviceAddress.CDChanger: return "CDC";
                case DeviceAddress.BootLidControlUnit: return "BootLidControlUnit";
                case DeviceAddress.RadioControlledClock: return "RadioControlledClock";
                case DeviceAddress.CheckControlModule: return "CCM";
                case DeviceAddress.ElectronicGearbox: return "EGS";
                case DeviceAddress.GraphicsNavigationDriver: return "GND";
                case DeviceAddress.Diagnostic: return "DIAG";
                case DeviceAddress.RemoteControlCentralLocking: return "RemoteControlCentralLocking";
                case DeviceAddress.GraphicsDriverRearScreen: return "GraphicsDriverRearScreen";
                case DeviceAddress.Immobiliser: return "EWS";
                case DeviceAddress.CentralInformationDisplay: return "CentralInformationDisplay";
                case DeviceAddress.RearMonitor: return "RearMonitor";
                case DeviceAddress.MultiFunctionSteeringWheel: return "MFL";
                case DeviceAddress.MirrorMemory: return "MirrorMemory";
                case DeviceAddress.CabrioFoldingModule: return "CabrioFoldingModule";
                case DeviceAddress.ASC: return "ASC";
                case DeviceAddress.SteeringAngleSensor: return "SteeringAngleSensor";
                case DeviceAddress.IntegratedHeatingAndAirConditioning: return "IHKA";
                case DeviceAddress.ParkDistanceControl: return "PDC";
                case DeviceAddress.AdaptiveHeadlightUnit: return "AdaptiveHeadlightUnit";
                case DeviceAddress.Radio: return "RAD";
                case DeviceAddress.DigitalSignalProcessingAudioAmplifier: return "DSP";
                case DeviceAddress.AuxilaryHeater: return "ZUH";
                case DeviceAddress.TirePressureControl: return "TirePressureControl";
                case DeviceAddress.SeatMemory: return "SeatMemory";
                case DeviceAddress.SiriusRadio: return "SiriusRadio";
                case DeviceAddress.SeatOcupancyRecognition: return "SeatOcupancyRecognition";
                case DeviceAddress.CDChangerDINsize: return "CDChangerDINsize";
                case DeviceAddress.NavigationEurope: return "NAV";
                case DeviceAddress.InstrumentClusterElectronics: return "IKE";
                case DeviceAddress.RevolutionCounter_SteeringColumn: return "RevolutionCounter_SteeringColumn";
                case DeviceAddress.HeadlightVerticalAimControl: return "LWR";
                case DeviceAddress.MirrorMemorySecond: return "MirrorMemorySecond";
                case DeviceAddress.MirrorMemoryThird: return "MirrorMemoryThird";
                case DeviceAddress.RearMultiInfoDisplay: return "RearMID";
                case DeviceAddress.AirBagModule: return "AirBagModule";
                case DeviceAddress.CruiseControlUnit: return "CruiseControlUnit";
                case DeviceAddress.RearIntegratedHeatingAndAirConditioning: return "RearIntegratedHeatingAndAirConditioning";
                case DeviceAddress.NavigationChina: return "NavigationChina";
                case DeviceAddress.EHC: return "EHC";
                case DeviceAddress.SpeedRecognitionSystem: return "SpeedRecognitionSystem";
                case DeviceAddress.NavigationJapan: return "NavigationJapan";
                case DeviceAddress.GlobalBroadcastAddress: return "GLO";
                case DeviceAddress.MultiInfoDisplay: return "MID";
                case DeviceAddress.Telephone: return "TEL";
                case DeviceAddress.Assist: return "Assist";
                case DeviceAddress.LightControlModule: return "LCM";
                case DeviceAddress.SeatMemorySecond: return "SeatMemorySecond";
                case DeviceAddress.IntegratedRadioInformationSystem: return "IntegratedRadioInformationSystem";
                case DeviceAddress.FrontDisplay: return "FrontDisplay";
                case DeviceAddress.RainLightSensor: return "RLS";
                case DeviceAddress.Television: return "Television";
                case DeviceAddress.OnBoardMonitor: return "OBM";
                case DeviceAddress.OBD: return "OBD";
                case DeviceAddress.CentralSwitchControlUnit: return "CentralSwitchControlUnit";
                case DeviceAddress.Broadcast: return "LOC";

                case DeviceAddress.imBMWTest: return "imBMWTest";
                case DeviceAddress.imBMWMenu: return "imBMWMenu";
                case DeviceAddress.imBMWPlayer: return "imBMWPlayer";
                case DeviceAddress.imBMWLogger: return "imBMWLogger";
                
                case DeviceAddress.Unset: return "Unset";
                case DeviceAddress.Unknown: return "Unknown";
            }
            return "NotSpecified(" + e.ToString() + ")";
        }
    }
}
