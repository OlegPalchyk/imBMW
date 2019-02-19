using System;
using imBMW.Features.Localizations;
using imBMW.iBus;
using imBMW.iBus.Devices.Real;
using imBMW.Tools;

namespace imBMW.Features.Menu.Screens
{
    public class AuxilaryHeaterScreen : MenuScreen
    {
        private string SteuernZuheizerOn = "SteuernZuheizerOn";
        private string SteuernZuheizerOff = "SteuernZuheizerOff";

        public static Message MessageStartAuxilaryHeater = new Message(DeviceAddress.GraphicsNavigationDriver, DeviceAddress.InstrumentClusterElectronics, 0x41, 0x12);
        public static Message MessageStopAuxilaryHeater = new Message(DeviceAddress.GraphicsNavigationDriver, DeviceAddress.InstrumentClusterElectronics, 0x41, 0x11);

        protected AuxilaryHeaterScreen()
        {
            TitleCallback = s => Localization.Current.AuxilaryHeater;
            StatusCallback = s => IntegratedHeatingAndAirConditioning.AuxilaryHeaterStatus.ToStringValue();

            SetItems();
        }

        protected virtual void SetItems()
        {
            ClearItems();
            //AddItem(new MenuItem(i => i.IsChecked ? Localization.Current.TurnOff : Localization.Current.TurnOn,
            //    i =>
            //    {
            //        if (i.IsChecked)
            //        {
            //            Manager.EnqueueMessage(MessageStopAuxilaryHeater);
            //        }
            //        else
            //        {
            //            Manager.EnqueueMessage(MessageStartAuxilaryHeater);
            //        }
            //    }, MenuItemType.Checkbox));

            //AddItem(new MenuItem(i => SteuernZuheizerOn, i =>
            //{
            //    DBusManager.Port.WriteBufferSize = 0;

            //    AuxilaryHeater.StartAuxilaryHeaterOverDBus();
            //}, MenuItemType.Button, MenuItemAction.None));

            //AddItem(new MenuItem(i => SteuernZuheizerOff, i =>
            //{
            //    DBusManager.Port.WriteBufferSize = 0;

            //    AuxilaryHeater.StopAuxilaryHeaterOverDBus();
            //}, MenuItemType.Button, MenuItemAction.None));

            string label1 = "W>IHKA: 93 00 22";
            AddItem(new MenuItem(i => label1, i =>
            {
                Logger.Trace("Pressed: " + label1);
                KBusManager.Instance.EnqueueMessage(AuxilaryHeater.AuxilaryHeaterWorkingResponse);
            }, MenuItemType.Button, MenuItemAction.None));

            string label2 = "IHKA>W: 92 00 21";
            AddItem(new MenuItem(i => label2, i =>
            {
                Logger.Trace("Pressed: " + label2);
                KBusManager.Instance.EnqueueMessage(IntegratedHeatingAndAirConditioning.StopAuxilaryHeater1);
            }, MenuItemType.Button, MenuItemAction.None));

            string label3 = "IHKA>W: 92 00 11";
            AddItem(new MenuItem(i => label3, i =>
            {
                Logger.Trace("Pressed: " + label3);
                KBusManager.Instance.EnqueueMessage(IntegratedHeatingAndAirConditioning.StopAuxilaryHeater2);
            }, MenuItemType.Button, MenuItemAction.None));



            AddItem(new MenuItem(i => "StartAuxilaryHeater", i =>
            {
                IntegratedHeatingAndAirConditioning.StartAuxilaryHeater();
            }, MenuItemType.Button, MenuItemAction.None));

            AddItem(new MenuItem(i => "StopAuxilaryHeater", i =>
            {
                IntegratedHeatingAndAirConditioning.StopAuxilaryHeater();
            }, MenuItemType.Button, MenuItemAction.None));

            this.AddBackButton();
        }

        public override bool OnNavigatedTo(MenuBase menu)
        {
            if (base.OnNavigatedTo(menu))
            {
                IntegratedHeatingAndAirConditioning.AuxilaryHeaterStatusChanged += IntegratedHeatingAndAirConditioning_AuxilaryHeaterStatusChanged;
                IntegratedHeatingAndAirConditioning.AuxilaryHeaterWorkingRequestsCounterChanged += IntegratedHeatingAndAirConditioning_AuxilaryHeaterWorkingRequestsCounterChanged;
                return true;
            }
            return false;
        }

        public override bool OnNavigatedFrom(MenuBase menu)
        {
            if (base.OnNavigatedFrom(menu))
            {
                IntegratedHeatingAndAirConditioning.AuxilaryHeaterStatusChanged -= IntegratedHeatingAndAirConditioning_AuxilaryHeaterStatusChanged;
                IntegratedHeatingAndAirConditioning.AuxilaryHeaterWorkingRequestsCounterChanged -= IntegratedHeatingAndAirConditioning_AuxilaryHeaterWorkingRequestsCounterChanged;
                return true;
            }
            return false;
        }

        private void IntegratedHeatingAndAirConditioning_AuxilaryHeaterStatusChanged(AuxilaryHeaterStatus status)
        {
            OnUpdateHeader(MenuScreenUpdateReason.Refresh);
        }

        private void IntegratedHeatingAndAirConditioning_AuxilaryHeaterWorkingRequestsCounterChanged(byte counter)
        {
            OnUpdateHeader(MenuScreenUpdateReason.Refresh);
        }

        protected static AuxilaryHeaterScreen instance;
        public static AuxilaryHeaterScreen Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AuxilaryHeaterScreen();
                }
                return instance;
            }
        }
    }
}
