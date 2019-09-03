using AnimationPlayer.Objects;
using AnimationPlayer.Reflections;
using System;

namespace AnimationPlayer.Models
{
    public class VideoPlayerModel : NotifyPropertyChangedEx
    {
        public VideoPlayerModel(AnimationVodObject animationVod)
        {
            this.AnimationVod = animationVod;
        }

        public AnimationVodObject AnimationVod { get; set; }

        /// <summary>
        /// 播放總時長
        /// </summary>
        private TimeSpan duration = new TimeSpan(0, 0, 0);
        public TimeSpan Duration
        {
            get { return duration; }
            set { SetProperty(ref duration, value); }
        }
        /// <summary>
        /// 目前播放位置, 目前播放位置實際上為nowDuration+nowDurationOffSet
        /// </summary>
        private TimeSpan nowDuration = new TimeSpan(0, 0, 0);
        public TimeSpan NowDuration
        {
            get { return nowDuration; }
            set { SetProperty(ref nowDuration, value); }
        }
        /// <summary>
        /// 紀錄要倒轉或快轉到的時間點, StreamLink透過此跳轉到該處, 但因跳轉後播放器之Time值又從0開始, 因此需要此變數來記錄
        /// </summary>
        private TimeSpan nowDurationOffSet = new TimeSpan(0, 0, 0);
        public TimeSpan NowDurationOffSet
        {
            get { return nowDurationOffSet; }
            set { SetProperty(ref nowDurationOffSet, value); }
        }
        /// <summary>
        /// 播放器狀態圖示
        /// </summary>
        private string status = "Play";
        public string Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }
        /// <summary>
        /// 播放器音量
        /// </summary>
        private int volume = 100;
        public int Volume
        {
            get { return volume; }
            set { SetProperty(ref volume, value); }
        }
        /// <summary>
        /// 播放音量圖示, 根據聲音大小顯示不同icon
        /// </summary>
        private string volumeIcon = "VolumeHigh";
        public string VolumeIcon
        {
            get { return volumeIcon; }
            set { SetProperty(ref volumeIcon, value); }
        }
        /// <summary>
        /// 播放速度
        /// </summary>
        private float rate = 1;
        public float Rate
        {
            get { return rate; }
            set { SetProperty(ref rate, value); }
        }

        /// <summary>
        /// 是否要添加Log
        /// </summary>
        private bool log = false;
        public bool Log
        {
            get { return log; }
            set { SetProperty(ref log, value); }
        }
    }
}
