using System;
using Windows.UI.Xaml;
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
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool && (bool)value)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (value is Visibility && (Visibility)value == Visibility.Visible);
        }
    }

    public class IsParameterBusyConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int num = (int) value;
            if (num == 0) return true;
            else return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
