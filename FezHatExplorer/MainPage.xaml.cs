using com.ianlee.FezHat;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.AllJoyn;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FezHatExplorer
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private AllJoynBusAttachment _busAttachment;
        private FezHatWatcher _fezHatWatcher;
        private FezHatItem _selectedFezHat;
        private DispatcherTimer _sensorPollTimer;

        public ObservableCollection<FezHatItem> FezHats { get; }
        public event PropertyChangedEventHandler PropertyChanged;

        public class FezHatItem : INotifyPropertyChanged
        {
            public string UniqueName { get; set; }
            public string DefaultAppName { get; set; }
            public string ModelNumber { get; set; }
            public DateTimeOffset? DateOfManufacture { get; set; }
            public double Temperature { get; set; }
            public double LightLevel { get; set; }
            public FezHatConsumer Consumer { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;
        }
    public MainPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            FezHats = new ObservableCollection<FezHatItem>();

            _sensorPollTimer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 1)};
            _sensorPollTimer.Tick += SensorPollTimerOnTick;
            _sensorPollTimer.Start();
        }

        private void SensorPollTimerOnTick(object sender, object o)
        {
            if (SelectedFezHat == null) return;
            Task.Run(async () =>
            {
                var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    SelectedFezHat.Temperature = (await SelectedFezHat.Consumer.GetTemperatureSensorValueAsync()).ValueF;
                    SelectedFezHat.LightLevel = (await SelectedFezHat.Consumer.GetLightSensorValueAsync()).Value;
                });
                Debug.WriteLine("{0} :\t Temp = {1} \t LightLevel = {2}", SelectedFezHat.UniqueName, SelectedFezHat.Temperature, SelectedFezHat.LightLevel);
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
            Debug.WriteLine("BUTTON DIO18 WAS PRESSED!");

            if (Dio18Color.R == 255)
            {
                Dio18Color = Colors.Blue;
            }
            else
            {
                Dio18Color = Colors.Red;
            }
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
                RaisePropertyChanged("SelectedFezHat");
            }
        }
        void RaisePropertyChanged(string property)
        {
            var changeWatchers = PropertyChanged;
            if (changeWatchers != null)
            {
                changeWatchers(this, new PropertyChangedEventArgs(property));
            }
        }
        async void OnRedLedToggled(object sender, RoutedEventArgs e)
        {
            if (SelectedFezHat == null) return;
            var value = ((ToggleSwitch)sender).IsOn;
            await SelectedFezHat.Consumer.SetRedLedStateAsync(value);
        }

        public Color Dio18Color
        {
            get {  return _dio18Color;}
            set
            {
                if (_dio18Color.GetHashCode() == value.GetHashCode()) return;
                _dio18Color = value;
                RaisePropertyChanged("Dio18Color");
            }   
        }
        private Color _dio18Color;
    }
}
