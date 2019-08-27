using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimationPlayer.Reflections;

namespace AnimationPlayer.Objects
{
    public class AnimationVodObject : NotifyPropertyChangedEx
    {
        public AnimationVodObject()
        {

        }

        public AnimationVodObject(string title, string link)
        {
            this.Title = title;
            this.Link = link;
        }

        private string title = "測試標題【全 測試 集】";
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        //private string link = "https://vpx.myself-bbs.com/45100/001/720p.m3u8";
        private string link = "https://vpx.myself-bbs.com/44685/001/720p.m3u8";
        public string Link
        {
            get { return link; }
            set { SetProperty(ref link, value); }
        }
    }
}
