using System;
using System.Globalization;


namespace TrendWatch.App.Helpers.Converters
{
    public class StarConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                int rating = (int)value;
                int starPosition = System.Convert.ToInt32(parameter);
                if (rating >= starPosition)
                {
                    if (Application.Current.Resources.ContainsKey("ActiveStarStyle"))
                        return Application.Current.Resources["ActiveStarStyle"] as Style;
                    else
                        throw new Exception("ActiveStarStyle not found.");
                }
                else
                {
                    if (Application.Current.Resources.ContainsKey("InactiveStarStyle"))
                        return Application.Current.Resources["InactiveStarStyle"] as Style;
                    else
                        throw new Exception("InactiveStarStyle not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); // Log the error message
                return null;
            }
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
