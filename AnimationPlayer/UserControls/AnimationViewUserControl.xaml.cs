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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using AnimationPlayer.Objects;
using AnimationPlayer.Models;
using Newtonsoft.Json;
using static AnimationPlayer.GlobalFunctions.AnimationRecentWatchJson;

namespace AnimationPlayer.UserControls
{
    /// <summary>
    /// AnimationViewUserControl.xaml 的互動邏輯
    /// </summary>
    public partial class AnimationViewUserControl : UserControl
    {
        // 測試用
        public AnimationViewUserControl()
        {
            this.DataContext = AnimationViewModel = new AnimationViewModel(new AnimationObject());
            InitializeComponent();
            AnimationViewModel.GetAnimationInfoAndVodListCompleted += GetAnimationInfoAndVodListCompleted;
        }
        // 實際使用
        public AnimationViewUserControl(AnimationObject animationObject)
        {
            this.DataContext = AnimationViewModel  = new AnimationViewModel(animationObject);
            InitializeComponent();
            AnimationViewModel.GetAnimationInfoAndVodListCompleted += GetAnimationInfoAndVodListCompleted;
        }

        private AnimationViewModel AnimationViewModel;

        /// <summary>
        /// 點擊關閉按鈕, 關閉Flyout
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_AnimationViewClose_Click(object sender, RoutedEventArgs e)
        {
            Flyout SearchFlyout = this.Parent as Flyout;    // 取得Flyout物件
            SearchFlyout.IsOpen = false;
        }

        /// <summary>
        /// 取得動畫資訊及列表完畢, 將內容更新至UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GetAnimationInfoAndVodListCompleted(object sender, EventArgs e)
        {
            this.Grid_Loading.Visibility = Visibility.Collapsed;
            this.IC_Infos.ItemsSource = AnimationViewModel.Infos;
            this.IC_VodList.ItemsSource = AnimationViewModel.VodList;
            if (this.AnimationViewModel.Animation.Recent_Watch_Index >= 0)
                AnimationViewModel.VodList[this.AnimationViewModel.Animation.Recent_Watch_Index].Recent_Watch = Visibility.Visible; // 設置最近觀看
        }
        
        /// <summary>
        /// 點擊任一集的動畫播放按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AnimationVodObject animationVodObject = ((ListBoxItem)sender).DataContext as AnimationVodObject;
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.Flyout_Animation.IsOpen = false;
            mainWindow.Flyout_Video.Content = new VideoPlayerUserControl(animationVodObject);
            mainWindow.Flyout_Video.IsOpen = true;
            // 開啟影片視窗並播放
            await this.Dispatcher.BeginInvoke(new Action(() =>
            {
                // 儲存近期播放
                if (this.AnimationViewModel.Animation.Recent_Watch_Index >= 0)
                    this.AnimationViewModel.VodList[this.AnimationViewModel.Animation.Recent_Watch_Index].Recent_Watch = Visibility.Collapsed;
                animationVodObject.Recent_Watch = Visibility.Visible;
                this.AnimationViewModel.Animation.Recent_Watch_Index = this.AnimationViewModel.VodList.IndexOf(animationVodObject);
                UpdateRecentWatch(this.AnimationViewModel.Animation);    // 將近期播放更新到檔案
            }));
        }
    }
}
