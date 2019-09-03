using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using AnimationPlayer.Properties;
using MahApps.Metro.Controls;

namespace AnimationPlayer.UserControls
{
    /// <summary>
    /// SettingUserControl.xaml 的互動邏輯
    /// </summary>
    public partial class SettingUserControl : UserControl
    {
        public SettingUserControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 測試與Chrome連接的按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_TestChromeConnection_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("chrome.exe", $"chrome-extension://{this.TB_ChromeExtensionID.Text}/html/Player.html#https://cstr-vod.castr.io/vdafddac30977c11e9bdcb/71983cc5-f510-4b36-b77a-fc3d216e15d3/index.m3u8");
        }

        /// <summary>
        /// 設定儲存按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Save();
            ((Flyout)this.Parent).IsOpen = false;
        }

        /// <summary>
        /// 設定取消按鈕, 關閉Flyout
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            ((Flyout)this.Parent).IsOpen = false;
        }
    }
}
