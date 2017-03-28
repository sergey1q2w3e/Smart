using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySmartHouse1.Common
{
    public class Notifier : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    public class HouseEntity : Notifier
    {
        

        private int? _humidity;
        private int? _temperature;
        private int _fanMode;
        private int _fanPower;
        private int _door;

        public int? Humidity
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

        public int? Temperature
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

        public int FanMode
        {
            get { return _fanMode; }
            set
            {
                if (_fanMode != value)
                {
                    _fanMode = value;
                    OnPropertyChanged("FanMode");
                }
            }
        }

        public int FanPower
        {
            get { return _fanPower; }
            set
            {
                if (_fanPower != value)
                {
                    _fanPower = value;
                    OnPropertyChanged("FanPower");
                }
            }
        }

        public int Door
        {
            get { return _door; }
            set
            {
                if (_door != value)
                {
                    _door = value;
                    OnPropertyChanged("Door");
                }
            }
        }
    }
}
