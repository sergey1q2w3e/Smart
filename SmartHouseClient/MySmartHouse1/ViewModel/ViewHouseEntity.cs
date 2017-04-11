using MySmartHouse1.Model;

namespace MySmartHouse1
{
    public class ViewHouseEntity : Notifier
    {
        public HouseEntity HouseEntity { get; set; }
        private int _isParameterBusy;
        private bool _isRefreshBusy;

        public int IsParameterBusy
        {
            get { return _isParameterBusy; }
            set
            {
                if (_isParameterBusy != value)
                {
                    _isParameterBusy = value;
                    OnPropertyChanged("IsParameterBusy");
                }
            }
        }

        public bool IsRefreshBusy
        {
            get { return _isRefreshBusy; }
            set
            {
                if (_isRefreshBusy != value)
                {
                    _isRefreshBusy = value;
                    OnPropertyChanged("IsRefreshBusy");
                }
            }
        }
        public ViewHouseEntity()
        {
            HouseEntity = new HouseEntity();
        }
    }
}
