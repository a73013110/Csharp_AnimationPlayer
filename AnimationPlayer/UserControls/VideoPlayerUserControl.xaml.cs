using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
using MaterialDesignThemes.Wpf;
using AnimationPlayer.Models;
using AnimationPlayer.Objects;
using Vlc.DotNet.Core;
using Vlc.DotNet.Core.Interops;

namespace AnimationPlayer.UserControls
{
    /// <summary>
    /// VideoPlayerUserControl.xaml 的互動邏輯
    /// </summary>
    public partial class VideoPlayerUserControl : UserControl
    {
        public VideoPlayerUserControl() // 測試用
        {
            this.DataContext = VideoPlayerModel = new VideoPlayerModel(new AnimationVodObject());
            InitializeComponent();
        }

        public VideoPlayerUserControl(AnimationVodObject animationVodObject)
        {
            this.DataContext = VideoPlayerModel = new VideoPlayerModel(animationVodObject);
            InitializeComponent();
        }

        public VideoPlayerModel VideoPlayerModel { get; set; }

        /// <summary>
        /// 當Vlc WPF元件載入後 => 初始化播放器 => 註冊播放器事件 => 開始播放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Vlc_VideoPlayer_Loaded(object sender, RoutedEventArgs e)
        {
            var currentDirectory = new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName;
            var vlcLibDirectory = new DirectoryInfo(Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));
            var options = new string[]
            {
                // VLC options can be given here. Please refer to the VLC command line documentation.
                //"-vv",
                $"--file-caching={Properties.Settings.Default.Vlc_file_caching}",
                $"--network-caching={Properties.Settings.Default.Vlc_network_caching}",
            };
            /// 記得要NuGet: VideoLAN.LibVLC.Windows, 否則沒有VLC dll檔無法執行
            this.Vlc_VideoPlayer.SourceProvider.CreatePlayer(vlcLibDirectory, options);
            #region 註冊播放器事件
            //this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.PositionChanged += this.PositionChangedEventHandler;        
            this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.TimeChanged += this.TimeChangedEventHandler;    // 當影片現在時間改變
            this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.Muted += this.MutedEventHandler;    // 當影片靜音
            this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.Unmuted += this.UnmutedEventHandler;    // 當影片取消靜音
            this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.AudioVolume += this.AudioVolumeEventHandler;    // 影片音量調整事件
            this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.Playing += this.PlayingEventHandler;    // 影片播放事件(此事件觸發不代表正在播放==)            /
            this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.EsAdded += this.EsAddedEventHandler;    // 影片播放事件(實際顯示在畫面上), 此方法為偵測添加"音源"事件, 換句話說: 有音源代表影片正在播放
            this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.Paused += this.PausedEventHandler;  // 影片暫停事件
            this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.EndReached += this.EndReachedEventHandler;  // 影片結束
            this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.Buffering += this.BufferingEventHandler;    // 當正在緩衝
            this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.Log += this.LogEventHandler;    // 添加Log
            //this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.AudioDevice += ((ss, ee) => { Console.WriteLine("AudioDevice"); });
            //this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.Backward += ((ss, ee) => { Console.WriteLine("Backward"); });
            //this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.ChapterChanged += ((ss, ee) => { Console.WriteLine("ChapterChanged"); });
            //this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.Corked += ((ss, ee) => { Console.WriteLine("Corked"); });
            //this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.EncounteredError += ((ss, ee) => { Console.WriteLine("EncounteredError"); });
            //this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.EsDeleted += ((ss, ee) => { Console.WriteLine("EsDeleted"); });
            //this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.EsSelected += ((ss, ee) => { Console.WriteLine("EsSelected"); });
            //this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.Forward += ((ss, ee) => { Console.WriteLine("Forward"); });
            //this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.LengthChanged += ((ss, ee) => { Console.WriteLine("LengthChanged"); });
            //this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.MediaChanged += ((ss, ee) => { Console.WriteLine("MediaChanged"); });
            //this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.Opening += ((ss, ee) => { Console.WriteLine("Opening"); });
            //this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.PausableChanged += ((ss, ee) => { Console.WriteLine("LengthChanged"); });
            //this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.ScrambledChanged += ((ss, ee) => { Console.WriteLine("ScrambledChanged"); });
            //this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.SeekableChanged += ((ss, ee) => { Console.WriteLine("SeekableChanged"); });
            //this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.Stopped += ((ss, ee) => { Console.WriteLine("Stopped"); });
            //this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.TitleChanged += ((ss, ee) => { Console.WriteLine("TitleChanged"); });
            //this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.Uncorked += ((ss, ee) => { Console.WriteLine("Uncorked"); });
            //this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.VideoOutChanged += ((ss, ee) => { Console.WriteLine("VideoOutChanged"); });
            #endregion

            // 開新執行序執行, 以免視窗卡住
            Task.Run(() =>
            {
                // 取得影片總時長, Timeout 10秒
                this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.Play(this.VideoPlayerModel.AnimationVod.Href);
                SpinWait.SpinUntil(() => this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.IsPlaying(), 10000);
                this.VideoPlayerModel.Duration = TimeSpan.FromSeconds((int)(this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.Length / 1000));
                this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.Stop();
                // 使用StreamLink取得影片資源並播放
                /// If判斷為避免在尚未執行"StartStreamLink"就關掉此Flyout時
                /// 觸發的"Btn_VideoPlayerViewClose_Click"事件沒有正常關閉StreamLink
                /// 導致關閉此Flyout後還照常播放影片的問題
                this.SettingMediaPlayerWithUIValue(() =>
                {
                    if (((Flyout)this.Parent).IsOpen) this.StartStreamLink();
                });
            });
            //this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.Play(this.VideoPlayerModel.AnimationVod.Link);
        }

        /// <summary>
        /// 關閉影片播放器的Flyout
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Btn_VideoPlayerViewClose_Click(object sender, RoutedEventArgs e)
        {
            this.SettingMediaPlayer(() => this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.Stop());
            this.KillStreamLink();
            Flyout Flyout_Video = this.Parent as Flyout;    // 取得Flyout物件
            Flyout_Video.IsOpen = false;
            ((MainWindow)Application.Current.MainWindow).Flyout_Animation.IsOpen = true;
        }
        /// <summary>
        /// 當滑鼠進入畫面, 動畫顯示控制面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
            this.Grid_InfoBar.BeginAnimation(Grid.OpacityProperty, animation);
        }
        /// <summary>
        /// 當滑鼠離開畫面, 動畫關閉控制面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
            this.Grid_InfoBar.BeginAnimation(Grid.OpacityProperty, animation);
        }
        /// <summary>
        /// 當滑鼠進入音量鍵, 動畫開啟音量調整面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SP_VolumePanel_MouseEnter(object sender, MouseEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation(0, 100, TimeSpan.FromMilliseconds(150));
            this.Slider_Volume.BeginAnimation(Slider.WidthProperty, animation);
        }
        /// <summary>
        /// 當滑鼠於離開音量鍵, 動畫關閉音量調整面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SP_VolumePanel_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation(100, 0, TimeSpan.FromMilliseconds(150));
            this.Slider_Volume.BeginAnimation(Slider.WidthProperty, animation);
        }

        /// <summary>
        /// 滑鼠左鍵點擊影片, 直接導向播放/暫停按鈕事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Vlc_VideoPlayer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Btn_PlayAndStop_Click(sender, new RoutedEventArgs());
        }

        /// <summary>
        /// 滑鼠按下進度條事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider_ProgressBar_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.SettingMediaPlayer(() =>
            {
                if (this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.IsPlaying()) this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.Pause();
            });
        }

        /// <summary>
        /// 滑鼠放開進度條事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider_ProgressBar_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.SettingMediaPlayerWithUIValue(() =>
            {
                // 取得ScrollBar移動後的時間與影片目前播放位置的差, >0: 影片前進, <0: 影片後退
                double distance = ((this.VideoPlayerModel.NowDuration) - (TimeSpan.FromMilliseconds(this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.Time) + this.VideoPlayerModel.NowDurationOffSet)).TotalSeconds;
                if (distance > 5 || distance < 0) this.RestartStreamLinkForSeeking();
                else
                {
                    this.SettingMediaPlayer(() =>
                    {
                        this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.Time += Convert.ToInt64(distance);
                        this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.Play();
                    });
                }
            });
        }

        /// <summary>
        /// 鍵盤快捷鍵
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider_ProgressBar_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                this.RestartStreamLinkForSeeking(new TimeSpan(0, 0, -10));  // 倒退10秒
            }
            else if (e.Key == Key.Right)
            {
                this.SettingMediaPlayer(() => this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.Time += 5000);
            }
            else if (e.Key == Key.Space || e.ImeProcessedKey == Key.Space) this.Btn_PlayAndStop_Click(sender, new RoutedEventArgs());   // 英文輸入法: e.Key == Key.Space, 中文輸入法: e.ImeProcessedKey == Key.Space
            else if (e.Key == Key.Escape) this.Btn_VideoPlayerViewClose_Click(sender, new RoutedEventArgs());
        }

        /// <summary>
        /// 開始/暫停播放按鈕點擊事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_PlayAndStop_Click(object sender, RoutedEventArgs e)
        {
            this.SettingMediaPlayer(() => this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.Pause());
        }

        /// <summary>
        /// 音量靜音事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Volume_Click(object sender, RoutedEventArgs e)
        {
            this.SettingMediaPlayer(() =>
            {
                this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.Audio.IsMute = this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.Audio.IsMute ? false : true;
            });
        }

        /// <summary>
        /// 音量大小改變事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider_Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.Vlc_VideoPlayer.SourceProvider.MediaPlayer == null) return;    // 程式剛執行就會呼叫此function, 此時MediaPlayer為null
            this.SettingMediaPlayer(() =>
            {
                if (this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.Audio.IsMute) this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.Audio.IsMute = false;
            });
            this.SettingMediaPlayerWithUIValue(() =>
            {                
                this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.Audio.Volume = (int)e.NewValue;
            });
        }

        /// <summary>
        /// 播放速度控制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Rate_Click(object sender, RoutedEventArgs e)
        {
            foreach (var child in ((StackPanel)((Button)sender).Parent).Children)   // 找到沒顯示的按鈕, 並顯示
            {
                if (((Button)child).Visibility == Visibility.Collapsed)
                {
                    ((Button)child).Visibility = Visibility.Visible;
                    break;
                }
            }
            ((Button)sender).Visibility = Visibility.Collapsed; // 將選擇的按鈕隱藏
            this.VideoPlayerModel.Rate = float.Parse(((Button)sender).DataContext.ToString());  // 因為VLC沒有RateChanged事件, 因此在這邊更新Model
            this.SettingMediaPlayer(() =>
            {
                this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.Rate = this.VideoPlayerModel.Rate;  
            });
        }

        /// <summary>
        /// 開啟全螢幕 Doesn't Work
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_FullScreen_Click(object sender, RoutedEventArgs e)
        {
            //this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.Video.FullScreen = true;
        }

        /// <summary>
        /// 開啟或隱藏Log面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MI_Log_Click(object sender, RoutedEventArgs e)
        {
            this.TB_StreamLinkLog.Text = this.TB_VLCMediaPlayerLog.Text = "";   // Reset
            this.VideoPlayerModel.Log = !this.VideoPlayerModel.Log; // 取消或允許Log
            this.Grid_Log.Visibility = this.Grid_Log.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;    // 顯示或隱藏Log面板
        }
    }
}
