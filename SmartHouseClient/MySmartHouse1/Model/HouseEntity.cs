using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySmartHouse1.Common
{
    public class HouseEntity : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private int _humidity;
        private int _temperature;
        private int _fanMode;
        private int _fanPower;
        private int _door;

        public int Humidity
        {
            get { return _humidity; }
            set
            {
                if (_humidity != value)
                {
                    _humidity = value;
                    OnPropertyChanged("Humidity");
                }
            }
        }

        public int Temperature
        {
            get { return _temperature; }
            set
            {
                if (_temperature != value)
                {
                    _temperature = value;
                    OnPropertyChanged("Temperature");
                }
            }
        }
        public int FanMode { get; set; }
        public int FanPower { get; set; }
        public int Door { get; set; }
    }
}
