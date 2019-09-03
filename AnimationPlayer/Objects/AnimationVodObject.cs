using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AnimationPlayer.Reflections;

namespace AnimationPlayer.Objects
{
    public class AnimationVodObject : NotifyPropertyChangedEx
    {
        public AnimationVodObject()
        {

        }

        public AnimationVodObject(string title, string href)
        {
            this.Title = title;
            this.Href = href;
        }

        /// <summary>
        /// 本集數及名稱
        /// </summary>
        private string title = "測試標題【全 測試 集】";
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        /// <summary>
        /// 本集影片片源
        /// </summary>
        //private string href = "https://vpx.myself-bbs.com/45100/001/720p.m3u8";
        private string href = "https://vpx.myself-bbs.com/44685/001/720p.m3u8";
        public string Href
        {
            get { return href; }
            set { SetProperty(ref href, value); }
        }

        /// <summary>
        /// 是否為最近觀看, 預設為否
        /// </summary>
        private Visibility recent_watch = Visibility.Collapsed;
        public Visibility Recent_Watch
        {
            get { return recent_watch; }
            set { SetProperty(ref recent_watch, value); }
        }
    }
}
