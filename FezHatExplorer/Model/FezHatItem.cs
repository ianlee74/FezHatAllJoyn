using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI;
using com.ianlee.FezHat;

namespace FezHatExplorer.Model
{
    public class FezHatItem : INotifyPropertyChanged
    {
        public FezHatConsumer Consumer { get; set; }

        public string UniqueName { get; set; }
        public string DefaultAppName { get; set; }
        public string ModelNumber { get; set; }
        public DateTimeOffset? DateOfManufacture { get; set; }


        public double Temperature
        {
            get { return _temperature; }

            set
            {
                if (_temperature == value) return;
                _temperature = value;
                OnPropertyChanged();
            }
        }
        private double _temperature;

        public double LightLevel
        {
            get { return _lightLevel; }
            set
            {
                if (_lightLevel == value) return;
                _lightLevel = value;
                OnPropertyChanged();
            }
        }
        private double _lightLevel;

        public bool RedLedIsOn
        {
            get { return _redLedIsOn; }
            set
            {
                if (_redLedIsOn == value) return;
                _redLedIsOn = value;
                OnPropertyChanged();
            }
        }
        private bool _redLedIsOn;

        public Color LedD2Color
        {
            get { return _ledD2Color; }
            set
            {
                if (_ledD2Color.GetHashCode() == value.GetHashCode()) return;
                _ledD2Color = value;
                OnPropertyChanged();
            }
        }
        private Color _ledD2Color;

        public bool ButtonDio18IsPressed
        {
            get { return _buttonDio18IsPressed; }
            set
            {
                if (_buttonDio18IsPressed == value) return;
                _buttonDio18IsPressed = value;
                OnPropertyChanged();
            }
        }
        private bool _buttonDio18IsPressed;

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
