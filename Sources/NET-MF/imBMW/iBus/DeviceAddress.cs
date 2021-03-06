using System;
using System.Reflection;

namespace imBMW.iBus
{
    public enum DeviceAddress : short
    {
        BodyModule = 0x00,
        ElectronicBodyModule = 0x02,
        SunroofControl = 0x08,
        DDE = 0x12,
        DME = 0x12,
        CDChanger = 0x18,
        BootLidControlUnit = 0x24,
        RadioControlledClock = 0x28,
        CheckControlModule = 0x30,
        ElectronicGearbox = 0x32,
        GraphicsNavigationDriver = 0x3B,
        Diagnostic = 0x3F,
        RemoteControlCentralLocking = 0x40,
        GraphicsDriverRearScreen = 0x43,
        Immobiliser = 0x44,
        CentralInformationDisplay = 0x46,
        RearMonitor = 0x47,
        MultiFunctionSteeringWheel = 0x50,
        MirrorMemory = 0x51,
        CabrioFoldingModule = 0x52,
        ASC = 0x56,
        SteeringAngleSensor = 0x57,
        IntegratedHeatingAndAirConditioning = 0x5B,
        ParkDistanceControl = 0x60,
        AdaptiveHeadlightUnit = 0x66,
        Radio = 0x68,
        DigitalSignalProcessingAudioAmplifier = 0x6A,
        AuxilaryHeater = 0x6B,
        TirePressureControl = 0x70,
        SeatMemory = 0x72,
        SiriusRadio = 0x73,
        SeatOcupancyRecognition = 0x74,
        CDChangerDINsize = 0x76,
        NavigationEurope = 0x7F,
        InstrumentClusterElectronics = 0x80,
        RevolutionCounter_SteeringColumn = 0x81,
        HeadlightVerticalAimControl = 0x9A,
        MirrorMemorySecond = 0x9B,
        MirrorMemoryThird = 0x9C,
        RearMultiInfoDisplay = 0xA0,
        AirBagModule = 0xA4,
        CruiseControlUnit = 0xA6,
        RearIntegratedHeatingAndAirConditioning = 0xA7,
        NavigationChina = 0xA8,
        EHC = 0xAC,
        SpeedRecognitionSystem = 0xB0,
        NavigationJapan = 0xBB,
        GlobalBroadcastAddress = 0xBF,
        MultiInfoDisplay = 0xC0,
        Telephone = 0xC8,
        Assist = 0xCA,
        LightControlModule = 0xD0,
        SeatMemorySecond = 0xDA,
        IntegratedRadioInformationSystem = 0xE0,
        FrontDisplay = 0xE7,
        RainLightSensor = 0xE8,
        DigitalSignalProcessingAudioController = 0xEA,
        Television = 0xED,
        OnBoardMonitor = 0xF0,
        OBD = 0xF1,
        CentralSwitchControlUnit = 0xF5,
        LocalBroadcastAddress = 0xFF,

        imBMWTest = 0xFB,
        imBMWMenu = 0xFC,
        imBMWPlayer = 0xFD,
        imBMWLogger = 0xFE,
        
        Unset = 0x100,
        Unknown = 0x101
    }

}