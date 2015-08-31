using com.ianlee.FezHat;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.AllJoyn;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using FezHatExplorer.Model;

namespace FezHatExplorer
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private AllJoynBusAttachment _busAttachment;
        private FezHatWatcher _fezHatWatcher;
        private FezHatItem _selectedFezHat;

        public ObservableCollection<FezHatItem> FezHats { get; }
        public event PropertyChangedEventHandler PropertyChanged;

        public MainPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            FezHats = new ObservableCollection<FezHatItem>
            {
                new FezHatItem()
                {
                    DefaultAppName = "Test FEZ",
                    UniqueName = "abc123"
                }
            };

            var sensorPollTimer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 1)};
            sensorPollTimer.Tick += SensorPollTimerOnTick;
            sensorPollTimer.Start();
        }

        private void SensorPollTimerOnTick(object sender, object o)
        {
            if (FezHats == null || FezHats.Count == 0) return;
            Task.Run(async () =>
            {
                foreach (var item in FezHats)
                {
                    if (item.Consumer == null) continue;
                    var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        item.Temperature = (await item.Consumer.GetTemperatureSensorValueAsync()).ValueF;
                        item.LightLevel = (await item.Consumer.GetLightSensorValueAsync()).Value;
                    });
                    Debug.WriteLine("{0} :\t Temp = {1} \t LightLevel = {2}", item.UniqueName, item.Temperature, item.LightLevel);
                }
            });
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;
            StartDiscoveringFezHats();
        }
        void StartDiscoveringFezHats()
        {
            _busAttachment = new AllJoynBusAttachment();
            _busAttachment.AuthenticationMechanisms.Add(AllJoynAuthenticationMechanism.SrpAnonymous);

            _fezHatWatcher = new FezHatWatcher(_busAttachment);
            _fezHatWatcher.Added += OnAdded;
            _fezHatWatcher.Start();
        }
        async void OnAdded(FezHatWatcher sender, AllJoynServiceInfo args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
              async () =>
              {
                  // Get the about data.
                  var aboutData = await AllJoynAboutDataView.GetDataBySessionPortAsync(args.UniqueName, _busAttachment, args.SessionPort);
                  Debug.WriteLine("Found {0} on {1} from manufacturer: {2}. Connecting...", aboutData.AppName, aboutData.DeviceName, aboutData.Manufacturer);

                  var joinResult = await FezHatConsumer.JoinSessionAsync(args, sender);                  
                  if (joinResult.Status != AllJoynStatus.Ok) return;
                  FezHats.Add(new FezHatItem()
                  {
                      UniqueName = args.UniqueName,
                      DefaultAppName = aboutData.AppName,
                      ModelNumber = aboutData.ModelNumber,
                      DateOfManufacture = aboutData.DateOfManufacture,
                      Consumer = joinResult.Consumer
                  });
                  joinResult.Consumer.SessionLost += OnFezHatLost;
                  joinResult.Consumer.Signals.ButtonDio18PressedReceived += Signals_ButtonDio18PressedReceived;
              }
            );
        }

        private void Signals_ButtonDio18PressedReceived(FezHatSignals sender, FezHatButtonDio18PressedReceivedEventArgs args)
        {
            var senderName = args.MessageInfo.SenderUniqueName;
            var item = FezHats.First(c => c.UniqueName == senderName);
            item.ButtonDio18IsPressed = !item.ButtonDio18IsPressed;
            Debug.WriteLine(string.Format("{0} : BUTTON DIO18 WAS PRESSED!", senderName));
        }

        async void OnFezHatLost(FezHatConsumer sender, AllJoynSessionLostEventArgs args)
        {
            await Dispatcher.RunAsync(
              CoreDispatcherPriority.Normal,
              () =>
              {
                  FezHats.Remove(FezHats.Single(entry => entry.Consumer == sender));
              }
            );
        }

        public FezHatItem SelectedFezHat
        {
            get
            {
                return (_selectedFezHat);
            }
            set
            {
                if (_selectedFezHat == value) return;
                _selectedFezHat = value;
                OnPropertyChanged();
            }
        }

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        async void OnRedLedToggled(object sender, RoutedEventArgs e)
        {
            //if (SelectedFezHat == null) return;
            var state = ((ToggleSwitch) sender).IsOn;
            var item = (FezHatItem)((ToggleSwitch)sender).DataContext;
            if (item.Consumer != null)
            {
                await item.Consumer.SetRedLedStateAsync(state);
            }
        }
    }
}
