using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AnimationPlayer.Objects;
using MahApps.Metro.Controls;

namespace AnimationPlayer.UserControls
{
    /// <summary>
    /// AnimationPreviewUserControl.xaml 的互動邏輯
    /// </summary>
    public partial class AnimationPreviewUserControl : UserControl
    {
        public AnimationPreviewUserControl()
        {
            InitializeComponent();
        }

        public AnimationPreviewUserControl(AnimationObject animationObject)
        {
            this.DataContext = animationObject; // 設置Model
            InitializeComponent();
        }

        /// <summary>
        /// 當滑鼠於圖片上方, 將其Highlight
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BD_ImageCover_MouseEnter(object sender, MouseEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
            this.BD_ImageCover.BeginAnimation(Border.OpacityProperty, animation);
            this.Btn_Watch.Foreground = new SolidColorBrush(Color.FromArgb(0xDF, 0xFF, 0xFF, 0xFF));
            this.Btn_Watch.Background = new SolidColorBrush(Color.FromArgb(0x7F, 0xFF, 0x00, 0x00));
        }

        /// <summary>
        /// 當滑鼠離開圖片, 將其恢復原樣
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BD_ImageCover_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
            this.BD_ImageCover.BeginAnimation(Border.OpacityProperty, animation);
            this.Btn_Watch.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x00, 0x00));
            this.Btn_Watch.Background = new SolidColorBrush(Color.FromArgb(0x7F, 0x59, 0x59, 0x59));
        }

        /// <summary>
        /// 欲觀看動畫的播放按鈕點擊事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Watch_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                AnimationObject animationObject = ((Button)sender).DataContext as AnimationObject;
                mainWindow.Flyout_Animation.Content = new AnimationViewUserControl(animationObject);
                mainWindow.Flyout_Animation.IsOpen = true;
            });            
        }
    }
}
