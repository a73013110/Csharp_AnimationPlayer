using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Diagnostics;

namespace AnimationPlayer
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            LeftWindowCommandsOverlayBehavior = WindowCommandsOverlayBehavior.HiddenTitleBar;
            // 針對視窗左上方的按鈕, 綁定事件
            this.Btn_PopularAnimation.Click += this.UC_CrawlerUserControl.Btn_PopularAnimation_Click;
            this.Btn_FavoriteAnimation.Click += this.UC_CrawlerUserControl.Btn_FavoriteAnimation_Click;
            this.Btn_RecentAnimation.Click += this.UC_CrawlerUserControl.Btn_RecentAnimation_Click;
            this.SB_Hint.MessageQueue = new MaterialDesignThemes.Wpf.SnackbarMessageQueue(TimeSpan.FromSeconds(1.2)); // 設定SnackBar訊息提示秒數
        }

        /// <summary>
        /// 關閉視窗時執行後續處理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                Environment.Exit(0);    // 關閉所有執行序
            }
            catch (Exception exception) { Console.WriteLine("視窗關閉時發現例外狀況: " + exception); }
        }
        /// <summary>
        /// 視窗狀態改變事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MetroWindow_StateChanged(object sender, EventArgs e)
        {
            switch (this.WindowState)
            {
                case WindowState.Normal:
                    this.TaskbarIcon.Visibility = Visibility.Collapsed;
                    break;
                case WindowState.Minimized:
                    this.TaskbarIcon.Visibility = Visibility.Visible;
                    this.Hide();
                    break;
                case WindowState.Maximized:
                    break;
                default:
                    break;
            }
        }
    }
}
