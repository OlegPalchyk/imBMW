using imBMW.Features.Localizations;
using imBMW.Features.Menu;
using imBMW.iBus.Devices.Real;
using imBMW.Tools;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System;
using System.Threading;

namespace imBMW.Multimedia
{
    public class iPodViaHeadset : AudioPlayerBase, IDisposable
    {
        public enum iPodCommand
        {
            Play,
            Pause,
            PlayPauseToggle,
            Next,
            Prev,
            VoiceOverCurrent,
            VoiceOverMenu,
            VolumeUp,
            VolumeDown
        }

        const int VoiceOverMenuTimeoutSeconds = 60;

        OutputPort iPod;
        OutputPort iPodVolumeUp;
        OutputPort iPodVolumeDown;
        QueueThreadWorker iPodCommands;
        MenuScreen menu;

        bool canControlVolume;
        bool isInVoiceOverMenu;
        DateTime voiceOverMenuStarted;

        public iPodViaHeadset(Cpu.Pin headsetControl, Cpu.Pin volumeUp = Cpu.Pin.GPIO_NONE, Cpu.Pin volumeDown = Cpu.Pin.GPIO_NONE)
        {
            Name = "iPod";
            iPod = new OutputPort(headsetControl, false);
            canControlVolume = volumeUp != Cpu.Pin.GPIO_NONE && volumeDown != Cpu.Pin.GPIO_NONE;
            if (canControlVolume)
            {
                iPodVolumeUp = new OutputPort(volumeUp, false);
                iPodVolumeDown = new OutputPort(volumeDown, false);
            }

            iPodCommands = new QueueThreadWorker(ExecuteIPodCommand);
        }

        void DisposePorts()
        {
            iPod.Dispose();
            if (canControlVolume)
            {
                iPodVolumeUp.Dispose();
                iPodVolumeDown.Dispose();
            }
        }

        public void Dispose()
        {
            DisposePorts();
            GC.SuppressFinalize(this);
        }

        ~iPodViaHeadset()
        {
            DisposePorts();
        }

        #region Headset logic

        void PressIPodButton(bool longPause = false, int milliseconds = 50)
        {
            iPod.Write(true);
            Thread.Sleep(milliseconds);
            iPod.Write(false);
            Thread.Sleep(longPause ? 300 : 25); // Let iPod understand the command
        }

        void ExecuteIPodCommand(object c)
        {
            var command = (iPodCommand)c;
            switch (command)
            {
                case iPodCommand.PlayPauseToggle:
                    IsPlaying = !IsPlaying;
                    break;

                case iPodCommand.Play:
                    IsPlaying = true;
                    break;

                case iPodCommand.Pause:
                    IsPlaying = false;
                    break;

                case iPodCommand.Next:
                    OnStatusChanged(PlayerEvent.Next);
                    PressIPodButton();
                    PressIPodButton(true);
                    break;

                case iPodCommand.Prev:
                    OnStatusChanged(PlayerEvent.Prev);
                    PressIPodButton();
                    PressIPodButton();
                    PressIPodButton(true);
                    break;

                case iPodCommand.VoiceOverCurrent:
                    if (IsInVoiceOverMenu)
                    {
                        if (IsPlaying)
                        {
                            PressIPodButton(true); // Select currently saying playlist
                            IsInVoiceOverMenu = false;
                            OnStatusChanged("VoiceOver", PlayerEvent.Playing);
                        }
                        else
                        {
                            IsPlaying = true; // Playing starts on VO select when paused
                        }
                    }
                    else
                    {
                        OnStatusChanged("VoiceOver", PlayerEvent.Voice);
                        PressIPodButton(false, 550); // Say current track
                    }
                    break;

                case iPodCommand.VoiceOverMenu:
                    IsInVoiceOverMenu = true;
                    break;

                case iPodCommand.VolumeUp:
                    iPodVolumeUp.Write(true);
                    Thread.Sleep(50);
                    iPodVolumeUp.Write(false);
                    Thread.Sleep(25);
                    break;

                case iPodCommand.VolumeDown:
                    iPodVolumeDown.Write(true);
                    Thread.Sleep(50);
                    iPodVolumeDown.Write(false);
                    Thread.Sleep(25);
                    break;
            }
            Logger.Info("iPod command: " + command.ToStringValue());
        }

        void EnqueueIPodCommand(iPodCommand command)
        {
            iPodCommands.Enqueue(command);
        }

        #endregion

        #region IAudioPlayer members

        public override void Play()
        {
            EnqueueIPodCommand(iPodCommand.Play);
        }

        public override void Pause()
        {
            EnqueueIPodCommand(iPodCommand.Pause);
        }

        public override void PlayPauseToggle()
        {
            EnqueueIPodCommand(iPodCommand.PlayPauseToggle);
        }

        public override void Next()
        {
            EnqueueIPodCommand(iPodCommand.Next);
        }

        public override void Prev()
        {
            EnqueueIPodCommand(iPodCommand.Prev);
        }

        public override void VoiceButtonPress()
        {
            VoiceOverCurrent();
        }

        public override void VoiceButtonLongPress()
        {
            VoiceOverMenu();
        }

        public override bool RandomToggle()
        {
            // Fixing IsPlaying flag, when playing iPod was connected to paused CDC
            isPlaying = !isPlaying;
            OnIsPlayingChanged(IsPlaying);
            return false; // Real status of iPod's shuffle-mode is unknown
        }

        public override void VolumeUp()
        {
            if (canControlVolume)
            {
                EnqueueIPodCommand(iPodCommand.VolumeUp);
            }
        }

        public override void VolumeDown()
        {
            if (canControlVolume)
            {
                EnqueueIPodCommand(iPodCommand.VolumeDown);
            }
        }

        public override bool IsPlaying
        {
            get
            {
                return isPlaying;
            }
            protected set
            {
                if (isPlaying == value)
                {
                    return;
                }
                if (IsInVoiceOverMenu)
                {
                    /**
                     * Trying to prevent the next situation:
                     * 1. VO menu started
                     * 2. PlayPause pressed
                     * 3. Playlist selected instead of PlayPause
                     * 4. IsPlaying flag does't match the iPod playing status
                     */
                    IsInVoiceOverMenu = false;
                    return;
                }
                PressIPodButton(true);
                isPlaying = value;
                IsInVoiceOverMenu = false;
                OnIsPlayingChanged(value);
            }
        }

        public override MenuScreen Menu
        {
            get
            {
                // TODO move to abstract
                if (menu == null)
                {
                    menu = new MenuScreen(Name);

                    var settingsScreen = new MenuScreen(s => Localization.Current.Settings);
                    settingsScreen.Status = Name;
                    settingsScreen.AddItem(new MenuItem(i => Localization.Current.Volume + " +", i => VolumeUp()));
                    settingsScreen.AddItem(new MenuItem(i => Localization.Current.Volume + " -", i => VolumeDown()));
                    settingsScreen.AddBackButton();

                    menu.AddItem(new MenuItem(i => IsPlaying ? Localization.Current.Pause : Localization.Current.Play, i => PlayPauseToggle()));
                    menu.AddItem(new MenuItem(i => Localization.Current.NextTrack, i => Next()));
                    menu.AddItem(new MenuItem(i => Localization.Current.PrevTrack, i => Prev()));
                    menu.AddItem(new MenuItem(i => Localization.Current.Settings, MenuItemType.Button, MenuItemAction.GoToScreen) { GoToScreen = settingsScreen });
                    menu.AddBackButton();
                    menu.UpdateHeader += (m, a) => { if (a.Reason == MenuScreenUpdateReason.StatusChanged) { settingsScreen.Status = m.Status; } };
                }
                return menu;
            }
        }

        #endregion

        public bool IsInVoiceOverMenu
        {
            get
            {
                if (isInVoiceOverMenu && (DateTime.Now - voiceOverMenuStarted).GetTotalSeconds() >= VoiceOverMenuTimeoutSeconds)
                {
                    IsInVoiceOverMenu = false;
                }
                return isInVoiceOverMenu;
            }
            private set
            {
                if (value)
                {
                    OnStatusChanged("VoiceOver", PlayerEvent.Current);
                    voiceOverMenuStarted = DateTime.Now;
                    PressIPodButton(false, 5000);
                }
                isInVoiceOverMenu = value;
            }
        }

        public void VoiceOverCurrent()
        {
            EnqueueIPodCommand(iPodCommand.VoiceOverCurrent);
        }

        public void VoiceOverMenu()
        {
            EnqueueIPodCommand(iPodCommand.VoiceOverMenu);
        }

    }
}
