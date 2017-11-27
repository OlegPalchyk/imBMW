using System;
using Microsoft.SPOT;
using System.IO;

namespace imBMW.Tools
{
    public class Settings
    {
        static Settings instance;

        protected string settingsPath;
        
        public bool Log { get; set; }

        public bool LogToSD { get; set; }

        public bool LogMessageToASCII { get; set; }

        public bool AutoLockDoors { get; set; }

        public bool AutoUnlockDoors { get; set; }

        public bool AutoCloseWindows { get; set; }

        public bool AutoCloseSunroof { get; set; }

        public bool MenuMFLControl { get; set; }

        public bool RadioSpaceCharAlt { get; set; }
        
        public NaviVersion NaviVersion { get; set; }
        
        public MenuMode MenuMode { get; set; }

        public string Language { get; set; }

        public string BluetoothPin { get; set; }

        public string MediaShield { get; set; }

        public static Settings Init(string path)
        {
            Instance = new Settings();
            Instance.InitDefault();
            //if (path != null && File.Exists(path))
            //{
            //    Instance.InitFile(path);
            //}
            //else
            //{
            //    Logger.Info("No settings file");
            //}
            return Instance;
        }

        protected virtual void InitDefault()
        {
            Log = false;
            LogToSD = false;
            LogMessageToASCII = true;
            MenuMFLControl = true;
            NaviVersion = Tools.NaviVersion.MK4;
            MenuMode = Tools.MenuMode.BordmonitorCDC;
            BluetoothPin = "0000";
        }

        protected virtual void InitFile(string path)
        {
            //try
            //{
            //    using (var sr = new StreamReader(path))
            //    {
            //        string s;
            //        while ((s = sr.ReadLine()) != null)
            //        {
            //            if (s == string.Empty || s[0] == '#')
            //            {
            //                continue;
            //            }
            //            var parts = s.Split('=');
            //            ProcessSetting(parts[0].Trim(), parts.Length > 1 ? parts[1].Trim() : null);
            //        }
            //    }
            //    settingsPath = path;
            //}
            //catch (Exception ex)
            //{
            //    Logger.Error(ex, "reading settings from file");
            //}
        }

        protected virtual void ProcessSetting(string name, string value)
        {
            try
            {
                Logger.Info("Setting: " + name + " = " + (value ?? ""));
                bool isTrue = value == "1" || value == "true" || value == "on" || value == "yes";
                switch (name)
                {
                    case "AutoLockDoors":
                        AutoLockDoors = isTrue;
                        break;
                    case "AutoUnlockDoors":
                        AutoUnlockDoors = isTrue;
                        break;
                    case "AutoCloseWindows":
                        AutoCloseWindows = isTrue;
                        break;
                    case "AutoCloseSunroof":
                        AutoCloseSunroof = isTrue;
                        break;
                    case "Log":
                        Log = isTrue;
                        break;
                    case "LogToSD":
                        LogToSD = isTrue;
                        break;
                    case "LogMessageToASCII":
                        LogMessageToASCII = isTrue;
                        break;
                    case "MenuModeMK2": // Deprecated. Use NaviVersion
                        NaviVersion = Tools.NaviVersion.MK2;
                        break;
                    case "MenuMFLControl":
                        MenuMFLControl = isTrue;
                        break;
                    case "RadioSpaceCharAlt":
                        RadioSpaceCharAlt = isTrue;
                        break;
                    case "MenuMode":
                        MenuMode = (Tools.MenuMode)byte.Parse(value);
                        break;
                    case "NaviVersion":
                        NaviVersion = (Tools.NaviVersion)byte.Parse(value);
                        break;
                    case "Language":
                        Language = value;
                        break;
                    case "BluetoothPin":
                        BluetoothPin = value;
                        break;
                    case "MediaShield":
                        MediaShield = value; // TODO make enum
                        break;
                    default:
                        Logger.Warning("  Unknown setting");
                        return;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "reading setting");
            }
        }

        public static Settings Instance
        {
            protected set
            {
                instance = value;
            }
            get
            {
                if (instance == null)
                {
                    Init(null);
                }
                return instance;
            }
        }
    }
}
