using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.ianlee.FezHat;

namespace FezHatExplorer
{
    public class FezHatItem : INotifyPropertyChanged
    {
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
                RaisePropertyChanged("Temperature");
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
                RaisePropertyChanged("LightLevel");
            }
        }
        private double _lightLevel;

        public FezHatConsumer Consumer { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged(string property)
        {
            var changeWatchers = PropertyChanged;
            if (changeWatchers != null)
            {
                changeWatchers(this, new PropertyChangedEventArgs(property));
            }
        }

    }
}
