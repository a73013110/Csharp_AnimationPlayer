using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using Vlc.DotNet.Core;
using System.Windows.Controls;

/// <summary>
/// 此文件主要為設定播放器的事件
/// 規則: 
/// (1) 若要設定或取得播放器相關的東西, 必須使用"ThreadPool.QueueUserWorkItem", 避免Freeze的問題
/// (2) 若要設定或取得UI相關的東西, 必須使用"Dispatcher.InvokeAsync", 因為此事件為Player的執行序, 無法直接控制UI執行序
/// (3) 若要設定或取得"UI Model"的東西, 直接使用即可
/// </summary>

namespace AnimationPlayer.UserControls
{
    public partial class VideoPlayerUserControl
    {
        /// 影片來源改變事件
        private void MediaChangedEventHandler(object sender, VlcMediaPlayerMediaChangedEventArgs e)
        {
            //Console.WriteLine("MediaChangedEventHandler");
        }
        /// 當影片進度改變
        private void PositionChangedEventHandler(object sender, VlcMediaPlayerPositionChangedEventArgs e)
        {
            //Console.WriteLine("PositionChangedEventHandler");
        }
        /// 當影片時間改變
        private void TimeChangedEventHandler(object sender, VlcMediaPlayerTimeChangedEventArgs e)
        {
            //Console.WriteLine("TimeChangedEventHandler");
            this.SettingUI(() => 
            {
                if (this.Grid_Loading.Visibility == Visibility.Visible && this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.IsPlaying()) this.Grid_Loading.Visibility = Visibility.Collapsed;
                else if (!this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.IsPlaying() && this.Grid_Loading.Visibility == Visibility.Collapsed) this.Grid_Loading.Visibility = Visibility.Visible;
            });
            // 回放功能必須使用StreamLink將起始位置導向欲回放位置, 但重啟StreamLink後雖導向正確位置, 但播放器Time參數也是從0開始, 因此用offset來記錄播放到哪
            this.VideoPlayerModel.NowDuration = TimeSpan.FromMilliseconds(e.NewTime) + this.VideoPlayerModel.NowDurationOffSet;
        }
        /// 當影片靜音
        private void MutedEventHandler(object sender, EventArgs e)
        {
            //Console.WriteLine("MutedEventHandler");
            this.VideoPlayerModel.VolumeIcon = "VolumeOff";
        }
        /// 當影片取消靜音
        private void UnmutedEventHandler(object sender, EventArgs e)
        {
            //Console.WriteLine("UnmutedEventHandler");
            this.AudioVolumeEventHandler(sender, new VlcMediaPlayerAudioVolumeEventArgs(this.VideoPlayerModel.Volume)); // 觸發音量調整事件
        }
        /// 影片音量調整事件
        private void AudioVolumeEventHandler(object sender, VlcMediaPlayerAudioVolumeEventArgs e)
        {
            //Console.WriteLine("AudioVolumeEventHandler");
            int volume = this.VideoPlayerModel.Volume;
            if (volume >= 0 && volume < 33) this.VideoPlayerModel.VolumeIcon = "VolumeLow";
            else if (volume >= 33 && volume < 66) this.VideoPlayerModel.VolumeIcon = "VolumeMedium";
            else if (volume >= 66 && volume <= 100) this.VideoPlayerModel.VolumeIcon = "VolumeHigh";
        }
        /// 影片播放事件(此事件觸發不代表正在播放==)
        private void PlayingEventHandler(object sender, VlcMediaPlayerPlayingEventArgs e)
        {
            //Console.WriteLine("PlayingEventHandler");
            this.VideoPlayerModel.Status = "Pause";
        }
        /// 影片播放事件(實際顯示在畫面上), 此方法為偵測添加"音源"事件, 換句話說: 有音源代表影片正在播放
        private void EsAddedEventHandler(object sender, VlcMediaPlayerEsChangedEventArgs e)
        {
            //Console.WriteLine("EsAddedEventHandler");
        }
        /// 影片暫停事件
        private void PausedEventHandler(object sender, VlcMediaPlayerPausedEventArgs e)
        {
            //Console.WriteLine("PausedEventHandler");
            this.VideoPlayerModel.Status = "Play";
        }
        /// 影片結束事件
        private void EndReachedEventHandler(object sender, VlcMediaPlayerEndReachedEventArgs e)
        {
            //Console.WriteLine("EndReachedEventHandler");
            this.SettingUI(() => this.Btn_VideoPlayerViewClose_Click(sender, new RoutedEventArgs()));
        }
        /// 影片緩衝事件
        private void BufferingEventHandler(object sender, VlcMediaPlayerBufferingEventArgs e)
        {
            //Console.WriteLine("BufferingEventHandler");
            this.SettingUI(() =>
            {
                if (!this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.IsPlaying()) this.Grid_Loading.Visibility = Visibility.Visible;
            });
        }
        /// 影片Log
        private void LogEventHandler(object sender, VlcMediaPlayerLogEventArgs e)
        {
            //Console.WriteLine("BufferingEventHandler");
            if (this.VideoPlayerModel.Log)
            {
                this.SettingUI(() =>
                {
                    this.TB_VLCMediaPlayerLog.Text += e.Level + " " + e.Message + Environment.NewLine; ;
                    ((ScrollViewer)this.TB_VLCMediaPlayerLog.Parent).ScrollToEnd();
                });
            }            
        }
    }
}
