using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AnimationPlayer.UserControls
{
    public partial class VideoPlayerUserControl
    {
        public Process StreamLinkProcess { get; set; } = null;

        private void StartStreamLink()
        {
            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = "./StreamLink/streamlink.bat",
                WorkingDirectory = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory),
                Arguments = $"{this.VideoPlayerModel.AnimationVod.Href} " +
                $"{Properties.Settings.Default.StreamLink_video_querity} " +
                $"--hls-start-offset={this.VideoPlayerModel.NowDurationOffSet} " +
                $"--hls-segment-threads={Properties.Settings.Default.StreamLink_hls_segment_threads} " +
                $"--loglevel=debug --player-passthrough=http --player-external-http ",
                RedirectStandardInput = true,  // 可輸入 CLI訊息,
                RedirectStandardOutput = true,  // 取得 CLI訊息
                UseShellExecute = false,    // 不使用控制台啟動, 必須與RedirectStandardOutput相反
                CreateNoWindow = true,  // 不顯示視窗
            };
            this.StreamLinkProcess = new Process();
            this.StreamLinkProcess.StartInfo = info;
            this.StreamLinkProcess.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
            {
                if (e.Data == null) return;
                if (this.VideoPlayerModel.Log)
                {
                    this.SettingUI(() =>
                    {
                        this.TB_StreamLinkLog.Text += e.Data + Environment.NewLine;
                        ((ScrollViewer)this.TB_StreamLinkLog.Parent).ScrollToEnd();
                    });
                }                
                if (e.Data.StartsWith("[cli][info]  http://127"))
                {
                    // 設置影片連結並播放
                    string _url = e.Data.Substring(e.Data.IndexOf("http"));
                    this.SettingMediaPlayer(() => this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.Play(_url));
                }
            };
            this.StreamLinkProcess.EnableRaisingEvents = true;
            this.StreamLinkProcess.Start();
            this.StreamLinkProcess.BeginOutputReadLine();
        }

        public void KillStreamLink()
        {
            try
            {
                ProcessStartInfo info = new ProcessStartInfo
                {
                    FileName = "taskkill",
                    Arguments = $"/F /T /PID {this.StreamLinkProcess.Id}",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                Process.Start(info);
            }
            catch (Exception e) { Console.WriteLine("並無執行StreamKink: " + e.Message); }            
        }

        private void RestartStreamLinkForSeeking(TimeSpan? offset = null)
        {
            this.SettingMediaPlayer(() => { if (this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.IsPlaying()) this.Vlc_VideoPlayer.SourceProvider.MediaPlayer.Pause(); });
            if (this.Grid_Loading.Visibility == Visibility.Collapsed) this.Grid_Loading.Visibility = Visibility.Visible;    // 顯示讀取畫面
            if (offset.HasValue) this.VideoPlayerModel.NowDuration = this.VideoPlayerModel.NowDuration + offset.Value;  // 根據offset來移動目前時間, 快進 or 倒退
            this.VideoPlayerModel.NowDurationOffSet = TimeSpan.FromSeconds((int)(this.VideoPlayerModel.NowDuration.TotalSeconds));  // 紀錄後續播放位置, 且秒數不為浮點數
            this.KillStreamLink();  // 關閉StreamLink
            this.StartStreamLink(); // 啟動StreamLink
        }
    }
}
