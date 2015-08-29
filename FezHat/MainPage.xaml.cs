using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.AllJoyn;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using com.ianlee.FezHat;
using GHIElectronics.UAP.Shields;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FezHat
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IFezHatService
    {
        private bool _redLedState = false;
        private FEZHAT _fezhat;
        private readonly bool _isRunningOnPi;
        private AllJoynBusAttachment _busAttachment;
        private FezHatProducer _producer;

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += OnLoaded;

            _isRunningOnPi = Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Devices.Gpio.GpioController");
            if (_isRunningOnPi)
            {
                Task.Run(async () => _fezhat = await FEZHAT.CreateAsync());
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _busAttachment = new AllJoynBusAttachment();
            PopulateAboutData(_busAttachment);
            _producer = new FezHatProducer(_busAttachment) { Service = this };
            _producer.Start();
        }

        private static void PopulateAboutData(AllJoynBusAttachment busAttachment)
        {
            busAttachment.AboutData.DateOfManufacture = DateTime.Now;
            busAttachment.AboutData.DefaultAppName = "Ian's FEZ HAT";
            busAttachment.AboutData.DefaultDescription = "Controls and monitors the devices on the FEZ HAT.";
            busAttachment.AboutData.SupportUrl = new Uri("http://ianlee.info");
        }

        public IAsyncOperation<FezHatSetRedLedStateResult> SetRedLedStateAsync(AllJoynMessageInfo info, bool interfaceMemberOn)
        {
            return (
                Task.Run(() =>
                {
                    SetRedLedState(interfaceMemberOn);
                    return FezHatSetRedLedStateResult.CreateSuccessResult();
                }).AsAsyncOperation());
        }

        private void SetRedLedState(bool state)
        {
            _redLedState = state;
            if (_isRunningOnPi)
            {
                _fezhat.DIO24On = _redLedState;
            }
        }

        public IAsyncOperation<FezHatGetRedLedStateResult> GetRedLedStateAsync(AllJoynMessageInfo info)
        {
            return (
                Task.Run(() =>
                {
                    return FezHatGetRedLedStateResult.CreateSuccessResult(_redLedState);
                }).AsAsyncOperation());
        }

        public IAsyncOperation<FezHatSetRgbLedD2ColorResult> SetRgbLedD2ColorAsync(AllJoynMessageInfo info, bool interfaceMemberOn, byte interfaceMemberRed, byte interfaceMemberGreen, byte interfaceMemberBlue)
        {
            return (
                Task.Run(() =>
                {
                    if (_isRunningOnPi)
                    {
                        _fezhat.D2.Color = new FEZHAT.Color(interfaceMemberRed, interfaceMemberGreen, interfaceMemberBlue);
                    }
                    return FezHatSetRgbLedD2ColorResult.CreateSuccessResult();
                }).AsAsyncOperation());
        }

        public IAsyncOperation<FezHatSetRgbLedD3ColorResult> SetRgbLedD3ColorAsync(AllJoynMessageInfo info, bool interfaceMemberOn, byte interfaceMemberRed, byte interfaceMemberGreen, byte interfaceMemberBlue)
        {
            return (
                Task.Run(() =>
                {
                    if (_isRunningOnPi)
                    {
                        _fezhat.D3.Color = new FEZHAT.Color(interfaceMemberRed, interfaceMemberGreen, interfaceMemberBlue);
                    }
                    return FezHatSetRgbLedD3ColorResult.CreateSuccessResult();
                }).AsAsyncOperation());
        }

        public IAsyncOperation<FezHatGetLightSensorValueResult> GetLightSensorValueAsync(AllJoynMessageInfo info)
        {
            return (
                Task.Run(() =>
                {
                    var lightLevel = 0.0;
                    if (_isRunningOnPi)
                    {
                        lightLevel = _fezhat.GetLightLevel();
                    }
                    return FezHatGetLightSensorValueResult.CreateSuccessResult(lightLevel);
                }).AsAsyncOperation());
        }

        public IAsyncOperation<FezHatGetTemperatureSensorValueResult> GetTemperatureSensorValueAsync(AllJoynMessageInfo info)
        {
            return (
                Task.Run(() =>
                {
                    var tempC = 0.0;
                    if (_isRunningOnPi)
                    {
                        tempC = _fezhat.GetTemperature();
                    }
                    return FezHatGetTemperatureSensorValueResult.CreateSuccessResult(tempC, tempC * 1.8 + 32);
                }).AsAsyncOperation());
        }
    }
}
