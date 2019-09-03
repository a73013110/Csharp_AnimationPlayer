using AnimationPlayer.UserControls;
using MahApps.Metro.Controls;
using System;
using System.Diagnostics;
using System.Windows;

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
            void Btn_VideoPlayerViewClose_Click(object s, RoutedEventArgs e)
            {
                if (this.Flyout_Video.IsOpen) ((VideoPlayerUserControl)this.Flyout_Video.Content).Btn_VideoPlayerViewClose_Click(s, e);
            }
            this.Btn_PopularAnimation.Click += Btn_VideoPlayerViewClose_Click;  // 精選動畫
            this.Btn_PopularAnimation.Click += this.UC_CrawlerUserControl.Btn_PopularAnimation_Click;
            this.Btn_FavoriteAnimation.Click += Btn_VideoPlayerViewClose_Click; // 我的最愛
            this.Btn_FavoriteAnimation.Click += this.UC_CrawlerUserControl.Btn_FavoriteAnimation_Click;
            this.Btn_RecentAnimation.Click += Btn_VideoPlayerViewClose_Click;   // 最近觀看
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
                foreach (var process in Process.GetProcessesByName("chromedriver")) process.Kill(); // 關閉chromedriver
                VideoPlayerUserControl videoPlayerUserControl = (VideoPlayerUserControl)this.Flyout_Video.Content;
                if (videoPlayerUserControl != null) videoPlayerUserControl.KillStreamLink();
                Environment.Exit(0);    // 關閉所有執行序
            }
            catch (Exception exception) { Console.WriteLine("視窗關閉時發現例外狀況: " + exception); }
        }
    }
}
