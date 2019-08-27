using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace AnimationPlayer.Reflections
{
    /// <summary>
    /// VideoPlayerUserControl裡面Slider_ProgressBar的Converter
    /// 將影片播放時間轉換成秒, 作為Slider的值
    /// </summary>
    public class Slider_ProgressBar_Converter : IValueConverter
    {
        // 當值從綁定源傳播給綁定目標時, 調用此方法
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            TimeSpan timeSpan = (TimeSpan)value;
            return (int)timeSpan.TotalSeconds;
        }
        // 當值從綁定目標傳播給綁定源時, 調用此方法
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            int second;
            if (value is IConvertible) second = ((IConvertible)value).ToInt32(null);    // 號稱速度最快的轉換型別方式
            else second = System.Convert.ToInt32(value);
            return TimeSpan.FromSeconds(second);
        }
    }

    /// <summary>
    /// 使Xaml的Image Tag可以將Source直接指定為Properties.Resources中的圖片
    /// </summary>
    [ValueConversion(typeof(System.Drawing.Bitmap), typeof(System.Windows.Media.ImageSource))]
    public class BitmapToImageSource_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bmp = value as System.Drawing.Bitmap;
            if (bmp == null) return null;
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
