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
            // 讀取影片畫質, 因為無法與介面綁定
            foreach (ComboBoxItem item in this.CB_Quality.Items)
                if (item.Content.ToString() == Settings.Default.StreamLink_video_querity)
                {
                    item.IsSelected = true;
                    break;
                }
        }

        /// <summary>
        /// 設定儲存按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default["StreamLink_video_querity"] = ((ComboBoxItem)this.CB_Quality.SelectedItem).Content.ToString(); // 設定影片畫質
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
