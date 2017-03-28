using System;
using Windows.UI.Xaml.Data;

namespace MySmartHouse1
{
    public class LockDoorImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            string culture)
        {
            int num = (int) value;
            if (num < -1 && num > 3) return null;
            if (num == -1) num = 0;
            return String.Format("Assets/door_{0}.png", num);
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            string culture)
        {
            return null;
        }
    }

    public class ToggleTrueFalseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            string culture)
        {
            int num = (int) value;
            if (num == 1) return true;
            else return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            string culture)
        {
            bool val = (bool) value;
            if (val) return 1;
            else return 0;
        }

    }
}
