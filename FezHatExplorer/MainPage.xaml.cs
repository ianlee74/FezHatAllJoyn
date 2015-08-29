using com.ianlee.FezHat;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Windows.Devices.AllJoyn;
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

        public ObservableCollection<FezHatItem> FezHats { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public class FezHatItem
        {
            public string UniqueName { get; set; }
            public FezHatConsumer Consumer { get; set; }
        }
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += OnLoaded;
            this.FezHats = new ObservableCollection<FezHatItem>();
        }
        void OnLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.DataContext = this;
            this.StartDiscoveringFezHats();
        }
        void StartDiscoveringFezHats()
        {
            this._busAttachment = new AllJoynBusAttachment();
            this._busAttachment.AuthenticationMechanisms.Add(AllJoynAuthenticationMechanism.SrpAnonymous);

            this._fezHatWatcher = new FezHatWatcher(this._busAttachment);
            this._fezHatWatcher.Added += OnAdded;
            this._fezHatWatcher.Start();
        }
        async void OnAdded(FezHatWatcher sender, AllJoynServiceInfo args)
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
              async () =>
              {
                  var joinResult = await FezHatConsumer.JoinSessionAsync(args, sender);

                  if (joinResult.Status != AllJoynStatus.Ok) return;
                  this.FezHats.Add(new FezHatItem()
                  {
                      UniqueName = args.UniqueName,
                      Consumer = joinResult.Consumer
                  });
                  joinResult.Consumer.SessionLost += OnFezHatLost;
              }
            );
        }
        async void OnFezHatLost(FezHatConsumer sender, AllJoynSessionLostEventArgs args)
        {
            await this.Dispatcher.RunAsync(
              CoreDispatcherPriority.Normal,
              () =>
              {
                  this.FezHats.Remove(
              this.FezHats.Single(entry => entry.Consumer == sender));
              }
            );
        }
        public FezHatItem SelectedFezHat
        {
            get
            {
                return (this._selectedFezHat);
            }
            set
            {
                if (this._selectedFezHat != value)
                {
                    this._selectedFezHat = value;
                    this.RaisePropertyChanged("SelectedFezHat");
                }
            }
        }
        void RaisePropertyChanged(string property)
        {
            var changeWatchers = this.PropertyChanged;
            if (changeWatchers != null)
            {
                changeWatchers(this, new PropertyChangedEventArgs(property));
            }
        }
        async void OnRedLedToggled(object sender, RoutedEventArgs e)
        {
            if (this.SelectedFezHat != null)
            {
                bool value = ((ToggleSwitch)sender).IsOn;
                await this.SelectedFezHat.Consumer.SetRedLedStateAsync(value);
            }
        }
    }
}
