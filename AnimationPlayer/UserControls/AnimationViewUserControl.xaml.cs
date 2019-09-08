using AnimationPlayer.Models;
using AnimationPlayer.Objects;
using AnimationPlayer.Properties;
using MahApps.Metro.Controls;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static AnimationPlayer.GlobalFunctions.AnimationObjectJson;
using Newtonsoft.Json.Linq;

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
            this.DataContext = AnimationViewModel = new AnimationViewModel(animationObject);
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
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.SB_Hint.MessageQueue.Enqueue("正在取得動畫連結...", "確認", () => mainWindow.SB_Hint.IsActive = false);
            AnimationVodObject animationVodObject = ((ListBoxItem)sender).DataContext as AnimationVodObject;
            // 先取得動畫m3u8後, 直接用chrome播放
            await Task.Run(() =>
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://v.myself-bbs.com/api/files/index/" + animationVodObject.Href);
                request.Method = "GET";
                request.Referer = "https://v.myself-bbs.com/player/play/" + animationVodObject.Href;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string responseString = null;
                if (response.StatusCode == HttpStatusCode.OK)
                    using (Stream responseStream = response.GetResponseStream())
                        using (StreamReader reader = new StreamReader(responseStream))
                            responseString = reader.ReadToEnd();
                response.Close();
                if (responseString != null)
                {
                    responseString = responseString.Replace("\\", "");
                    JObject jObject = JObject.Parse(responseString);
                    Process.Start("chrome.exe", $"chrome-extension://{Settings.Default.M3u8Player_ID}/html/Player.html?v={jObject["host"][0]}{jObject["video"]["auto"]}&type=application/x-mpegURL");
                }
            });

            // 儲存近期播放
            await Task.Run(() =>
            {
                if (this.AnimationViewModel.Animation.Recent_Watch_Index >= 0)
                    this.AnimationViewModel.VodList[this.AnimationViewModel.Animation.Recent_Watch_Index].Recent_Watch = Visibility.Collapsed;
                animationVodObject.Recent_Watch = Visibility.Visible;
                this.AnimationViewModel.Animation.Recent_Watch_Index = this.AnimationViewModel.VodList.IndexOf(animationVodObject);
                SetAnimationObjectToJson(this.AnimationViewModel.Animation);    // 將近期播放更新到檔案
            });
        }
    }
}
