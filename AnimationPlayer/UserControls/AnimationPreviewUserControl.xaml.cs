using AnimationPlayer.Objects;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using static AnimationPlayer.GlobalFunctions.AnimationObjectJson;

namespace AnimationPlayer.UserControls
{
    /// <summary>
    /// AnimationPreviewUserControl.xaml 的互動邏輯
    /// </summary>
    public partial class AnimationPreviewUserControl : UserControl
    {
        /// <summary>
        /// 產生AnimationPreviewUserControl, 但不顯示移除按鈕
        /// </summary>
        /// <param name="animationObject"></param>
        public AnimationPreviewUserControl(AnimationObject animationObject)
        {
            this.DataContext = animationObject; // 設置Model
            InitializeComponent();
        }
        /// <summary>
        /// 產生AnimationPreviewUserControl, 並顯示移除按鈕, 根據傳入的方法設定為該按鈕的Click事件
        /// </summary>
        /// <param name="animationObject">動畫資訊</param>
        /// <param name="Btn_Remove_RoutedEventHandler">點擊刪除按鈕事件處理</param>
        public AnimationPreviewUserControl(AnimationObject animationObject, RoutedEventHandler Btn_Remove_RoutedEventHandler)
        {
            this.DataContext = animationObject; // 設置Model
            InitializeComponent();
            this.Btn_Remove.Click += Btn_Remove_RoutedEventHandler;
            this.Btn_Remove.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 我的最愛按鈕點擊事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Favorite_Click(object sender, RoutedEventArgs e)
        {
            SetAnimationObjectToJson((AnimationObject)(((ToggleButton)sender).DataContext));
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
